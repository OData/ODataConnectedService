using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen;

namespace ODataConnectedService.Tests.TestHelpers
{
    internal class TestThreadHelper : IThreadHelper
    {
        /// <inheritdoc/>
        public Task<T> RunAsync<T>(Func<Task<T>> backgroundTask)
        {
            return Task.Run(backgroundTask);
        }

        /// <inheritdoc/>
        public Task<T> RunInUiThreadAsync<T>(Func<T> backgroundTask)
        {
            return Task.Run(backgroundTask);
        }
    }
}
