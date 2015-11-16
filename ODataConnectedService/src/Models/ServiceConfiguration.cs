using System;

namespace Microsoft.OData.ConnectedService.Models
{
    internal class ServiceConfiguration
    {
        public string ServiceName { get; set; }
        public string Endpoint { get; set; }
        public Version EdmxVersion { get; set; }
        public string GeneratedFileNamePrefix { get; set; }
        public bool UseNameSpacePrefix { get; set; }
        public string NamespacePrefix { get; set; }
        public bool UseDataServiceCollection { get; set; }
    }

    internal class ServiceConfigurationV4 : ServiceConfiguration
    {
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public bool IncludeT4File { get; set; }
    }
}
