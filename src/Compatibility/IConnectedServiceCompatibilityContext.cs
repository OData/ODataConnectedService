namespace Microsoft.OData.ConnectedService.Compatibility
{
    interface IConnectedServiceCompatibilityContext
    {
        ODataConnectedServiceInstance ServiceInstance { get; set; }
        IConnectedServiceCompatibilityHandler HandlerHelper { get; set; }
        IConnectedServiceCompatibilityLogger Logger { get; set; }
    }
}
