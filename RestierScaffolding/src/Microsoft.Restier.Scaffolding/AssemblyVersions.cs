// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Restier.Scaffolding.Versions;

    internal static class AssemblyVersions
    {
        private const string AssemblyVersionsFile = @"Templates\AssemblyVersions.xml";
        private const string AssemblyElementXPath = "/Assemblies/Assembly";

        public const string WebApiAssemblyName = "System.Web.Http";
        public const string ODataAssemblyName = "System.Web.OData";
        public const string ODataRestierAssemblyName = "Microsoft.Restier.Core";
        public const string EntityFrameworkAssemblyName = "EntityFramework";

        public static readonly Version WebApiAssemblyMinVersion = new Version(5, 2, 2);
        public static readonly Version WebApiAssemblyMaxVersion = new Version(6, 0, 0, 0);
        public static readonly Version ODataRestierAssemblyMinVersion = new Version(0, 4, 0, 0);
        public static readonly Version ODataRestierAssemblyMaxVersion = new Version(2, 0, 0, 0);

        private static IDictionary<string, string> Versions
        {
            get;
            set;
        }

        /// <summary>
        /// This function retrieves the latest version information from the build file for a provided assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>The latest version information for the specified assembly from the build file. If the version information for the
        /// specified assembly is not present in the assembly build file, then the function returns null.</returns>
        public static Version GetLatestAssemblyVersion(string assemblyName)
        {
            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }

            if (Versions == null)
            {
                Versions = VersionFileReader.GetVersions(AssemblyVersionsFile, AssemblyElementXPath);
                if (Versions == null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Resources.VersionsFileMissing, AssemblyVersionsFile));
                }
            }

            string assemblyVersion;
            Versions.TryGetValue(assemblyName, out assemblyVersion);

            Version version;
            if (!Version.TryParse(assemblyVersion, out version))
            {
                return null;
            }

            return version;
        }
    }
}
