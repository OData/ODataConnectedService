// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.Restier.Scaffolding.Versions;
    using Microsoft.VisualStudio.ComponentModelHost;
    using NuGet.VisualStudio;

    internal class PackageVersions
    {
        private const string PackageVersionsFile = @"Templates\PackageVersions{0}.xml";
        private const string PackageElementXPath = "/Packages/Package";

        // This should be updated whenever we ship a new set of packages.
        private static readonly Version _latestKnownPackageVersion = new Version(5, 2, 3);

        private static IDictionary<string, IDictionary<string, string>> PackageFileContents
        {
            get;
            set;
        }

        /// <summary>
        /// This function retrieves the package versions from the package version file based on the project references.
        /// </summary>
        /// <param name="context">The <see cref="CodeGenerationContext"/> provided by core scaffolding.</param>
        /// <returns>A <see cref="Dictionary"/> containing package id as the key and version as the value.</returns>
        public static IDictionary<string, string> GetPackageVersions(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IDictionary<string, string> packageVersions;

            string packageVersionsFile = GetPackageReferenceFileName(context);
            Contract.Assert(packageVersionsFile != null);

            // We are storing the computed package versions in the packageFileContents to avoid multiple calls to the file system.
            if (PackageFileContents == null)
            {
                PackageFileContents = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                PackageFileContents.TryGetValue(packageVersionsFile, out packageVersions);

                if (packageVersions != null)
                {
                    return packageVersions;
                }
            }

            packageVersions = VersionFileReader.GetVersions(packageVersionsFile, PackageElementXPath);
            if (packageVersions == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.VersionsFileMissing, packageVersionsFile));
            }

            PackageFileContents[packageVersionsFile] = packageVersions;
            return packageVersions;
        }

        /// <summary>
        /// This function determines the Package version file name based on the project references.
        /// Package version file is the data file that contains the matching versions for nuget packages.
        /// </summary>
        /// <param name="context">>The <see cref="CodeGenerationContext"/> provided by core scaffolding.</param>
        /// <returns>The package version file name to be loaded.</returns>
        private static string GetPackageReferenceFileName(CodeGenerationContext context)
        {
            IEnumerable<IVsPackageMetadata> installedPackages = GetInstalledPackages(context);

            string packageFileName = GetPackageVersionsFileName(_latestKnownPackageVersion);

            GetPackageFileNameForPackage(context, installedPackages,
                packageId: NuGetPackages.WebApiNuGetPackageId,
                assemblyReferenceName: AssemblyVersions.WebApiAssemblyName,
                minSupportedAssemblyReferenceVersion: AssemblyVersions.WebApiAssemblyMinVersion,
                packageFileName: ref packageFileName);

            return packageFileName;
        }

        // The parameter 'latestKnownAssemblyVersion' can be null and if yes will be
        // infered from 'assemblyReferenceName' parameter.
        // I added that as a parameter to make unit testing easier.
        internal static void GetPackageFileNameForPackage(CodeGenerationContext context,
            IEnumerable<IVsPackageMetadata> installedPackages,
            string packageId,
            string assemblyReferenceName,
            Version minSupportedAssemblyReferenceVersion,
            ref string packageFileName)
        {
            if (ProjectReferences.IsAssemblyReferenced(context.ActiveProject, assemblyReferenceName))
            {
                // If the project has reference to MVC version 5.0 or
                // above the corresponding package versions file is loaded from the file system.
                IVsPackageMetadata installedPackage = installedPackages.
                    Where(package => String.Equals(packageId, package.Id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                Version versionForPackageFile;
                // First try to get the package version and if not fallback to assembly reference version.
                if (installedPackage == null || !SemanticVersionParser.TryParse(installedPackage.VersionString, out versionForPackageFile))
                {
                    // Note: In this scenario where user does not have a package
                    // reference but only has assembly reference, we will end up
                    // using 5.1.0 instead of 5.1.1 version of package file (even
                    // if the assembly reference is the same as the one shipped in 5.1.1)
                    // because the assembly version did not change for 5.1.1
                    versionForPackageFile = ProjectReferences.GetAssemblyVersion(context.ActiveProject, assemblyReferenceName);
                }

                // Version should have been either available from package or atleast the assembly reference.
                Contract.Assert(versionForPackageFile != null);

                // For example : If 5.1 is the latest tooling version and if we release just the NuGet packages for a higher version of runtime (may be 5.2),
                // we will still continue to scaffold the latest tooling version (in this case 5.1).
                if (versionForPackageFile >= minSupportedAssemblyReferenceVersion && versionForPackageFile <= _latestKnownPackageVersion)
                {
                    packageFileName = GetPackageVersionsFileName(versionForPackageFile);
                }
            }
        }

        private static string GetPackageVersionsFileName(Version version)
        {
            return String.Format(CultureInfo.InvariantCulture, PackageVersionsFile,
                version.Major + "." + version.Minor + "." + version.Build);
        }

        internal static IEnumerable<IVsPackageMetadata> GetInstalledPackages(CodeGenerationContext context)
        {
            var packageInstallerServices = context.ServiceProvider
                .GetService<IComponentModel, SComponentModel>().GetService<IVsPackageInstallerServices>();

            return packageInstallerServices.GetInstalledPackages(context.ActiveProject);
        }
    }
}
