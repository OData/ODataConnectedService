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
        public IVsPackageInstallerServices PackageInstallerServices { get; protected set; }

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
                this.PackageInstallerServices = componentModel.GetService<IVsPackageInstallerServices>();
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
            if (PackageInstallerServices != null)
            {
                try
                {
                    if (!PackageInstallerServices.IsPackageInstalled(this.Project, packageName))
                    {
                        Version packageVersion = null;
                        PackageInstaller.InstallPackage(packageSource, this.Project, packageName, packageVersion, false);

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
    }
}
