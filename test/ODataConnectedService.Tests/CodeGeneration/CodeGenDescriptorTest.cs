//---------------------------------------------------------------------------
// <copyright file="CodeGenDescriptorTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.CodeGen.CodeGeneration;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen.PackageInstallation;
using Microsoft.OData.CodeGen.Templates;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ODataConnectedService.Tests;
using ODataConnectedService.Tests.TestHelpers;

namespace Microsoft.OData.ConnectedService.Tests.CodeGeneration
{
    [TestClass]
    public class CodeGenDescriptorTest
    {
        readonly static string TestProjectRootPath = Path.Combine(Directory.GetCurrentDirectory(), "TempODataConnectedServiceTest");
        const string ServicesRootFolder = "ConnectedServicesRoot";
        const string MetadataUri = "http://service/$metadata";

        [TestInitialize]
        public void Init()
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

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
                        OmitVersioningInfo = true,
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
                        OmitVersioningInfo = false,
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
            if (configObject == null)
            {
                throw new ArgumentNullException(string.Format(CultureInfo.InvariantCulture, "configObject"));
            }
            var serviceConfig = configObject as ServiceConfigurationV4;
            serviceConfig.IncludeT4File = false;
            serviceConfig.Endpoint = "http://service/$metadata";

            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, "TestService");

            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, "TestService", codeGenFactory, handlerHelper);
            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();

            var generator = codeGenFactory.LastCreatedInstance;
            Assert.AreEqual(serviceConfig.UseDataServiceCollection, generator.UseDataServiceCollection);
            Assert.AreEqual(serviceConfig.EnableNamingAlias, generator.EnableNamingAlias);
            Assert.AreEqual(serviceConfig.IgnoreUnexpectedElementsAndAttributes, generator.IgnoreUnexpectedElementsAndAttributes);
            Assert.AreEqual(serviceConfig.MakeTypesInternal, generator.MakeTypesInternal);
            Assert.AreEqual(serviceConfig.OmitVersioningInfo, generator.OmitVersioningInfo);
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
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                MakeTypesInternal = true,
                UseDataServiceCollection = false,
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "MyFile",
                IncludeT4File = false,
                Endpoint = "http://localhost:9000"
            };
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName,
                new TestODataT4CodeGeneratorFactory(), handlerHelper);
            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();
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
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "Main",
                GenerateMultipleFiles = true,
                Endpoint = "http://localhost:9000"
            };
            var codeGen = new TestODataT4CodeGenerator();
            var codeGenFactory = new TestODataT4CodeGeneratorFactory(codeGen);
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName,
                codeGenFactory, handlerHelper);
            var template = new StringBuilder();
            codeGen.MultipleFilesManager = new ODataT4CodeGenerator.FilesManager(template);
            codeGen.MultipleFilesManager.StartNewFile("File1.cs", false);
            template.Append("Contents1");
            codeGen.MultipleFilesManager.EndBlock();
            codeGen.MultipleFilesManager.StartNewFile("File2.cs", false);
            template.Append("Contents2");
            codeGen.MultipleFilesManager.EndBlock();
            //The file manager expects the files to have been saved in the Temp directory
            // when ODataT4CodeGenerator.TransformText() was called. Since we're using a dummy code generator
            // we need to manually ensure those files exist
            codeGen.MultipleFilesManager.GenerateFilesAsync(true).Wait();
            var file1TempPath = codeGen.MultipleFilesManager.files[0].TemporaryFilePath;
            var file2TempPath = codeGen.MultipleFilesManager.files[1].TemporaryFilePath;
            try
            {
                codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();
                Assert.AreNotEqual(file1TempPath, file2TempPath);
                Assert.AreEqual(Path.Combine(Path.GetTempPath(), "File1.cs"), file1TempPath);
                Assert.AreEqual(Path.Combine(Path.GetTempPath(), "File2.cs"), file2TempPath);
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
            finally
            {
                File.Delete(file1TempPath);
                File.Delete(file2TempPath);
            }
        }

        [TestMethod]
        public void StartNewFile_WithNullName_ThrowsArgumentNullException()
        {
            var manager = new ODataT4CodeGenerator.FilesManager(new StringBuilder());

            Assert.ThrowsException<ArgumentNullException>(() => manager.StartNewFile(null, false));
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(".")]
        [DataRow("..")]
        [DataRow("../Outside.cs")]
        [DataRow(@"..\Outside.cs")]
        [DataRow("Folder/Outside.cs")]
        [DataRow(@"Folder\Outside.cs")]
        [DataRow(@"C:\Outside.cs")]
        [DataRow("Trailing.")]
        [DataRow("Trailing ")]
        public async Task GenerateFilesAsync_WithNonFileNameValue_ThrowsInvalidOperationException(string fileName)
        {
            await AssertGeneratedFileNameRejectedAsync(fileName).ConfigureAwait(false);
        }

        public static IEnumerable<object[]> ReservedFileNameTestData
        {
            get
            {
                string[] reservedFileNames =
                {
                    "CON", "PRN", "AUX", "NUL",
                    "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                    "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
                };

                foreach (string reservedFileName in reservedFileNames)
                {
                    yield return new object[] { $"{reservedFileName}.cs" };
                }

                yield return new object[] { "con.CS" };
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(ReservedFileNameTestData), DynamicDataSourceType.Property)]
        public async Task GenerateFilesAsync_WithReservedFileName_ThrowsInvalidOperationException(string fileName)
        {
            await AssertGeneratedFileNameRejectedAsync(fileName).ConfigureAwait(false);
        }

        public static IEnumerable<object[]> ControlCharacterTestData
        {
            get
            {
                for (int value = 0; value < 32; value++)
                {
                    yield return new object[] { $"Name{(char)value}Part.cs" };
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(ControlCharacterTestData), DynamicDataSourceType.Property)]
        public async Task GenerateFilesAsync_WithControlCharacter_ThrowsInvalidOperationException(string fileName)
        {
            await AssertGeneratedFileNameRejectedAsync(fileName).ConfigureAwait(false);
        }

        public static IEnumerable<object[]> InvalidGeneratedFileNameCharacterTestData
        {
            get
            {
                char[] invalidCharacters = { '/', '\\', ':', '"', '<', '>', '|', '?', '*' };
                foreach (char invalidCharacter in invalidCharacters)
                {
                    yield return new object[] { $"Name{invalidCharacter}Part.cs" };
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(InvalidGeneratedFileNameCharacterTestData), DynamicDataSourceType.Property)]
        public async Task GenerateFilesAsync_WithInvalidFileNameCharacter_ThrowsInvalidOperationException(string fileName)
        {
            await AssertGeneratedFileNameRejectedAsync(fileName).ConfigureAwait(false);
        }

        [DataTestMethod]
        [DataRow("COM0.cs")]
        [DataRow("COM10.cs")]
        [DataRow("LPT0.cs")]
        [DataRow("LPT10.cs")]
        [DataRow("Console.cs")]
        [DataRow("Name Part.cs")]
        public async Task GenerateFilesAsync_WithSimilarValidFileName_GeneratesFile(string fileName)
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            manager.StartNewFile(fileName, false);
            template.Append("Contents");
            manager.EndBlock();

            try
            {
                await manager.GenerateFilesAsync(true).ConfigureAwait(false);

                Assert.IsTrue(File.Exists(manager.files[0].TemporaryFilePath));
            }
            finally
            {
                File.Delete(manager.files[0].TemporaryFilePath);
            }
        }

        private static async Task AssertGeneratedFileNameRejectedAsync(string fileName)
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            manager.StartNewFile("Valid.cs", false);
            template.Append("Valid");
            manager.EndBlock();
            manager.StartNewFile(fileName, false);
            template.Append("Invalid");
            manager.EndBlock();

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => manager.GenerateFilesAsync(true)).ConfigureAwait(false);

            Assert.IsTrue(manager.files.All(block => block.TemporaryFilePath == null));
        }

        [TestMethod]
        public async Task GenerateFilesAsync_WhenSplitIsFalse_DoesNotCreateQueuedFile()
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            string fileName = "../NotGenerated.cs";
            manager.StartNewFile(fileName, false);
            template.Append("Contents");
            manager.EndBlock();

            await manager.GenerateFilesAsync(false).ConfigureAwait(false);

            Assert.IsNull(manager.files[0].TemporaryFilePath);
            Assert.AreEqual("Contents", template.ToString());
        }

        [TestMethod]
        public async Task GenerateFilesAsync_AfterBlocksAreCleared_DoesNotCreateFiles()
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            string fileName = $"Cleared-{Guid.NewGuid():N}.cs";
            string expectedPath = Path.Combine(Path.GetTempPath(), fileName);
            manager.StartNewFile(fileName, false);
            template.Append("Cleared contents");
            manager.EndBlock();
            manager.files.Clear();

            await manager.GenerateFilesAsync(true).ConfigureAwait(false);

            Assert.IsFalse(File.Exists(expectedPath));
        }

        [TestMethod]
        public async Task GenerateFilesAsync_AfterBlockIsRemovedAndAnotherIsAdded_GeneratesOnlyCurrentBlock()
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            string removedFileName = $"Removed-{Guid.NewGuid():N}.cs";
            string currentFileName = $"Current-{Guid.NewGuid():N}.cs";
            string removedPath = Path.Combine(Path.GetTempPath(), removedFileName);

            manager.StartNewFile(removedFileName, false);
            template.Append("Removed contents");
            manager.EndBlock();
            manager.files.RemoveAt(0);
            manager.StartNewFile(currentFileName, false);
            template.Append("Current contents");
            manager.EndBlock();

            try
            {
                await manager.GenerateFilesAsync(true).ConfigureAwait(false);

                Assert.IsFalse(File.Exists(removedPath));
                Assert.IsTrue(File.Exists(manager.files[0].TemporaryFilePath));
                Assert.AreEqual("Current contents", File.ReadAllText(manager.files[0].TemporaryFilePath));
            }
            finally
            {
                string temporaryFilePath = manager.files[0].TemporaryFilePath;
                if (File.Exists(temporaryFilePath))
                {
                    File.Delete(temporaryFilePath);
                }
            }
        }

        [DataTestMethod]
        [DataRow("Same.cs", "Same.cs")]
        [DataRow("Same.cs", "same.cs")]
        public async Task GenerateFilesAsync_WithDuplicateNames_RejectsBatchBeforeCreatingFiles(
            string firstFileName,
            string secondFileName)
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            manager.StartNewFile(firstFileName, false);
            template.Append("First");
            manager.EndBlock();
            manager.StartNewFile(secondFileName, false);
            template.Append("Second");
            manager.EndBlock();

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => manager.GenerateFilesAsync(true)).ConfigureAwait(false);

            Assert.IsTrue(manager.files.All(block => block.TemporaryFilePath == null));
        }

        [TestMethod]
        public async Task GenerateFilesAsync_WithChangedBlockName_RejectsOutputBeforeCreatingFile()
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            string outsideFileName = $"Outside-{Guid.NewGuid():N}.cs";
            string changedName = $"../{outsideFileName}";
            string outsidePath = Path.GetFullPath(Path.Combine(Path.GetTempPath(), changedName));
            manager.StartNewFile($"Original-{Guid.NewGuid():N}.cs", false);
            template.Append("Contents");
            manager.EndBlock();
            manager.files[0].Name = changedName;

            try
            {
                await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => manager.GenerateFilesAsync(true)).ConfigureAwait(false);

                Assert.IsFalse(File.Exists(outsidePath));
                Assert.IsNull(manager.files[0].TemporaryFilePath);
            }
            finally
            {
                if (File.Exists(outsidePath))
                {
                    File.Delete(outsidePath);
                }
            }
        }

        [TestMethod]
        public async Task CopyGeneratedFilesAsync_WithChangedBlockName_RejectsOutputBeforeAddingFile()
        {
            var template = new StringBuilder();
            var manager = new ODataT4CodeGenerator.FilesManager(template);
            var fileHandler = new Mock<IFileHandler>();
            var logger = new Mock<IMessageLogger>();
            string originalFileName = $"Original-{Guid.NewGuid():N}.cs";
            string outsideFileName = $"Outside-{Guid.NewGuid():N}.cs";
            string referenceFolder = Path.Combine(TestProjectRootPath, ServicesRootFolder, "MyService");
            string outsidePath = Path.GetFullPath(Path.Combine(referenceFolder, "..", outsideFileName));
            manager.StartNewFile(originalFileName, false);
            template.Append("Contents");
            manager.EndBlock();

            try
            {
                await manager.GenerateFilesAsync(true).ConfigureAwait(false);
                manager.files[0].Name = $"../{outsideFileName}";

                await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                    manager.CopyGeneratedFilesAsync(true, fileHandler.Object, logger.Object, referenceFolder, true, false)).ConfigureAwait(false);

                Assert.IsFalse(File.Exists(outsidePath));
                fileHandler.Verify(
                    handler => handler.AddFileAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ODataFileOptions>()),
                    Times.Never);
            }
            finally
            {
                string temporaryFilePath = manager.files[0].TemporaryFilePath;
                if (File.Exists(temporaryFilePath))
                {
                    File.Delete(temporaryFilePath);
                }
            }
        }

        [TestMethod]
        public void Test_GeneratesAndSavesCodeFileWithProxy()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
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

            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();
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
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
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

            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();
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
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
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
            var languageOption = ODataT4CodeGenerator.LanguageOption.CSharp;
            if (lang == "cs")
            {
                languageOption = ODataT4CodeGenerator.LanguageOption.CSharp;
            }
            else
            {
                languageOption = ODataT4CodeGenerator.LanguageOption.VB;
            }

            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, (LanguageOption)languageOption, serviceConfig).Wait();

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
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
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
            var languageOption = lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB;
            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, (LanguageOption)languageOption, serviceConfig).Wait();

            var csdlFilePath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, String.Concat(serviceName, "Csdl.xml"));
            Assert.IsNotNull(csdlFilePath);
        }

        public static void TestAddGeneratedClientCode_GeneratesT4Templates_AllSettingsSet()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion4,
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

            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();

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

        [DataTestMethod]
        [DataRow("cs", "TestConfigBasic.txt")]
        [DataRow("vb", "TestConfigBasicVB.txt")]
        public void TestAddGeneratedClientCode_GeneratesT4TemplateFiles_WithIncludeT4File_WithExcludedSchemaTypes(string lang, string referenceFile)
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                Endpoint = "https://service/$metadata",
                GeneratedFileNamePrefix = "Reference",
                IncludeT4File = true,
                ExcludedSchemaTypes = new List<string>()
                {
                    "Type1",
                    "Type2"
                }
            };

            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName, codeGenFactory, handlerHelper,
                lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB);
            var languageOption = lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB;
            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, (LanguageOption)languageOption, serviceConfig).Wait();

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
            ttExpectedText = Regex.Replace(ttExpectedText, "(public const string ExcludedSchemaTypes = )\"\";", "$1\"" + string.Join(",", serviceConfig.ExcludedSchemaTypes) + "\";");
            var ttSavedText = File.ReadAllText(tt.SourceFile);
            Assert.AreEqual(ttExpectedText, ttSavedText);
        }

        [DataTestMethod]
        [DataRow("cs", "TestConfigBasic.txt")]
        [DataRow("vb", "TestConfigBasicVB.txt")]
        public void TestAddGeneratedClientCode_GeneratesT4TemplateFiles_WithIncludeT4File_WithExcludedOperationImports(string lang, string referenceFile)
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                Endpoint = "https://service/$metadata",
                GeneratedFileNamePrefix = "Reference",
                IncludeT4File = true,
                ExcludedOperationImports = new List<string>()
                {
                    "OperationImport1",
                    "OperationImport2"
                }
            };

            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName, codeGenFactory, handlerHelper,
                lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB);

            var languageOption = lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB;

            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, (LanguageOption)languageOption, serviceConfig).Wait();

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
            ttExpectedText = Regex.Replace(ttExpectedText, "(public const string ExcludedOperationImports = )\"\";", "$1\"" + string.Join(",", (serviceConfig as ServiceConfigurationV4).ExcludedOperationImports) + "\";");
            var ttSavedText = File.ReadAllText(tt.SourceFile);
            Assert.AreEqual(ttExpectedText, ttSavedText);
        }

        [DataTestMethod]
        [DataRow("cs", "TestConfigBasic.txt")]
        [DataRow("vb", "TestConfigBasicVB.txt")]
        public void TestAddGeneratedClientCode_GeneratesT4TemplateFiles_WithIncludeT4File_WithExcludedBoundOperations(string lang, string referenceFile)
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            ServiceConfiguration serviceConfig = new ServiceConfigurationV4()
            {
                ServiceName = serviceName,
                Endpoint = "https://service/$metadata",
                GeneratedFileNamePrefix = "Reference",
                IncludeT4File = true,
                ExcludedBoundOperations = new List<string>()
                {
                    "BoundOperation1(Type1)",
                    "BoundOperation2(Type2)",
                    "BoundOperation3(Type1)"
                }
            };

            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var codeGenDescriptor = SetupCodeGenDescriptor(serviceConfig, serviceName, codeGenFactory, handlerHelper,
                lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB);

            var languageOption = lang == "cs" ? ODataT4CodeGenerator.LanguageOption.CSharp : ODataT4CodeGenerator.LanguageOption.VB;
            codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, (LanguageOption)languageOption, serviceConfig).Wait();

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
            ttExpectedText = Regex.Replace(ttExpectedText, "(public const string ExcludedBoundOperations = )\"\";", "$1\"" + string.Join(",", (serviceConfig as ServiceConfigurationV4).ExcludedBoundOperations) + "\";");
            var ttSavedText = File.ReadAllText(tt.SourceFile);
            Assert.AreEqual(ttExpectedText, ttSavedText);
        }

        [Ignore]
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
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4ClientNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4EdmNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4ODataNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4SpatialNuGetPackage));
        }

