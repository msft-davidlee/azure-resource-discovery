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
        public bool GenerateFiles(string content)
        {
            var manifest = JsonSerializer.Deserialize<Manifest>(content);
            throw new NotImplementedException();
        }
    }
}
