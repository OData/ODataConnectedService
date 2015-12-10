// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using VSLangProj;

namespace System.Web.OData.Design.Scaffolding
{
    internal static class ProjectReferences
    {
        public static bool IsAssemblyReferenced(Project activeProject, string assemblyReferenceName)
        {
            if (activeProject == null)
            {
                throw new ArgumentNullException("activeProject");
            }

            if (assemblyReferenceName == null)
            {
                throw new ArgumentNullException("assemblyReferenceName");
            }

            Reference assemblyReference = activeProject.GetAssemblyReference(assemblyReferenceName);
            return assemblyReference != null;
        }

        /// <summary>
        /// Returns the assembly version for the specified assembly from the project system. If the assembly is not referenced, returns the
        /// latest version of the assembly from the version build file.
        /// </summary>
        /// <param name="context">The <see cref="CodeGenerationContext"/></param>
        /// <param name="assemblyName">The name of the assembly.</param>
        /// <returns>The project reference <see cref="Version"/> of the specified assembly. If the assembly is not referenced, then the function
        /// returns the latest version of the assembly from the version build file. If the latest version is not available, returns null.</returns>
        public static Version GetAssemblyVersion(Project activeProject, string assemblyName)
        {
            if (activeProject == null)
            {
                throw new ArgumentNullException("activeProject");
            }

            if (assemblyName == null)
            {
                throw new ArgumentNullException("assemblyName");
            }

            Reference assemblyReference = activeProject.GetAssemblyReference(assemblyName);
            if ((assemblyReference != null) && (assemblyReference.Version != null))
            {
                return new Version(assemblyReference.Version);
            }

            // If there's no reference to the assembly, then this is a first time scaffolding scenario, we'll be adding a reference to the current version from the build file.
            Version currentVersion = AssemblyVersions.GetLatestAssemblyVersion(assemblyName);

            return currentVersion;
        }
    }
}
