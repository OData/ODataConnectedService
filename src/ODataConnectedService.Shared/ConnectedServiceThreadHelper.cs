using System;

using System.Threading.Tasks;
using Microsoft.OData.CodeGen;
using Microsoft.VisualStudio.Shell;

namespace ODataConnectedService.Shared
{
    internal class ConnectedServiceThreadHelper : IThreadHelper
    {
        /// <inheritdoc/>
        public async Task<T> RunAsync<T>(Func<Task<T>> backgroundTask)
        {
            return await ThreadHelper.JoinableTaskFactory.RunAsync(backgroundTask);
        }

        /// <inheritdoc/>
        public async Task<T> RunInUiThreadAsync<T>(Func<T> foregroundTask)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return foregroundTask();
        }
    }
}
