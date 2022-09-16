using System.Text.Json;

namespace AzureResourceDiscovery.Core
{
    public class AzurePolicyResult
    {
        public AzurePolicyResult(AzurePolicy azurePolicy, string name, string displayName, string description)
        {
            AzurePolicy = azurePolicy;
            Name = name;
            DisplayName = displayName;
            Description = description;
        }

        public AzurePolicy AzurePolicy { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Description { get; }
    }

    public class AzurePolicyGenerator
    {
        public bool GenerateFiles(string content, Action<AzurePolicyResult> processAzurePolicyResult)
        {
            var manifest = JsonSerializer.Deserialize<Manifest>(content);

            if (manifest == null) return false;

            if (manifest.UniqueResources != null)
            {
                foreach (var uniqueResource in manifest.UniqueResources)
                {
                    AzurePolicy azurePolicy = new();

                    if (uniqueResource.ResourceGroupNames == null) throw new ApplicationException("ResourceGroupNames cannot be null!");
                    if (string.IsNullOrEmpty(uniqueResource.Name)) throw new ApplicationException("Name cannot be null.");
                    if (string.IsNullOrEmpty(uniqueResource.ResourceType)) throw new ApplicationException("ResourceType cannot be null.");
                    if (string.IsNullOrEmpty(uniqueResource.ResourceId)) throw new ApplicationException("ResourceId cannot be null.");

                    azurePolicy.If.UniqueResource(
                        uniqueResource.ResourceType,
                        UniqueResource.TagKey,
                        uniqueResource.ResourceId);

                    if (string.IsNullOrEmpty(uniqueResource.ResourceId)) throw new ApplicationException("ResourceId cannot be null!");

                    azurePolicy.ThenEffectModify.Details.AddOrReplaceTag("ard-resource-id", uniqueResource.ResourceId);
                    
                    azurePolicy.ThenEffectModify.Details.RoleDefinationIds.Add(Constants.RoleDefinationIds.TagContributor);

                    processAzurePolicyResult(new AzurePolicyResult(azurePolicy, uniqueResource.Name, "Enforce ard-resource-id", $"Enforce ard-resource-id for {uniqueResource.Name}"));
                }
            }

            return true;
        }
    }
}
