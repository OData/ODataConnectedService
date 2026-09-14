//-----------------------------------------------------------------------------
// <copyright file="ConnectedServicePackageInstaller.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.Common;
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
        private static readonly ConcurrentDictionary<string, string> InstalledPackageVersions =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

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
            if (PackageInstaller == null)
            {
                await (this.MessageLogger?.WriteMessageAsync(LogMessageCategory.Error, "The packages were not installed. An error occurred during the installation of packages.")).ConfigureAwait(false);
                throw new InvalidOperationException("The Visual Studio NuGet package installer is unavailable.");
            }

            try
            {
                await Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                string[] targetFrameworks = GetTargetFrameworkMonikers(this.Project);
                string packageKey = GetPackageKey(this.Project, packageName);
                string packageVersion = await NuGetPackageVersionResolver.GetLatestCompatibleVersionAsync(
                    packageSource, packageName, targetFrameworks).ConfigureAwait(false);
                await Shell.ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                bool isInstalled = this.IsPackageInstalled(packageName, packageVersion);
                if (!isInstalled)
                {
                    PackageInstaller.InstallPackage(packageSource, this.Project, packageName, packageVersion, false);
                }

                if (packageVersion != null)
                {
                    InstalledPackageVersions[packageKey] = packageVersion;
                }

                string action = isInstalled ? "already installed" : "was added";
                await (this.MessageLogger?.WriteMessageAsync(
                    LogMessageCategory.Information,
                    $"Nuget Package \"{packageName}\" for OData client {action}.")).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await (this.MessageLogger?.WriteMessageAsync(LogMessageCategory.Error, $"Nuget Package \"{packageName}\" for OData client not installed. Error: {ex.Message}.")).ConfigureAwait(false);
                throw;
            }
        }

        private bool IsPackageInstalled(string packageName, string packageVersion)
        {
            Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (this.PackageInstallerServices == null)
            {
                return false;
            }

            return packageVersion == null
                ? this.PackageInstallerServices.IsPackageInstalled(this.Project, packageName)
                : this.PackageInstallerServices.IsPackageInstalledEx(this.Project, packageName, packageVersion);
        }

        internal static string[] GetTargetFrameworkMonikers(Project project)
        {
            string targetFrameworks = GetProjectProperty(project, "TargetFrameworks");
            if (!string.IsNullOrWhiteSpace(targetFrameworks))
            {
                return targetFrameworks
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(framework => framework.Trim())
                    .Where(framework => framework.Length > 0)
                    .ToArray();
            }

            string targetFrameworkMoniker = GetProjectProperty(project, "TargetFrameworkMoniker");
            if (!string.IsNullOrWhiteSpace(targetFrameworkMoniker))
            {
                return new[] { targetFrameworkMoniker };
            }

            string targetFramework = GetProjectProperty(project, "TargetFramework") ?? GetProjectProperty(project, "TargetFrameworkVersion");
            return string.IsNullOrWhiteSpace(targetFramework) ? Array.Empty<string>() : new[] { targetFramework };
        }

        private static string GetProjectProperty(Project project, string propertyName)
        {
            try
            {
                return Convert.ToString(project?.Properties?.Item(propertyName)?.Value, CultureInfo.InvariantCulture);
            }
            catch (ArgumentException) { return null; }
            catch (COMException) { return null; }
        }

        internal static bool TryGetInstalledPackageVersion(Project project, string packageName, out string packageVersion)
        {
            return InstalledPackageVersions.TryGetValue(GetPackageKey(project, packageName), out packageVersion);
        }

        private static string GetPackageKey(Project project, string packageName)
        {
            return $"{project?.FullName}|{packageName}";
        }

    }
}
