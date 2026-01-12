//-----------------------------------------------------------------------------------
// <copyright file="ODataCliFileHandlerTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Microsoft.OData.CodeGen.Logging;
using Moq;

namespace Microsoft.OData.Cli.Tests.FileHandling
{
    public class ODataCliFileHandlerTests
    {
        public ODataCliFileHandlerTests()
        {
            EnsureMSBuildLoadedIfNot();
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIsGreaterThanOrEqualTo9_0_0()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("9.0.0");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIs9_1_0()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("9.1.0");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnTrue_WhenODataClientVersionIsPrereleaseAsync()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("9.0.0-preview.3");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientVersionIsLessThan9_0_0()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("8.0.0");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientVersionIs7_6_4()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("7.6.4");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenODataClientReferenceNotFound()
        {
            // Arrange
            var project = CreateProjectWithoutODataClient();
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenProjectIsNull()
        {
            // Arrange
            var fileHandler = CreateFileHandler(null);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldReturnFalse_WhenVersionCannotBeParsed()
        {
            // Arrange
            var project = CreateProjectWithODataClientVersion("invalid-version");
            var fileHandler = CreateFileHandler(project);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.False(result);
        }

        private static Project CreateProjectWithODataClientVersion(string version)
        {
            // Create a .csproj in memory
            var pre = ProjectRootElement.Create();
            pre.Sdk = "Microsoft.NET.Sdk";

            var pg = pre.AddPropertyGroup();
            pg.AddProperty("TargetFramework", "net8.0");

            var ig = pre.AddItemGroup();
            var pr = ig.AddItem("PackageReference", "Microsoft.OData.Client");
            pr.AddMetadata("Version", version, expressAsAttribute: true);

            // Create an evaluated Project from the XML
            var project = new Project(pre);
            project.ReevaluateIfNecessary();

            return project;
        }

        private static Project CreateProjectWithoutODataClient()
        {
            var pre = ProjectRootElement.Create();
            pre.Sdk = "Microsoft.NET.Sdk";

            var pg = pre.AddPropertyGroup();
            pg.AddProperty("TargetFramework", "net8.0");

            var ig = pre.AddItemGroup();
            var pr = ig.AddItem("PackageReference", "Newtonsoft.Json");
            pr.AddMetadata("Version", "13.0.1", expressAsAttribute: true);

            var project = new Project(pre);
            project.ReevaluateIfNecessary();

            return project;
        }

        private static ODataCliFileHandler CreateFileHandler(Project project)
        {
            var loggerMock = new Mock<IMessageLogger>();
            return new ODataCliFileHandler(loggerMock.Object, project);
        }

        private void EnsureMSBuildLoadedIfNot()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                try
                {
                    MSBuildLocator.RegisterDefaults();
                }
                catch (InvalidOperationException)
                {
                    // MSBuild assemblies were already loaded before registration
                    // This can happen if another test class already loaded MSBuild types
                    // Safe to ignore since MSBuild is already available
                }
            }
        }
    }
}
