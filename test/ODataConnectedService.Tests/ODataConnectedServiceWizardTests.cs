//-----------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceWizardTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.OData.ConnectedService;
using System.ComponentModel;
using Microsoft.OData.ConnectedService.Models;
using FluentAssertions;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Views;
using System.IO;
using System.Windows.Documents;
using System.Windows;
using System.Linq;
using Xunit;

namespace ODataConnectedService.Tests
{

    public class ODataConnectedServiceWizardTests: IDisposable
    {

        UserSettings initialSettings = UserSettings.Load(null);
        readonly string MetadataPath = Path.GetFullPath("TestMetadataCsdl.xml");
        readonly string MetadataPathV3 = Path.GetFullPath("TestMetadataCsdlV3.xml");
        readonly string MetadataPathSimple = Path.GetFullPath("TestMetadataCsdlSimple.xml");

        public ODataConnectedServiceWizardTests()
        {
            ResetUserSettings();
        }

        public void Dispose()
        {
            RestoreUserSettings();
        }

        private ServiceConfigurationV4 GetTestConfig()
        {
            return new ServiceConfigurationV4()
            {
                Endpoint = "https://service/$metadata",
                EdmxVersion = Constants.EdmxVersion4,
                ServiceName = "MyService",
                IncludeCustomHeaders = true,
                CustomHttpHeaders = "Key1:Val1\nKey2:Val2",
                IncludeWebProxy = true,
                WebProxyHost = "http://localhost:8080",
                IncludeWebProxyNetworkCredentials = true,
                WebProxyNetworkCredentialsDomain = "domain",
                WebProxyNetworkCredentialsUsername = "username",
                WebProxyNetworkCredentialsPassword = "password",
                UseDataServiceCollection = true,
                GenerateMultipleFiles = true,
                UseNamespacePrefix = true,
                NamespacePrefix = "Namespace",
                GeneratedFileNamePrefix = "GeneratedCode",
                EnableNamingAlias = true,
                OpenGeneratedFilesInIDE = true,
                MakeTypesInternal = true,
                IgnoreUnexpectedElementsAndAttributes = true,
                IncludeT4File = true,
                ExcludedOperationImports = new List<string>()
                { 
                    "GetPersonWithMostFriends",
                    "ResetDataSource"
                },
                ExcludedSchemaTypes = new List<string>()
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender"
                }
            };
        }

        private void ResetUserSettings()
        {
            var settings = new UserSettings();
            settings.Save();
        }

        private void RestoreUserSettings()
        {
            initialSettings.Save();
        }

        [Fact]
        public void TestLoadUserSettingsWhenWizardIsCreated()
        {
            var settings = new UserSettings();
            settings.ServiceName = "Some Service";
            settings.MruEndpoints.Add("Endpoint");
            settings.Save();

            var context = new TestConnectedServiceProviderContext();
            var wizard = new ODataConnectedServiceWizard(context);

            Assert.Equal("Some Service", wizard.UserSettings.ServiceName);
            Assert.Contains("Endpoint", wizard.UserSettings.MruEndpoints);
        }

        

        [Fact]
        public void TestConstructor_ShouldUseDefaultSettingsWhenNotUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(false, savedConfig);

            var wizard = new ODataConnectedServiceWizard(context);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            Assert.Equal(Constants.DefaultServiceName, endpointPage.ServiceName);
            Assert.Null(endpointPage.Endpoint);
            Assert.Null(endpointPage.EdmxVersion);
            Assert.False(endpointPage.IncludeCustomHeaders);
            Assert.Null(endpointPage.CustomHttpHeaders);
            Assert.False(endpointPage.IncludeWebProxy);
            Assert.Null(endpointPage.WebProxyHost);
            Assert.False(endpointPage.IncludeWebProxyNetworkCredentials);
            Assert.Null(endpointPage.WebProxyNetworkCredentialsDomain);
            Assert.Null(endpointPage.WebProxyNetworkCredentialsUsername);
            Assert.Null(endpointPage.WebProxyNetworkCredentialsPassword);

