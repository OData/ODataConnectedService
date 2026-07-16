//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceFileHandlerTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ODataConnectedService.Tests.TestHelpers;
using VSLangProj;

namespace ODataConnectedService.Tests.FileHandling
{
    [TestClass]
    public class ConnectedServiceFileHandlerTests
    {
        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIsGreaterThanOrEqualTo9_0_0Async()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("9.0.0");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIsPrereleaseAsync()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("9.0.0-preview.3");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientVersionIsLessThan9_0_0_Async()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("8.0.0");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientReferenceNotFoundAsync()
        {
            // Arrange
            var project = CreateProjectWithoutODataClient();
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CheckODataClientVersionAsync_ShouldCacheVersion_AndReuseOnSubsequentCallsAsync()
        {
            // Arrange
            var referenceMock = new Mock<Reference>();
            referenceMock.SetupGet(r => r.Name).Returns("Microsoft.OData.Client");
            referenceMock.SetupGet(r => r.Version).Returns("9.0.0.0");
            referenceMock.SetupGet(r => r.SourceProject).Returns((Project)null);

            var referencesMock = new Mock<References>();
            var callCount = 0;
            referencesMock.Setup(r => r.GetEnumerator())
                .Returns(() =>
                {
                    callCount++;
                    return new List<Reference> { referenceMock.Object }.GetEnumerator();
                });

            var vsProjectMock = new Mock<VSProject>();
            vsProjectMock.SetupGet(vsp => vsp.References).Returns(referencesMock.Object);

            var projectMock = new Mock<Project>();
            projectMock.SetupGet(p => p.Object).Returns(vsProjectMock.Object);

            var fileHandler = CreateFileHandler(projectMock.Object);

            // Act
            var result1 = await fileHandler.EmitNativeDateTimeTypesAsync().ConfigureAwait(false);
            var result2 = await fileHandler.EmitContainerPropertyAttributeAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.AreEqual(1, callCount, "References should only be enumerated once due to caching");
        }

        private static Project CreateProjectWithODataClientVersion(string version)
        {
            var referenceMock = new Mock<Reference>();
            referenceMock.SetupGet(r => r.Name).Returns("Microsoft.OData.Client");
            referenceMock.SetupGet(r => r.Version).Returns(version);
            referenceMock.SetupGet(r => r.SourceProject).Returns((Project)null);

            var referencesMock = new Mock<References>();
            referencesMock.Setup(r => r.GetEnumerator())
                .Returns(new List<Reference> { referenceMock.Object }.GetEnumerator());

            var vsProjectMock = new Mock<VSProject>();
            vsProjectMock.SetupGet(vsp => vsp.References).Returns(referencesMock.Object);

            var projectMock = new Mock<Project>();
            projectMock.SetupGet(p => p.Object).Returns(vsProjectMock.Object);

            return projectMock.Object;
        }

        private static Project CreateProjectWithoutODataClient()
        {
            var referenceMock = new Mock<Reference>();
            referenceMock.SetupGet(r => r.Name).Returns("System.Core");
            referenceMock.SetupGet(r => r.Version).Returns("4.0.0.0");
            referenceMock.SetupGet(r => r.SourceProject).Returns((Project)null);

            var referencesMock = new Mock<References>();
            referencesMock.Setup(r => r.GetEnumerator())
                .Returns(new List<Reference> { referenceMock.Object }.GetEnumerator());

            var vsProjectMock = new Mock<VSProject>();
            vsProjectMock.SetupGet(vsp => vsp.References).Returns(referencesMock.Object);

            var projectMock = new Mock<Project>();
            projectMock.SetupGet(p => p.Object).Returns(vsProjectMock.Object);

            return projectMock.Object;
        }

        private static ConnectedServiceFileHandler CreateFileHandler(Project project)
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

            return new ConnectedServiceFileHandler(context, project, threadHelper);
        }
    }
}
