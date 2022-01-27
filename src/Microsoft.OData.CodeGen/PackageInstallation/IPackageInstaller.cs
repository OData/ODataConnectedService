//-----------------------------------------------------------------------------
// <copyright file="IPackageInstaller.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.OData.CodeGen.PackageInstallation
{
    /// <summary>
    /// Package installation interface
    /// </summary>
    public interface IPackageInstaller
    {
        /// <summary>
        /// Checks and installs a nuget package to the project 
        /// </summary>
        /// <param name="packageSource">The package source to be installed.</param>
        /// <param name="packageName">The name of the package to install.</param>
        Task CheckAndInstallNuGetPackageAsync(string packageSource, string packageName);
    }
}
