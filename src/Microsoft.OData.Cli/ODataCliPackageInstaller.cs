//-----------------------------------------------------------------------------
// <copyright file="ODataCliPackageInstaller.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.OData.Cli.PackageInstallers;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.PackageInstallation;

namespace Microsoft.OData.Cli
{
    /// <summary>
    /// An implementation of the <see cref="IPackageInstaller"./>
    /// </summary>
    public class ODataCliPackageInstaller : IPackageInstaller
    {
        private Project project;
        private IMessageLogger messageLogger;

        /// <summary>
        /// Creates an instance of <see cref="ODataCliPackageInstaller"/> 
        /// </summary>
        /// <param name="project">The currently loaded <see cref="Project"/></param>
        /// <param name="messageLogger">The <see cref="IMessageLogger"/> object to use for logging.</param>
        public ODataCliPackageInstaller(Project project, IMessageLogger messageLogger)
        {
            this.project = project;
            this.messageLogger = messageLogger;
        }

        /// <summary>
        /// Installs Nuget Packages to a project.
        /// </summary>
        /// <param name="packageSource">The source of the package or where the package will be installed from. </param>
        /// <param name="packageName">The name of the package to install. </param>
        /// <returns>Returns a Completed Task on successfully installing the package.</returns>
        public async Task CheckAndInstallNuGetPackageAsync(string packageSource, string packageName)
        {
            if (this.project == null)
            {
                return;
            }

            PackageInstallerHelpers packageInstallerHelper = new PackageInstallerHelpers(this.project, packageSource, this.messageLogger);
            string[] projectTargetFrameworks = this.project.GetProjectTargetFrameworks();
            foreach (string projectTargetFramework in projectTargetFrameworks)
            {
                if (projectTargetFramework.Contains("net4"))
                {
                    await packageInstallerHelper.InstallPackagesOnDotNetV4FrameworkProjects(packageName, projectTargetFramework);
                }
                else
                {
                    await packageInstallerHelper.InstallPackagesOnDotNetCoreFrameworks(packageName, this.messageLogger, projectTargetFramework);
                }
            }
        }
    }
}
