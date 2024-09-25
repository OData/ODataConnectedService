//-----------------------------------------------------------------------------
// <copyright file="IThreadHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Microsoft.OData.ConnectedService.Threading
{
    /// <summary>
    /// A Thread helper to assist users to marshal certain work in certain threads.
    /// </summary>
    public interface IThreadHelper
    {
        /// <summary>
        /// Runs the block provided in the foreground UI thread.
        /// </summary>
        /// <typeparam name="T">Return type generic type parameter.</typeparam>
        /// <param name="foregroundTask">A func representing the foreground task.</param>
        /// <returns>A task respresenting the completion of the task.</returns>
        Task<T> RunInUiThreadAsync<T>(Func<T> foregroundTask);
    }
}
