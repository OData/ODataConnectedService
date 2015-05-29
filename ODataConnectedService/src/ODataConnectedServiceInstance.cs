using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    public class ODataConnectedServiceInstance : ConnectedServiceInstance
    {
        public string Endpoint { get; set; }
        public string NamespacePrefix { get; set; }
        public bool UseDataServiceCollection { get; set; }
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public bool GenByDataSvcUtil { get; set; }
    }
}
