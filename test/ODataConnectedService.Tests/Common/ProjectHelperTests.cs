//-----------------------------------------------------------------------------------
// <copyright file="ProjectHelperTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Data.Services.Design;
using EnvDTE;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ODataConnectedService.Tests
{
    [TestClass]
    public class ProjectHelperTests
    {
        [TestMethod]
        public void TestGetProjectFromHierarchy()
        {
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true
            };
            var context = SetupContext(serviceConfig);
            var project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            Assert.IsNotNull(project);
            Assert.AreEqual("TestProject", project.Name);
            Assert.AreEqual(@"\Path\to\MyProject\TestProject.csproj", project.FullName);
        }

        [TestMethod]
        public void TestGetFullPath()
        {
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true
            };
            var context = SetupContext(serviceConfig);
            var project = ProjectHelper.GetProjectFromHierarchy(context.ProjectHierarchy);
            var fullpath = ProjectHelper.GetFullPath(project);
            Assert.AreEqual(@"\Path\to\MyProject\TestProject.csproj", fullpath);
        }

        [TestMethod]
        public void TestGetLanguageOption()
        {
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true
            };

            // Assert CSharp Language Option
            var contextCS = SetupContext(serviceConfig);
            var projectCS = ProjectHelper.GetProjectFromHierarchy(contextCS.ProjectHierarchy);
            var languageOptionCS = ProjectHelper.GetLanguageOption(projectCS);
            Assert.IsNotNull(languageOptionCS);
            Assert.AreEqual(LanguageOption.GenerateCSharpCode, languageOptionCS);

            // Assert VB Language Option
            var contextVB = SetupContext(serviceConfig, EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB);
            var projectVB = ProjectHelper.GetProjectFromHierarchy(contextVB.ProjectHierarchy);
            var languageOptionVB = ProjectHelper.GetLanguageOption(projectVB);
            Assert.IsNotNull(languageOptionVB);
            Assert.AreEqual(LanguageOption.GenerateVBCode, languageOptionVB);
        }

        static TestConnectedServiceHandlerContext SetupContext(ServiceConfiguration serviceConfig)
        {
            return SetupContext(serviceConfig, EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp);
        }

        static TestConnectedServiceHandlerContext SetupContext(ServiceConfiguration serviceConfig, string languageOption)
        {
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                Name = "TestService",
                MetadataTempFilePath = "http://service/$metadata",
                ServiceConfig = serviceConfig
            };

            // Mock EnvDTE.Project
            var mock = new Mock<Project>();
            // Mock Project FullPath
            mock.Setup(p => p.Properties.Item("FullPath").Value).Returns(@"\Path\to\MyProject\TestProject.csproj");
            // Mock Language
            mock.Setup(p => p.CodeModel.Language).Returns(languageOption);
            // Mock Project Name
            mock.Setup(p => p.Name).Returns("TestProject");
            // Mock Project FullName
            mock.Setup(p => p.FullName).Returns(@"\Path\to\MyProject\TestProject.csproj");
            var proj = mock.Object;
            // Cast EnvDTE.Project Object to an object
            var project = (object)proj;

            // Mock IVsHierarchy
            var projectHierarchyMock = new Mock<IVsHierarchy>();
            projectHierarchyMock.Setup(h => h.GetProperty(It.IsAny<uint>(), It.IsAny<int>(), out project));
            var context = new TestConnectedServiceHandlerContext(
                serviceInstance: serviceInstance, projectHierarchy: projectHierarchyMock.Object);
            return context;
        }
    }
}
