﻿using EnvDTE;
using NuGet;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODataConnectedService.Tests.TestHelpers
{
    public class TestVsPackageInstallerServices : IVsPackageInstallerServices
    {

        public TestVsPackageInstallerServices()
        {
            InstalledPackages = new HashSet<string>();
            PackagesQueried = new HashSet<string>();
        }

        public HashSet<string> InstalledPackages = new HashSet<string>();
        public HashSet<string> PackagesQueried { get; private set; }
        public IEnumerable<IVsPackageMetadata> GetInstalledPackages()
        {
            throw new System.NotImplementedException();
        }

        public bool IsPackageInstalled(Project project, string id)
        {
            PackagesQueried.Add(id);
            return InstalledPackages.Contains(id);
        }

        public bool IsPackageInstalled(Project project, string id, SemanticVersion version)
        {
            PackagesQueried.Add(id);
            return InstalledPackages.Contains(id);
        }

        public bool IsPackageInstalledEx(Project project, string id, string versionString)
        {
            PackagesQueried.Add(id);
            return InstalledPackages.Contains(id);
        }

        public IEnumerable<IVsPackageMetadata> GetInstalledPackages(Project project)
        {
            throw new System.NotImplementedException();
        }
    }
}
