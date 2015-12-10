// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.NuGet;

namespace Microsoft.Restier.Scaffolding
{
    public interface IFrameworkDependency
    {
        /// <summary>
        /// Checks if code generators for this framework are supported, returning true if they are.
        /// </summary>
        /// <param name="context">The CodeGenerationContext provided by the core scaffolder.</param>
        /// <returns>True if the framework is supported for this operation, otherwise false.</returns>
        bool IsSupported(CodeGenerationContext context);

        /// <summary>
        /// Checks if the framework dependency is already installed, returning true if it is.
        /// </summary>
        /// <param name="context">The CodeGenerationContext provided by the core scaffolder.</param>
        /// <returns>True if the framework dependendency is already installed for the Active Project.</returns>
        bool IsDependencyInstalled(CodeGenerationContext context);

        /// <summary>
        /// Gets the required NuGet packages to install this framework dependency.
        /// </summary>
        /// <param name="context">The CodeGenerationContext provided by the core scaffolder.</param>
        /// <returns>A set of NuGetPackage for required NuGet packages.</returns>
        /// <remarks>
        /// This method will only be called if IsDependencyInstalled returns true.
        /// </remarks>
        IEnumerable<NuGetPackage> GetRequiredPackages(CodeGenerationContext context);

        /// <summary>
        /// Attempts to install the framework, returning a status object representing the result.
        /// </summary>
        /// <param name="context">The CodeGenerationContext provided by the core scaffolder.</param>
        /// <returns>A status object.</returns>
        /// <remarks>
        /// This method will only be called if IsDependencyInstalled returns true.
        /// 
        /// EnsureDependencyInstalled will be called prior to attempting to scaffold the targeted asset.
        /// </remarks>
        FrameworkDependencyStatus EnsureDependencyInstalled(CodeGenerationContext context);

        /// <summary>
        /// Updates the project configuration.
        /// </summary>
        /// <param name="context">The CodeGenerationContext provided by the core scaffolder.</param>
        /// <remarks>
        /// This method will only be called if IsDependencyInstalled returns true.
        ///
        /// UpdateConfiguration will be called after successful scaffolding of the targeted asset.
        /// </remarks>
        void UpdateConfiguration(CodeGenerationContext context);

        /// <summary>
        /// Records telemetry information for the scaffolder.
        /// </summary>
        /// <param name="context">The CodeGenerationContext provided by the core scaffolder.</param>
        /// <param name="model">The model object.</param>
        void RecordControllerTelemetryOptions(CodeGenerationContext context, ConfigScaffolderModel model);
    }
}
