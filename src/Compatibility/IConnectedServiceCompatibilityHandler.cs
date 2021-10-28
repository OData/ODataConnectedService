using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Compatibility
{
    interface IConnectedServiceCompatibilityHandler
    {
        string GetServiceArtifactsRootFolder();
        Task AddFileAsync(string inputFile, string outputFile, AddFileOptions addFileOptions);
        Task AddFileAsync(string v1, string v2);
    }
}