#if !VS2022PLUS
        [Ignore]
        [TestMethod]
        public void TestAddNugetPackageAsync_ShouldNotInstalledODataClientLibrariesIfAlreadyInstalled()
        {
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, "service");
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var codeGenFactory = new TestODataT4CodeGeneratorFactory();
            var descriptor = SetupCodeGenDescriptor(new ServiceConfigurationV4(), "service", codeGenFactory, new TestConnectedServiceHandlerHelper());
            var handlerHelper = new TestConnectedServiceHandlerHelper();
            var serviceName = "MyService";

            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = new ServiceConfigurationV4(),
                Name = serviceName
            };

            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var installerServices = new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)).PackageInstallerServices as TestVsPackageInstallerServices;
            installerServices.InstalledPackages.Add(Microsoft.OData.CodeGen.Common.Constants.V4SpatialNuGetPackage);
            installerServices.InstalledPackages.Add(Microsoft.OData.CodeGen.Common.Constants.V4EdmNuGetPackage);
            installerServices.InstalledPackages.Add(Microsoft.OData.CodeGen.Common.Constants.V4ODataNuGetPackage);
            installerServices.InstalledPackages.Add(Microsoft.OData.CodeGen.Common.Constants.V4ClientNuGetPackage);

            descriptor.AddNugetPackagesAsync().Wait();

            var installer = descriptor.PackageInstaller as TestVsPackageInstaller;
            Assert.IsFalse(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4ClientNuGetPackage));
            Assert.IsFalse(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4EdmNuGetPackage));
            Assert.IsFalse(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4ODataNuGetPackage));
            Assert.IsFalse(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V4SpatialNuGetPackage));
        }
