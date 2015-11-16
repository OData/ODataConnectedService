using Microsoft.OData.ConnectedService.Models;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    internal class ODataConnectedServiceInstance : ConnectedServiceInstance
    {
        public ServiceConfiguration ServiceConfig { get; set; }
        public string MetadataTempFilePath { get; set; }
    }
}
