using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureResourceDiscovery.Core
{
    public class AzurePolicyGenerator
    {
        public bool GenerateFiles(string filePath)
        {
            var manifest = JsonSerializer.Deserialize<Manifest>(File.ReadAllText(filePath));
            throw new NotImplementedException();
        }
    }
}