#endif

        [Ignore]
        [TestMethod]
        public void TestV3AddNugetPackageAsync_ShouldInstallODataLibrariesForV3()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion3
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(new ConnectedServiceFileHandler(context, project, new TestThreadHelper()), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)));

            descriptor.AddNugetPackagesAsync().Wait();
            var installer = descriptor.PackageInstaller as TestVsPackageInstaller;

            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V3ClientNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V3EdmNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V3ODataNuGetPackage));
            Assert.IsTrue(installer.InstalledPackages.Contains(Microsoft.OData.CodeGen.Common.Constants.V3SpatialNuGetPackage));
        }

        [TestMethod]
        public void TestV2AddGeneratedClientCode_GeneratesCodeForv2()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var serviceConfig = new ServiceConfiguration()
            {
                Endpoint = Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", "SampleServiceV2.xml"),
                GeneratedFileNamePrefix = "Reference",
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion2
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(new ConnectedServiceFileHandler(context, project, new TestThreadHelper()), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)));
            descriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();
            var addedFile = handlerHelper.AddedFiles.FirstOrDefault();
            var generatedCode = File.ReadAllText(addedFile.SourceFile);
            var expectedCode = GeneratedCodeHelpers.LoadReferenceContent("SampleServiceV2.cs");

            Assert.IsNotNull(addedFile);
            Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.cs"), addedFile.CreatedFile);
            GeneratedCodeHelpers.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        [TestMethod]
        public void TestV2AddGeneratedClientCode_GeneratesCodeForv2_ForVB()
        {
            var serviceName = "MyService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.VB);
            var serviceConfig = new ServiceConfiguration()
            {
                Endpoint = Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", "SampleServiceV2.xml"),
                GeneratedFileNamePrefix = "Reference",
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion2
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(new ConnectedServiceFileHandler(context, project, new TestThreadHelper()), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)));
            descriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateVBCode, serviceConfig).Wait();
            var addedFile = handlerHelper.AddedFiles.FirstOrDefault();
            var generatedCode = File.ReadAllText(addedFile.SourceFile);
            var expectedCode = GeneratedCodeHelpers.LoadReferenceContent("SampleServiceV2.vb");

            Assert.IsNotNull(addedFile);
            Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.vb"), addedFile.CreatedFile);
            GeneratedCodeHelpers.VerifyGeneratedCode(expectedCode, generatedCode);
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
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion3
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(new ConnectedServiceFileHandler(context, project, new TestThreadHelper()), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)));
            descriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig).Wait();
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
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion3
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper();
            handlerHelper.ServicesRootFolder = ServicesRootFolder;
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var descriptor = new TestV3CodeGenDescriptor(new ConnectedServiceFileHandler(context, project, new TestThreadHelper()), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)));
            descriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateVBCode, serviceConfig).Wait();
            var addedFile = handlerHelper.AddedFiles.FirstOrDefault();
            var generatedCode = File.ReadAllText(addedFile.SourceFile);
            var expectedCode = GeneratedCodeHelpers.LoadReferenceContent("SampleServiceV3.vb");

            Assert.IsNotNull(addedFile);
            Assert.AreEqual(Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName, "Reference.vb"), addedFile.CreatedFile);
            GeneratedCodeHelpers.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        [TestMethod]
        public void TestV4AddGeneratedClientCode_WithODataClient9_EmitsNativeDateTimeTypes()
        {
            var generatedCode = GenerateV4CodeWithODataClientVersion("9.0.0");

            Assert.IsTrue(generatedCode.Contains("global::System.DateOnly OrderDate"),
                "Expected native System.DateOnly to be emitted for Microsoft.OData.Client 9.0.0.");
            Assert.IsTrue(generatedCode.Contains("global::System.TimeOnly OrderTime"),
                "Expected native System.TimeOnly to be emitted for Microsoft.OData.Client 9.0.0.");
        }

        [TestMethod]
        public void TestV4AddGeneratedClientCode_WithODataClientVersionLessThan9_EmitsEdmDateTimeTypes()
        {
            var generatedCode = GenerateV4CodeWithODataClientVersion("8.0.0");

            Assert.IsTrue(generatedCode.Contains("global::Microsoft.OData.Edm.Date OrderDate"),
                "Expected legacy Microsoft.OData.Edm.Date to be emitted for Microsoft.OData.Client 8.0.0.");
            Assert.IsTrue(generatedCode.Contains("global::Microsoft.OData.Edm.TimeOfDay OrderTime"),
                "Expected legacy Microsoft.OData.Edm.TimeOfDay to be emitted for Microsoft.OData.Client 8.0.0.");
        }

        /// <summary>
        /// Runs the real V4 code generator end-to-end against a metadata document that contains
        /// Edm.Date and Edm.TimeOfDay properties, using an injected package provider to simulate the
        /// installed Microsoft.OData.Client version, and returns the generated source code.
        /// </summary>
        static string GenerateV4CodeWithODataClientVersion(string odataClientVersion)
        {
            var serviceName = "DateTimeService";
            var referenceFolderPath = Path.Combine(TestProjectRootPath, ServicesRootFolder, serviceName);
            Directory.CreateDirectory(referenceFolderPath);
            Project project = CreateTestProject(TestProjectRootPath, ODataT4CodeGenerator.LanguageOption.CSharp);
            var serviceConfig = new ServiceConfigurationV4()
            {
                Endpoint = Path.Combine(Directory.GetCurrentDirectory(), "CodeGeneration", "SampleServiceV4WithDateOnlyAndTimeOnly.xml"),
                ServiceName = serviceName,
                GeneratedFileNamePrefix = "Reference",
                EdmxVersion = Microsoft.OData.CodeGen.Common.Constants.EdmxVersion4,
                IncludeT4File = false
            };
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                ServiceConfig = serviceConfig,
                Name = serviceName
            };

            var handlerHelper = new TestConnectedServiceHandlerHelper { ServicesRootFolder = ServicesRootFolder };
            ConnectedServiceHandlerContext context = new TestConnectedServiceHandlerContext(serviceInstance, handlerHelper);

            var packagesProvider = new FakeInstalledPackagesProvider(new InstalledPackageInfo("Microsoft.OData.Client", odataClientVersion));
            var fileHandler = new ConnectedServiceFileHandler(context, project, new TestThreadHelper(), new ConnectedServiceMessageLogger(context), packagesProvider);

            var descriptor = new TestV4CodeGenDescriptor(
                fileHandler,
                new ConnectedServiceMessageLogger(context),
                new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)),
                new ODataT4CodeGeneratorFactory());

            descriptor.AddGeneratedClientCodeAsync(serviceConfig.Endpoint, referenceFolderPath, LanguageOption.GenerateCSharpCode, serviceConfig)
                .ConfigureAwait(false).GetAwaiter().GetResult();

            var addedFile = handlerHelper.AddedFiles.FirstOrDefault();
            Assert.IsNotNull(addedFile, "The V4 code generator did not produce any output file.");

            return File.ReadAllText(addedFile.SourceFile);
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

            return new TestV4CodeGenDescriptor(new ConnectedServiceFileHandler(context, project, new TestThreadHelper()), new ConnectedServiceMessageLogger(context), new ConnectedServicePackageInstaller(context, project, new ConnectedServiceMessageLogger(context)), codeGenFactory);
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
        public TestV4CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger messageLogger, IPackageInstaller packageInstaller, IODataT4CodeGeneratorFactory codeGenFactory)
            : base(fileHandler, messageLogger, packageInstaller, codeGenFactory)
        {
        }
    }

    sealed class FakeInstalledPackagesProvider : IInstalledPackagesProvider
    {
        private readonly IReadOnlyList<InstalledPackageInfo> packages;

        public FakeInstalledPackagesProvider(params InstalledPackageInfo[] packages)
        {
            this.packages = packages;
        }

        public Task<IReadOnlyList<InstalledPackageInfo>> GetInstalledPackagesAsync()
        {
            return Task.FromResult(this.packages);
        }
    }

    class TestV3CodeGenDescriptor : V3CodeGenDescriptor
    {
        public TestV3CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger messageLogger, IPackageInstaller packageInstaller) : base(fileHandler, messageLogger, packageInstaller)
        {
        }

    }

    class TestODataT4CodeGenerator : ODataT4CodeGenerator
    {
        public override Task<string> TransformTextAsync()
        {
            return Task.FromResult("Generated code");
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

        public class TestHttpCreator : ODataT4CodeGenerator.IHttpRequestCreator, IDisposable
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

            public void Dispose()
            {
                response?.Dispose();
                GC.SuppressFinalize(this);
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
