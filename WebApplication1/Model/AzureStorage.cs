using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileManagerAPI.Model
{
    public class AzureStorage
    {
        public string AccountName { get; set; }

        public string DefaultEndpointsProtocol { get; set; }

        public string AccountKey { get; set; }

        public string EndpointSuffix { get; set; }

        public string Container { get; set; }

    }

    public class AzureKeyVault
    {
        public string Url { get; set; }

        public string KeyName { get; set; }

    }
}
