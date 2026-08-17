//-----------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceHandlerTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
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
        public static IEnumerable<object[]> CustomHeaderCases
        {
            get
            {
                string[] methods = { "AddServiceInstanceAsync", "UpdateServiceInstanceAsync" };
                string[] values = { null, string.Empty, "X-One: value", "X-One: value\nX-Two: another-value" };

                foreach (string method in methods)
                {
                    foreach (bool store in new[] { false, true })
                    {
                        foreach (string value in values)
                        {
                            yield return new object[] { method, store, value };
                        }
                    }
                }
            }
        }

        public static IEnumerable<object[]> WebProxyCredentialCases
        {
            get
            {
                string[] methods = { "AddServiceInstanceAsync", "UpdateServiceInstanceAsync" };
                object[][] values =
                {
                    new object[] { null, null },
                    new object[] { string.Empty, string.Empty },
                    new object[] { "user", null },
                    new object[] { "user", "password" }
                };

                foreach (string method in methods)
                {
                    foreach (bool store in new[] { false, true })
                    {
                        foreach (object[] value in values)
                        {
                            yield return new object[] { method, store, value[0], value[1] };
                        }
                    }
                }
            }
        }

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
            var context = SetupContext(serviceConfig);

            await InvokeHandlerAsync(serviceHandler, context, method);

            var descriptor = descriptorFactory.CreatedInstance as TestCodeGenDescriptor;
            Assert.True(descriptor.AddedClientCode);
            Assert.True(descriptor.AddedNugetPackages);
            Assert.Equal(generatorVersion, descriptor.Version);
            Assert.Same(serviceConfig, context.SavedExtendedDesignData);
            Assert.Equal(serviceConfig.ServiceName, ((ServiceConfiguration)context.SavedExtendedDesignData).ServiceName);
        }

        [StaTheory]
        [MemberData(nameof(CustomHeaderCases))]
        public async Task TestAddUpdateServiceInstance_EndToEndDesignerDataOmitsCustomHttpHeadersAsync(string method, bool store, string headerValue)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true,
                IncludeCustomHeaders = true,
                CustomHttpHeaders = headerValue,
                StoreCustomHttpHeaders = store
            };
            var context = SetupContext(serviceConfig);

            await InvokeHandlerAsync(serviceHandler, context, method);

            var savedServiceConfig = (ServiceConfiguration)context.SavedExtendedDesignData;
            var descriptor = (TestCodeGenDescriptor)descriptorFactory.CreatedInstance;
            AssertPersistedCopyExpected(serviceConfig, savedServiceConfig, headerValue != null || store);
            Assert.Null(savedServiceConfig.CustomHttpHeaders);
            Assert.False(savedServiceConfig.StoreCustomHttpHeaders);
            Assert.Equal(headerValue, serviceConfig.CustomHttpHeaders);
            Assert.Equal(headerValue, descriptor.GeneratedServiceConfiguration.CustomHttpHeaders);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.CustomHttpHeaders)}\":", context.SavedExtendedDesignDataJson);
            Assert.Contains($"\"{nameof(ServiceConfiguration.ServiceName)}\":\"TestService\"", context.SavedExtendedDesignDataJson);
        }

        [StaTheory]
        [MemberData(nameof(WebProxyCredentialCases))]
        public async Task TestAddUpdateServiceInstance_EndToEndDesignerDataOmitsWebProxyCredentialsAsync(
            string method,
            bool store,
            string username,
            string password)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                UseDataServiceCollection = false,
                MakeTypesInternal = true,
                IncludeWebProxy = true,
                IncludeWebProxyNetworkCredentials = true,
                WebProxyHost = "http://example.com:80",
                WebProxyNetworkCredentialsDomain = "example",
                WebProxyNetworkCredentialsUsername = username,
                WebProxyNetworkCredentialsPassword = password,
                StoreWebProxyNetworkCredentials = store
            };
            var context = SetupContext(serviceConfig);

            await InvokeHandlerAsync(serviceHandler, context, method);

            var savedServiceConfig = (ServiceConfiguration)context.SavedExtendedDesignData;
            var descriptor = (TestCodeGenDescriptor)descriptorFactory.CreatedInstance;
            AssertPersistedCopyExpected(serviceConfig, savedServiceConfig, username != null || password != null || store);
            Assert.Null(savedServiceConfig.WebProxyNetworkCredentialsUsername);
            Assert.Null(savedServiceConfig.WebProxyNetworkCredentialsPassword);
            Assert.False(savedServiceConfig.StoreWebProxyNetworkCredentials);
            Assert.Equal(username, serviceConfig.WebProxyNetworkCredentialsUsername);
            Assert.Equal(password, serviceConfig.WebProxyNetworkCredentialsPassword);
            Assert.Equal(username, descriptor.GeneratedServiceConfiguration.WebProxyNetworkCredentialsUsername);
            Assert.Equal(password, descriptor.GeneratedServiceConfiguration.WebProxyNetworkCredentialsPassword);
            Assert.Equal("http://example.com:80", savedServiceConfig.WebProxyHost);
            Assert.Equal("example", savedServiceConfig.WebProxyNetworkCredentialsDomain);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsUsername)}\":", context.SavedExtendedDesignDataJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsPassword)}\":", context.SavedExtendedDesignDataJson);
            Assert.Contains($"\"{nameof(ServiceConfiguration.WebProxyHost)}\":\"http://example.com:80\"", context.SavedExtendedDesignDataJson);
            Assert.Contains($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsDomain)}\":\"example\"", context.SavedExtendedDesignDataJson);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("X-One: value", "user", null)]
        [InlineData("X-One: value\nX-Two: another-value", "user", "password")]
        public void ServiceConfigurationSerialization_OmitsOptionalRequestValues(
            string customHttpHeaders,
            string username,
            string password)
        {
            var serviceConfig = new ServiceConfiguration()
            {
                ServiceName = "TestService",
                CustomHttpHeaders = customHttpHeaders,
                WebProxyNetworkCredentialsUsername = username,
                WebProxyNetworkCredentialsPassword = password
            };

            var newtonsoftJson = Newtonsoft.Json.JsonConvert.SerializeObject(serviceConfig);
            var systemTextJson = System.Text.Json.JsonSerializer.Serialize(serviceConfig);
            var dataContractXml = SerializeWithDataContract(serviceConfig);

            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.CustomHttpHeaders)}\":", newtonsoftJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsUsername)}\":", newtonsoftJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsPassword)}\":", newtonsoftJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.CustomHttpHeaders)}\":", systemTextJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsUsername)}\":", systemTextJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsPassword)}\":", systemTextJson);
            Assert.DoesNotContain($"<{nameof(ServiceConfiguration.CustomHttpHeaders)}", dataContractXml);
            Assert.DoesNotContain($"<{nameof(ServiceConfiguration.WebProxyNetworkCredentialsUsername)}", dataContractXml);
            Assert.DoesNotContain($"<{nameof(ServiceConfiguration.WebProxyNetworkCredentialsPassword)}", dataContractXml);
            Assert.Contains($"\"{nameof(ServiceConfiguration.ServiceName)}\":\"TestService\"", newtonsoftJson);
            Assert.Contains($"\"{nameof(ServiceConfiguration.ServiceName)}\":\"TestService\"", systemTextJson);
            Assert.Contains($"<{nameof(ServiceConfiguration.ServiceName)}>TestService</{nameof(ServiceConfiguration.ServiceName)}>", dataContractXml);
        }

        [StaTheory]
        [InlineData("AddServiceInstanceAsync")]
        [InlineData("UpdateServiceInstanceAsync")]
        public async Task TestAddUpdateServiceInstance_EndToEndDesignerDataOmitsValuesAfterClearAndReAddAsync(string method)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            var serviceConfig = new ServiceConfiguration()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                StoreCustomHttpHeaders = true,
                CustomHttpHeaders = "X-Old: old-value",
                StoreWebProxyNetworkCredentials = true,
                WebProxyNetworkCredentialsUsername = "old-user",
                WebProxyNetworkCredentialsPassword = "old-password"
            };
            var context = SetupContext(serviceConfig);

            await InvokeHandlerAsync(serviceHandler, context, method);
            AssertPersistedOptionalRequestValuesAreOmitted(context);

            serviceConfig.CustomHttpHeaders = null;
            serviceConfig.WebProxyNetworkCredentialsUsername = null;
            serviceConfig.WebProxyNetworkCredentialsPassword = null;
            await InvokeHandlerAsync(serviceHandler, context, method);
            AssertPersistedOptionalRequestValuesAreOmitted(context);

            serviceConfig.CustomHttpHeaders = "X-New: new-value";
            serviceConfig.WebProxyNetworkCredentialsUsername = "new-user";
            serviceConfig.WebProxyNetworkCredentialsPassword = "new-password";
            await InvokeHandlerAsync(serviceHandler, context, method);
            AssertPersistedOptionalRequestValuesAreOmitted(context);

            Assert.Equal("X-New: new-value", serviceConfig.CustomHttpHeaders);
            Assert.Equal("new-user", serviceConfig.WebProxyNetworkCredentialsUsername);
            Assert.Equal("new-password", serviceConfig.WebProxyNetworkCredentialsPassword);
        }

        [StaTheory]
        [InlineData("AddServiceInstanceAsync")]
        [InlineData("UpdateServiceInstanceAsync")]
        public async Task TestAddUpdateServiceInstance_PreservesV4DesignerSettingsAsync(string method)
        {
            var descriptorFactory = new TestCodeGenDescriptorFactory();
            var serviceHandler = new ODataConnectedServiceHandler(descriptorFactory);
            var serviceConfig = new ServiceConfigurationV4()
            {
                EdmxVersion = new Version(4, 0, 0, 0),
                ServiceName = "TestService",
                EnableNamingAlias = true,
                CustomHttpHeaders = "X-One: value"
            };
            var context = SetupContext(serviceConfig);

            await InvokeHandlerAsync(serviceHandler, context, method);

            var savedServiceConfig = Assert.IsType<ServiceConfigurationV4>(context.SavedExtendedDesignData);
            Assert.True(savedServiceConfig.EnableNamingAlias);
            Assert.Equal("TestService", savedServiceConfig.ServiceName);
            Assert.Null(savedServiceConfig.CustomHttpHeaders);
        }

        private static void AssertPersistedCopyExpected(
            ServiceConfiguration serviceConfig,
            ServiceConfiguration savedServiceConfig,
            bool copyExpected)
        {
            if (copyExpected)
            {
                Assert.NotSame(serviceConfig, savedServiceConfig);
            }
            else
            {
                Assert.Same(serviceConfig, savedServiceConfig);
            }
        }

        private static void AssertPersistedOptionalRequestValuesAreOmitted(TestConnectedServiceHandlerContext context)
        {
            var savedServiceConfig = (ServiceConfiguration)context.SavedExtendedDesignData;
            Assert.Null(savedServiceConfig.CustomHttpHeaders);
            Assert.Null(savedServiceConfig.WebProxyNetworkCredentialsUsername);
            Assert.Null(savedServiceConfig.WebProxyNetworkCredentialsPassword);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.CustomHttpHeaders)}\":", context.SavedExtendedDesignDataJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsUsername)}\":", context.SavedExtendedDesignDataJson);
            Assert.DoesNotContain($"\"{nameof(ServiceConfiguration.WebProxyNetworkCredentialsPassword)}\":", context.SavedExtendedDesignDataJson);
        }

        private static async Task InvokeHandlerAsync(
            ODataConnectedServiceHandler serviceHandler,
            TestConnectedServiceHandlerContext context,
            string method)
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                await (typeof(ODataConnectedServiceHandler).GetMethod(method).Invoke(
                    serviceHandler, new object[] { context, tokenSource.Token }) as Task);
            }
        }

        private static string SerializeWithDataContract(ServiceConfiguration serviceConfig)
        {
            var serializer = new DataContractSerializer(typeof(ServiceConfiguration));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, serviceConfig);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
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
        public ServiceConfiguration GeneratedServiceConfiguration { get; private set; }

        public override Task AddGeneratedClientCodeAsync(string metadataUri, string outputDirectory, LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
        {
            AddedClientCode = true;
            GeneratedServiceConfiguration = serviceConfiguration;
            return Task.CompletedTask;
        }

        public override Task AddNugetPackagesAsync()
        {
            AddedNugetPackages = true;
            return Task.CompletedTask;
        }
    }
}
