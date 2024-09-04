//-----------------------------------------------------------------------------
// <copyright file="IFileHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.OData.CodeGen.FileHandling
{
    /// <summary>
    /// Interface for file handling
    /// </summary>
    public interface IFileHandler
    {
        /// <summary>
        /// Adds a file to a target path.
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="targetPath">The path target where you want to copy a file to </param>
        /// <param name="oDataFileOptions">The options to use when adding a file to a target path.</param>
        Task<string> AddFileAsync(string fileName, string targetPath, ODataFileOptions oDataFileOptions = null);
        
        /// <summary>
        /// Sets a file as an embedded resource
        /// </summary>
        /// <param name="fileName">The name of the file to set as an embedded resource</param>
        Task SetFileAsEmbeddedResourceAsync(string fileName);

        /// <summary>
        /// Emits container property attribute
        /// </summary>
        /// <returns></returns>
        Task<bool> EmitContainerPropertyAttributeAsync();
    }
}
