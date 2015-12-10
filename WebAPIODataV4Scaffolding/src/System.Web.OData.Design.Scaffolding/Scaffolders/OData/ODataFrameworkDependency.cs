// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web.OData.Design.Scaffolding.Telemetry;
using System.Web.OData.Design.Scaffolding.VisualStudio;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.NuGet;

namespace System.Web.OData.Design.Scaffolding
{
    [Export]
    public class ODataFrameworkDependency : IFrameworkDependency
    {
        [ImportingConstructor]
        public ODataFrameworkDependency(INuGetRepository repository, IVisualStudioIntegration visualStudioIntegration)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (visualStudioIntegration == null)
            {
                throw new ArgumentNullException("visualStudioIntegration");
            }

            Repository = repository;
            VisualStudioIntegration = visualStudioIntegration;
        }

        protected IVisualStudioIntegration VisualStudioIntegration
        {
            get;
            private set;
        }

        protected INuGetRepository Repository
        {
            get;
            private set;
        }

        public bool IsDependencyInstalled(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            bool isODataAssemblyReferenced = ProjectReferences.IsAssemblyReferenced(context.ActiveProject, AssemblyVersions.ODataAssemblyName);

            // It is possible that this function could be called multiple times to check the status of dependency installation.
            // Hence, updating the value of any previously stored state with the most recent status of the referenced assembly.
            context.Items[ContextKeys.IsODataAssemblyReferencedKey] = isODataAssemblyReferenced;

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            bool isWebApiAssemblyReferenced = ProjectReferences.IsAssemblyReferenced(context.ActiveProject, AssemblyVersions.WebApiAssemblyName);

            // It is possible that this function could be called multiple times to check the status of dependency installation.
            // Hence, updating the value of any previously stored state with the most recent status of the referenced assembly.
            context.Items[ContextKeys.IsWebApiAssemblyReferencedKey] = isWebApiAssemblyReferenced;

            return isWebApiAssemblyReferenced && isODataAssemblyReferenced;
        }

        public IEnumerable<NuGetPackage> GetRequiredPackages(CodeGenerationContext context)
        {
            return NuGetPackages.WebApiPackageSet.Select(id => Repository.GetPackage(context, id))
                   .Concat(NuGetPackages.ODataPackageSet.Select(id => Repository.GetPackage(context, id)));
        }

        public FrameworkDependencyStatus EnsureDependencyInstalled(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            ODataDependencyInstaller dependencyInstaller = new ODataDependencyInstaller(context, VisualStudioIntegration);
            return dependencyInstaller.Install();
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "editor", Justification = "This is called for a side-effect.")]
        public void UpdateConfiguration(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            Project project = context.ActiveProject;
            string webConfigPath = Path.Combine(project.GetFullPath(), CommonFilenames.WebConfig);

            IEditorInterfaces editor = VisualStudioIntegration.Editor.GetOrOpenDocument(webConfigPath);
        }

        /// <summary>
        /// Returns true if the version of "Microsoft.AspNet.WebApi.OData" package (OData V3 package)
        /// is less than 5.2.0. Otherwise, returns true. Used for generating code differently for those cases.
        /// </summary>
        public bool IsODataLegacy(CodeGenerationContext context)
        {
            string odataPackageVersionString = Repository.GetPackageVersion(context, NuGetPackages.ODataNuGetPackageId);
            Contract.Assert(odataPackageVersionString != null);

            Version odataPackageVersion;
            bool result = SemanticVersionParser.TryParse(odataPackageVersionString, out odataPackageVersion);
            Contract.Assert(result);

            return odataPackageVersion < new Version(5, 2, 0);
        }

        /// <summary>
        /// OData scaffolders are only supported for C# Projects without a reference to a previous version of WebAPI or OData
        /// </summary>
        public bool IsSupported(CodeGenerationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return ScaffolderFilter.DisplayWebApiScaffolders(context)
                   && ScaffolderFilter.DisplayODataScaffolders(context);
        }

        public void RecordControllerTelemetryOptions(CodeGenerationContext context, ControllerScaffolderModel model)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            WebApiControllerScaffolderOptions options = WebApiControllerScaffolderOptions.CreatedController;

            if (model.IsAsyncSelected)
            {
                options |= WebApiControllerScaffolderOptions.IsAsyncSelected;
            }

            context.AddTelemetryData(TelemetrySharedKeys.WebApiControllerScaffolderOptions, (uint)options);
        }
    }
}
