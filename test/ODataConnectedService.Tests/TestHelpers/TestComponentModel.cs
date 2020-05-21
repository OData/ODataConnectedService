//---------------------------------------------------------------------------------
// <copyright file="TestComponentModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using Microsoft.VisualStudio.ComponentModelHost;
using NuGet.VisualStudio;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace ODataConnectedService.Tests.TestHelpers
{
    public class TestComponentModel : IComponentModel
    {
        public ComposablePartCatalog DefaultCatalog => throw new System.NotImplementedException();

        public ExportProvider DefaultExportProvider => throw new System.NotImplementedException();

        public ICompositionService DefaultCompositionService => throw new System.NotImplementedException();

        public ComposablePartCatalog GetCatalog(string catalogName)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> GetExtensions<T>() where T : class
        {
            throw new System.NotImplementedException();
        }

        public T GetService<T>() where T : class
        {
            if (typeof(T) == typeof(IVsPackageInstallerServices))
            {
                return new TestVsPackageInstallerServices() as T;
            }

            if (typeof(T) == typeof(IVsPackageInstaller))
            {
                return new TestVsPackageInstaller() as T;
            }

            return null;
        }
    }
}
