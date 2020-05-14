﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using Microsoft.VisualStudio.ConnectedServices;
using EnvDTE;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.ConnectedService.CodeGeneration;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Templates;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using System.Collections.Generic;
using System.Net;
using ODataConnectedService.Tests;
using System.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using NuGet.VisualStudio;
using NuGet;
using System;
using System.Reflection;
using System.Linq;
using ODataConnectedService.Tests.TestHelpers;

namespace Microsoft.OData.ConnectedService.Tests.CodeGeneration
{
    [TestClass]
    public class CodeGenDescriptorTest
    {
        readonly static string TestProjectRootPath = Path.Combine(Directory.GetCurrentDirectory(), "TempODataConnectedServiceTest");
        readonly static string ServicesRootFolder = "ConnectedServicesRoot";
        readonly static string MetadataUri = "http://service/$metadata";

        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                Directory.Delete(TestProjectRootPath, true);
            }
            catch (DirectoryNotFoundException) { }
        }

        public static IEnumerable<object[]> ClientCodeServiceConfigToCodeGeneratorData
        {
            get
            {
                yield return new ServiceConfigurationV4[]
                {
                    new ServiceConfigurationV4()
                    {
                        IncludeCustomHeaders = true,
                        CustomHttpHeaders = "Key:Val",
                        IncludeWebProxy = true,
                        WebProxyHost = "http://localhost:8080",
                        IncludeWebProxyNetworkCredentials = true,
                        WebProxyNetworkCredentialsDomain = "domain",
                        WebProxyNetworkCredentialsUsername = "username",
                        WebProxyNetworkCredentialsPassword = "password",
                        ExcludedOperationImports = new List<string>() { "Func1", "Action2" },
                        ExcludedSchemaTypes = new List<string>() { "Service.Type1", "Service.Type2" },
                        UseDataServiceCollection = true,
                        IgnoreUnexpectedElementsAndAttributes = true,
                        EnableNamingAlias = true,
                        UseNamespacePrefix = true,
                        NamespacePrefix = "Namespace",
                        MakeTypesInternal = true,
                        GeneratedFileNamePrefix = "GeneratedCode",
                        GenerateMultipleFiles = true,
                        OpenGeneratedFilesInIDE = true,
                        IncludeT4File = false
                    }
                };

                yield return new ServiceConfigurationV4[]
                {
                    new ServiceConfigurationV4()
                    {
                        IncludeCustomHeaders = false,
                        CustomHttpHeaders = null,
                        IncludeWebProxy = false,
                        WebProxyHost = null,
                        IncludeWebProxyNetworkCredentials = false,
                        WebProxyNetworkCredentialsDomain = null,
                        WebProxyNetworkCredentialsUsername = null,
                        WebProxyNetworkCredentialsPassword = null,
                        ExcludedOperationImports = null,
                        ExcludedSchemaTypes = null,
                        UseDataServiceCollection = false,
                        IgnoreUnexpectedElementsAndAttributes = false,
                        EnableNamingAlias = false,
                        UseNamespacePrefix = false,
                        NamespacePrefix = "Namespace",
                        MakeTypesInternal = false,
                        GeneratedFileNamePrefix = "Reference",
                        GenerateMultipleFiles = false,
                        OpenGeneratedFilesInIDE = false,
                        IncludeT4File = false
                    }
                };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(ClientCodeServiceConfigToCodeGeneratorData), DynamicDataSourceType.Property)]
        public void TestAddGeneratedClientCode_PassesServiceConfigOptionsToCodeGenerator(object configObject)
        {
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var serviceConfig = configObject as ServiceConfigurationV4;
            serviceConfig.IncludeT4File = false;

            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, "TestService", codeGenFactory, handlerHelper);
            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();

            var generator = codeGenFactory.LastCreatedInstance;
            Assert.AreEqual(serviceConfig.UseDataServiceCollection, generator.UseDataServiceCollection);
            Assert.AreEqual(serviceConfig.EnableNamingAlias, generator.EnableNamingAlias);
            Assert.AreEqual(serviceConfig.IgnoreUnexpectedElementsAndAttributes, generator.IgnoreUnexpectedElementsAndAttributes);
            Assert.AreEqual(serviceConfig.MakeTypesInternal, generator.MakeTypesInternal);
            Assert.AreEqual(serviceConfig.NamespacePrefix, generator.NamespacePrefix);
            Assert.AreEqual(serviceConfig.ExcludedOperationImports, generator.ExcludedOperationImports);
            Assert.AreEqual(serviceConfig.ExcludedSchemaTypes, generator.ExcludedSchemaTypes);
            Assert.AreEqual(MetadataUri, generator.MetadataDocumentUri);
            Assert.AreEqual(ODataT4CodeGenerator.LanguageOption.CSharp, generator.TargetLanguage);
        }

        [TestMethod]
        public void TestAddGeneratedClientCode_GeneratesAndSavesCodeFile()
        {
            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                MakeTypesInternal = true,
                UseDataServiceCollection = false,
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "MyFile",
                IncludeT4File = false
            };
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName,
                new TestODataT4CodeGeneratorFactory(), handlerHelper);
            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();
            using (var reader = new StreamReader(handlerHelper.AddedFileInputFileName))
            {
                var generatedCode = reader.ReadToEnd();
                Assert.AreEqual("Generated code", generatedCode);
                Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "MyFile.cs"),
                    handlerHelper.AddedFileTargetFilePath);
            }
        }

        [TestMethod]
        public void TestAddGenerateClientCode_GeneratesMultipleFiles()
        {
            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "Main",
                GenerateMultipleFiles = true
            };

            var codeGen = new TestODataT4CodeGenerator();
            var codeGenFactory = new TestODataT4CodeGeneratorFactory(codeGen);
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName,
                codeGenFactory, handlerHelper);

            var template = new StringBuilder();
            codeGen.MultipleFilesManager = ODataT4CodeGenerator.FilesManager.Create(null, template);
            codeGen.MultipleFilesManager.StartNewFile("File1.cs", false);
            template.Append("Contents1");
            codeGen.MultipleFilesManager.EndBlock();

            codeGen.MultipleFilesManager.StartNewFile("File2.cs", false);
            template.Append("Contents2");
            codeGen.MultipleFilesManager.EndBlock();

            //The file manager expects the files to have been saved in the Temp directory
            // when ODataT4CodeGenerator.TransformText() was called. Since we're using a dummy code generator
            // we need to manually ensure those files exist
            var file1TempPath = Path.Combine(Path.GetTempPath(), "File1.cs");
            File.WriteAllText(file1TempPath, "Contents1");
            var file2TempPath = Path.Combine(Path.GetTempPath(), "File2.cs");
            File.WriteAllText(file2TempPath, "Contents2");

            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();
            var expectedMainFilePath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Main.cs");
            var mainFile = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == expectedMainFilePath);
            Assert.IsNotNull(mainFile);
            Assert.AreEqual("Generated code", File.ReadAllText(mainFile.SourceFile));
            var expectedFile1Path = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "File1.cs");
            var file1 = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == expectedFile1Path);
            Assert.IsNotNull(file1);
            Assert.AreEqual("Contents1", File.ReadAllText(file1.SourceFile));
            var expectedFile2Path = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "File2.cs");
            var file2 = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == expectedFile2Path);
            Assert.IsNotNull(file2);
            Assert.AreEqual("Contents2", File.ReadAllText(file2.SourceFile));
        }

        [TestMethod]
        public void Test_GeneratesAndSavesCodeFileWithProxy()
        {
            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                UseDataServiceCollection = false,
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "MyFile",
                IncludeT4File = false,
                IncludeWebProxy = true,
                IncludeWebProxyNetworkCredentials = true,
                WebProxyHost = "http://example.com:80",
                WebProxyNetworkCredentialsUsername = "user",
                WebProxyNetworkCredentialsPassword = "pass",
                Endpoint = "http://localhost:9000"
            };


            var testT4Factory = new TestODataT4NetworkCodeGeneratorFactory();
            testT4Factory.EDMX = ODataT4CodeGeneratorTestDescriptors.Simple.Metadata;


            var handlerHelper = new TestConnectedServiceHandlerHelper();

            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName,
              testT4Factory, handlerHelper);

            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();
            using (var reader = new StreamReader(handlerHelper.AddedFileInputFileName))
            {
                var generatedCode = reader.ReadToEnd();
                ODataT4CodeGeneratorTestDescriptors.Simple.Verify(generatedCode, true/*isCSharp*/, false/*useDSC*/);

                var requestGenerator = (TestODataT4NetworkCodeGeneratorFactory.TestHttpCreator)ODataT4CodeGenerator.CodeGenerationContext.RequestCreator;
                var proxy = requestGenerator.mock.Object.Proxy;
                Assert.IsNotNull(proxy);
                Assert.IsNotNull(proxy.Credentials);
                Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "MyFile.cs"),
                    handlerHelper.AddedFileTargetFilePath);
            }
        }

        [TestMethod]
        public void Test_GeneratesAndSavesCodeFileWithoutProxy()
        {

            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                UseDataServiceCollection = false,
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "MyFile",
                IncludeT4File = false,
                Endpoint = "http://localhost:9000"
            };


            var testT4Factory = new TestODataT4NetworkCodeGeneratorFactory();
            testT4Factory.EDMX = ODataT4CodeGeneratorTestDescriptors.Simple.Metadata;

            var handlerHelper = new TestConnectedServiceHandlerHelper();

            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName,
              testT4Factory, handlerHelper);

            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();
            using (var reader = new StreamReader(handlerHelper.AddedFileInputFileName))
            {
                var generatedCode = reader.ReadToEnd();
                ODataT4CodeGeneratorTestDescriptors.Simple.Verify(generatedCode, true/*isCSharp*/, false/*useDSC*/);
                var requestGenerator = (TestODataT4NetworkCodeGeneratorFactory.TestHttpCreator)ODataT4CodeGenerator.CodeGenerationContext.RequestCreator;
                var proxy = requestGenerator.mock.Object.Proxy;
                Assert.IsNull(proxy);
                Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "MyFile.cs"),
                    handlerHelper.AddedFileTargetFilePath);
            }
        }

        [DataTestMethod]
        [DataRow("cs", "TestConfigBasic.txt")]
        [DataRow("vb", "TestConfigBasicVB.txt")]
        public void TestAddGeneratedClientCode_GeneratesT4TemplateFiles(string lang, string referenceFile)
        {
            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                Endpoint = "https://service/$metadata",
                GeneratedFileNamePrefix = "Reference",
                IncludeT4File = true
            };

            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName, codeGenFactory, handlerHelper,
                lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB);

            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();

            var ttIncludeSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ODataT4CodeGenerator.ttinclude");
            var ttIncludeOutputPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.ttinclude");
            var fileManagerSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ODataT4CodeGenFilesManager.ttinclude");
            var fileManagerOutputPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "ODataT4CodeGenFilesManager.ttinclude");
            var ttOutputPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.tt");
            var csdlFileName = String.Concat(serviceName, "Csdl.xml");

            Assert.IsTrue(handlerHelper.AddedFiles.Contains((fileManagerOutputPath, fileManagerSourcePath)));
            var ttInclude = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == ttIncludeOutputPath);
            Assert.IsNotNull(ttInclude);
            var ttIncludeOriginalText = File.ReadAllText(ttIncludeSourcePath);
            var ttIncludeExpectedText = ttIncludeOriginalText.Replace("output extension=\".cs\"", $"output extension=\".{lang}\"");
            var ttIncludeSavedText = File.ReadAllText(ttInclude.SourceFile);
            Assert.AreEqual(ttIncludeExpectedText, ttIncludeSavedText);
            var tt = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == ttOutputPath);
            Assert.IsNotNull(tt);
            var ttExpectedText = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", referenceFile));
            ttExpectedText = ttExpectedText.Replace("$$CsdlFullPath$$", Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, csdlFileName));
            ttExpectedText = ttExpectedText.Replace("$$CsdlRelativePath$$", csdlFileName);
            var ttSavedText = File.ReadAllText(tt.SourceFile);
            Assert.AreEqual(ttExpectedText, ttSavedText);
        }

        [DataTestMethod]
        [DataRow("cs")]
        [DataRow("vb")]
        public void TestAddGeneratedClientCode_GeneratesCsdlFiles(string lang)
        {
            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                Endpoint = "https://service/$metadata",
                GeneratedFileNamePrefix = "Reference",
                IncludeT4File = true
            };

            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName, codeGenFactory, handlerHelper,
                lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB);

            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();

            var csdlFilePath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, String.Concat(serviceName, "Csdl.xml"));
            Assert.IsNotNull(csdlFilePath);
        }

        public void TestAddGeneratedClientCode_GeneratesT4Templates_AllSettingsSet()
        {
            var serviceName = "MyService";
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                EdmxVersion = Common.Constants.EdmxVersion4,
                ServiceName = serviceName,
                IncludeT4File = true,
                Endpoint = "https://service/$metadata",
                IncludeCustomHeaders = true,
                CustomHttpHeaders = "Key1:val\nKey2:val2",
                IncludeWebProxy = true,
                WebProxyHost = "http://localhost:8080",
                IncludeWebProxyNetworkCredentials = true,
                WebProxyNetworkCredentialsDomain = "domain",
                WebProxyNetworkCredentialsUsername = "username",
                WebProxyNetworkCredentialsPassword = "password",
                ExcludedSchemaTypes = new List<string>() { "Namespace.Type1", "Namespace.Type2", "Namespace.Type3" },
                ExcludedOperationImports = new List<string>() { "Operation1", "Operation2" },
                GeneratedFileNamePrefix = "Reference",
                UseNamespacePrefix = true,
                UseDataServiceCollection = true,
                NamespacePrefix = "MyNamespace",
                EnableNamingAlias = true,
                GenerateMultipleFiles = true,
                MakeTypesInternal = true,
                IgnoreUnexpectedElementsAndAttributes = true,
                OpenGeneratedFilesInIDE = true
            };

            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName, codeGenFactory, handlerHelper);

            codeGenDescriptor.AddGeneratedClientCodeAsync().Wait();

            var ttIncludeSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ODataT4CodeGenerator.ttinclude");
            var ttIncludeOutputPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.ttinclude");
            var fileManagerSourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "ODataT4CodeGenFilesManager.ttinclude");
            var fileManagerOutputPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "ODataT4CodeGenFilesManager.ttinclude");
            var ttOutputPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.tt");

            Assert.IsTrue(handlerHelper.AddedFiles.Contains((fileManagerOutputPath, fileManagerSourcePath)));
            var ttInclude = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == ttIncludeOutputPath);
            Assert.IsNotNull(ttInclude);
            var ttIncludeOriginalText = File.ReadAllText(ttIncludeSourcePath);
            var ttIncludeSavedText = File.ReadAllText(ttInclude.SourceFile);
            Assert.AreEqual(ttIncludeOriginalText, ttIncludeSavedText);
            var tt = handlerHelper.AddedFiles.FirstOrDefault(f => f.CreatedFile == ttOutputPath);
            Assert.IsNotNull(tt);
            var ttExpectedText = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", "TestConfigAllOptionsSet.txt"));
            ttExpectedText = ttExpectedText.Replace("$$CsdlFullPath$$", Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Csdl.xml"));
            var ttSavedText = File.ReadAllText(tt.SourceFile);
            Assert.AreEqual(ttExpectedText, ttSavedText);
        }

        [TestMethod]
        public void TestAddNugetPackagesAsync_ShouldInstallODataClientLibrariesIfNotAlreadyInstalled()
        {
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, "service");
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var descriptor = SetupCodeGenDescriptor(new ServiceConfigurationV4(), "service", codeGenFactory, new TestConnectedServiceHandlerHelper());

            descriptor.AddNugetPackagesAsync().Wait();

            var installer = descriptor.PackageInstaller as TestVsPackageInstaller;
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V4ClientNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V4EdmNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V4ODataNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V4SpatialNuGetPackage));
        }

        [TestMethod]
        public void TestAddNugetPackageAsync_ShouldNotInstalledODataClientLibrariesIfAlreadyInstalled()
        {
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, "service");
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var descriptor = SetupCodeGenDescriptor(new ServiceConfigurationV4(), "service", codeGenFactory, new TestConnectedServiceHandlerHelper());
            var installerServices = descriptor.PackageInstallerServices as TestVsPackageInstallerServices;
            installerServices.InstalledPackages.Add(Common.Constants.V4SpatialNuGetPackage);
            installerServices.InstalledPackages.Add(Common.Constants.V4EdmNuGetPackage);
            installerServices.InstalledPackages.Add(Common.Constants.V4ODataNuGetPackage);
            installerServices.InstalledPackages.Add(Common.Constants.V4ClientNuGetPackage);

            descriptor.AddNugetPackagesAsync().Wait();

            var installer = descriptor.PackageInstaller as TestVsPackageInstaller;
            Assert.IsFalse(installer.InstalledPackages.Contains(Common.Constants.V4ClientNuGetPackage));
            Assert.IsFalse(installer.InstalledPackages.Contains(Common.Constants.V4EdmNuGetPackage));
            Assert.IsFalse(installer.InstalledPackages.Contains(Common.Constants.V4ODataNuGetPackage));
            Assert.IsFalse(installer.InstalledPackages.Contains(Common.Constants.V4SpatialNuGetPackage));
        }

        [TestMethod]
        public void TestV3AddNugetPackageAsync_ShouldInstallODataLibrariesForV3()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = Common.Constants.EdmxVersion3
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(serviceConfig.Endpoint ?? MetadataUri, context, project);

            descriptor.AddNugetPackagesAsync().Wait();
            var installer = descriptor.PackageInstaller as TestVsPackageInstaller;

            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V3ClientNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V3EdmNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V3ODataNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Common.Constants.V3SpatialNuGetPackage));
        }

        [TestMethod]
        public void TestV3AddGeneratedClientCode_GeneratesCodeForv3()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var serviceConfig = new ServiceConfiguration()
            {
                Endpoint = Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", "SampleServiceV3.xml"),
                GeneratedFileNamePrefix = "Reference",
                EdmxVersion = Common.Constants.EdmxVersion3
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(serviceConfig.Endpoint, context, project);
            descriptor.AddGeneratedClientCodeAsync().Wait();
            var addedFile = handlerHelper.AddedFiles.FirstOrDefault();
            var generatedCode = File.ReadAllText(addedFile.SourceFile);
            var expectedCode = GeneratedCodeHelpers.LoadReferenceContent("SampleServiceV3.cs");

            Assert.IsNotNull(addedFile);
            Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.cs"), addedFile.CreatedFile);
            GeneratedCodeHelpers.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        [TestMethod]
        public void TestV3AddGeneratedClientCode_GeneratesCodeForv3_ForVB()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.VB);
            var serviceConfig = new ServiceConfiguration()
            {
                Endpoint = Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", "SampleServiceV3.xml"),
                GeneratedFileNamePrefix = "Reference",
                EdmxVersion = Common.Constants.EdmxVersion3
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(serviceConfig.Endpoint, context, project);
            descriptor.AddGeneratedClientCodeAsync().Wait();
            var addedFile = handlerHelper.AddedFiles.FirstOrDefault();
            var generatedCode = File.ReadAllText(addedFile.SourceFile);
            var expectedCode = GeneratedCodeHelpers.LoadReferenceContent("SampleServiceV3.vb");

            Assert.IsNotNull(addedFile);
            Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.vb"), addedFile.CreatedFile);
            GeneratedCodeHelpers.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        static V4CodeGenDescriptor SetupCodeGenDescriptor(ServiceConfiguration serviceConfig, string serviceName, IODataT4CodeGeneratorFactory codeGenFactory, TestConnectedServiceHandlerHelper handlerHelper, ODataT4CodeGenerator.LanguageOption targetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp)
        {
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, targetLanguage);
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            return new TestV4CodeGenDescriptor(serviceConfig.Endpoint ?? MetadataUri, context, project, codeGenFactory);
        }

        static Project CreateTestProject(string projectPath, ODataT4CodeGenerator.LanguageOption targetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp)
        {
            var fullPathPropertyMock = new Mock<Property>();
            fullPathPropertyMock.SetupGet(p => p.Value).Returns(projectPath);

            var projectPropertiesMock = new Mock<Properties>();
            projectPropertiesMock.Setup(p => p.Item(It.Is<string>(s => s == "FullPath")))
                .Returns(fullPathPropertyMock.Object);



            var projectMock = new Mock<Project>();
            projectMock.SetupGet(p => p.Properties)
                .Returns(projectPropertiesMock.Object);
            var projectCodeModelMock = new Mock<CodeModel>();
            if (targetLanguage == ODataT4CodeGenerator.LanguageOption.CSharp)
            {
                projectCodeModelMock.Setup(p => p.Language)
                    .Returns(EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp);
            }
            else
            {
                projectCodeModelMock.Setup(p => p.Language)
                    .Returns(EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB);
            }
            projectMock.SetupGet(p => p.CodeModel)
                .Returns(projectCodeModelMock.Object);
            return projectMock.Object;
        }
    }

    class TestV4CodeGenDescriptor : V4CodeGenDescriptor
    {
        public TestV4CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project, IODataT4CodeGeneratorFactory codeGenFactory)
            : base(metadataUri, context, project, codeGenFactory)
        {
        }
        protected override void Init()
        {
            PackageInstallerServices = new TestVsPackageInstallerServices();
            PackageInstaller = new TestVsPackageInstaller();
        }
    }

    class TestV3CodeGenDescriptor: V3CodeGenDescriptor
    {
        public TestV3CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project) : base(metadataUri, context, project)
        {
        }

        protected override void Init()
        {
            PackageInstallerServices = new TestVsPackageInstallerServices();
            PackageInstaller = new TestVsPackageInstaller();
        }
    }

    class TestODataT4CodeGenerator : ODataT4CodeGenerator
    {
        public override string TransformText()
        {
            return "Generated code";
        }
    }
    class TestODataT4CodeGeneratorUsingProxy : ODataT4CodeGenerator
    {

    }

    class TestODataT4CodeGeneratorFactory : IODataT4CodeGeneratorFactory
    {
        private ODataT4CodeGenerator generatorToReturn = null;

        public TestODataT4CodeGeneratorFactory(ODataT4CodeGenerator generatorToReturn = null)
        {
            this.generatorToReturn = generatorToReturn;
        }

        public ODataT4CodeGenerator LastCreatedInstance { get; private set; }

        public ODataT4CodeGenerator Create()
        {
            var generator = generatorToReturn ?? new TestODataT4CodeGenerator();
            LastCreatedInstance = generator;
            return generator;
        }
    }

    internal class TestODataT4NetworkCodeGeneratorFactory : IODataT4CodeGeneratorFactory
    {
        public ODataT4CodeGenerator LastCreatedInstance { get; private set; }

        public int MyProperty { get; set; }

        public string EDMX { get; set; }
        public ODataT4CodeGenerator Create()
        {
            var generator = new TestODataT4CodeGeneratorUsingProxy();
            LastCreatedInstance = generator;
            ODataT4CodeGenerator.CodeGenerationContext.RequestCreator = new TestHttpCreator(EDMX);

            return generator;
        }

        public class TestHttpCreator : ODataT4CodeGenerator.IHttpRequestCreator
        {
            internal Mock<HttpWebRequest> mock;
            private WebResponse response;

            public TestHttpCreator(string edmx)
            {

                var edmxStream = new MemoryStream(Encoding.ASCII.GetBytes(edmx));
                response = new TestWebResponse(edmxStream);
                this.mock = new Mock<HttpWebRequest>();
                mock.Setup(inst => inst.GetResponse()).Returns(response);
                mock.SetupProperty(inst => inst.Proxy);



            }

            public HttpWebRequest Create(System.Uri metadataUri)
            {
                return this.mock.Object;
            }
        }

        private class TestWebResponse : WebResponse
        {
            private readonly Stream stream;

            public TestWebResponse(Stream stream)
            {

                this.stream = stream;
            }
            public override Stream GetResponseStream()
            {
                return stream;
            }
        }
    }
}
