using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AzureResourceDiscovery.Core
{
    public class AzurePolicy
    {
        public AzurePolicy()
        {
            If = new AzurePolicyDtoIf();
            ThenEffectModify = new AzurePolicyThenEffectModify();
        }

        [JsonPropertyName("if")]
        public AzurePolicyDtoIf If { get; }

        [JsonPropertyName("then")]
        public AzurePolicyThenEffectModify ThenEffectModify { get; }

        public override string ToString()
        {
            // See: https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-character-encoding
            // Used to allow single quote, otherwise it will be converted.
            JsonSerializerOptions options = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            return JsonSerializer.Serialize(this, options);
        }
    }

    public class AzurePolicyDtoIf
    {
        public AzurePolicyDtoIf()
        {
            AnyOf = new List<AzurePolicyDtoField>();
        }

        [JsonPropertyName("anyOf")]
        public List<AzurePolicyDtoField> AnyOf { get; }

        public void AnyOfResourceGroupNames(List<string> names)
        {
            foreach (var name in names)
            {
                AnyOf.Add(new AzurePolicyDtoField
                {
                    Value = "[resourceGroup().name]",
                    IsEquals = name
                });
            }
        }
    }

    public class AzurePolicyDtoField
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("field")]
        public string? Name { get; set; }

        [JsonPropertyName("equals")]
        public string? IsEquals { get; set; }
    }

    public class AzurePolicyThenEffectModify
    {
        public AzurePolicyThenEffectModify()
        {
            Details = new();
        }

        public string Modify = "Modify";

        [JsonPropertyName("details")]
        public AzurePolicyThenEffectDetails Details { get; set; }
    }

    public class AzurePolicyThenEffectDetails
    {
        public AzurePolicyThenEffectDetails()
        {
            Operations = new();
        }

        [JsonPropertyName("conflictEffect")]
        public string conflictEffect = "deny";

        public List<AzurePolicyThenEffectDetailsOperation> Operations { get; set; }

        public void AddOrReplaceTag(string key, string value)
        {
            Operations.Add(new AzurePolicyThenEffectDetailsOperation("addOrReplace", $"tags['{key}']", value));
        }
    }

    public class AzurePolicyThenEffectDetailsOperation
    {
        [JsonPropertyName("operation")]
        public string Operation { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        public AzurePolicyThenEffectDetailsOperation(string operation, string field, string value)
        {
            Operation = operation;
            Field = field;
            Value = value;
        }
    }
}
