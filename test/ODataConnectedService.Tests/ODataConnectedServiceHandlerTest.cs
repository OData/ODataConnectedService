//-----------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceHandlerTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.CodeGeneration;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen.PackageInstallation;
using Microsoft.OData.ConnectedService.Tests.TestHelpers;
using Microsoft.VisualStudio.Shell.Interop;
using Moq;
using Xunit;

namespace Microsoft.OData.ConnectedService.Tests
{
    public class ODataConnectedServiceHandlerTest
    {
        [StaTheory]
        [InlineData("AddServiceInstanceAsync", 4, "V4")]
        [InlineData("AddServiceInstanceAsync", 3, "V3")]
        [InlineData("AddServiceInstanceAsync", 2, "V3")]
        [InlineData("AddServiceInstanceAsync", 1, "V3")]
        [InlineData("UpdateServiceInstanceAsync", 4, "V4")]
        [InlineData("UpdateServiceInstanceAsync", 3, "V3")]
        [InlineData("UpdateServiceInstanceAsync", 2, "V3")]
        [InlineData("UpdateServiceInstanceAsync", 1, "V3")]
        public async Task TestUpdateServiceInstance_GeneratesCodeAndSavesConfigAsync(string method, int edmxVersion, string generatorVersion)
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
            await (typeof(ODataConnectedServiceHandler).GetMethod(method).Invoke(
                serviceHandler, new object[] { context, tokenSource.Token }) as Task);
            var descriptor = descriptorFactory.CreatedInstance as TestCodeGenDescriptor;
            Assert.True(descriptor.AddedClientCode);
            Assert.True(descriptor.AddedNugetPackages);
            Assert.Equal(generatorVersion, descriptor.Version);
            Assert.Equal(serviceConfig, context.SavedExtendedDesignData);
        }

        [StaTheory]
        [InlineData("AddServiceInstanceAsync", 4, false)]
        [InlineData("AddServiceInstanceAsync", 4, true)]
        [InlineData("UpdateServiceInstanceAsync", 4, false)]
        [InlineData("UpdateServiceInstanceAsync", 4, true)]
        public async Task TestAddUpdateServiceInstance_SavesCustomHttpHeadersToDesignerDataAccordingToStoreCustomHttpHeaders(string method, int edmxVersion, bool store)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            const string headerValue = @"Authorization: Bearer xyz12345-randomstring";
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(edmxVersion, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true,
                IncludeCustomHeaders = true,
                CustomHttpHeaders = headerValue,
                StoreCustomHttpHeaders = store
            };
            var tokenSource = new CancellationTokenSource();
            var context = SetupContext(serviceConfig);
            await (typeof(ODataConnectedServiceHandler).GetMethod(method).Invoke(
                serviceHandler, new object[] { context, tokenSource.Token }) as Task);

            // CustomHttpHeaders should be null when StoreCustomHttpHeaders is false since we are not saving them to DesignerData
            Assert.Equal(serviceConfig, context.SavedExtendedDesignData);
            Assert.Equal(serviceConfig.CustomHttpHeaders, store ? headerValue : null);
        }

        [StaTheory]
        [InlineData("AddServiceInstanceAsync", 4, false)]
        [InlineData("AddServiceInstanceAsync", 4, true)]
        [InlineData("UpdateServiceInstanceAsync", 4, false)]
        [InlineData("UpdateServiceInstanceAsync", 4, true)]
        public async Task TestAddUpdateServiceInstance_SavesWebProxyDetailsToDesignerDataAccordingToStoreWebProxyNetworkCredentialsAsync(string method, int edmxVersion, bool store)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            const string username = "user";
            const string password = "pass";
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(edmxVersion, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true,
                IncludeWebProxy = true,
                IncludeWebProxyNetworkCredentials = true,
                WebProxyHost = "http://example.com:80",
                WebProxyNetworkCredentialsUsername = username,
                WebProxyNetworkCredentialsPassword = password,
                StoreWebProxyNetworkCredentials = store
            };
            var tokenSource = new CancellationTokenSource();
            var context = SetupContext(serviceConfig);
            await (typeof(ODataConnectedServiceHandler).GetMethod(method).Invoke(
                serviceHandler, new object[] { context, tokenSource.Token }) as Task);

            // WebProxy username and password should be null when StoreWebProxyNetworkCredentials is false since we are not saving them to DesignerData
            Assert.Equal(serviceConfig, context.SavedExtendedDesignData);
            Assert.Equal(serviceConfig.WebProxyNetworkCredentialsUsername, store ? username : null);
            Assert.Equal(serviceConfig.WebProxyNetworkCredentialsPassword, store ? password : null);
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
        protected override BaseCodeGenDescriptor CreateV3CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger messageLogger, IPackageInstaller packageInstaller)
        {
            var descriptor = new TestCodeGenDescriptor(fileHandler, messageLogger, packageInstaller);
            descriptor.Version = "V3";
            CreatedInstance = CreatedInstance ?? descriptor;
            return descriptor;
        }

        protected override BaseCodeGenDescriptor CreateV4CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger messageLogger, IPackageInstaller packageInstaller)
        {
            var descriptor = new TestCodeGenDescriptor(fileHandler, messageLogger, packageInstaller);
            descriptor.Version = "V4";
            CreatedInstance = CreatedInstance ?? descriptor;
            return descriptor;
        }
    }

    class TestCodeGenDescriptor : BaseCodeGenDescriptor
    {
        public TestCodeGenDescriptor(IFileHandler fileHandler, IMessageLogger messageLogger, IPackageInstaller packageInstaller)
            : base(fileHandler, messageLogger, packageInstaller)
        {
            ClientDocUri = "https://odata.org";
        }
        public string Version { get; set; }
        public bool AddedClientCode { get; private set; }
        public bool AddedNugetPackages { get; private set; }

        public override Task AddGeneratedClientCodeAsync(string metadataUri, string outputDirectory, LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
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
