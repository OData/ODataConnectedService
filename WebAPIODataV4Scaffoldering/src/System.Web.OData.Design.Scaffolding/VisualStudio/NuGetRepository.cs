// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.Contracts;
using System.IO;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.NuGet;

namespace System.Web.OData.Design.Scaffolding.VisualStudio
{
    [Export(typeof(INuGetRepository))]
    internal class NuGetRepository : INuGetRepository
    {
        //private const string WSRNuGetPackagesRegistryKeyName = "AspNetWebFrameworksAndTools5";
        //private static readonly NuGetRegistryRepository _repository = new NuGetRegistryRepository(WSRNuGetPackagesRegistryKeyName, isPreUnzipped: true);
        private static readonly string Source = Path.GetDirectoryName(typeof(TemplateSearchDirectories).Assembly.Location);
        private static readonly NuGetSourceRepository Repository = new NuGetSourceRepository(Source);

        public NuGetPackage GetPackage(CodeGenerationContext context, string id)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            // There's no error handling here because these errors are of the 'your install is broken'
            // variety.
            string version = GetPackageVersion(context, id);
            Contract.Assert(!String.IsNullOrEmpty(version));

            return new NuGetPackage(id, version, Repository);
        }

        /// <summary>
        /// This function returns the package version corresponding to the specified package id.
        /// </summary>
        /// <param name="context">The <see cref="CodeGenerationContext"/> provided by core scaffolding.</param>
        /// <param name="id">The package id.</param>
        /// <returns>The package version if the package id is present in the package versions file, else returns null.</returns>
        public string GetPackageVersion(CodeGenerationContext context, string id)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            IDictionary<string, string> packageVersions = PackageVersions.GetPackageVersions(context);

            string value;
            packageVersions.TryGetValue(id, out value);

            return value;
        }
    }
}
