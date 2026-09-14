//-----------------------------------------------------------------------------------
// <copyright file="ConnectedServicePackageInstallerTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using EnvDTE;
using Microsoft.OData.ConnectedService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ODataConnectedService.Tests
{
    [TestClass]
    public class ConnectedServicePackageInstallerTests
    {
        [TestMethod]
        public void GetTargetFrameworkMonikers_ReturnsAllTargetFrameworks()
        {
            Project project = CreateProject(("TargetFrameworks", "net10.0; net6.0;net472"));

            string[] targetFrameworks = ConnectedServicePackageInstaller.GetTargetFrameworkMonikers(project);

            CollectionAssert.AreEqual(new[] { "net10.0", "net6.0", "net472" }, targetFrameworks);
        }

        [TestMethod]
        public void GetTargetFrameworkMonikers_FallsBackToTargetFrameworkMoniker()
        {
            Project project = CreateProject(("TargetFrameworkMoniker", ".NETFramework,Version=v4.7.2"));

            string[] targetFrameworks = ConnectedServicePackageInstaller.GetTargetFrameworkMonikers(project);

            CollectionAssert.AreEqual(new[] { ".NETFramework,Version=v4.7.2" }, targetFrameworks);
        }

        private static Project CreateProject(params (string Name, string Value)[] projectProperties)
        {
            var properties = new Mock<Properties>();
            foreach ((string name, string value) in projectProperties)
            {
                var property = new Mock<Property>();
                property.SetupGet(item => item.Value).Returns(value);
                properties.Setup(items => items.Item(name)).Returns(property.Object);
            }

            var project = new Mock<Project>();
            project.SetupGet(item => item.Properties).Returns(properties.Object);
            return project.Object;
        }
    }
}
