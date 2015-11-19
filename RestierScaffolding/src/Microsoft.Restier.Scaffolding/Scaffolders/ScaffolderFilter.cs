// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.Versioning;
    using Microsoft.AspNet.Scaffolding;

    internal static class ScaffolderFilter
    {
        /// <summary>
        /// Verifies whether the project is a CSharp project and a supported version or no version at all is installed of WebApi packages are installed.
        /// </summary>
        public static bool DisplayRestierScaffolders(CodeGenerationContext codeGenerationContext)
        {
            return
                DisplayScaffolders(codeGenerationContext, AssemblyVersions.WebApiAssemblyName, AssemblyVersions.WebApiAssemblyMinVersion, AssemblyVersions.WebApiAssemblyMaxVersion)
                && DisplayScaffolders(codeGenerationContext, AssemblyVersions.ODataRestierAssemblyName, AssemblyVersions.ODataRestierAssemblyMinVersion, AssemblyVersions.ODataRestierAssemblyMaxVersion);
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The target framework string in the project file may not be valid. We don't want to crash if this happens.")]
        private static bool DisplayScaffolders(CodeGenerationContext context, string projectReferenceName, Version minVersion, Version maxExcludedVersion)
        {
            if (IsApplicableProject(context))
            {
                var referenceDetails = IsValidProjectReference(context, projectReferenceName, minVersion, maxExcludedVersion);

                // We want to show when the reference exists and is of supported version
                // or when the reference does not exist.
                return referenceDetails != ReferenceDetails.ReferenceVersionNotSupported;
            }

            return false;
        }

        private static bool IsApplicableProject(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (ProjectLanguage.CSharp == context.ActiveProject.GetCodeLanguage())
            {
                FrameworkName targetFramework = null;

                // GetTargetFramework() may:
                // 1) Throw an exception if TargetFramework string is not valid during the internal parsing.
                // 2) Return null if the active project is not null but the TargetFrameworkMoniker is null.
                //
                // Both of them fall into the case in which the mvc scaffolding does not support the target framework.
                try
                {
                    targetFramework = context.ActiveProject.GetTargetFramework();
                }
                catch
                {
                    return false;
                }

                if (targetFramework != null &&
                    targetFramework.Identifier == ".NETFramework" && targetFramework.Version >= new Version(4, 5))
                {
                    return true;
                }
            }

            return false;
        }

        private static ReferenceDetails IsValidProjectReference(CodeGenerationContext context,
            string projectReferenceName, Version minVersion, Version maxExcludedVersion)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            IEnumerable<string> projectReferences = context.ActiveProject.GetAssemblyReferences();
            foreach (string assemblyPath in projectReferences)
            {
                AssemblyName assemblyName = null;
                try
                {
                    assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
                }
                catch
                {
                    continue;
                }

                if (assemblyName != null &&
                    String.Equals(assemblyName.Name, projectReferenceName, StringComparison.Ordinal))
                {
                    if (assemblyName.Version >= minVersion &&
                        assemblyName.Version <= maxExcludedVersion)
                    {
                        return ReferenceDetails.ReferenceVersionSupported;
                    }
                    else
                    {
                        return ReferenceDetails.ReferenceVersionNotSupported;
                    }
                }
            }
            return ReferenceDetails.ReferenceDoesNotExist;
        }

        private enum ReferenceDetails
        {
            ReferenceDoesNotExist,

            ReferenceVersionNotSupported,

            ReferenceVersionSupported
        }
    }
}
