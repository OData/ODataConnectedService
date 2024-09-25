using System;

using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Threading;

namespace ODataConnectedService.Tests.TestHelpers
{
    internal class TestThreadHelper : IThreadHelper
    { 
        /// <inheritdoc/>
        public Task<T> RunInUiThreadAsync<T>(Func<T> backgroundTask)
        {
            return Task.Run(backgroundTask);
        }
    }
}
