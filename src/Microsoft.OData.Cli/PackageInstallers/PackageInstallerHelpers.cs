//-----------------------------------------------------------------------------
// <copyright file="PackageInstallerHelpers.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.OData.CodeGen.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;

namespace Microsoft.OData.Cli.PackageInstallers
{
    /// <summary>
    /// A helper class containing helper methods to use in installing nuget packages 
    /// to projects. Using the Nuget.Client V3 APIs for installing packages to projects targeting
    /// DotNetFramework that has the packages and packages.config folder structure.
    /// For projects that have the projectreference kind of structure using the dotnet cli process to install them. 
    /// link to package documentation: https://docs.microsoft.com/en-us/nuget/reference/nuget-client-sdk
    /// </summary>
    internal class PackageInstallerHelpers
    {
        private readonly Project project;
        private readonly string packageSource;
        private readonly IMessageLogger messageLogger;
        private SourceRepository SourceRepository { get; set; }
        private SourceRepositoryProvider SourceRepositoryProvider { get; set; }
        private string RootPath { get; set; }
        private ISettings DefaultSettings { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="PackageInstallerHelpers"/> class
        /// </summary>
        /// <param name="project">The <see cref="Project"/> object.</param>
        /// <param name="packageSource">The source of the package Ids being installed into the project.</param>
        /// <param name="messageLogger">An <see cref="IMessageLogger"/> to use for logging.</param>
        internal PackageInstallerHelpers(Project project, string packageSource, IMessageLogger messageLogger)
        {
            this.project = project;
            this.packageSource = packageSource;
            this.messageLogger = messageLogger;
            this.Init(this.packageSource);
        }

        /// <summary>
        /// This method initializes some of the common methods or classes that will be used in installing the packages.
        /// </summary>
        /// <param name="packageSource">The package source of the package ids to be installed</param>
        private void Init(string packageSource)
        {
            IEnumerable<Lazy<INuGetResourceProvider>> providers = Repository.Provider.GetCoreV3();
            PackageSource pSource = new PackageSource(packageSource);
            this.SourceRepository = new SourceRepository(pSource, providers);

            //path to packages folder. Create the folder if it does not exist and download the packages there.
            if (ProjectHelper.CheckIfSolutionAndProjectFilesAreInSameFolder(this.project.DirectoryPath))
            {
                this.RootPath = Directory.CreateDirectory(this.project.DirectoryPath + "/packages").FullName;
            }
            else 
            {
                this.RootPath = Directory.CreateDirectory(Directory.GetParent(this.project.DirectoryPath).FullName + "/packages").FullName;
            }
            
            this.DefaultSettings = Settings.LoadDefaultSettings(this.RootPath);
            PackageSourceProvider packageSourceProvider = new PackageSourceProvider(this.DefaultSettings);
            this.SourceRepositoryProvider = new SourceRepositoryProvider(packageSourceProvider, providers);
        }

        /// <summary>
        /// Installs packages on any project that targets the dotnet v4. The packages are added to the packages folder
        /// and the packages.config file and the .csproj file is updated. 
        /// </summary>
        /// <param name="packageId">The package to install</param>
        /// <param name="projectTargetVersion">The version of .net framework that the project targets.</param>
        /// <returns>A completed task of the package installation</returns>
        internal async Task InstallPackagesOnDotNetV4FrameworkProjects(string packageId, string projectTargetVersion)
        {
            using (SourceCacheContext cacheContext = new SourceCacheContext())
            {
                Logger logger = new Logger(this.messageLogger);
                IEnumerable<SourceRepository> repositories = this.SourceRepositoryProvider.GetRepositories();
                HashSet<SourcePackageDependencyInfo> availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                NuGetFramework nuGetFramework = NuGetFramework.ParseFolder(projectTargetVersion?.Split('=')[1]);
                NuGetVersion packageVersion = await GetPackageLatestNugetVersionAsync(packageId, this.SourceRepository, nuGetFramework.DotNetFrameworkName);

                //get all the package dependencies
                await GetPackageDependencies(
                    new PackageIdentity(packageId, packageVersion),
                    nuGetFramework, cacheContext, logger, repositories, availablePackages);

                PackageResolverContext resolverContext = new PackageResolverContext(
                    DependencyBehavior.Lowest,
                    new[] { packageId },
                    Enumerable.Empty<string>(),
                    Enumerable.Empty<PackageReference>(),
                    Enumerable.Empty<PackageIdentity>(),
                    availablePackages,
                    this.SourceRepositoryProvider.GetRepositories().Select(s => s.PackageSource),
                    logger);

                PackageResolver resolver = new PackageResolver();
                IEnumerable<SourcePackageDependencyInfo> packagesToInstall = resolver.Resolve(resolverContext, CancellationToken.None)
                    .Select(p => availablePackages.Single(x => PackageIdentityComparer.Default.Equals(x, p)));
                PackagePathResolver packagePathResolver = new PackagePathResolver(this.RootPath);
                ClientPolicyContext clientPolicyContext = ClientPolicyContext.GetClientPolicy(this.DefaultSettings, logger);
                PackageExtractionContext packageExtractionContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    clientPolicyContext,
                    logger);

                foreach (SourcePackageDependencyInfo packageToInstall in packagesToInstall)
                {
                    PackageReaderBase packageReader;
                    DownloadResourceResult downloadResult = null;
                    string installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                    if (installedPath == null)
                    {
                        DownloadResource downloadResource = await packageToInstall.Source.GetResourceAsync<DownloadResource>(CancellationToken.None);
                        downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                            packageToInstall,
                            new PackageDownloadContext(cacheContext),
                            SettingsUtility.GetGlobalPackagesFolder(this.DefaultSettings),
                            logger, CancellationToken.None);

                        await PackageExtractor.ExtractPackageAsync(
                            downloadResult.PackageSource,
                            downloadResult.PackageStream,
                            packagePathResolver,
                            packageExtractionContext,
                            CancellationToken.None);


                        installedPath = packagePathResolver.GetInstalledPath(packageToInstall);
                        packageReader = downloadResult.PackageReader;
                    }
                    else
                    {
                        packageReader = new PackageFolderReader(installedPath);
                    }

                    ///update the packages.config file
                    await UpdatePackagesConfigFile(packageToInstall, packageReader, downloadResult, nuGetFramework);
                    UpdateProjectFile(packageReader, installedPath, packageToInstall, nuGetFramework);
                }
            }
        }

