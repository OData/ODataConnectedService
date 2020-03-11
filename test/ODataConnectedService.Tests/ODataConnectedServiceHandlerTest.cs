// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.ConnectedService.CodeGeneration;
using Microsoft.OData.ConnectedService.Models;
using Moq;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;

namespace Microsoft.OData.ConnectedService.Tests
{
    [TestClass]
    public class ODataConnectedServiceHandlerTest
    {
        [DataTestMethod]
        [DataRow("AddServiceInstanceAsync", 4, "V4")]
        [DataRow("AddServiceInstanceAsync", 3, "V3")]
        [DataRow("AddServiceInstanceAsync", 2, "V3")]
        [DataRow("AddServiceInstanceAsync", 1, "V3")]
        [DataRow("UpdateServiceInstanceAsync", 4, "V4")]
        [DataRow("UpdateServiceInstanceAsync", 3, "V3")]
        [DataRow("UpdateServiceInstanceAsync", 2, "V3")]
        [DataRow("UpdateServiceInstanceAsync", 1, "V3")]
        public void TestUpdateServiceInstance_GeneratesCodeAndSavesConfig(string method, int edmxVersion, string generatorVersion)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(edmxVersion, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true
            };
            var tokenSource = new CancellationTokenSource();
            var context = SetupContext(serviceConfig);
            (typeof(ODataConnectedServiceHandler).GetMethod(method).Invoke(
                serviceHandler, new object[] { context, tokenSource.Token }) as Task).Wait();
            var descriptor = descriptorFactory.CreatedInstance as TestCodeGenDescriptor;
            Assert.IsTrue(descriptor.AddedClientCode);
            Assert.IsTrue(descriptor.AddedNugetPackages);
            Assert.AreEqual(generatorVersion, descriptor.Version);
            Assert.AreEqual(serviceConfig, context.SavedExtendedDesignData);
        }

        static TestConnectedServiceHandlerContext SetupContext(ServiceConfiguration serviceConfig)
        {
            var serviceInstance = new ODataConnectedServiceInstance()
            {
                Name = "TestService",
                MetadataTempFilePath = "http://service/$metadata",
                ServiceConfig = serviceConfig
            };
            var projectHierarchyMock = new Mock<IVsHierarchy>();
            object project;
            projectHierarchyMock.Setup(h => h.GetProperty(It.IsAny<uint>(), It.IsAny<int>(), out project));
            var context = new TestConnectedServiceHandlerContext(
                serviceInstance: serviceInstance, projectHierarchy: projectHierarchyMock.Object);
            return context;
        }
    }

    class TestCodeGenDescriptorFactory: CodeGenDescriptorFactory
    {
        public BaseCodeGenDescriptor CreatedInstance { get; private set; }
        protected override BaseCodeGenDescriptor CreateV3CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
        {
            var descriptor = new TestCodeGenDescriptor(metadataUri, context, project);
            descriptor.Version = "V3";
            CreatedInstance = CreatedInstance ?? descriptor;
            return descriptor;
        }

        protected override BaseCodeGenDescriptor CreateV4CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
        {
            var descriptor = new TestCodeGenDescriptor(metadataUri, context, project);
            descriptor.Version = "V4";
            CreatedInstance = CreatedInstance ?? descriptor;
            return descriptor;
        }
    }

    class TestCodeGenDescriptor : BaseCodeGenDescriptor
    {
        public TestCodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
            : base(metadataUri, context, project)
        {
            ClientDocUri = "https://odata.org";
        }
        public string Version { get; set; }
        public bool AddedClientCode { get; private set; }
        public bool AddedNugetPackages { get; private set; }

        protected override void Init() { }
        public override Task AddGeneratedClientCodeAsync()
        {
            AddedClientCode = true;
            return Task.CompletedTask;
        }

        public override Task AddNugetPackagesAsync()
        {
            AddedNugetPackages = true;
            return Task.CompletedTask;
        }
    }
}
