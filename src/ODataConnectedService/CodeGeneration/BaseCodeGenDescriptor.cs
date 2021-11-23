//-----------------------------------------------------------------------------
// <copyright file="BaseCodeGenDescriptor.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Data.Services.Design;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Shell = Microsoft.VisualStudio.Shell;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    internal abstract class BaseCodeGenDescriptor
    {
        public IVsPackageInstaller PackageInstaller { get; protected set; }
        public IVsPackageInstallerServices PackageInstallerServices { get; protected set; }
        public ConnectedServiceHandlerContext Context { get; private set; }
        public ServiceConfiguration ServiceConfiguration { get; set; }
        public Project Project { get; private set; }
        public string MetadataUri { get; private set; }
        public string ClientNuGetPackageName { get; set; }
        public string ClientDocUri { get; set; }
        protected string GeneratedFileNamePrefix =>
            string.IsNullOrWhiteSpace(this.ServiceConfiguration.GeneratedFileNamePrefix)
                ? Common.Constants.DefaultReferenceFileName : this.ServiceConfiguration.GeneratedFileNamePrefix;

        protected string CurrentAssemblyPath => Path.GetDirectoryName(this.GetType().Assembly.Location);
        
        protected LanguageOption TargetProjectLanguage => this.Project.GetLanguageOption();

        protected BaseCodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
        {
            this.Init();

            this.MetadataUri = metadataUri;
            this.Context = context;
            this.Project = project;
            this.ServiceConfiguration = ((ODataConnectedServiceInstance)this.Context.ServiceInstance).ServiceConfig;
        }

        protected virtual void Init()
        {
            var componentModel = (IComponentModel)Shell.Package.GetGlobalService(typeof(SComponentModel));
            this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
            this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
        }

        public abstract Task AddNugetPackagesAsync();
        public abstract Task AddGeneratedClientCodeAsync();

        protected string GetReferenceFileFolder()
        {
            var serviceReferenceFolderName = this.Context.HandlerHelper.GetServiceArtifactsRootFolder();

            var referenceFolderPath = Path.Combine(
                this.Project.GetFullPath(),
                serviceReferenceFolderName,
                this.Context.ServiceInstance.Name);

            return referenceFolderPath;
        }

        internal async Task CheckAndInstallNuGetPackageAsync(string packageSource, string nugetPackage)
        {
            try
            {
                if (!PackageInstallerServices.IsPackageInstalled(this.Project, nugetPackage))
                {
                    Version packageVersion = null;
                    PackageInstaller.InstallPackage(packageSource, this.Project, nugetPackage, packageVersion, false);

                    await (Context.Logger?.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OData client was added.")).ConfigureAwait(false);
                }
                else
                {
                    await (Context.Logger?.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OData client already installed.")).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                await (Context.Logger?.WriteMessageAsync(LoggerMessageCategory.Error, $"Nuget Package \"{nugetPackage}\" for OData client not installed. Error: {ex.Message}.")).ConfigureAwait(false);
            }
        }
    }
}