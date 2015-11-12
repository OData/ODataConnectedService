using System;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    internal class ODataConnectedServiceInstance : ConnectedServiceInstance
    {
        public ODataConnectedServiceInstance()
        {
            this.InstanceId = "ODataCodeGen";
            this.Name = "OData Connected Service";
        }

        public string Endpoint { get; set; }
        public string NamespacePrefix { get; set; }
        public bool UseDataServiceCollection { get; set; }
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public Version EdmxVersion { get; set; }
        public string MetadataTempFilePath { get; set; }
        public string GeneratedFileNamePrefix { get; set; }
        public bool IncludeT4File { get; set; }
    }
}