            var operationsPage = wizard.OperationImportsViewModel;
            endpointPage.Endpoint = MetadataPath;
            endpointPage.MetadataTempPath = MetadataPath;
            endpointPage.EdmxVersion = Constants.EdmxVersion4;
            operationsPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            operationsPage.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "GetNearestAirport", IsSelected = true },
                new OperationImportModel() { Name = "GetPersonWithMostFriends", IsSelected = true },
                new OperationImportModel() { Name = "ResetDataSource", IsSelected = true }
            });

            var typesPage = wizard.SchemaTypesViewModel;
            typesPage.OnPageEnteringAsync(new WizardEnteringArgs(operationsPage)).Wait();
            typesPage.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>()
            {
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline", "Airline") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport", "Airport") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.City", "City") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee", "Employee") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight", "Flight") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location", "Location") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person", "Person") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender", "PersonGender") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip", "Trip") { IsSelected = true },
            });

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(typesPage)).Wait();
            Assert.Equal(Constants.DefaultReferenceFileName, advancedPage.GeneratedFileNamePrefix);
            Assert.False(advancedPage.UseNamespacePrefix);
            Assert.Equal(Constants.DefaultReferenceFileName, advancedPage.NamespacePrefix);
            Assert.False(advancedPage.UseDataServiceCollection);
            Assert.False(advancedPage.EnableNamingAlias);
            Assert.False(advancedPage.OpenGeneratedFilesInIDE);
            Assert.False(advancedPage.IncludeT4File);
            Assert.False(advancedPage.GenerateMultipleFiles);
            Assert.False(advancedPage.IgnoreUnexpectedElementsAndAttributes);
            Assert.False(advancedPage.MakeTypesInternal);
        }

        [Fact]
        public void TestConstructor_LoadsSavedConfigWhenUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);

            var wizard = new ODataConnectedServiceWizard(context);

            Assert.Equal(savedConfig, wizard.ServiceConfig);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            Assert.Equal("https://service/$metadata", endpointPage.Endpoint);
            Assert.Equal("MyService", endpointPage.ServiceName);
            Assert.True(endpointPage.IncludeCustomHeaders);
            Assert.Equal("Key1:Val1\nKey2:Val2", endpointPage.CustomHttpHeaders);
            Assert.True(endpointPage.IncludeWebProxy);
            Assert.Equal("http://localhost:8080", endpointPage.WebProxyHost);
            Assert.True(endpointPage.IncludeWebProxyNetworkCredentials);
            Assert.Equal("domain", endpointPage.WebProxyNetworkCredentialsDomain);
            // username and password are not restored from the config
            Assert.Null(endpointPage.WebProxyNetworkCredentialsUsername);
            Assert.Null(endpointPage.WebProxyNetworkCredentialsPassword);

            var operationsPage = wizard.OperationImportsViewModel;
            endpointPage.MetadataTempPath = MetadataPath;
            endpointPage.EdmxVersion = Constants.EdmxVersion4;
            operationsPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            operationsPage.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "GetNearestAirport", IsSelected = true },
                new OperationImportModel() { Name = "GetPersonWithMostFriends", IsSelected = false },
                new OperationImportModel() { Name = "ResetDataSource", IsSelected = false }
            });

            var typesPage = wizard.SchemaTypesViewModel;
            typesPage.OnPageEnteringAsync(new WizardEnteringArgs(operationsPage)).Wait();
            typesPage.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>()
            {
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline", "Airline") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport", "Airport") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.City", "City") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee", "Employee") { IsSelected = false },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight", "Flight") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location", "Location") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person", "Person") { IsSelected = false },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender", "PersonGender") { IsSelected = false },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip", "Trip") { IsSelected = true },
            });

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(typesPage)).Wait();
            Assert.Equal("GeneratedCode", advancedPage.GeneratedFileNamePrefix);
            Assert.True(advancedPage.UseNamespacePrefix);
            Assert.Equal("Namespace", advancedPage.NamespacePrefix);
            Assert.True(advancedPage.UseDataServiceCollection);
            Assert.True(advancedPage.EnableNamingAlias);
            Assert.True(advancedPage.OpenGeneratedFilesInIDE);
            Assert.True(advancedPage.IncludeT4File);
            Assert.True(advancedPage.GenerateMultipleFiles);
            Assert.True(advancedPage.IgnoreUnexpectedElementsAndAttributes);
            Assert.True(advancedPage.MakeTypesInternal);
        }

        [Fact]
        public void TestDisableReaOonlyFieldsWhenUpdating()
        {
            ServiceConfigurationV4 savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            var wizard = new ODataConnectedServiceWizard(context);
            
            // endpoint page
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            var endpointView = endpointPage.View as ConfigODataEndpoint;

            Assert.False(endpointView.ServiceName.IsEnabled);
            Assert.False(endpointView.Endpoint.IsEnabled);
            Assert.False(endpointView.OpenConnectedServiceJsonFileButton.IsEnabled);

            // if endpoint is a http address, then file dialog should be disabled
            savedConfig.Endpoint = "http://service";
            endpointPage.OnPageEnteringAsync(null).Wait();
            Assert.False(endpointView.OpenEndpointFileButton.IsEnabled);

            // advanced settings page
            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            var advancedView = advancedPage.View as AdvancedSettings;
            Assert.False(advancedView.IncludeT4File.IsEnabled);
            Assert.False(advancedView.GenerateMultipleFiles.IsEnabled);
        }

        [Fact]
        public void TestGetFinishedServiceInstanceAsync_SavesUserSettingsAndReturnsServiceInstanceWithConfigFromTheWizard()
        {
            var context = new TestConnectedServiceProviderContext(false);
            var wizard = new ODataConnectedServiceWizard(context);
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.ServiceName = "TestService";
            endpointPage.Endpoint = MetadataPath;
            endpointPage.EdmxVersion = Constants.EdmxVersion4;
            endpointPage.IncludeCustomHeaders = true;
            endpointPage.CustomHttpHeaders = "Key:val";
            endpointPage.IncludeWebProxy = true;
            endpointPage.WebProxyHost = "http://localhost:8080";
            endpointPage.IncludeWebProxyNetworkCredentials = true;
            endpointPage.WebProxyNetworkCredentialsDomain = "domain";
            endpointPage.WebProxyNetworkCredentialsUsername = "user";
            endpointPage.WebProxyNetworkCredentialsPassword = "pass";

            var operationsPage = wizard.OperationImportsViewModel;
            endpointPage.OnPageLeavingAsync(new WizardLeavingArgs(operationsPage)).Wait();
            operationsPage.OperationImports = new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "GetNearestAirport", IsSelected = false },
                new OperationImportModel() { Name = "GetPersonWithMostFriends", IsSelected = true },
                new OperationImportModel() { Name = "ResetDataSource", IsSelected = false }
            };

            var typesPage = wizard.SchemaTypesViewModel;
            typesPage.OnPageEnteringAsync(new WizardEnteringArgs(operationsPage)).Wait();
            typesPage.SchemaTypes = new List<SchemaTypeModel>()
            {
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline", "Airline") { IsSelected = false },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport", "Airport") { IsSelected = false },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.City", "City") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee", "Employee") { IsSelected = false },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight", "Flight") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location", "Location") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person", "Person") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender", "PersonGender") { IsSelected = true },
                new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip", "Trip") { IsSelected = true },
            };

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.GeneratedFileNamePrefix = "GeneratedFile";
            advancedPage.UseNamespacePrefix = true;
            advancedPage.NamespacePrefix = "TestNamespace";
            advancedPage.UseDataServiceCollection = true;
            advancedPage.EnableNamingAlias = true;
            advancedPage.MakeTypesInternal = true;
            advancedPage.IncludeT4File = true;
            advancedPage.GenerateMultipleFiles = true;
            advancedPage.OpenGeneratedFilesInIDE = true;

            operationsPage.OnPageLeavingAsync(new WizardLeavingArgs(typesPage)).Wait();
            typesPage.OnPageLeavingAsync(new WizardLeavingArgs(advancedPage)).Wait();
            advancedPage.OnPageLeavingAsync(new WizardLeavingArgs(null)).Wait();
            var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
            var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

            // saved user settings
            var settings = UserSettings.Load(null);
            Assert.Equal("TestService", settings.ServiceName);
            Assert.Equal(MetadataPath, settings.Endpoint);
            // moves endpoint to top of mru list
            Assert.Equal(MetadataPath, settings.MruEndpoints.First());
            Assert.Equal(1, settings.MruEndpoints.Count(e => e == MetadataPath));
            Assert.True(settings.IncludeCustomHeaders);
            Assert.Equal("Key:val", settings.CustomHttpHeaders);
            Assert.True(settings.IncludeWebProxy);
            Assert.Equal("http://localhost:8080", settings.WebProxyHost);
            Assert.True(settings.IncludeWebProxyNetworkCredentials);
            Assert.Equal("domain", settings.WebProxyNetworkCredentialsDomain);
            Assert.Equal("user", settings.WebProxyNetworkCredentialsUsername);
            Assert.Equal("pass", settings.WebProxyNetworkCredentialsPassword);
            settings.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetNearestAirport", "ResetDataSource" });
            settings.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee"
            });
            Assert.Equal("GeneratedFile", config.GeneratedFileNamePrefix);
            Assert.True(config.UseNamespacePrefix);
            Assert.Equal("TestNamespace", settings.NamespacePrefix);
            Assert.True(settings.EnableNamingAlias);
            Assert.True(settings.MakeTypesInternal);
            Assert.True(settings.IncludeT4File);
            Assert.True(settings.GenerateMultipleFiles);
            Assert.True(settings.OpenGeneratedFilesInIDE);


            // service configuration created
            Assert.Equal("GeneratedFile", serviceInstance.InstanceId);
            Assert.Equal("TestService", serviceInstance.Name);
            Assert.Equal(endpointPage.MetadataTempPath, serviceInstance.MetadataTempFilePath);
            Assert.Equal("TestService", config.ServiceName);
            Assert.Equal(MetadataPath, config.Endpoint);
            Assert.Equal(Constants.EdmxVersion4, config.EdmxVersion);
            Assert.True(config.IncludeCustomHeaders);
            Assert.Equal("Key:val", config.CustomHttpHeaders);
            Assert.True(config.IncludeWebProxy);
            Assert.Equal("http://localhost:8080", config.WebProxyHost);
            Assert.True(config.IncludeWebProxyNetworkCredentials);
            Assert.Equal("domain", config.WebProxyNetworkCredentialsDomain);
            Assert.Equal("user", config.WebProxyNetworkCredentialsUsername);
            Assert.Equal("pass", config.WebProxyNetworkCredentialsPassword);
            config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetNearestAirport", "ResetDataSource" });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee"
            });
            Assert.Equal("GeneratedFile", config.GeneratedFileNamePrefix);
            Assert.True(config.UseNamespacePrefix);
            Assert.Equal("TestNamespace", config.NamespacePrefix);
            Assert.True(config.EnableNamingAlias);
            Assert.True(config.MakeTypesInternal);
            Assert.True(config.IncludeT4File);
            Assert.True(config.GenerateMultipleFiles);
            Assert.True(config.OpenGeneratedFilesInIDE);
        }

        [Fact]
        public void GetFinishedServiceInstanceAsync_WhenUpdating_ShouldUseSavedConfigWhenUserDoesNotVisitPages()
        {
            var savedConfig = GetTestConfig();
            savedConfig.Endpoint = MetadataPath;
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            var wizard = new ODataConnectedServiceWizard(context);
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(null).Wait();
            endpointPage.OnPageLeavingAsync(null).Wait();

            var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
            var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

            Assert.Equal("GeneratedCode", serviceInstance.InstanceId);
            Assert.Equal("MyService", serviceInstance.Name);
            Assert.NotNull(serviceInstance.MetadataTempFilePath);
            Assert.Equal("MyService", config.ServiceName);
            Assert.Equal(MetadataPath, config.Endpoint);
            Assert.Equal(Constants.EdmxVersion4, config.EdmxVersion);
            Assert.True(config.IncludeCustomHeaders);
            Assert.Equal("Key1:Val1\nKey2:Val2", config.CustomHttpHeaders);
            Assert.True(config.IncludeWebProxy);
            Assert.Equal("http://localhost:8080", config.WebProxyHost);
            Assert.True(config.IncludeWebProxyNetworkCredentials);
            Assert.Equal("domain", config.WebProxyNetworkCredentialsDomain);
            Assert.Null(config.WebProxyNetworkCredentialsUsername);
            Assert.Null(config.WebProxyNetworkCredentialsPassword);
            config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetPersonWithMostFriends", "ResetDataSource" });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender"
            });
            Assert.Equal("GeneratedCode", config.GeneratedFileNamePrefix);
            Assert.True(config.UseNamespacePrefix);
            Assert.Equal("Namespace", config.NamespacePrefix);
            Assert.True(config.EnableNamingAlias);
            Assert.True(config.MakeTypesInternal);
            Assert.True(config.IncludeT4File);
            Assert.True(config.GenerateMultipleFiles);
            Assert.True(config.OpenGeneratedFilesInIDE);
        }

        [Fact]
        public void ShouldPreserveState_WhenMovingBetweenPagesAndBack()
        {
            var context = new TestConnectedServiceProviderContext();
            var wizard = new ODataConnectedServiceWizard(context);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(null).Wait();
            endpointPage.Endpoint = MetadataPath;
            endpointPage.OnPageLeavingAsync(null).Wait();

            var typesPage = wizard.SchemaTypesViewModel;
            typesPage.OnPageEnteringAsync(null).Wait();
            var typeEmployee = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Employee");
            typeEmployee.IsSelected = false;
            typesPage.OnPageLeavingAsync(null).Wait();

            var operationsPage = wizard.OperationImportsViewModel;
            operationsPage.OnPageEnteringAsync(null).Wait();
            var operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
            operationNearestAirport.IsSelected = false;
            operationsPage.OnPageLeavingAsync(null).Wait();

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(null).Wait();
            advancedPage.UseDataServiceCollection = true;
            advancedPage.MakeTypesInternal = true;
            advancedPage.UseNamespacePrefix = true;
            advancedPage.OnPageLeavingAsync(null).Wait();

            endpointPage.OnPageEnteringAsync(null).Wait();
            Assert.Equal(Constants.DefaultServiceName, endpointPage.ServiceName);
            Assert.Equal(MetadataPath, endpointPage.Endpoint);
            endpointPage.ServiceName = "Service";
            endpointPage.IncludeCustomHeaders = true;
            endpointPage.CustomHttpHeaders = "A:b";
            endpointPage.OnPageLeavingAsync(null).Wait();

            advancedPage.OnPageEnteringAsync(null).Wait();
            Assert.True(advancedPage.UseNamespacePrefix);
            Assert.True(advancedPage.UseDataServiceCollection);
            Assert.True(advancedPage.MakeTypesInternal);
            advancedPage.NamespacePrefix = "MyNamespace";
            advancedPage.GenerateMultipleFiles = true;
            advancedPage.UseDataServiceCollection = false;
            advancedPage.OnPageLeavingAsync(null).Wait();

            operationsPage.OnPageEnteringAsync(null).Wait();
            operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
            Assert.False(operationNearestAirport.IsSelected);
            var operationResetDataSource = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "ResetDataSource");
            operationResetDataSource.IsSelected = false;
            operationsPage.OnPageLeavingAsync(null).Wait();

            typesPage.OnPageEnteringAsync(null).Wait();
            typeEmployee = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Employee");
            Assert.False(typeEmployee.IsSelected);
            typeEmployee.IsSelected = true;
            var typeFlight = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Flight");
            typeFlight.IsSelected = false;
            typesPage.OnPageLeavingAsync(null).Wait();

            var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
            var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

            Assert.Equal("Service", config.ServiceName);
            Assert.Equal(MetadataPath, config.Endpoint);
            Assert.True(config.IncludeCustomHeaders);
            Assert.Equal("A:b", config.CustomHttpHeaders);
            Assert.True(config.GenerateMultipleFiles);
            Assert.True(config.MakeTypesInternal);
            Assert.True(config.UseNamespacePrefix);
            Assert.Equal("MyNamespace", config.NamespacePrefix);
            Assert.False(config.UseDataServiceCollection);
            config.ExcludedOperationImports.ShouldAllBeEquivalentTo(new List<string>()
            {
                "GetNearestAirport",
                "ResetDataSource"
            });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight"
            });
        }

        [Fact]
        public void ShouldPreserveState_WhenMovingBetweenPagesAndBack_WhenUpdating()
        {
            var savedConfig = GetTestConfig();
            savedConfig.Endpoint = MetadataPath;
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            var wizard = new ODataConnectedServiceWizard(context);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(null).Wait();
            endpointPage.OnPageLeavingAsync(null).Wait();

            var typesPage = wizard.SchemaTypesViewModel;
            typesPage.OnPageEnteringAsync(null).Wait();
            var typeGender = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "PersonGender");
            Assert.False(typeGender.IsSelected);
            typeGender.IsSelected = true;
            var typePerson = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Person");
            Assert.False(typePerson.IsSelected);
            typesPage.OnPageLeavingAsync(null).Wait();

            var operationsPage = wizard.OperationImportsViewModel;
            operationsPage.OnPageEnteringAsync(null).Wait();
            var operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
            Assert.True(operationNearestAirport.IsSelected);
            operationNearestAirport.IsSelected = false;
            operationsPage.OnPageLeavingAsync(null).Wait();

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(null).Wait();
            advancedPage.UseDataServiceCollection = true;
            advancedPage.MakeTypesInternal = true;
            advancedPage.UseNamespacePrefix = true;
            advancedPage.OnPageLeavingAsync(null).Wait();

            endpointPage.OnPageEnteringAsync(null).Wait();
            Assert.Equal(savedConfig.ServiceName, endpointPage.ServiceName);
            Assert.Equal(savedConfig.Endpoint, endpointPage.Endpoint);
            endpointPage.IncludeCustomHeaders = true;
            endpointPage.CustomHttpHeaders = "A:b";
            endpointPage.OnPageLeavingAsync(null).Wait();

            advancedPage.OnPageEnteringAsync(null).Wait();
            Assert.True(advancedPage.UseNamespacePrefix);
            Assert.True(advancedPage.UseDataServiceCollection);
            Assert.True(advancedPage.MakeTypesInternal);
            advancedPage.NamespacePrefix = "MyNamespace";
            advancedPage.GenerateMultipleFiles = true;
            advancedPage.UseDataServiceCollection = false;
            advancedPage.OnPageLeavingAsync(null).Wait();

            operationsPage.OnPageEnteringAsync(null).Wait();
            operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
            Assert.False(operationNearestAirport.IsSelected);
            var operationResetDataSource = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "ResetDataSource");
            Assert.False(operationResetDataSource.IsSelected);
            operationResetDataSource.IsSelected = true;
            operationsPage.OnPageLeavingAsync(null).Wait();

            typesPage.OnPageEnteringAsync(null).Wait();
            typeGender = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "PersonGender");
            Assert.True(typeGender.IsSelected);
            var typeFlight = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Flight");
            typeFlight.IsSelected = false;
            typePerson = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Person");
            Assert.False(typePerson.IsSelected);
            typesPage.OnPageLeavingAsync(null).Wait();

            var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
            var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

            Assert.Equal(savedConfig.ServiceName, config.ServiceName);
            Assert.Equal(savedConfig.Endpoint, config.Endpoint);
            Assert.True(config.IncludeCustomHeaders);
            Assert.Equal("A:b", config.CustomHttpHeaders);
            Assert.True(config.GenerateMultipleFiles);
            Assert.True(config.MakeTypesInternal);
            Assert.True(config.UseNamespacePrefix);
            Assert.Equal("MyNamespace", config.NamespacePrefix);
            Assert.False(config.UseDataServiceCollection);
            config.ExcludedOperationImports.ShouldAllBeEquivalentTo(new List<string>()
            {
                "GetNearestAirport",
                "GetPersonWithMostFriends"
            });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
            });
        }

        [Fact]
        public void ShouldReloadOperationsAndTypesForNewEndpoint_WhenEndpointIsChangedBeforeFinishing()
        {
            var context = new TestConnectedServiceProviderContext();
            var wizard = new ODataConnectedServiceWizard(context);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(null).Wait();
            endpointPage.Endpoint = MetadataPath;
            endpointPage.OnPageLeavingAsync(null).Wait();

            var typesPage = wizard.SchemaTypesViewModel;
            typesPage.OnPageEnteringAsync(null).Wait();
            typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Employee").IsSelected = false;
            typesPage.OnPageLeavingAsync(null).Wait();

            var operationsPage = wizard.OperationImportsViewModel;
            operationsPage.OnPageEnteringAsync(null).Wait();
            operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport").IsSelected = false;
            operationsPage.OnPageLeavingAsync(null).Wait();

            // go back to first page and change endpoint
            endpointPage.OnPageEnteringAsync(null).Wait();
            endpointPage.Endpoint = MetadataPathSimple;
            endpointPage.OnPageLeavingAsync(null).Wait();

            typesPage.OnPageEnteringAsync(null).Wait();
            typesPage.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>()
            {
                new SchemaTypeModel("SimpleService.Models.OtherThing", "OtherThing") { IsSelected = true },
                new SchemaTypeModel("SimpleService.Models.Thing", "Thing") { IsSelected = true }
            });
            typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "OtherThing").IsSelected = false;
            typesPage.OnPageLeavingAsync(null).Wait();

            operationsPage.OnPageEnteringAsync(null).Wait();
            operationsPage.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
            {
                new OperationImportModel() { Name = "GetRandomThing", IsSelected = true },
                new OperationImportModel() { Name = "ResetThings", IsSelected = true }
            });
            operationsPage.OperationImports.FirstOrDefault(o => o.Name == "ResetThings").IsSelected = false;

            var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
            var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

            Assert.Equal(MetadataPathSimple, config.Endpoint);
            config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "ResetThings" });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>() { "SimpleService.Models.OtherThing" });
        }

        [Fact]
        public void UnsupportedFeaturesAreDisabledOrHidden_WhenServiceIsV3OrLess()
        {
            var context = new TestConnectedServiceProviderContext();
            var wizard = new ODataConnectedServiceWizard(context);
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.Endpoint = MetadataPathV3;
            endpointPage.OnPageLeavingAsync(null).Wait();
            Assert.Equal(Constants.EdmxVersion1, endpointPage.EdmxVersion);

            var operationsPage = wizard.OperationImportsViewModel;
            operationsPage.OnPageEnteringAsync(null).Wait();
            Assert.False(operationsPage.View.IsEnabled);
            Assert.False(operationsPage.IsSupportedODataVersion);

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(null).Wait();
            var advancedView = advancedPage.View as AdvancedSettings;
            advancedView.settings.RaiseEvent(new RoutedEventArgs(Hyperlink.ClickEvent));
            Assert.Equal(Visibility.Hidden, advancedView.AdvancedSettingsForv4.Visibility);
        }
    }

    internal class OperationImportModelComparer : IEqualityComparer<OperationImportModel>
    {
        public bool Equals(OperationImportModel x, OperationImportModel y)
        {
            return x.Name == y.Name && x.IsSelected == y.IsSelected;
        }

        public int GetHashCode(OperationImportModel obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    internal class TestConnectedServiceProviderContext: ConnectedServiceProviderContext
    {
        ServiceConfigurationV4 savedData;

        public TestConnectedServiceProviderContext(bool isUpdating = false, ServiceConfigurationV4 savedData = null) : base()
        {
            this.savedData = savedData;

            if (isUpdating)
            {
                UpdateContext = new ConnectedServiceUpdateContext(new Version(), new TestVsHierarchyItem());
            }
        }

        public override IDictionary<string, object> Args => throw new NotImplementedException();

        public override XmlConfigHelper CreateReadOnlyXmlConfigHelper()
        {
            throw new NotImplementedException();
        }

        public override TData GetExtendedDesignerData<TData>()
        {
            return savedData as TData;
        }

        public override Microsoft.VisualStudio.Shell.IVsHierarchyItem GetServiceFolder(string serviceFolderName)
        {
            return new TestVsHierarchyItem();
        }

        public override void InitializeUpdateContext(Microsoft.VisualStudio.Shell.IVsHierarchyItem serviceFolder)
        {
            throw new NotImplementedException();
        }

        public override void SetExtendedDesignerData<TData>(TData data)
        {
            savedData = data as ServiceConfigurationV4;
        }

        public override IDisposable StartBusyIndicator(string message = null)
        {
            throw new NotImplementedException();
        }
    }

    public class TestVsHierarchyItem : IVsHierarchyItem
    {
        public IVsHierarchyItemIdentity HierarchyIdentity => throw new NotImplementedException();

        public IVsHierarchyItem Parent => throw new NotImplementedException();

        public IEnumerable<IVsHierarchyItem> Children => throw new NotImplementedException();

        public bool AreChildrenRealized => throw new NotImplementedException();

        public string Text => "Item";

        public string CanonicalName => "CanonicalName";

        public bool IsBold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsCut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsDisposed => false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
    }
}
