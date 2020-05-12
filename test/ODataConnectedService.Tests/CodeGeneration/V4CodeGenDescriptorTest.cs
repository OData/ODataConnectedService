//---------------------------------------------------------------------------------
// <copyright file="V4CodeGenDescriptorTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

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

namespace Microsoft.OData.ConnectedService.Tests.CodeGeneration
{
    [TestClass]
    public class V4CodeGenDescriptorTest
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
                        UseDataServiceCollection = true,
                        IgnoreUnexpectedElementsAndAttributes = true,
                        EnableNamingAlias = true,
                        NamespacePrefix = "Prefix",
                        MakeTypesInternal = true,
                        ExcludedOperationImports = new List<string>() { "Func1", "Action2" }
                    }
                };

                yield return new ServiceConfigurationV4[]
                {
                    new ServiceConfigurationV4()
                    {
                        UseDataServiceCollection = false,
                        IgnoreUnexpectedElementsAndAttributes = false,
                        EnableNamingAlias = false,
                        NamespacePrefix = null,
                        MakeTypesInternal = false,
                        ExcludedOperationImports = null
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
            // @todo add mock Httpcreator and work with it to create the files

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
                    .Returns("{B5E9BD34-6D3E-4B5D-925E-8A43B79820B4}"); // for C# = EnvDTE.CodeModelLanguageConstants.vsCMLanguageCSharp
            }
            else
            {
                projectCodeModelMock.Setup(p => p.Language)
                    .Returns("{B5E9BD33-6D3E-4B5D-925E-8A43B79820B4}"); // for VB = EnvDTE.CodeModelLanguageConstants.vsCMLanguageVB
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
        protected override void Init() { }
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
        public ODataT4CodeGenerator LastCreatedInstance { get; private set; }
        public ODataT4CodeGenerator Create()
        {
            var generator = new TestODataT4CodeGenerator();
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
