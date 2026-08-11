//-----------------------------------------------------------------------------
// <copyright file="ConnectedServicePackageInstaller.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.PackageInstallation;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using NuGet.VisualStudio;
using Shell = Microsoft.VisualStudio.Shell;
#if VS2022PLUS
using System.Linq;
using System.Threading;
using Microsoft.ServiceHub.Framework;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.ServiceBroker;
using NuGet.VisualStudio.Contracts;
#endif


namespace Microsoft.OData.ConnectedService
{
    /// <summary>
    /// An implementation of the <see cref="IPackageInstaller"./>
    /// </summary>
    public class ConnectedServicePackageInstaller : IPackageInstaller
    {
        public ConnectedServiceHandlerContext Context { get; private set; }
        public Project Project { get; private set; }
        public IMessageLogger MessageLogger { get; private set; }
        public IVsPackageInstaller PackageInstaller { get; protected set; }

#if !VS2022PLUS
        public IVsPackageInstallerServices PackageInstallerServices { get; protected set; }
#endif

        /// <summary>
        /// Creates an instance of <see cref="ConnectedServicePackageInstaller"/> 
        /// </summary>
        /// <param name="context">A <see cref="ConnectedServiceHandlerContext"/> objetc</param>
        /// <param name="project">The project.</param>
        /// <param name="messageLogger">A message logger.</param>
        public ConnectedServicePackageInstaller(ConnectedServiceHandlerContext context, Project project, IMessageLogger messageLogger)
        {
            this.Init();
            this.Context = context;
            this.Project = project;
            this.MessageLogger = messageLogger;
        }

        /// <summary>
        /// Initializes the package installer services
        /// </summary>
        public void Init()
        {
            var componentModel = (IComponentModel)Shell.Package.GetGlobalService(typeof(SComponentModel));
            if (componentModel != null)
            {
#if !VS2022PLUS
                this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
#endif
                this.PackageInstaller = componentModel.GetService<IVsPackageInstaller>();
            }
        }

        /// <summary>
        /// Checks and installs nuget packages in the project
        /// </summary>
        /// <param name="packageSource">The source of the package</param>
        /// <param name="packageName">The name of the package to be installed</param>
        public async Task CheckAndInstallNuGetPackageAsync(string packageSource, string packageName)
        {
            if (PackageInstaller != null)
            {
                try
                {
                    if (!await this.IsPackageInstalledAsync(packageName).ConfigureAwait(false))
                    {
                        PackageInstaller.InstallPackage(packageSource, this.Project, packageName, (string)null, false);

                        await (this.MessageLogger?.WriteMessageAsync(LogMessageCategory.Information, $"Nuget Package \"{packageName}\" for OData client was added.")).ConfigureAwait(false);
                    }
                    else
                    {
                        await (this.MessageLogger?.WriteMessageAsync(LogMessageCategory.Information, $"Nuget Package \"{packageName}\" for OData client already installed.")).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    await (this.MessageLogger?.WriteMessageAsync(LogMessageCategory.Error, $"Nuget Package \"{packageName}\" for OData client not installed. Error: {ex.Message}.")).ConfigureAwait(false);
                }
            }
            else
            {
                await (this.MessageLogger?.WriteMessageAsync(LogMessageCategory.Error, $"The packages were not installed. An error occurred during the installation of packages.")).ConfigureAwait(false);
            }
        }

#if VS2022PLUS
        /// <summary>
        /// Determines whether the specified package is installed in the project using the brokered
        /// <see cref="INuGetProjectService"/>, which supersedes the obsolete <c>IVsPackageInstallerServices</c> API.
        /// </summary>
        /// <param name="packageName">The package id to look for.</param>
        /// <returns>True if the package is installed; otherwise false.</returns>
        private async Task<bool> IsPackageInstalledAsync(string packageName)
        {
            await Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            Guid projectGuid = this.TryGetProjectGuid();
            IBrokeredServiceContainer serviceContainer = Shell.Package.GetGlobalService(typeof(SVsBrokeredServiceContainer)) as IBrokeredServiceContainer;

            if (projectGuid == Guid.Empty || serviceContainer == null)
            {
                return false;
            }

            IServiceBroker serviceBroker = serviceContainer.GetFullAccessServiceBroker();
            INuGetProjectService nugetProjectService = await serviceBroker
                .GetProxyAsync<INuGetProjectService>(NuGetServices.NuGetProjectServiceV1)
                .ConfigureAwait(false);

            try
            {
                if (nugetProjectService == null)
                {
                    return false;
                }

                InstalledPackagesResult result = await nugetProjectService
                    .GetInstalledPackagesAsync(projectGuid, CancellationToken.None)
                    .ConfigureAwait(false);

                return result != null &&
                    result.Status == InstalledPackageResultStatus.Successful &&
                    result.Packages != null &&
                    result.Packages.Any(p => p.Id.Equals(packageName, StringComparison.OrdinalIgnoreCase));
            }
            finally
            {
                (nugetProjectService as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Resolves the project GUID required by <see cref="INuGetProjectService"/>. Must be called on the UI thread.
        /// </summary>
        /// <returns>The project GUID, or <see cref="Guid.Empty"/> when it could not be resolved.</returns>
        private Guid TryGetProjectGuid()
        {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            if (Shell.Package.GetGlobalService(typeof(SVsSolution)) is IVsSolution solution &&
                solution.GetProjectOfUniqueName(this.Project.UniqueName, out IVsHierarchy hierarchy) == VSConstants.S_OK &&
                hierarchy != null &&
                hierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid projectGuid) == VSConstants.S_OK)
            {
                return projectGuid;
            }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

            return Guid.Empty;
        }
#else
        /// <summary>
        /// Determines whether the specified package is installed in the project using <c>IVsPackageInstallerServices</c>.
        /// </summary>
        /// <param name="packageName">The package id to look for.</param>
        /// <returns>True if the package is installed; otherwise false.</returns>
        private Task<bool> IsPackageInstalledAsync(string packageName)
        {
            return Task.FromResult(this.PackageInstallerServices != null &&
                this.PackageInstallerServices.IsPackageInstalled(this.Project, packageName));
        }
#endif
    }
}
