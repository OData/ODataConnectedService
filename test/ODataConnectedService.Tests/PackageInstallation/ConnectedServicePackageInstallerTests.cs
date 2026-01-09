//-----------------------------------------------------------------------------
// <copyright file="ConnectedServicePackageInstallerTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet.VisualStudio;

namespace ODataConnectedService.Tests.PackageInstallation
{
    [TestClass]
    public class ConnectedServicePackageInstallerTests
    {
        private Mock<IVsPackageInstaller2> packageInstallerMock;
        private Mock<IVsPackageInstallerServices> packageInstallerServicesMock;
        private Mock<IMessageLogger> messageLoggerMock;
        private Mock<Project> projectMock;
        private Mock<Properties> propertiesMock;
        private TestConnectedServiceHandlerContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.packageInstallerMock = new Mock<IVsPackageInstaller2>();
            this.packageInstallerServicesMock = new Mock<IVsPackageInstallerServices>();
            this.messageLoggerMock = new Mock<IMessageLogger>();
            this.projectMock = new Mock<Project>();
            this.propertiesMock = new Mock<Properties>();

            var serviceConfig = new ServiceConfiguration { ServiceName = "TestService" };
            var serviceInstance = new ODataConnectedServiceInstance
            {
                ServiceConfig = serviceConfig,
                Name = "TestService"
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            this.context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldInstallPackage_WhenPackageNotInstalledAndFrameworkIsDotNetAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "Microsoft.OData.Client";

            this.SetupProject(".NETFramework,Version=v4.7.2");
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(false);

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.packageInstallerMock.Verify(
                p => p.InstallLatestPackage(packageSource, this.projectMock.Object, packageName, true, false),
                Times.Once);
            this.messageLoggerMock.Verify(
                m => m.WriteMessageAsync(LogMessageCategory.Information, It.Is<string>(s => s.Contains("added")), It.IsAny<object[]>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldSkipInstallation_WhenPackageAlreadyInstalledAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "Microsoft.OData.Client";

            this.SetupProject(".NETFramework,Version=v4.7.2");
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(true);

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.packageInstallerMock.Verify(
                p => p.InstallLatestPackage(It.IsAny<string>(), It.IsAny<Project>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never);
            this.packageInstallerMock.Verify(
                p => p.InstallPackage(It.IsAny<string>(), It.IsAny<Project>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
            this.messageLoggerMock.Verify(
                m => m.WriteMessageAsync(LogMessageCategory.Information, It.Is<string>(s => s.Contains("already installed")), It.IsAny<object[]>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldSkipSystemTextJson_WhenFrameworkIsDotNetAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "System.Text.Json";

            this.SetupProject(".NETFramework,Version=v4.7.2");
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(false);

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.packageInstallerMock.Verify(
                p => p.InstallLatestPackage(It.IsAny<string>(), It.IsAny<Project>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never);
            this.packageInstallerMock.Verify(
                p => p.InstallPackage(It.IsAny<string>(), It.IsAny<Project>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldUseInstallPackage_WhenFrameworkVersionCannotBeParsedAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "Microsoft.OData.Client";

            this.SetupProject("InvalidFrameworkVersion");
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(false);

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.packageInstallerMock.Verify(
                p => p.InstallPackage(packageSource, this.projectMock.Object, packageName, (string)null, false),
                Times.Once);
            this.messageLoggerMock.Verify(
                m => m.WriteMessageAsync(LogMessageCategory.Information, It.Is<string>(s => s.Contains("added")), It.IsAny<object[]>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldLogError_WhenInstallationFailsAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "Microsoft.OData.Client";
            const string errorMessage = "Installation failed";

            this.SetupProject(".NETFramework,Version=v4.7.2");
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(false);
            this.packageInstallerMock
                .Setup(p => p.InstallLatestPackage(It.IsAny<string>(), It.IsAny<Project>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Throws(new Exception(errorMessage));

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.messageLoggerMock.Verify(
                m => m.WriteMessageAsync(LogMessageCategory.Error, It.Is<string>(s => s.Contains("not installed") && s.Contains(errorMessage)), It.IsAny<object[]>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldInstallLatestPackage_WhenFrameworkVersionIsNet5OrHigherAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "Microsoft.OData.Client";

            this.SetupProject(".NETCoreApp,Version=v5.0");
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(false);

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.packageInstallerMock.Verify(
                p => p.InstallLatestPackage(packageSource, this.projectMock.Object, packageName, true, false),
                Times.Once);
        }

        [TestMethod]
        public async Task CheckAndInstallNuGetPackageAsync_ShouldHandleNullTargetFrameworkMonikerAsync()
        {
            // Arrange
            const string packageSource = "https://api.nuget.org/v3/index.json";
            const string packageName = "Microsoft.OData.Client";

            this.SetupProject(null);
            this.packageInstallerServicesMock
                .Setup(s => s.IsPackageInstalled(It.IsAny<Project>(), packageName))
                .Returns(false);

            var installer = this.CreatePackageInstaller();

            // Act
            await installer.CheckAndInstallNuGetPackageAsync(packageSource, packageName).ConfigureAwait(false);

            // Assert
            this.packageInstallerMock.Verify(
                p => p.InstallPackage(packageSource, this.projectMock.Object, packageName, (string)null, false),
                Times.Once);
        }

        private ConnectedServicePackageInstaller CreatePackageInstaller()
        {
            var installer = new ConnectedServicePackageInstaller(this.context, this.projectMock.Object, this.messageLoggerMock.Object);

            return installer;
        }

        private void SetupProject(string targetFrameworkMoniker)
        {
            var propertyMock = new Mock<Property>();
            propertyMock.SetupGet(p => p.Value).Returns(targetFrameworkMoniker);

            this.propertiesMock
                .Setup(p => p.Item("TargetFrameworkMoniker"))
                .Returns(propertyMock.Object);

            this.projectMock.SetupGet(p => p.Properties).Returns(this.propertiesMock.Object);
        }
    }
}
