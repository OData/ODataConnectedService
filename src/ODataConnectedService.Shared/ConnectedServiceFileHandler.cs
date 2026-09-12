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
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using VSLangProj;
using Task = System.Threading.Tasks.Task;

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
        public async Task<string> AddFileAsync(string fileName, string targetPath, ODataFileOptions oDataFileOptions)
        {
            Task<string> addFileTask = await this.threadHelper.RunInUiThreadAsync(() =>
                oDataFileOptions != null
                    ? this.Context.HandlerHelper.AddFileAsync(fileName, targetPath, new AddFileOptions { SuppressOverwritePrompt = oDataFileOptions.SuppressOverwritePrompt, OpenOnComplete = oDataFileOptions.OpenOnComplete })
                    : this.Context.HandlerHelper.AddFileAsync(fileName, targetPath));
            return await addFileTask.ConfigureAwait(false);
        }

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
                if (this.packagesProvider == null)
                {
                    string packageVersion = await this.threadHelper.RunInUiThreadAsync(() =>
                    {
                        ConnectedServicePackageInstaller.TryGetInstalledPackageVersion(
                            this.Project,
                            Microsoft.OData.CodeGen.Common.Constants.V4ClientNuGetPackage,
                            out string installedVersion);
                        return installedVersion;
                    });
                    this.isOdataClientPackageInstalled =
                        ODataClientVersionChecker.TryParseVersion(packageVersion, out this.odataClientVersion);
                }
                else
                {
                    IReadOnlyList<InstalledPackageInfo> installedPackages =
                        await this.packagesProvider.GetInstalledPackagesAsync().ConfigureAwait(false);

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
