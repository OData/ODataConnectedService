//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceFileHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.FileHandling;
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

        // Cache the OData Client version to avoid multiple project references enumeration
        private Version odataClientVersion = null;
        private bool isOdataClientVersionCached = false;

        public Project Project { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="ConnectedServiceFileHandler"/>
        /// </summary>
        /// <param name="context">The <see cref="ConnectedServiceHandlerContext"/ object></param>
        /// <param name="project">An object of the project.</param>
        /// <param name="threadHelper">A thread helper that marshals the thread to the correct thread.</param>
        public ConnectedServiceFileHandler(ConnectedServiceHandlerContext context, Project project, IThreadHelper threadHelper)
        {
            this.Context = context;
            this.Project = project;
            this.threadHelper = threadHelper;
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
            => this.CheckODataClientVersionAsync(version => version >= Version.Parse("9.0.0") || version >= Version.Parse("9.0.0.0"));

        /// <summary>
        /// Checks if the Microsoft.OData.Client reference meets a version condition.
        /// </summary>
        /// <param name="versionPredicate">A predicate to evaluate against the OData Client version.</param>
        /// <returns>True if the reference exists and meets the version condition; otherwise false.</returns>
        private Task<bool> CheckODataClientVersionAsync(Func<Version, bool> versionPredicate)
        {
            return this.threadHelper.RunInUiThreadAsync(() =>
            {
                if (this.isOdataClientVersionCached)
                {
                    return this.odataClientVersion != null && versionPredicate(this.odataClientVersion);
                }

#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                if (this.Project.Object is VSProject vsProject)
                {
                    foreach (Reference reference in vsProject.References)
                    {
                        if (reference.SourceProject == null &&
                            reference.Name.Equals("Microsoft.OData.Client", StringComparison.Ordinal))
                        {
                            var currentVersion = reference.Version;
                            if (currentVersion.Contains("-"))
                            {
                                currentVersion = currentVersion.Substring(0, currentVersion.IndexOf('-'));
                            }

                            this.odataClientVersion = Version.Parse(currentVersion);
                            this.isOdataClientVersionCached = true;
                            return versionPredicate(this.odataClientVersion);
                        }
                    }
                }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

                this.odataClientVersion = null;
                this.isOdataClientVersionCached = true;
                return false;
            });
        }
    }
}
