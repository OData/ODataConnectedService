using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Compatibility
{
    interface IConnectedServiceCompatibilityLogger
    {
        Task WriteMessageAsync(LogLevel information, string message, params object[] args);
    }
}
