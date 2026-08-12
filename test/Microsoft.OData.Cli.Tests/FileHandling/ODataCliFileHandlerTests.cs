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
            var loggerMock = new Mock<IMessageLogger>();
            var fileHandler = CreateFileHandler(project, loggerMock);

            // Act
            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            // Assert
            Assert.False(result);
            loggerMock.Verify(
                logger => logger.WriteMessageAsync(
                    LogMessageCategory.Warning,
                    It.Is<string>(message => message.Contains("could not be resolved")),
                    It.IsAny<object[]>()),
                Times.Once);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldUseCentralPackageVersion()
        {
            var project = CreateCentrallyManagedProject("9.0.0");
            var fileHandler = CreateFileHandler(project);

            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldPreferVersionOverride()
        {
            var project = CreateCentrallyManagedProject("8.0.0", "9.0.0");
            var fileHandler = CreateFileHandler(project);

            var result = await fileHandler.EmitNativeDateTimeTypesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldEvaluatePropertyBasedVersion()
        {
            var pre = ProjectRootElement.Create();
            pre.AddPropertyGroup().AddProperty("ODataClientVersion", "9.0.0");
            var packageReference = pre.AddItemGroup().AddItem("PackageReference", "Microsoft.OData.Client");
            packageReference.AddMetadata("Version", "$(ODataClientVersion)", expressAsAttribute: true);
            var project = new Project(pre);
            project.ReevaluateIfNecessary();

            var result = await CreateFileHandler(project).EmitNativeDateTimeTypesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldSupportVersionRangeWithSupportedMinimum()
        {
            var project = CreateProjectWithODataClientVersion("[9.0.0,10.0.0)");

            var result = await CreateFileHandler(project).EmitNativeDateTimeTypesAsync();

            Assert.True(result);
        }

        [Fact]
        public async Task EmitNativeDateTimeTypesAsync_ShouldPreferRestoredAssetsVersion()
        {
            string projectDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(Path.Combine(projectDirectory, "obj"));
            string assetsFile = Path.Combine(projectDirectory, "obj", "project.assets.json");
            File.WriteAllText(
                assetsFile,
                @"{ ""version"": 3, ""libraries"": { ""Microsoft.OData.Client/9.0.0"": { ""type"": ""package"" } } }");

            try
            {
                var pre = ProjectRootElement.Create();
                pre.AddPropertyGroup().AddProperty("ProjectAssetsFile", assetsFile);
                var packageReference = pre.AddItemGroup().AddItem("PackageReference", "Microsoft.OData.Client");
                packageReference.AddMetadata("Version", "8.0.0", expressAsAttribute: true);
                var project = new Project(pre);
                project.ReevaluateIfNecessary();

                var result = await CreateFileHandler(project).EmitNativeDateTimeTypesAsync();

                Assert.True(result);
            }
            finally
            {
                ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
                Directory.Delete(projectDirectory, true);
            }
        }

        private static Project CreateProjectWithODataClientVersion(string version)
        {
            // Create a .csproj in memory
            var pre = ProjectRootElement.Create();

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
            // Create a .csproj in memory
            var pre = ProjectRootElement.Create();

            var pg = pre.AddPropertyGroup();
            pg.AddProperty("TargetFramework", "net8.0");

            var ig = pre.AddItemGroup();
            var pr = ig.AddItem("PackageReference", "Newtonsoft.Json");
            pr.AddMetadata("Version", "13.0.1", expressAsAttribute: true);

            var project = new Project(pre);
            project.ReevaluateIfNecessary();

            return project;
        }

        private static Project CreateCentrallyManagedProject(string centralVersion, string? versionOverride = null)
        {
            var pre = ProjectRootElement.Create();
            var propertyGroup = pre.AddPropertyGroup();
            propertyGroup.AddProperty("TargetFramework", "net8.0");
            propertyGroup.AddProperty("ManagePackageVersionsCentrally", "true");

            var packageVersion = pre.AddItemGroup().AddItem("PackageVersion", "Microsoft.OData.Client");
            packageVersion.AddMetadata("Version", centralVersion, expressAsAttribute: true);

            var packageReference = pre.AddItemGroup().AddItem("PackageReference", "Microsoft.OData.Client");
            if (!string.IsNullOrEmpty(versionOverride))
            {
                packageReference.AddMetadata("VersionOverride", versionOverride, expressAsAttribute: true);
            }

            var project = new Project(pre);
            project.ReevaluateIfNecessary();
            return project;
        }

        private static ODataCliFileHandler CreateFileHandler(Project? project, Mock<IMessageLogger>? loggerMock = null)
        {
            loggerMock ??= new Mock<IMessageLogger>();
            loggerMock
                .Setup(logger => logger.WriteMessageAsync(It.IsAny<LogMessageCategory>(), It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns(Task.CompletedTask);
            return new ODataCliFileHandler(loggerMock.Object, project!);
        }

        private static void EnsureMSBuildLoadedIfNot()
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
                    //Assert.False(MSBuildLocator.IsRegistered, "MSBuild should have been registered before this point.");
                }
            }
        }
    }
}
