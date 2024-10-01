using System;

using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Threading;
using Microsoft.VisualStudio.Shell;

namespace ODataConnectedService.Shared
{
    internal class ConnectedServiceThreadHelper : IThreadHelper
    {
        /// <inheritdoc/>
        public async Task<T> RunInUiThreadAsync<T>(Func<T> foregroundTask)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return foregroundTask();
        }
    }
}
