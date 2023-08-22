//-----------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.Common;
using Xunit;

namespace ODataConnectedService.Tests
{
    public class ExtensionMethodsTests
    {
        private static ConnectedServiceUserSettings CreateUserSettings()
        {
            var userSettings = new ConnectedServiceUserSettings(configName: "TestSettings");

            userSettings.CustomHttpHeaders = "Key:Test";
            userSettings.EnableNamingAlias = true;
            userSettings.Endpoint = "http://test";
            userSettings.ExcludedBoundOperations = new List<string> { "TestBoundOperation" };
            userSettings.ExcludedOperationImports = new List<string> { "TestOperationImport" };
            userSettings.ExcludedSchemaTypes = new List<string> { "TestSchemaType" };
            userSettings.GeneratedFileNamePrefix = "Test";
            userSettings.GenerateMultipleFiles = true;
            userSettings.IgnoreUnexpectedElementsAndAttributes = true;
            userSettings.IncludeCustomHeaders = true;
            userSettings.IncludeT4File = true;
            userSettings.IncludeWebProxy = true;
            userSettings.IncludeWebProxyNetworkCredentials = true;
            userSettings.MakeTypesInternal = true;
            userSettings.OmitVersioningInfo = true;
            userSettings.NamespacePrefix = "Test";
            userSettings.OpenGeneratedFilesInIDE = true;
            userSettings.ServiceName = "Test";
            userSettings.UseDataServiceCollection = true;
            userSettings.UseNamespacePrefix = true;
            userSettings.WebProxyHost = "Test";
            userSettings.WebProxyNetworkCredentialsDomain = "Test";
            userSettings.WebProxyNetworkCredentialsPassword = "Test";
            userSettings.WebProxyNetworkCredentialsUsername = "Test";
            userSettings.AddMruEndpoint("http://test");

            return userSettings;
        }

        private static ServiceConfigurationV4 CreateServiceConfiguration()
        {
            var serviceConfig = new ServiceConfigurationV4();
            serviceConfig.CustomHttpHeaders = "Key:Test";
            serviceConfig.EdmxVersion = new Version(4, 0, 0, 0);
            serviceConfig.EnableNamingAlias = true;
            serviceConfig.Endpoint = "http://test";
            serviceConfig.ExcludedBoundOperations = new List<string> { "TestBoundOperation" };
            serviceConfig.ExcludedOperationImports = new List<string> { "TestOperationImport" };
            serviceConfig.ExcludedSchemaTypes = new List<string> { "TestSchemaType" };
            serviceConfig.GeneratedFileNamePrefix = "Test";
            serviceConfig.GenerateMultipleFiles = true;
            serviceConfig.IgnoreUnexpectedElementsAndAttributes = true;
            serviceConfig.IncludeCustomHeaders = true;
            serviceConfig.IncludeT4File = true;
            serviceConfig.IncludeWebProxy = true;
            serviceConfig.IncludeWebProxyNetworkCredentials = true;
            serviceConfig.MakeTypesInternal = true;
            serviceConfig.OmitVersioningInfo = true;
            serviceConfig.NamespacePrefix = "Test";
            serviceConfig.OpenGeneratedFilesInIDE = true;
            serviceConfig.ServiceName = "Test";
            serviceConfig.UseDataServiceCollection = true;
            serviceConfig.UseNamespacePrefix = true;
            serviceConfig.WebProxyHost = "Test";
            serviceConfig.WebProxyNetworkCredentialsDomain = "Test";
            serviceConfig.WebProxyNetworkCredentialsPassword = "Test";
            serviceConfig.WebProxyNetworkCredentialsUsername = "Test";

            return serviceConfig;
        }

