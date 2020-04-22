using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace ODataConnectedService.Tests
{
    [TestClass]
    public class ODataConnectedServiceWizardTests
    {

        UserSettings initialSettings = UserSettings.Load(null);
        readonly string MetadataPath = Path.GetFullPath("TestMetadataCsdl.xml");
        readonly string MetadataPathV3 = Path.GetFullPath("TestMetadataCsdlV3.xml");
        readonly string MetadataPathSimple = Path.GetFullPath("TestMetadataCsdlSimple.xml");

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

        [TestInitialize]
        public void ResetUserSettings()
        {
            var settings = new UserSettings();
            settings.Save();
        }

        [TestCleanup]
        public void RestoreUserSettings()
        {
            initialSettings.Save();
        }

        [TestMethod]
        public void TestLoadUserSettingsWhenWizardIsCreated()
        {
            var settings = new UserSettings();
            settings.ServiceName = "Some Service";
            settings.MruEndpoints.Add("Endpoint");
            settings.Save();

            var context = new TestConnectedServiceProviderContext();
            var wizard = new ODataConnectedServiceWizard(context);

            Assert.AreEqual("Some Service", wizard.UserSettings.ServiceName);
            Assert.IsTrue(wizard.UserSettings.MruEndpoints.Contains("Endpoint"));
        }

        

        [TestMethod]
        public void TestConstructor_ShouldUseDefaultSettingsWhenNotUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(false, savedConfig);

            var wizard = new ODataConnectedServiceWizard(context);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            Assert.AreEqual(Constants.DefaultServiceName, endpointPage.ServiceName);
            Assert.IsNull(endpointPage.Endpoint);
            Assert.IsNull(endpointPage.EdmxVersion);
            Assert.IsFalse(endpointPage.IncludeCustomHeaders);
            Assert.IsNull(endpointPage.CustomHttpHeaders);
            Assert.IsFalse(endpointPage.IncludeWebProxy);
            Assert.IsNull(endpointPage.WebProxyHost);
            Assert.IsFalse(endpointPage.IncludeWebProxyNetworkCredentials);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsDomain);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsUsername);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsPassword);

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
            Assert.AreEqual(Constants.DefaultReferenceFileName, advancedPage.GeneratedFileNamePrefix);
            Assert.IsFalse(advancedPage.UseNamespacePrefix);
            Assert.AreEqual(Constants.DefaultReferenceFileName, advancedPage.NamespacePrefix);
            Assert.IsFalse(advancedPage.UseDataServiceCollection);
            Assert.IsFalse(advancedPage.EnableNamingAlias);
            Assert.IsFalse(advancedPage.OpenGeneratedFilesInIDE);
            Assert.IsFalse(advancedPage.IncludeT4File);
            Assert.IsFalse(advancedPage.GenerateMultipleFiles);
            Assert.IsFalse(advancedPage.IgnoreUnexpectedElementsAndAttributes);
            Assert.IsFalse(advancedPage.MakeTypesInternal);
        }

        [TestMethod]
        public void TestConstructor_LoadsSavedConfigWhenUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);

            var wizard = new ODataConnectedServiceWizard(context);

            Assert.AreEqual(savedConfig, wizard.ServiceConfig);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            Assert.AreEqual("https://service/$metadata", endpointPage.Endpoint);
            Assert.AreEqual("MyService", endpointPage.ServiceName);
            Assert.IsTrue(endpointPage.IncludeCustomHeaders);
            Assert.AreEqual("Key1:Val1\nKey2:Val2", endpointPage.CustomHttpHeaders);
            Assert.IsTrue(endpointPage.IncludeWebProxy);
            Assert.AreEqual("http://localhost:8080", endpointPage.WebProxyHost);
            Assert.IsTrue(endpointPage.IncludeWebProxyNetworkCredentials);
            Assert.AreEqual("domain", endpointPage.WebProxyNetworkCredentialsDomain);
            // username and password are not restored from the config
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsUsername);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsPassword);

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
            Assert.AreEqual("GeneratedCode", advancedPage.GeneratedFileNamePrefix);
            Assert.IsTrue(advancedPage.UseNamespacePrefix);
            Assert.AreEqual("Namespace", advancedPage.NamespacePrefix);
            Assert.IsTrue(advancedPage.UseDataServiceCollection);
            Assert.IsTrue(advancedPage.EnableNamingAlias);
            Assert.IsTrue(advancedPage.OpenGeneratedFilesInIDE);
            Assert.IsTrue(advancedPage.IncludeT4File);
            Assert.IsTrue(advancedPage.GenerateMultipleFiles);
            Assert.IsTrue(advancedPage.IgnoreUnexpectedElementsAndAttributes);
            Assert.IsTrue(advancedPage.MakeTypesInternal);
        }

        [TestMethod]
        public void TestDisableReaOonlyFieldsWhenUpdating()
        {
            ServiceConfigurationV4 savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            var wizard = new ODataConnectedServiceWizard(context);
            
            // endpoint page
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            var endpointView = endpointPage.View as ConfigODataEndpoint;

            Assert.IsFalse(endpointView.ServiceName.IsEnabled);
            Assert.IsFalse(endpointView.Endpoint.IsEnabled);
            Assert.IsFalse(endpointView.OpenConnectedServiceJsonFileButton.IsEnabled);

            // if endpoint is a http address, then file dialog should be disabled
            savedConfig.Endpoint = "http://service";
            endpointPage.OnPageEnteringAsync(null).Wait();
            Assert.IsFalse(endpointView.OpenEndpointFileButton.IsEnabled);

            // advanced settings page
            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            var advancedView = advancedPage.View as AdvancedSettings;
            Assert.IsFalse(advancedView.IncludeT4File.IsEnabled);
            Assert.IsFalse(advancedView.GenerateMultipleFiles.IsEnabled);
        }

        [TestMethod]
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
            Assert.AreEqual("TestService", settings.ServiceName);
            Assert.AreEqual(MetadataPath, settings.Endpoint);
            Assert.AreEqual(true, settings.IncludeCustomHeaders);
            Assert.AreEqual("Key:val", settings.CustomHttpHeaders);
            Assert.AreEqual(true, settings.IncludeWebProxy);
            Assert.AreEqual("http://localhost:8080", settings.WebProxyHost);
            Assert.AreEqual(true, settings.IncludeWebProxyNetworkCredentials);
            Assert.AreEqual("domain", settings.WebProxyNetworkCredentialsDomain);
            Assert.AreEqual("user", settings.WebProxyNetworkCredentialsUsername);
            Assert.AreEqual("pass", settings.WebProxyNetworkCredentialsPassword);
            settings.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetNearestAirport", "ResetDataSource" });
            settings.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee"
            });
            Assert.AreEqual("GeneratedFile", config.GeneratedFileNamePrefix);
            Assert.AreEqual(true, config.UseNamespacePrefix);
            Assert.AreEqual("TestNamespace", settings.NamespacePrefix);
            Assert.AreEqual(true, settings.EnableNamingAlias);
            Assert.AreEqual(true, settings.MakeTypesInternal);
            Assert.AreEqual(true, settings.IncludeT4File);
            Assert.AreEqual(true, settings.GenerateMultipleFiles);
            Assert.AreEqual(true, settings.OpenGeneratedFilesInIDE);


            // service configuration created
            Assert.AreEqual("GeneratedFile", serviceInstance.InstanceId);
            Assert.AreEqual("TestService", serviceInstance.Name);
            Assert.AreEqual(endpointPage.MetadataTempPath, serviceInstance.MetadataTempFilePath);
            Assert.AreEqual("TestService", config.ServiceName);
            Assert.AreEqual(MetadataPath, config.Endpoint);
            Assert.AreEqual(Constants.EdmxVersion4, config.EdmxVersion);
            Assert.AreEqual(true, config.IncludeCustomHeaders);
            Assert.AreEqual("Key:val", config.CustomHttpHeaders);
            Assert.AreEqual(true, config.IncludeWebProxy);
            Assert.AreEqual("http://localhost:8080", config.WebProxyHost);
            Assert.AreEqual(true, config.IncludeWebProxyNetworkCredentials);
            Assert.AreEqual("domain", config.WebProxyNetworkCredentialsDomain);
            Assert.AreEqual("user", config.WebProxyNetworkCredentialsUsername);
            Assert.AreEqual("pass", config.WebProxyNetworkCredentialsPassword);
            config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetNearestAirport", "ResetDataSource" });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee"
            });
            Assert.AreEqual("GeneratedFile", config.GeneratedFileNamePrefix);
            Assert.AreEqual(true, config.UseNamespacePrefix);
            Assert.AreEqual("TestNamespace", config.NamespacePrefix);
            Assert.AreEqual(true, config.EnableNamingAlias);
            Assert.AreEqual(true, config.MakeTypesInternal);
            Assert.AreEqual(true, config.IncludeT4File);
            Assert.AreEqual(true, config.GenerateMultipleFiles);
            Assert.AreEqual(true, config.OpenGeneratedFilesInIDE);
        }

        [TestMethod]
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

            Assert.AreEqual("GeneratedCode", serviceInstance.InstanceId);
            Assert.AreEqual("MyService", serviceInstance.Name);
            Assert.IsNotNull(serviceInstance.MetadataTempFilePath);
            Assert.AreEqual("MyService", config.ServiceName);
            Assert.AreEqual(MetadataPath, config.Endpoint);
            Assert.AreEqual(Constants.EdmxVersion4, config.EdmxVersion);
            Assert.AreEqual(true, config.IncludeCustomHeaders);
            Assert.AreEqual("Key1:Val1\nKey2:Val2", config.CustomHttpHeaders);
            Assert.AreEqual(true, config.IncludeWebProxy);
            Assert.AreEqual("http://localhost:8080", config.WebProxyHost);
            Assert.AreEqual(true, config.IncludeWebProxyNetworkCredentials);
            Assert.AreEqual("domain", config.WebProxyNetworkCredentialsDomain);
            Assert.AreEqual(null, config.WebProxyNetworkCredentialsUsername);
            Assert.AreEqual(null, config.WebProxyNetworkCredentialsPassword);
            config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetPersonWithMostFriends", "ResetDataSource" });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
            {
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender"
            });
            Assert.AreEqual("GeneratedCode", config.GeneratedFileNamePrefix);
            Assert.AreEqual(true, config.UseNamespacePrefix);
            Assert.AreEqual("Namespace", config.NamespacePrefix);
            Assert.AreEqual(true, config.EnableNamingAlias);
            Assert.AreEqual(true, config.MakeTypesInternal);
            Assert.AreEqual(true, config.IncludeT4File);
            Assert.AreEqual(true, config.GenerateMultipleFiles);
            Assert.AreEqual(true, config.OpenGeneratedFilesInIDE);
        }

        [TestMethod]
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
            Assert.AreEqual(Constants.DefaultServiceName, endpointPage.ServiceName);
            Assert.AreEqual(MetadataPath, endpointPage.Endpoint);
            endpointPage.ServiceName = "Service";
            endpointPage.IncludeCustomHeaders = true;
            endpointPage.CustomHttpHeaders = "A:b";
            endpointPage.OnPageLeavingAsync(null).Wait();

            advancedPage.OnPageEnteringAsync(null).Wait();
            Assert.IsTrue(advancedPage.UseNamespacePrefix);
            Assert.IsTrue(advancedPage.UseDataServiceCollection);
            Assert.IsTrue(advancedPage.MakeTypesInternal);
            advancedPage.NamespacePrefix = "MyNamespace";
            advancedPage.GenerateMultipleFiles = true;
            advancedPage.UseDataServiceCollection = false;
            advancedPage.OnPageLeavingAsync(null).Wait();

            operationsPage.OnPageEnteringAsync(null).Wait();
            operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
            Assert.IsFalse(operationNearestAirport.IsSelected);
            var operationResetDataSource = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "ResetDataSource");
            operationResetDataSource.IsSelected = false;
            operationsPage.OnPageLeavingAsync(null).Wait();

            typesPage.OnPageEnteringAsync(null).Wait();
            typeEmployee = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Employee");
            Assert.IsFalse(typeEmployee.IsSelected);
            typeEmployee.IsSelected = true;
            var typeFlight = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Flight");
            typeFlight.IsSelected = false;
            typesPage.OnPageLeavingAsync(null).Wait();

            var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
            var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

            Assert.AreEqual("Service", config.ServiceName);
            Assert.AreEqual(MetadataPath, config.Endpoint);
            Assert.IsTrue(config.IncludeCustomHeaders);
            Assert.AreEqual("A:b", config.CustomHttpHeaders);
            Assert.IsTrue(config.GenerateMultipleFiles);
            Assert.IsTrue(config.MakeTypesInternal);
            Assert.IsTrue(config.UseNamespacePrefix);
            Assert.AreEqual("MyNamespace", config.NamespacePrefix);
            Assert.IsFalse(config.UseDataServiceCollection);
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

        [TestMethod]
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

            Assert.AreEqual(MetadataPathSimple, config.Endpoint);
            config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "ResetThings" });
            config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>() { "SimpleService.Models.OtherThing" });
        }

        [TestMethod]
        public void UnsupportedFeaturesAreDisabledOrHidden_WhenServiceIsV3OrLess()
        {
            var context = new TestConnectedServiceProviderContext();
            var wizard = new ODataConnectedServiceWizard(context);
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.Endpoint = MetadataPathV3;
            endpointPage.OnPageLeavingAsync(null).Wait();
            Assert.AreEqual(Constants.EdmxVersion1, endpointPage.EdmxVersion);

            var operationsPage = wizard.OperationImportsViewModel;
            operationsPage.OnPageEnteringAsync(null).Wait();
            Assert.IsFalse(operationsPage.View.IsEnabled);
            Assert.IsFalse(operationsPage.IsSupportedODataVersion);

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(null).Wait();
            var advancedView = advancedPage.View as AdvancedSettings;
            advancedView.settings.RaiseEvent(new RoutedEventArgs(Hyperlink.ClickEvent));
            Assert.AreEqual(advancedView.AdvancedSettingsForv4.Visibility, Visibility.Hidden);
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
