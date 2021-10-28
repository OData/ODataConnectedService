using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Compatibility
{
    class ODataConnectedServiceHandler : IConnectedServiceCompatibilityHandler
    {
        private readonly ConnectedServiceHandlerHelper _contextHandlerHelper;

        public ODataConnectedServiceHandler(ConnectedServiceHandlerHelper contextHandlerHelper)
        {
            _contextHandlerHelper = contextHandlerHelper;
        }

        public Task AddFileAsync(string inputFile, string outputFile, AddFileOptions addFileOptions)
        {
            return this._contextHandlerHelper.AddFileAsync(inputFile, outputFile,
                new VisualStudio.ConnectedServices.AddFileOptions() {
                    OpenOnComplete = addFileOptions.OpenOnComplete,
                    SuppressOverwritePrompt = addFileOptions.SuppressOverwritePrompt,
                });
        }

        public Task AddFileAsync(string inputFile, string outputFile)
        {
            return this._contextHandlerHelper.AddFileAsync(inputFile, outputFile);
        }

        public string GetServiceArtifactsRootFolder()
        {
            return this._contextHandlerHelper.GetServiceArtifactsRootFolder();
        }
    }
}
