
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Compatibility
{
    class ODataConnectedServiceContext :IConnectedServiceCompatibilityContext
    {

        public ODataConnectedServiceContext(ConnectedServiceHandlerContext context)
        {
            this.Logger = new ODataConnectedServiceLogger(context.Logger);
            this.HandlerHelper = new ODataConnectedServiceHandler(context.HandlerHelper);
        }

        public ODataConnectedServiceInstance ServiceInstance { get; set; }
        public IConnectedServiceCompatibilityHandler HandlerHelper { get; set; }
        public IConnectedServiceCompatibilityLogger Logger { get; set; }
    }
}