        /// <summary>
        /// Gets all the dependencies of the package to be installed.
        /// This ensures that a package gets installed with all its dependencies.
        /// </summary>
        /// <param name="package">The package to be installed.</param>
        /// <param name="framework">The target framework of the project on which the package is to installed.</param>
        /// <param name="cacheContext">The <see cref="SourceCacheContext"/> instance containing the control settings for the disk space.</param>
        /// <param name="logger">The <see cref="ILogger"/> instance to be used for logging.</param>
        /// <param name="repositories">A list of the <see cref="SourceRepository"/> to be used.</param>
        /// <param name="availablePackages">A list of all the <see cref="SourcePackageDependencyInfo"/> associated with the package to be installed.</param>
        /// <returns></returns>
        private async Task GetPackageDependencies(
            PackageIdentity package,
            NuGetFramework framework,
            SourceCacheContext cacheContext,
            ILogger logger,
            IEnumerable<SourceRepository> repositories,
            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package)) return;

            foreach (SourceRepository sourceRepository in repositories)
            {
                DependencyInfoResource dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>();
                SourcePackageDependencyInfo dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None);

                if (dependencyInfo == null) continue;

                availablePackages.Add(dependencyInfo);
                foreach (PackageDependency dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        framework, cacheContext, logger, repositories, availablePackages);
                }
            }
        }

        /// <summary>
        /// This method gets the latest version of the package that is compatible with
        /// The target framework of the project provided.
        /// </summary>
        /// <param name="packageId">The nuget package to be installed</param>
        /// <param name="sourceRepository">The <see cref="SourceRepository"/> to use.</param>
        /// <param name="projectTargetVersion">The version of net framework/.netcore that the project targets.</param>
        /// <returns>The <see cref="NuGetVersion"/> of the package</returns>
        private async Task<NuGetVersion> GetPackageLatestNugetVersionAsync(
            string packageId,
            SourceRepository sourceRepository,
            string projectTargetFramework)
        {
            NuGetVersion packageVersion = null;
            PackageSearchResource searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();

            string[] targetProjectFrameworks = new[] { projectTargetFramework };
            SearchFilter searchFilter = new SearchFilter(false)
            {
                SupportedFrameworks = targetProjectFrameworks
            };

            IEnumerable<IPackageSearchMetadata> jsonNugetPackages = await searchResource
                .SearchAsync(packageId, searchFilter, 0, 10, new Logger(this.messageLogger), CancellationToken.None);

            //The first one is the latest package that is compatible with the supported framework
            IPackageSearchMetadata jsonPackage = jsonNugetPackages.First();
            packageVersion = NuGetVersion.Parse(jsonPackage.Identity.Version.ToString());

            return packageVersion;
        }

        /// <summary>
        /// This method ensures that the packages.config file is created and all the installed packages are added to it.
        /// </summary>
        /// <param name="packageToInstall">The nuget package to install with all its dependencies</param>
        /// <param name="packageReader">The <see cref="PackageReaderBase"/> to use in reading the package.</param>
        /// <param name="downloadResult">The <see cref="DownloadResourceResult"/> for the package.</param>
        /// <param name="nuGetFramework">The <see cref="NuGetFramework"/></param>
        /// <returns>The <see cref="NuspecReader"/> for reading the package's nuspec files.</returns>
        private async Task<NuspecReader> UpdatePackagesConfigFile(
            SourcePackageDependencyInfo packageToInstall,
            PackageReaderBase packageReader,
            DownloadResourceResult downloadResult,
            NuGetFramework nuGetFramework)
        {
            NuspecReader nuspecReader = await packageReader.GetNuspecReaderAsync(CancellationToken.None).ConfigureAwait(false);
            Dictionary<string, object> metadata = new Dictionary<string, object>()
            {
                { "Name", packageToInstall.Id },
                {"TargetFrameWork", nuGetFramework }
            };

            IDictionary<string, object> metadataObject = new ExpandoObject()  as IDictionary<string, object>;
            foreach (KeyValuePair<string, object> kvp in metadata)
            {
                metadataObject.Add(kvp.Key, kvp.Value);
            }

            Dictionary<string, object> meta = new Dictionary<string, object>
            {
                { "Name", metadataObject.ToList().FirstOrDefault().Value },
                { "TargetFramework", metadataObject.ToList().LastOrDefault().Value }
            };

            PackagesConfigNuGetProject packagesConfigNuGetProject = new PackagesConfigNuGetProject(this.project.DirectoryPath, meta);
            await packagesConfigNuGetProject.InstallPackageAsync(
                packageToInstall,
                downloadResult,
                new EmptyNuGetProjectContext(),
                CancellationToken.None);

            return nuspecReader;
        }

        /// <summary>
        /// This methods updates the project's .csproj file. It adds all the installed packages to the .csproj file.
        /// </summary>
        /// <param name="packageReader">The <see cref="PackageReaderBase"/> to use in reading the nuget package</param>
        /// <param name="installedPath">The folder path where the package was installed or downloaded to</param>
        /// <param name="packageToInstall">The package to install with all its dependencies.</param>
        /// <param name="nuGetFramework">The <see cref="NuGetFramework"/></param>
        private void UpdateProjectFile(
            PackageReaderBase packageReader,
            string installedPath,
            SourcePackageDependencyInfo packageToInstall,
            NuGetFramework nuGetFramework )
        {
            IEnumerable<FrameworkSpecificGroup> dllItems = packageReader.GetLibItems();
            if (dllItems != null)
            {
                FrameworkReducer frameworkReducer = new FrameworkReducer();
                NuGetFramework nearest = frameworkReducer.GetNearest(nuGetFramework, dllItems.Select(x => x.TargetFramework));
                string targetDllFramework = dllItems
                            .Where(x => x.TargetFramework.Equals(nearest))
                            .SelectMany(x => x.Items).Where(x => x.Contains(".dll")).FirstOrDefault();

                if (!string.IsNullOrEmpty(targetDllFramework) && !string.IsNullOrEmpty(installedPath))
                {
                    string targetDllFrameworkPath = Path.Combine(installedPath, targetDllFramework.ToString());
                    string dllName = Assembly.LoadFile(targetDllFrameworkPath).FullName;
                    string dllNameWithProcessArchitecture = dllName+","+" processorArchitecture=MSIL";
                    ProjectItem item = this.project.GetItems("Reference").FirstOrDefault(a => a.EvaluatedInclude.Contains(packageToInstall.Id));
                    if (item == null)
                    {
                        if (ProjectHelper.CheckIfSolutionAndProjectFilesAreInSameFolder(this.project.DirectoryPath))
                        {
                            ProjectItem projectItem = this.project.AddItem("Reference", dllNameWithProcessArchitecture).FirstOrDefault();
                            projectItem.HasMetadata("HintPath");
                            projectItem.SetMetadataValue("HintPath", $"packages\\{packageToInstall.Id}.{ packageToInstall.Version}\\{targetDllFramework.Replace(@"/", "\\")}".Replace(@"\\", "//"));
                        }
                        else
                        {
                            ProjectItem projectItem = this.project.AddItem("Reference", dllNameWithProcessArchitecture).FirstOrDefault();
                            projectItem.SetMetadataValue("HintPath", $"..\\packages\\{packageToInstall.Id}.{ packageToInstall.Version}\\{targetDllFramework.Replace(@"/", "\\")}".Replace(@"\\", "//"));
                        }

                        this.project.Save();
                    }

                    ProjectItem dataAnnotationsRef = this.project.GetItems("Reference").FirstOrDefault(a => a.EvaluatedInclude.Contains("System.ComponentModel.DataAnnotations"));
                    if (dataAnnotationsRef == null)
                    {
                        ProjectHelper.AddProjectItem(this.project, "Reference", "System.ComponentModel.DataAnnotations");
                    }

                    ProjectItem packageConfigRef = this.project.GetItems("None").FirstOrDefault(a => a.EvaluatedInclude.Contains("packages.config"));
                    if (packageConfigRef == null)
                    {
                        ProjectHelper.AddProjectItem(this.project, "None", "packages.config");
                    }
                }
            }
        }

        /// <summary>
        /// Installs Packages on any project that uses the PackageReference method to reference packages.
        /// Uses the dotnet Cli
        /// </summary>
        /// <param name="packageName">The name of the package to install</param>
        /// <param name="messageLogger">An instance of <see cref="IMessageLogger"> to use in logging</param>
        internal async Task InstallPackagesOnDotNetCoreFrameworks(string packageName, IMessageLogger messageLogger, string projectTargetFramework)
        {

            ILogger logger = new Logger(this.messageLogger);
            CancellationToken cancellationToken = CancellationToken.None;

            FindPackageByIdResource resource = await this.SourceRepository.GetResourceAsync<FindPackageByIdResource>();
            NuGetFramework nuGetFramework = NuGetFramework.ParseFolder(projectTargetFramework?.Split('=')[1]);
            NuGetVersion packageVersion = await GetPackageLatestNugetVersionAsync(packageName, this.SourceRepository, nuGetFramework.DotNetFrameworkName);
            using (MemoryStream packageStream = new MemoryStream())
            {
                await resource.CopyNupkgToStreamAsync(
               packageName,
               packageVersion,
               packageStream,
               new SourceCacheContext(),
               logger,
               cancellationToken);
            }

            ProjectItem checkIfItemExists = this.project.GetItems("PackageReference").FirstOrDefault(a => a.EvaluatedInclude.Contains(packageName));
            if (checkIfItemExists == null)
            {
                ProjectItem item = this.project.AddItem("PackageReference", packageName).FirstOrDefault();
                item.Xml.AddMetadata("Version", packageVersion.OriginalVersion, true);
                this.project.Save();
            }
        }
    }
}
