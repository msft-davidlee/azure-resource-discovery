﻿using System.Text.Encodings.Web;
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
            AllOf = new List<AzurePolicyDtoField>();
            AnyOf = new List<AzurePolicyDtoField>();
        }

        [JsonPropertyName("allOf")]
        public List<AzurePolicyDtoField>? AllOf { get; set; }

        [JsonPropertyName("anyOf")]
        public List<AzurePolicyDtoField>? AnyOf { get; set; }

        public void AnyResource(Dictionary<string, string> tags)
        {
            AllOf = null;

            if (AnyOf == null) throw new ApplicationException("AnyOf property state is invalid.");

            AnyOf.AddRange(tags.Select((x) => new AzurePolicyDtoField { Field = $"tags['{x.Key}']", IsNotEquals = x.Value }));
        }

        public void UniqueResource(string type, string tagKey, string tagValue)
        {
            AnyOf = null;

            if (AllOf == null) throw new ApplicationException("AllOf property state is invalid.");

            AllOf.AddRange(new[]
            {
                new AzurePolicyDtoField
                {
                    Field = "type",
                    IsEquals = type
                },
                new AzurePolicyDtoField
                {
                    Field = $"tags['{tagKey}']",
                    IsNotEquals = tagValue
                }
            });
        }
    }

    public class AzurePolicyDtoField
    {
        [JsonPropertyName("field")]
        public string? Field { get; set; }

        [JsonPropertyName("equals")]
        public string? IsEquals { get; set; }

        [JsonPropertyName("notEquals")]
        public string? IsNotEquals { get; set; }
    }

    public class AzurePolicyThenEffectModify
    {
        public AzurePolicyThenEffectModify()
        {
            Details = new();
        }

        [JsonPropertyName("effect")]
        public string Effect { get; set; } = "modify";

        [JsonPropertyName("details")]
        public AzurePolicyThenEffectDetails Details { get; set; }
    }

    public class AzurePolicyThenEffectDetails
    {
        public AzurePolicyThenEffectDetails()
        {
            Operations = new();
            RoleDefinationIds = new();
        }

        [JsonPropertyName("operations")]
        public List<AzurePolicyThenEffectDetailsOperation> Operations { get; set; }

        public void AddOrReplaceTag(string key, string value)
        {
            Operations.Add(new AzurePolicyThenEffectDetailsOperation("addOrReplace", $"tags['{key}']", value));
        }

        [JsonPropertyName("roleDefinitionIds")]
        public List<string> RoleDefinationIds { get; set; }
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

    public static class Constants
    {
        public const string ArdSolutionId = "ard-solution-id";
        public const string ArdEnvironment = "ard-environment";
        public const string ArdRegion = "ard-region";

        public static class RoleDefinationIds
        {
            public const string TagContributor = "/providers/microsoft.authorization/roleDefinitions/4a9ae827-6dc8-4573-8ac7-8239d42aa03f";
        }
    }
}
