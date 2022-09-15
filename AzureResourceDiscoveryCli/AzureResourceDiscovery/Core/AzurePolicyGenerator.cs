using System.Text.Json;

namespace AzureResourceDiscovery.Core
{
    public class AzurePolicyGenerator
    {
        public bool GenerateFiles(string content, Action<AzurePolicy> processAzurePolicy)
        {
            var manifest = JsonSerializer.Deserialize<Manifest>(content);

            if (manifest == null) return false;

            if (manifest.UniqueResources != null)
            {
                foreach (var uniqueResource in manifest.UniqueResources)
                {
                    AzurePolicy azurePolicy = new();

                    if (uniqueResource.ResourceGroupNames == null) throw new ApplicationException("ResourceGroupNames cannot be null!");

                    azurePolicy.If.AnyOfResourceGroupNames(uniqueResource.ResourceGroupNames);

                    if (string.IsNullOrEmpty(uniqueResource.ResourceId)) throw new ApplicationException("ResourceId cannot be null!");

                    azurePolicy.ThenEffectModify.Details.AddOrReplaceTag("ard-resource-id", uniqueResource.ResourceId);

                    processAzurePolicy(azurePolicy);
                }
            }

            return true;
        }
    }
}