        [Fact]
        public void TestCopyPropertiesFromUserSettingsToServiceConfiguration()
        {
            var userSettings = CreateUserSettings();

            var serviceConfig = new ServiceConfigurationV4();

            serviceConfig.CopyPropertiesFrom(userSettings);

            Assert.Equal("Key:Test", serviceConfig.CustomHttpHeaders);
            Assert.True(serviceConfig.EnableNamingAlias);
            Assert.Equal("http://test", serviceConfig.Endpoint);
            Assert.Single(serviceConfig.ExcludedBoundOperations, "TestBoundOperation");
            Assert.Single(serviceConfig.ExcludedOperationImports, "TestOperationImport");
            Assert.Single(serviceConfig.ExcludedSchemaTypes, "TestSchemaType");
            Assert.Equal("Test", serviceConfig.GeneratedFileNamePrefix);
            Assert.True(serviceConfig.GenerateMultipleFiles);
            Assert.True(serviceConfig.IgnoreUnexpectedElementsAndAttributes);
            Assert.True(serviceConfig.IncludeCustomHeaders);
            Assert.True(serviceConfig.IncludeT4File);
            Assert.True(serviceConfig.IncludeWebProxy);
            Assert.True(serviceConfig.IncludeWebProxyNetworkCredentials);
            Assert.True(serviceConfig.MakeTypesInternal);
            Assert.True(serviceConfig.OmitVersioningInfo);
            Assert.Equal("Test", serviceConfig.NamespacePrefix);
            Assert.True(serviceConfig.OpenGeneratedFilesInIDE);
            Assert.Equal("Test", serviceConfig.ServiceName);
            Assert.True(serviceConfig.UseDataServiceCollection);
            Assert.True(serviceConfig.UseNamespacePrefix);
            Assert.Equal("Test", serviceConfig.WebProxyHost);
            Assert.Equal("Test", serviceConfig.WebProxyNetworkCredentialsDomain);
            Assert.Equal("Test", serviceConfig.WebProxyNetworkCredentialsPassword);
            Assert.Equal("Test", serviceConfig.WebProxyNetworkCredentialsUsername);
        }

        [Fact]
        public void TestCopyPropertiesFromServiceConfigurationToUserSettings()
        {
            var serviceConfig = CreateServiceConfiguration();

            var userSettings = new ConnectedServiceUserSettings(configName: "TestUserSettings");

            userSettings.CopyPropertiesFrom(serviceConfig);

            Assert.Equal("Key:Test", userSettings.CustomHttpHeaders);
            Assert.True(userSettings.EnableNamingAlias);
            Assert.Equal("http://test", userSettings.Endpoint);
            Assert.Single(userSettings.ExcludedBoundOperations, "TestBoundOperation");
            Assert.Single(userSettings.ExcludedOperationImports, "TestOperationImport");
            Assert.Single(userSettings.ExcludedSchemaTypes, "TestSchemaType");
            Assert.Equal("Test", userSettings.GeneratedFileNamePrefix);
            Assert.True(userSettings.GenerateMultipleFiles);
            Assert.True(userSettings.IgnoreUnexpectedElementsAndAttributes);
            Assert.True(userSettings.IncludeCustomHeaders);
            Assert.True(userSettings.IncludeT4File);
            Assert.True(userSettings.IncludeWebProxy);
            Assert.True(userSettings.IncludeWebProxyNetworkCredentials);
            Assert.True(userSettings.MakeTypesInternal);
            Assert.True(userSettings.OmitVersioningInfo);
            Assert.Equal("Test", userSettings.NamespacePrefix);
            Assert.True(userSettings.OpenGeneratedFilesInIDE);
            Assert.Equal("Test", userSettings.ServiceName);
            Assert.True(userSettings.UseDataServiceCollection);
            Assert.True(userSettings.UseNamespacePrefix);
            Assert.Equal("Test", userSettings.WebProxyHost);
            Assert.Equal("Test", userSettings.WebProxyNetworkCredentialsDomain);
            Assert.Equal("Test", userSettings.WebProxyNetworkCredentialsPassword);
            Assert.Equal("Test", userSettings.WebProxyNetworkCredentialsUsername);
        }
    }
}
