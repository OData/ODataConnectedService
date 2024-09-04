﻿//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceFileHandler.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
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

        public Project Project { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="ConnectedServiceFileHandler"/>
        /// </summary>
        /// <param name="context">The <see cref="ConnectedServiceHandlerContext"/ object></param>
        /// <param name="project">An object of the project.</param>
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
        public async Task<string> AddFileAsync(string fileName, string targetPath, ODataFileOptions oDataFileOptions)
        {
            if (oDataFileOptions != null)
            {
                return await this.Context.HandlerHelper.AddFileAsync(fileName, targetPath, new AddFileOptions { SuppressOverwritePrompt = oDataFileOptions.SuppressOverwritePrompt, OpenOnComplete = oDataFileOptions.OpenOnComplete }).ConfigureAwait(true);
            }
            else
            {
                return await this.Context.HandlerHelper.AddFileAsync(fileName, targetPath).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Sets the CSDL file as an embedded resource
        /// </summary>
        /// <param name="fileName">The name of the file to set as embedded resource</param>
        public async Task SetFileAsEmbeddedResourceAsync(string fileName)
        {
            await this.threadHelper.RunInUiThreadAsync(() =>
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                if (Package.GetGlobalService(typeof(DTE)) is DTE dte)
                {
                    var projectItem = this.Project.ProjectItems.Item("Connected Services").ProjectItems.Item(((ODataConnectedServiceInstance)this.Context.ServiceInstance).ServiceConfig.ServiceName).ProjectItems.Item(fileName);
                    projectItem.Properties.Item("BuildAction").Value = prjBuildAction.prjBuildActionEmbeddedResource;
                    return true;
                }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
                return false;
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// Sets the container property attribute to either true or false
        /// </summary>
        /// <returns>A value of either true or false</returns>
        public async Task<bool> EmitContainerPropertyAttributeAsync()
        {
           return await threadHelper.RunInUiThreadAsync(() =>
            {
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
                if (this.Project.Object is VSProject vsProject)
                {
                    foreach (Reference reference in vsProject.References)
                    {
                        if (reference.SourceProject == null)
                        {
                            // Assembly reference (For project reference, SourceProject != null)
                            if (reference.Name.Equals("Microsoft.OData.Client", StringComparison.Ordinal))
                            {
                                return Version.Parse(reference.Version) > Version.Parse("7.6.4.0");
                            }
                        }
                    }
                }
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

                return false;
            }).ConfigureAwait(true);
        }
    }
}
