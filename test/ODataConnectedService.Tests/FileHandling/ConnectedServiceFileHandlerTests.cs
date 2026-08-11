//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceFileHandlerTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ODataConnectedService.Tests.TestHelpers;

namespace ODataConnectedService.Tests.FileHandling
{
    [TestClass]
    public class ConnectedServiceFileHandlerTests
    {
        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIsGreaterThanOrEqualTo9_0_0Async()
        {
            // Arrange
            var project = new Mock<Project>().Object;
            var fileHandler = CreateFileHandler(project, CreateProvider("Microsoft.OData.Client", "9.0.0"));

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIsPrereleaseAsync()
        {
            // Arrange
            var project = new Mock<Project>().Object;
            var fileHandler = CreateFileHandler(project, CreateProvider("Microsoft.OData.Client", "9.0.0-preview.3"));

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientVersionIsLessThan9_0_0_Async()
        {
            // Arrange
            var project = new Mock<Project>().Object;
            var fileHandler = CreateFileHandler(project, CreateProvider("Microsoft.OData.Client", "8.0.0"));

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientReferenceNotFoundAsync()
        {
            // Arrange
            var project = new Mock<Project>().Object;
            var fileHandler = CreateFileHandler(project, new FakeInstalledPackagesProvider());

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CheckODataClientVersionAsync_ShouldCacheVersion_AndReuseOnSubsequentCallsAsync()
        {
            // Arrange
            var provider = new FakeInstalledPackagesProvider(new InstalledPackageInfo("Microsoft.OData.Client", "9.0.0"));
            var projectMock = new Mock<Project>();
            var fileHandler = CreateFileHandler(projectMock.Object, provider);

            // Act
            var result1 = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);
            var result2 = await fileHandler.EmitContainerPropertyAttributeAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.AreEqual(1, provider.CallCount, "Installed packages should only be enumerated once due to caching");
        }

        [TestMethod]
        public async Task CheckODataClientVersionAsync_ShouldCacheMissingPackageAsync()
        {
            var provider = new FakeInstalledPackagesProvider();
            var fileHandler = CreateFileHandler(new Mock<Project>().Object, provider);

            Assert.IsFalse(await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false));
            Assert.IsFalse(await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false));
            Assert.AreEqual(1, provider.CallCount);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldMatchPackageIdCaseInsensitivelyAsync()
        {
            var project = new Mock<Project>().Object;
            var fileHandler = CreateFileHandler(project, CreateProvider("microsoft.odata.client", "9.0.0"));

            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldWarnOnce_WhenInstalledVersionCannotBeParsedAsync()
        {
            var logger = new Mock<IMessageLogger>();
            logger.Setup(value => value.WriteMessageAsync(It.IsAny<LogMessageCategory>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(Task.CompletedTask);
            var provider = CreateProvider("Microsoft.OData.Client", "invalid-version");
            var fileHandler = CreateFileHandler(new Mock<Project>().Object, provider, logger.Object);

            Assert.IsFalse(await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false));
            Assert.IsFalse(await fileHandler.EmitContainerPropertyAttributeAsync().ConfigureAwait(false));
            logger.Verify(
                value => value.WriteMessageAsync(
                    LogMessageCategory.Warning,
                    It.Is<string>(message => message.Contains("could not be resolved")),
                    It.IsAny<object[]>()),
                Times.Once);
        }

        private static FakeInstalledPackagesProvider CreateProvider(string packageId, string version)
        {
            return new FakeInstalledPackagesProvider(new InstalledPackageInfo(packageId, version));
        }

        private static ConnectedServiceFileHandler CreateFileHandler(
            Project project,
            IInstalledPackagesProvider packagesProvider = null,
            IMessageLogger logger = null)
        {
            var serviceConfig = new ServiceConfigurationV4 { ServiceName = "TestService" };
            var serviceInstance = new ODataConnectedServiceInstance
            {
                ServiceConfig = serviceConfig,
                Name = "TestService"
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper
            {
                ServicesRootFolder = "ConnectedServices"
            };

            var context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);
            var threadHelper = new TestThreadHelper();

            logger = logger ?? new Mock<IMessageLogger>().Object;
            return new ConnectedServiceFileHandler(context, project, threadHelper, logger, packagesProvider);
        }

        private sealed class FakeInstalledPackagesProvider : IInstalledPackagesProvider
        {
            private readonly IReadOnlyList<InstalledPackageInfo> packages;

            public FakeInstalledPackagesProvider(params InstalledPackageInfo[] packages)
            {
                this.packages = packages;
            }

            public int CallCount { get; private set; }

            public Task<IReadOnlyList<InstalledPackageInfo>> GetInstalledPackagesAsync()
            {
                this.CallCount++;
                return Task.FromResult(this.packages);
            }
        }
    }
}
