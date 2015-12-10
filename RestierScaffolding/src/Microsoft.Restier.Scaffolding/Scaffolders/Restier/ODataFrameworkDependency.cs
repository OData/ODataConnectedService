// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using EnvDTE;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.AspNet.Scaffolding.NuGet;
    using Microsoft.Restier.Scaffolding.VisualStudio;

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

            bool isODataRestierAssemblyReferenced = ProjectReferences.IsAssemblyReferenced(context.ActiveProject, AssemblyVersions.ODataRestierAssemblyName);
            context.Items[ContextKeys.IsODataRestierAssemblyReferencedKey] = isODataRestierAssemblyReferenced;

            bool isEntityFrameworkAssemblyReferenced = ProjectReferences.IsAssemblyReferenced(context.ActiveProject, AssemblyVersions.EntityFrameworkAssemblyName);
            context.Items[ContextKeys.IsEntityFrameworkAssemblyReferencedKey] = isEntityFrameworkAssemblyReferenced;

            return isODataRestierAssemblyReferenced && isEntityFrameworkAssemblyReferenced;
        }

        public IEnumerable<NuGetPackage> GetRequiredPackages(CodeGenerationContext context)
        {
            return NuGetPackages.WebApiPackageSet.Select(id => Repository.GetPackage(context, id))
                   .Concat(NuGetPackages.RestierPackageSet.Select(id => Repository.GetPackage(context, id)));
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
            string odataPackageVersionString = Repository.GetPackageVersion(context, NuGetPackages.ODataWebApiNuGetPackageId);
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

            return ScaffolderFilter.DisplayRestierScaffolders(context);
        }

        public void RecordControllerTelemetryOptions(CodeGenerationContext context, ConfigScaffolderModel model)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            //WebApiControllerScaffolderOptions options = WebApiControllerScaffolderOptions.CreatedController;

            //if (model.IsAsyncSelected)
            //{
            //    options |= WebApiControllerScaffolderOptions.IsAsyncSelected;
            //}

            //context.AddTelemetryData(TelemetrySharedKeys.WebApiControllerScaffolderOptions, (uint)options);
        }
    }
}
