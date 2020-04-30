using EnvDTE;
using NuGet;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataConnectedService.Tests.TestHelpers
{
    public class TestVsPackageInstaller : IVsPackageInstaller
    {

        public TestVsPackageInstaller()
        {
            InstalledPackages = new HashSet<string>();
        }

        public HashSet<string> InstalledPackages { get; private set; }

        public void InstallPackage(string source, Project project, string packageId, Version version, bool ignoreDependencies)
        {
            InstalledPackages.Add(packageId);
        }

        public void InstallPackage(string source, Project project, string packageId, string version, bool ignoreDependencies)
        {
            InstalledPackages.Add(packageId);
        }

        public void InstallPackage(IPackageRepository repository, Project project, string packageId, string version, bool ignoreDependencies, bool skipAssemblyReferences)
        {
            InstalledPackages.Add(packageId);
        }

        public void InstallPackagesFromRegistryRepository(string keyName, bool isPreUnzipped, bool skipAssemblyReferences, Project project, IDictionary<string, string> packageVersions)
        {
            throw new NotImplementedException();
        }

        public void InstallPackagesFromRegistryRepository(string keyName, bool isPreUnzipped, bool skipAssemblyReferences, bool ignoreDependencies, Project project, IDictionary<string, string> packageVersions)
        {
            throw new NotImplementedException();
        }

        public void InstallPackagesFromVSExtensionRepository(string extensionId, bool isPreUnzipped, bool skipAssemblyReferences, Project project, IDictionary<string, string> packageVersions)
        {
            throw new NotImplementedException();
        }

        public void InstallPackagesFromVSExtensionRepository(string extensionId, bool isPreUnzipped, bool skipAssemblyReferences, bool ignoreDependencies, Project project, IDictionary<string, string> packageVersions)
        {
            throw new NotImplementedException();
        }
    }
}
