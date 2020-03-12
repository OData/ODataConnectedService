﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
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
        public IVsPackageInstaller PackageInstaller { get; private set; }
        public IVsPackageInstallerServices PackageInstallerServices { get; private set; }
        public ConnectedServiceHandlerContext Context { get; private set; }
        public ServiceConfiguration ServiceConfiguration { get; set; }
        public Project Project { get; private set; }
        public string MetadataUri { get; private set; }
        public string ClientNuGetPackageName { get; set; }
        public string ClientDocUri { get; set; }
        protected string GeneratedFileNamePrefix
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.ServiceConfiguration.GeneratedFileNamePrefix)
                    ? Common.Constants.DefaultReferenceFileName : this.ServiceConfiguration.GeneratedFileNamePrefix;
            }
        }

        protected string CurrentAssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(this.GetType().Assembly.Location);
            }
        }

        public BaseCodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext Context, Project project)
        {
            this.Init();

            this.MetadataUri = metadataUri;
            this.Context = Context;
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
                ProjectHelper.GetProjectFullPath(this.Project),
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

                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OData client was added.");
                }
                else
                {
                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, $"Nuget Package \"{nugetPackage}\" for OData client already installed.");
                }
            }
            catch (Exception ex)
            {
                await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Error, $"Nuget Package \"{nugetPackage}\" for OData client not installed. Error: {ex.Message}.");
            }
        }
    }
}
