//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceFileHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.ConnectedService.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using VSLangProj;
using Task = System.Threading.Tasks.Task;
#if VS2022PLUS
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
    /// An implementation of the <see cref="IFileHandler"/>
    /// </summary>
    public class ConnectedServiceFileHandler : IFileHandler
    {
        private ConnectedServiceHandlerContext Context;
        private readonly IThreadHelper threadHelper;
        private readonly IMessageLogger messageLogger;
        private readonly IInstalledPackagesProvider packagesProvider;

        // Cache the OData Client version to avoid multiple installed-package queries.
        private Version odataClientVersion = null;
        private bool isOdataClientVersionCached = false;
        private bool isOdataClientPackageInstalled = false;
        private bool versionResolutionWarningLogged = false;

        public Project Project { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="ConnectedServiceFileHandler"/>
        /// </summary>
        /// <param name="context">The <see cref="ConnectedServiceHandlerContext"/ object></param>
        /// <param name="project">An object of the project.</param>
        /// <param name="threadHelper">A thread helper that marshals the thread to the correct thread.</param>
        public ConnectedServiceFileHandler(ConnectedServiceHandlerContext context, Project project, IThreadHelper threadHelper)
            : this(context, project, threadHelper, new ConnectedServiceMessageLogger(context))
        {
        }

        internal ConnectedServiceFileHandler(
            ConnectedServiceHandlerContext context,
            Project project,
            IThreadHelper threadHelper,
            IMessageLogger messageLogger,
            IInstalledPackagesProvider packagesProvider = null)
        {
            this.Context = context;
            this.Project = project;
            this.threadHelper = threadHelper;
            this.messageLogger = messageLogger;
            this.packagesProvider = packagesProvider;
        }

        /// <summary>
        /// Adds a file to a target path.
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="targetPath">The path target where you want to copy a file to </param>
        /// <param name="oDataFileOptions">The options to use when adding a file to a target path.</param>
        /// <returns>Returns the path to the file that was added</returns>
        public Task<string> AddFileAsync(string fileName, string targetPath, ODataFileOptions oDataFileOptions)
            => oDataFileOptions != null
                ? this.Context.HandlerHelper.AddFileAsync(fileName, targetPath, new AddFileOptions { SuppressOverwritePrompt = oDataFileOptions.SuppressOverwritePrompt, OpenOnComplete = oDataFileOptions.OpenOnComplete })
                : this.Context.HandlerHelper.AddFileAsync(fileName, targetPath);

        /// <summary>
        /// Sets the CSDL file as an embedded resource.
        /// <remark>Since this method may be executed in a background thread this will require to switch to the main thread.</remark>
        /// </summary>
        /// <param name="fileName">The name of the file to set as embedded resource</param>
        public async Task SetFileAsEmbeddedResourceAsync(string fileName)
        {
            await this.threadHelper.RunInUiThreadAsync(() =>
            {
#pragma warning disable VSTHRD010 // This invokes the code in the required main thread.
                if (Package.GetGlobalService(typeof(DTE)) is DTE dte)
                {
                    var projectItem = this.Project.ProjectItems.Item("Connected Services").ProjectItems.Item(((ODataConnectedServiceInstance)this.Context.ServiceInstance).ServiceConfig.ServiceName).ProjectItems.Item(fileName);
                    projectItem.Properties.Item("BuildAction").Value = prjBuildAction.prjBuildActionEmbeddedResource;
                    return true;
                }
#pragma warning restore VSTHRD010 // This invokes the code in the required main thread.
                return false;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets the container property attribute to either true or false
        /// <remark>Since this method may be executed in a background thread this will require to switch to the main thread.</remark>
        /// </summary>
        /// <returns>A value of either true or false</returns>
        public Task<bool> EmitContainerPropertyAttributeAsync()
            => this.CheckODataClientVersionAsync(version => version > Version.Parse("7.6.4.0"));

        /// <summary>
        /// Determines asynchronously whether native date and time types are supported by the connected OData service.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. True if native date and time types are supported; otherwise, false.</returns>
        public Task<bool> EmitNativeDateTimeTypesAsync()
            => this.CheckODataClientVersionAsync(ODataClientVersionChecker.SupportsNativeDateTimeTypes);

        /// <summary>
        /// Checks if the Microsoft.OData.Client reference meets a version condition.
        /// </summary>
        /// <param name="versionPredicate">A predicate to evaluate against the OData Client version.</param>
        /// <returns>True if the reference exists and meets the version condition; otherwise false.</returns>
        private async Task<bool> CheckODataClientVersionAsync(Func<Version, bool> versionPredicate)
        {
            if (!this.isOdataClientVersionCached)
            {
                IReadOnlyList<InstalledPackageInfo> installedPackages =
                    await this.GetInstalledPackagesAsync().ConfigureAwait(false);

                InstalledPackageInfo odataClientPackage = installedPackages?.FirstOrDefault(
                    package => package.Id.Equals(
                        Microsoft.OData.CodeGen.Common.Constants.V4ClientNuGetPackage,
                        StringComparison.OrdinalIgnoreCase));

                this.isOdataClientPackageInstalled = odataClientPackage != null;
                if (odataClientPackage != null &&
                    ODataClientVersionChecker.TryParseVersion(odataClientPackage.Version, out Version parsedVersion))
                {
                    this.odataClientVersion = parsedVersion;
                }

                this.isOdataClientVersionCached = true;
            }

            Version version = this.odataClientVersion;

            if (version == null &&
                this.isOdataClientPackageInstalled &&
                !this.versionResolutionWarningLogged &&
                this.messageLogger != null)
            {
                this.versionResolutionWarningLogged = true;
                await this.messageLogger.WriteMessageAsync(
                    LogMessageCategory.Warning,
                    "Microsoft.OData.Client is installed, but its version could not be resolved. Legacy date and time types will be generated.")
                    .ConfigureAwait(false);
            }

            return version != null && versionPredicate(version);
        }

        /// <summary>
        /// Gets the packages installed in the project, using the injected
        /// <see cref="IInstalledPackagesProvider"/> when one is supplied (for testing) or the
        /// platform-specific NuGet query otherwise.
        /// </summary>
        /// <returns>The installed packages.</returns>
        private Task<IReadOnlyList<InstalledPackageInfo>> GetInstalledPackagesAsync()
        {
            if (this.packagesProvider != null)
            {
                return this.packagesProvider.GetInstalledPackagesAsync();
            }

            return this.ResolveInstalledPackagesAsync();
        }

#if VS2022PLUS
        /// <summary>
        /// Resolves the installed packages using the brokered <see cref="INuGetProjectService"/>,
        /// which supersedes the obsolete <c>IVsPackageInstallerServices</c> API.
        /// </summary>
        /// <returns>The installed packages.</returns>
        private async Task<IReadOnlyList<InstalledPackageInfo>> ResolveInstalledPackagesAsync()
        {
            Guid projectGuid = await this.threadHelper.RunInUiThreadAsync(() => this.TryGetProjectGuid()).ConfigureAwait(false);
            IBrokeredServiceContainer serviceContainer = await this.threadHelper.RunInUiThreadAsync(() =>
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                return Package.GetGlobalService(typeof(SVsBrokeredServiceContainer)) as IBrokeredServiceContainer;
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
            }).ConfigureAwait(false);

            if (projectGuid == Guid.Empty || serviceContainer == null)
            {
                return Array.Empty<InstalledPackageInfo>();
            }

            IServiceBroker serviceBroker = serviceContainer.GetFullAccessServiceBroker();
            INuGetProjectService nugetProjectService = await serviceBroker
                .GetProxyAsync<INuGetProjectService>(NuGetServices.NuGetProjectServiceV1)
                .ConfigureAwait(false);

            try
            {
                if (nugetProjectService == null)
                {
                    return Array.Empty<InstalledPackageInfo>();
                }

                InstalledPackagesResult result = await nugetProjectService
                    .GetInstalledPackagesAsync(projectGuid, CancellationToken.None)
                    .ConfigureAwait(false);

                if (result == null ||
                    result.Status != InstalledPackageResultStatus.Successful ||
                    result.Packages == null)
                {
                    return Array.Empty<InstalledPackageInfo>();
                }

                return result.Packages
                    .Select(package => new InstalledPackageInfo(package.Id, package.Version))
                    .ToList();
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
            if (Package.GetGlobalService(typeof(SVsSolution)) is IVsSolution solution &&
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
        /// Resolves the installed packages using <c>IVsPackageInstallerServices</c>.
        /// </summary>
        /// <returns>The installed packages.</returns>
        private Task<IReadOnlyList<InstalledPackageInfo>> ResolveInstalledPackagesAsync()
        {
            return this.threadHelper.RunInUiThreadAsync<IReadOnlyList<InstalledPackageInfo>>(() =>
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                IVsPackageInstallerServices packageInstallerServices = null;
                if (Package.GetGlobalService(typeof(SComponentModel)) is IComponentModel componentModel)
                {
                    packageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
                }

                if (packageInstallerServices == null)
                {
                    return Array.Empty<InstalledPackageInfo>();
                }

                return packageInstallerServices
                    .GetInstalledPackages(this.Project)
                    .Select(metadata => new InstalledPackageInfo(metadata.Id, metadata.VersionString))
                    .ToList();
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
            });
        }
#endif
    }

    /// <summary>
    /// Provides the packages installed in a project. Abstracts the VS-version-specific NuGet
    /// query API so OData client version resolution can be unit tested.
    /// </summary>
    internal interface IInstalledPackagesProvider
    {
        /// <summary>
        /// Gets the packages installed in the project.
        /// </summary>
        /// <returns>The installed packages.</returns>
        Task<IReadOnlyList<InstalledPackageInfo>> GetInstalledPackagesAsync();
    }

    /// <summary>
    /// Represents an installed NuGet package id and version, independent of the VS-version-specific API.
    /// </summary>
    internal sealed class InstalledPackageInfo
    {
        /// <summary>
        /// Creates an instance of <see cref="InstalledPackageInfo"/>.
        /// </summary>
        /// <param name="id">The package id.</param>
        /// <param name="version">The installed package version string.</param>
        public InstalledPackageInfo(string id, string version)
        {
            this.Id = id;
            this.Version = version;
        }

        /// <summary>Gets the package id.</summary>
        public string Id { get; }

        /// <summary>Gets the installed package version string.</summary>
        public string Version { get; }
    }
}
