//-----------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceWizardTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using FluentAssertions;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.OData.CodeGen;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Xunit;

namespace ODataConnectedService.Tests
{

    public sealed class ODataConnectedServiceWizardTests: IDisposable
    {
        readonly UserSettings initialSettings;
        readonly string MetadataPath = Path.GetFullPath("TestMetadataCsdl.xml");
        readonly string MetadataPathV3 = Path.GetFullPath("TestMetadataCsdlV3.xml");
        readonly string MetadataPathSimple = Path.GetFullPath("TestMetadataCsdlSimple.xml");

        public ODataConnectedServiceWizardTests()
        {
            initialSettings = new UserSettings();
            initialSettings.Load();
            // Reset user settings
            new UserSettings().Save();
        }

        // Will be executed after every test to restore initial settings
        public void Dispose()
        {
            initialSettings.Save();
        }

        private ServiceConfigurationV4 GetTestConfig()
        {
            return new ServiceConfigurationV4()
            {
                Endpoint = "https://service/$metadata",
                EdmxVersion = Constants.EdmxVersion4,
                ServiceName = "MyService",
                IncludeCustomHeaders = true,
                StoreCustomHttpHeaders = true,
                CustomHttpHeaders = "Key1:Val1\nKey2:Val2",
                IncludeWebProxy = true,
                WebProxyHost = "http://localhost:8080",
                IncludeWebProxyNetworkCredentials = true,
                StoreWebProxyNetworkCredentials = true,
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
                ExcludedBoundOperations = new List<string>()
                {
                    "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)",
                    "ShareTrip(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "UpdatePersonLastName(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"
                },
                ExcludedSchemaTypes = new List<string>()
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip"
                }
            };
        }

        [Fact]
        public void TestLoadUserSettingsWhenWizardIsCreated()
        {
            var settings = new UserSettings();
            settings.ServiceName = "OData Service";
            settings.AddMruEndpoint("Endpoint");
            settings.Save();

            var context = new TestConnectedServiceProviderContext();
            using (var wizard = new ODataConnectedServiceWizard(context))
            {
                Assert.Equal("OData Service", wizard.UserSettings.ServiceName);
                Assert.Contains("Endpoint", wizard.UserSettings.MruEndpoints);
            }
        }

        [Fact]
        public void TestConstructor_ShouldUseDefaultSettingsWhenNotUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(false, savedConfig);

            using (var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
                Assert.Equal(Constants.DefaultServiceName, endpointPage.UserSettings.ServiceName);
                Assert.Null(endpointPage.UserSettings.Endpoint);
                Assert.Null(endpointPage.EdmxVersion);
                Assert.False(endpointPage.UserSettings.IncludeCustomHeaders);
                Assert.False(endpointPage.UserSettings.StoreCustomHttpHeaders);
                Assert.Null(endpointPage.UserSettings.CustomHttpHeaders);
                Assert.False(endpointPage.UserSettings.IncludeWebProxy);
                Assert.Null(endpointPage.UserSettings.WebProxyHost);
                Assert.False(endpointPage.UserSettings.IncludeWebProxyNetworkCredentials);
                Assert.False(endpointPage.UserSettings.StoreWebProxyNetworkCredentials);
                Assert.Null(endpointPage.UserSettings.WebProxyNetworkCredentialsDomain);
                Assert.Null(endpointPage.UserSettings.WebProxyNetworkCredentialsUsername);
                Assert.Null(endpointPage.UserSettings.WebProxyNetworkCredentialsPassword);

                var operationsPage = wizard.OperationImportsViewModel;
                endpointPage.UserSettings.Endpoint = MetadataPath;
                endpointPage.MetadataTempPath = MetadataPath;
                endpointPage.EdmxVersion = Constants.EdmxVersion4;
                operationsPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
                operationsPage.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
                {
                    new OperationImportModel
                    {
                        ReturnType = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                        ParametersString = "(Edm.Double lat, Edm.Double lon)",
                        Name = "GetNearestAirport",
                        IsSelected = true
                    },
                    new OperationImportModel
                    {
                        ReturnType = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                        ParametersString = "()",
                        Name = "GetPersonWithMostFriends",
                        IsSelected = true
                    },
                    new OperationImportModel
                    {
                        ReturnType = "void",
                        ParametersString = "()",
                        Name = "ResetDataSource",
                        IsSelected = true
                    }
                });

                var typesPage = wizard.SchemaTypesViewModel;
                typesPage.OnPageEnteringAsync(new WizardEnteringArgs(operationsPage)).Wait();
                typesPage.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
                {
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline", "Airline")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport", "Airport") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.City", "City") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee", "Employee") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight", "Flight") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location", "Location") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person", "Person") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>
                        {
                            new BoundOperationModel
                            {
                                Name = "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "GetFavoriteAirline",
                                IsSelected = true
                            },
                            new BoundOperationModel
                            {
                                Name = "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "GetFriendsTrips",
                                IsSelected = true
                            },
                            new BoundOperationModel
                            {
                                Name = "ShareTrip(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "ShareTrip",
                                IsSelected = true
                            },
                            new BoundOperationModel
                            {
                                Name = "UpdatePersonLastName(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "UpdatePersonLastName",
                                IsSelected = true
                            }
                        }
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender", "PersonGender") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PlanItem", "PlanItem") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PublicTransportation", "PublicTransportation") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip", "Trip") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>{
                            new BoundOperationModel
                            {
                                Name = "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)",
                                ShortName = "GetInvolvedPeople",
                                IsSelected = true
                            }
                        }
                    },
                });

                var advancedPage = wizard.AdvancedSettingsViewModel;
                advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(typesPage)).Wait();
                Assert.Equal(Constants.DefaultReferenceFileName, advancedPage.UserSettings.GeneratedFileNamePrefix);
                Assert.False(advancedPage.UserSettings.UseNamespacePrefix);
                Assert.Null(advancedPage.UserSettings.NamespacePrefix);
                Assert.True(advancedPage.UserSettings.UseDataServiceCollection);
                Assert.True(advancedPage.UserSettings.EnableNamingAlias);
                Assert.False(advancedPage.UserSettings.OpenGeneratedFilesInIDE);
                Assert.False(advancedPage.UserSettings.IncludeT4File);
                Assert.False(advancedPage.UserSettings.GenerateMultipleFiles);
                Assert.True(advancedPage.UserSettings.IgnoreUnexpectedElementsAndAttributes);
                Assert.False(advancedPage.UserSettings.MakeTypesInternal);
            }
        }

        [Fact]
        public void TestConstructor_LoadsSavedConfigWhenUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);

            using (var wizard = new ODataConnectedServiceWizard(context))
            {
                Assert.Equal(savedConfig, wizard.ServiceConfig);

                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
                Assert.Equal("https://service/$metadata", endpointPage.UserSettings.Endpoint);
                Assert.Equal("MyService", endpointPage.UserSettings.ServiceName);
                Assert.True(endpointPage.UserSettings.IncludeCustomHeaders);
                Assert.True(endpointPage.UserSettings.StoreCustomHttpHeaders);
                Assert.Equal("Key1:Val1\nKey2:Val2", endpointPage.UserSettings.CustomHttpHeaders);
                Assert.True(endpointPage.UserSettings.IncludeWebProxy);
                Assert.Equal("http://localhost:8080", endpointPage.UserSettings.WebProxyHost);
                Assert.True(endpointPage.UserSettings.IncludeWebProxyNetworkCredentials);
                Assert.True(endpointPage.UserSettings.StoreWebProxyNetworkCredentials);
                Assert.Equal("domain", endpointPage.UserSettings.WebProxyNetworkCredentialsDomain);                                
                // username and password are not restored from the config
                Assert.Equal("username", endpointPage.UserSettings.WebProxyNetworkCredentialsUsername);
                Assert.Equal("password", endpointPage.UserSettings.WebProxyNetworkCredentialsPassword);

                var operationsPage = wizard.OperationImportsViewModel;
                endpointPage.MetadataTempPath = MetadataPath;
                endpointPage.EdmxVersion = Constants.EdmxVersion4;
                operationsPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
                operationsPage.OperationImports.ShouldBeEquivalentTo(new List<OperationImportModel>()
                {
                    new OperationImportModel
                    {
                        ReturnType = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                        ParametersString = "(Edm.Double lat, Edm.Double lon)",
                        Name = "GetNearestAirport",
                        IsSelected = true
                    },
                    new OperationImportModel
                    {
                        ReturnType = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                        ParametersString = "()",
                        Name = "GetPersonWithMostFriends",
                        IsSelected = false
                    },
                    new OperationImportModel
                    {
                        ReturnType = "void",
                        ParametersString = "()",
                        Name = "ResetDataSource",
                        IsSelected = false
                    }
                });

                var typesPage = wizard.SchemaTypesViewModel;
                typesPage.OnPageEnteringAsync(new WizardEnteringArgs(operationsPage)).Wait();
                typesPage.SchemaTypes.ShouldBeEquivalentTo(new List<SchemaTypeModel>
                {
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airline", "Airline")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport", "Airport")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.City", "City")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee", "Employee")
                    {
                        IsSelected = false,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight", "Flight")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Location", "Location")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person", "Person")
                    {
                        IsSelected = false,
                        BoundOperations = new List<BoundOperationModel>
                        {
                            new BoundOperationModel
                            {
                                Name = "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "GetFavoriteAirline",
                                IsSelected = false
                            },
                            new BoundOperationModel
                            {
                                Name = "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "GetFriendsTrips",
                                IsSelected = false
                            },
                            new BoundOperationModel
                            {
                                Name = "ShareTrip(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "ShareTrip",
                                IsSelected = false
                            },
                            new BoundOperationModel
                            {
                                Name = "UpdatePersonLastName(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                ShortName = "UpdatePersonLastName",
                                IsSelected = false
                            }
                        }
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender",
                        "PersonGender")
                    {
                        IsSelected = false,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PlanItem", "PlanItem") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PublicTransportation", "PublicTransportation") {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip", "Trip")
                    {
                        IsSelected = false,
                        BoundOperations = new List<BoundOperationModel>
                        {
                            new BoundOperationModel
                            {
                                Name = "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)",
                                ShortName = "GetInvolvedPeople",
                                IsSelected = false
                            }
                        }
                    },
                });

                var advancedPage = wizard.AdvancedSettingsViewModel;
                advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(typesPage)).Wait();
                Assert.Equal("GeneratedCode", advancedPage.UserSettings.GeneratedFileNamePrefix);
                Assert.True(advancedPage.UserSettings.UseNamespacePrefix);
                Assert.Equal("Namespace", advancedPage.UserSettings.NamespacePrefix);
                Assert.True(advancedPage.UserSettings.UseDataServiceCollection);
                Assert.True(advancedPage.UserSettings.EnableNamingAlias);
                Assert.True(advancedPage.UserSettings.OpenGeneratedFilesInIDE);
                Assert.True(advancedPage.UserSettings.IncludeT4File);
                Assert.True(advancedPage.UserSettings.GenerateMultipleFiles);
                Assert.True(advancedPage.UserSettings.IgnoreUnexpectedElementsAndAttributes);
                Assert.True(advancedPage.UserSettings.MakeTypesInternal);
            }
        }

        [Fact]
        public void TestDisableReadOnlyFieldsWhenUpdating()
        {
            ServiceConfigurationV4 savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            using (var wizard = new ODataConnectedServiceWizard(context))
            {
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
        }

        [Fact]
        public void TestGetFinishedServiceInstanceAsync_SavesUserSettingsAndReturnsServiceInstanceWithConfigFromTheWizard()
        {
            var context = new TestConnectedServiceProviderContext(false);
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.UserSettings.ServiceName = "TestService";
                endpointPage.UserSettings.Endpoint = MetadataPath;
                endpointPage.EdmxVersion = Constants.EdmxVersion4;
                endpointPage.UserSettings.IncludeCustomHeaders = true;
                endpointPage.UserSettings.StoreCustomHttpHeaders = true;
                endpointPage.UserSettings.CustomHttpHeaders = "Key:val";
                endpointPage.UserSettings.IncludeWebProxy = true;                
                endpointPage.UserSettings.WebProxyHost = "http://localhost:8080";
                endpointPage.UserSettings.IncludeWebProxyNetworkCredentials = true;
                endpointPage.UserSettings.StoreWebProxyNetworkCredentials = true;
                endpointPage.UserSettings.WebProxyNetworkCredentialsDomain = "domain";
                endpointPage.UserSettings.WebProxyNetworkCredentialsUsername = "user";
                endpointPage.UserSettings.WebProxyNetworkCredentialsPassword = "pass";

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
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person", "Person")
                    {
                        IsSelected = true,
                        BoundOperations = new List<BoundOperationModel>()
                        {
                            new BoundOperationModel
                            {
                                Name = "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                IsSelected = false
                            },
                            new BoundOperationModel
                            {
                                Name = "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                                IsSelected = true
                            }
                        }
                    },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender", "PersonGender") { IsSelected = true },
                    new SchemaTypeModel("Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip", "Trip") { IsSelected = true },
                };

                var advancedPage = wizard.AdvancedSettingsViewModel;
                advancedPage.UserSettings.GeneratedFileNamePrefix = "GeneratedFile";
                advancedPage.UserSettings.UseNamespacePrefix = true;
                advancedPage.UserSettings.NamespacePrefix = "TestNamespace";
                advancedPage.UserSettings.UseDataServiceCollection = true;
                advancedPage.UserSettings.EnableNamingAlias = true;
                advancedPage.UserSettings.MakeTypesInternal = true;
                advancedPage.UserSettings.IncludeT4File = true;
                advancedPage.UserSettings.GenerateMultipleFiles = true;
                advancedPage.UserSettings.OpenGeneratedFilesInIDE = true;

                operationsPage.OnPageLeavingAsync(new WizardLeavingArgs(typesPage)).Wait();
                typesPage.OnPageLeavingAsync(new WizardLeavingArgs(advancedPage)).Wait();
                advancedPage.OnPageLeavingAsync(new WizardLeavingArgs(null)).Wait();
                var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
                var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

                // saved user settings
                var settings = new UserSettings();
                settings.Load();
                Assert.Equal("TestService", settings.ServiceName);
                Assert.Equal(MetadataPath, settings.Endpoint);
                // moves endpoint to top of mru list
                Assert.Equal(MetadataPath, settings.MruEndpoints.First());
                Assert.Equal(1, settings.MruEndpoints.Count(e => e == MetadataPath));
                Assert.True(settings.IncludeCustomHeaders);
                Assert.True(settings.StoreCustomHttpHeaders);
                Assert.Null(settings.CustomHttpHeaders); // Custom HTTP headers may contain sensitive details like auth tokens
                Assert.True(settings.IncludeWebProxy);
                Assert.Equal("http://localhost:8080", settings.WebProxyHost);
                Assert.True(settings.IncludeWebProxyNetworkCredentials);
                Assert.True(settings.StoreWebProxyNetworkCredentials);
                Assert.Equal("domain", settings.WebProxyNetworkCredentialsDomain);
                // We don't persist web proxy network credentials
                Assert.Null(settings.WebProxyNetworkCredentialsUsername);
                Assert.Null(settings.WebProxyNetworkCredentialsPassword);
                settings.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetNearestAirport", "ResetDataSource" });
                settings.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>()
                {
                    "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"
                });
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
                Assert.True(config.StoreCustomHttpHeaders);
                Assert.Equal("Key:val", config.CustomHttpHeaders);
                Assert.True(config.IncludeWebProxy);
                Assert.Equal("http://localhost:8080", config.WebProxyHost);
                Assert.True(config.IncludeWebProxyNetworkCredentials);
                Assert.True(config.StoreCustomHttpHeaders);
                Assert.Equal("domain", config.WebProxyNetworkCredentialsDomain);
                Assert.Equal("user", config.WebProxyNetworkCredentialsUsername);
                Assert.Equal("pass", config.WebProxyNetworkCredentialsPassword);
                config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetNearestAirport", "ResetDataSource" });
                config.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>()
                {
                    "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"
                });
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
        }

        [Fact]
        public void GetFinishedServiceInstanceAsync_WhenUpdating_ShouldUseSavedConfigWhenUserDoesNotVisitPages()
        {
            var savedConfig = GetTestConfig();
            savedConfig.Endpoint = MetadataPath;
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
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
                Assert.True(config.StoreCustomHttpHeaders);
                Assert.Equal("Key1:Val1\nKey2:Val2", config.CustomHttpHeaders);
                Assert.True(config.IncludeWebProxy);
                Assert.Equal("http://localhost:8080", config.WebProxyHost);
                Assert.True(config.IncludeWebProxyNetworkCredentials);
                Assert.True(config.StoreWebProxyNetworkCredentials);
                Assert.Equal("domain", config.WebProxyNetworkCredentialsDomain);
                Assert.Equal("username", config.WebProxyNetworkCredentialsUsername);
                Assert.Equal("password", config.WebProxyNetworkCredentialsPassword);
                config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "GetPersonWithMostFriends", "ResetDataSource" });
                config.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>()
                {
                    "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)",
                    "ShareTrip(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "UpdatePersonLastName(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"
                });
                config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.PersonGender",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip"
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
        }

        [Fact]
        public void ShouldPreserveState_WhenMovingBetweenPagesAndBack()
        {
            var context = new TestConnectedServiceProviderContext();
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.OnPageEnteringAsync(null).Wait();
                endpointPage.UserSettings.Endpoint = MetadataPath;
                endpointPage.OnPageLeavingAsync(null).Wait();

                var typesPage = wizard.SchemaTypesViewModel;
                typesPage.OnPageEnteringAsync(null).Wait();
                var typesPageView = typesPage.View as SchemaTypes;
                Assert.Equal(typesPage.SchemaTypesCount, typesPage.SchemaTypes.Count());
                Assert.Equal(typesPage.BoundOperationsCount, typesPage.SchemaTypes.SelectMany(x => x.BoundOperations).Count());
                Assert.Equal(typesPageView?.SelectedSchemaTypesCount.Text, typesPage.SchemaTypes.Count().ToString(CultureInfo.InvariantCulture));
                Assert.Equal(typesPageView?.SelectedBoundOperationsCount.Text, typesPage.SchemaTypes.SelectMany(x => x.BoundOperations).Count().ToString(CultureInfo.InvariantCulture));
                var typeEmployee = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Employee");
                typeEmployee.IsSelected = false;
                Assert.Equal(typesPageView?.SelectedSchemaTypesCount.Text,
                    (typesPage.SchemaTypesCount - typesPage.ExcludedSchemaTypeNames.Count()).ToString(CultureInfo.InvariantCulture));
                var boundOperationGetInvolvedPeople = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Trip")
                    ?.BoundOperations.FirstOrDefault(o => o.Name.Contains("GetInvolvedPeople"));
                boundOperationGetInvolvedPeople.IsSelected = false;
                Assert.Equal(typesPageView?.SelectedBoundOperationsCount.Text,
                    (typesPage.BoundOperationsCount -
                     typesPage.ExcludedBoundOperationsNames.Count()).ToString(CultureInfo.InvariantCulture));
                typesPage.OnPageLeavingAsync(null).Wait();

                var operationsPage = wizard.OperationImportsViewModel;
                operationsPage.OnPageEnteringAsync(null).Wait();
                var operationsPageView = operationsPage.View as OperationImports;
                Assert.Equal(operationsPage.OperationImportsCount, operationsPage.OperationImports.Count());
                Assert.Equal(operationsPageView?.SelectedOperationImportsCount.Text, operationsPage.OperationImportsCount.ToString(CultureInfo.InvariantCulture));
                var operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
                operationNearestAirport.IsSelected = false;
                Assert.Equal(operationsPageView?.SelectedOperationImportsCount.Text,
                    (operationsPage.OperationImportsCount - operationsPage.ExcludedOperationImportsNames.Count())
                    .ToString(CultureInfo.InvariantCulture));
                operationsPage.OnPageLeavingAsync(null).Wait();

                var advancedPage = wizard.AdvancedSettingsViewModel;
                advancedPage.OnPageEnteringAsync(null).Wait();
                advancedPage.UserSettings.UseDataServiceCollection = true;
                advancedPage.UserSettings.MakeTypesInternal = true;
                advancedPage.UserSettings.UseNamespacePrefix = true;
                advancedPage.OnPageLeavingAsync(null).Wait();

                endpointPage.OnPageEnteringAsync(null).Wait();
                Assert.Equal(Constants.DefaultServiceName, endpointPage.UserSettings.ServiceName);
                Assert.Equal(MetadataPath, endpointPage.UserSettings.Endpoint);
                endpointPage.UserSettings.ServiceName = "Service";
                endpointPage.UserSettings.IncludeCustomHeaders = true;
                endpointPage.UserSettings.StoreCustomHttpHeaders = true;
                endpointPage.UserSettings.StoreWebProxyNetworkCredentials = true;
                endpointPage.UserSettings.CustomHttpHeaders = "A:b";
                endpointPage.OnPageLeavingAsync(null).Wait();

                advancedPage.OnPageEnteringAsync(null).Wait();
                Assert.True(advancedPage.UserSettings.UseNamespacePrefix);
                Assert.True(advancedPage.UserSettings.UseDataServiceCollection);
                Assert.True(advancedPage.UserSettings.MakeTypesInternal);
                advancedPage.UserSettings.NamespacePrefix = "MyNamespace";
                advancedPage.UserSettings.GenerateMultipleFiles = true;
                advancedPage.UserSettings.UseDataServiceCollection = false;
                advancedPage.OnPageLeavingAsync(null).Wait();

                operationsPage.OnPageEnteringAsync(null).Wait();
                operationNearestAirport = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "GetNearestAirport");
                Assert.False(operationNearestAirport.IsSelected);
                Assert.Equal(operationsPageView?.SelectedOperationImportsCount.Text,
                    (operationsPage.OperationImportsCount - operationsPage.ExcludedOperationImportsNames.Count())
                    .ToString(CultureInfo.InvariantCulture));
                var operationResetDataSource = operationsPage.OperationImports.FirstOrDefault(o => o.Name == "ResetDataSource");
                operationResetDataSource.IsSelected = false;
                operationsPage.OnPageLeavingAsync(null).Wait();

                typesPage.OnPageEnteringAsync(null).Wait();
                typeEmployee = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Employee");
                Assert.False(typeEmployee.IsSelected);
                Assert.Equal(typesPageView?.SelectedSchemaTypesCount.Text,
                    (typesPage.SchemaTypesCount - typesPage.ExcludedSchemaTypeNames.Count()).ToString(CultureInfo.InvariantCulture));
                typeEmployee.IsSelected = true;
                var typeFlight = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Flight");
                typeFlight.IsSelected = false;
                typesPage.OnPageLeavingAsync(null).Wait();

                var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
                var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

                Assert.Equal("Service", config.ServiceName);
                Assert.Equal(MetadataPath, config.Endpoint);
                Assert.True(config.IncludeCustomHeaders);
                Assert.True(config.StoreCustomHttpHeaders);
                Assert.True(config.StoreWebProxyNetworkCredentials);
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
                config.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>()
                {
                    "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)"
                });
                config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight"
                });
            }
        }

        [Fact]
        public void ShouldPreserveState_WhenMovingBetweenPagesAndBack_WhenUpdating()
        {
            var savedConfig = GetTestConfig();
            savedConfig.Endpoint = MetadataPath;
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
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

                var boundOperationGetFavoriteAirline = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Person").BoundOperations.FirstOrDefault(o => o.Name == "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)");
                Assert.False(boundOperationGetFavoriteAirline.IsSelected);

                var advancedPage = wizard.AdvancedSettingsViewModel;
                advancedPage.OnPageEnteringAsync(null).Wait();
                advancedPage.UserSettings.UseDataServiceCollection = true;
                advancedPage.UserSettings.MakeTypesInternal = true;
                advancedPage.UserSettings.UseNamespacePrefix = true;
                advancedPage.OnPageLeavingAsync(null).Wait();

                endpointPage.OnPageEnteringAsync(null).Wait();
                Assert.Equal(savedConfig.ServiceName, endpointPage.UserSettings.ServiceName);
                Assert.Equal(savedConfig.Endpoint, endpointPage.UserSettings.Endpoint);
                endpointPage.UserSettings.IncludeCustomHeaders = true;
                endpointPage.UserSettings.CustomHttpHeaders = "A:b";
                endpointPage.OnPageLeavingAsync(null).Wait();

                advancedPage.OnPageEnteringAsync(null).Wait();
                Assert.True(advancedPage.UserSettings.UseNamespacePrefix);
                Assert.True(advancedPage.UserSettings.UseDataServiceCollection);
                Assert.True(advancedPage.UserSettings.MakeTypesInternal);
                advancedPage.UserSettings.NamespacePrefix = "MyNamespace";
                advancedPage.UserSettings.GenerateMultipleFiles = true;
                advancedPage.UserSettings.UseDataServiceCollection = false;
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
                boundOperationGetFavoriteAirline = typesPage.SchemaTypes.FirstOrDefault(t => t.ShortName == "Person").BoundOperations.FirstOrDefault(o => o.Name == "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)");
                Assert.False(boundOperationGetFavoriteAirline.IsSelected);
                typesPage.OnPageLeavingAsync(null).Wait();

                var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
                var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

                Assert.Equal(savedConfig.ServiceName, config.ServiceName);
                Assert.Equal(savedConfig.Endpoint, config.Endpoint);
                Assert.True(config.IncludeCustomHeaders);
                Assert.True(config.StoreCustomHttpHeaders);
                Assert.True(config.StoreWebProxyNetworkCredentials);
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
                config.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>()
                {
                    "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)",
                    "ShareTrip(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "UpdatePersonLastName(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"
                });
                config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>()
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip"
                });
            }
        }

        [Fact]
        public void ShouldReloadOperationsAndTypesForNewEndpoint_WhenEndpointIsChangedBeforeFinishing()
        {
            var context = new TestConnectedServiceProviderContext();
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.OnPageEnteringAsync(null).Wait();
                endpointPage.UserSettings.Endpoint = MetadataPath;
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
                endpointPage.UserSettings.Endpoint = MetadataPathSimple;
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
                    new OperationImportModel
                    {
                        ReturnType = "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Thing",
                        ParametersString = "()",
                        Name = "GetRandomThing",
                        IsSelected = true
                    },
                    new OperationImportModel
                    {
                        ReturnType = "void",
                        ParametersString = "()",
                        Name = "ResetThings",
                        IsSelected = true
                    }
                });
                operationsPage.OperationImports.FirstOrDefault(o => o.Name == "ResetThings").IsSelected = false;

                var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
                var config = serviceInstance.ServiceConfig as ServiceConfigurationV4;

                Assert.Equal(MetadataPathSimple, config.Endpoint);
                config.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "ResetThings" });
                config.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>() { "SimpleService.Models.OtherThing" });
            }
        }

        [Fact]
        public void ShouldDeselectOperations_WhenRelatedTypeIsDeselectedBefore()
        {
            var context = new TestConnectedServiceProviderContext();
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.OnPageEnteringAsync(null).Wait();
                endpointPage.UserSettings.Endpoint = MetadataPath;
                endpointPage.OnPageLeavingAsync(null).Wait();

                var typesPage = wizard.SchemaTypesViewModel;
                typesPage.OnPageEnteringAsync(null).Wait();
                typesPage.SchemaTypes.First(t => t.ShortName == "Airport").IsSelected = false;
                typesPage.OnPageLeavingAsync(null).Wait();

                var operationsPage = wizard.OperationImportsViewModel;
                operationsPage.OnPageEnteringAsync(null).Wait();
                operationsPage.OperationImports.First(o => o.Name == "GetNearestAirport").IsSelected.Should().BeFalse();
                operationsPage.OnPageLeavingAsync(null).Wait();

                var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
                var config = serviceInstance?.ServiceConfig as ServiceConfigurationV4;

                config?.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string> { "GetNearestAirport" });
                config?.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>());
                config?.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Airport",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Flight"
                });
            }
        }

        [Fact]
        public void ShouldDeselectBoundOperations_WhenRelatedTypeIsDeselectedBefore()
        {
            var context = new TestConnectedServiceProviderContext();
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.OnPageEnteringAsync(null).Wait();
                endpointPage.UserSettings.Endpoint = MetadataPath;
                endpointPage.OnPageLeavingAsync(null).Wait();

                var typesPage = wizard.SchemaTypesViewModel;
                typesPage.OnPageEnteringAsync(null).Wait();
                typesPage.SchemaTypes.First(t => t.ShortName == "Person").IsSelected = false;
                typesPage.OnPageLeavingAsync(null).Wait();

                var operationsPage = wizard.OperationImportsViewModel;
                operationsPage.OnPageEnteringAsync(null).Wait();
                operationsPage.OperationImports.First(o => o.Name == "GetPersonWithMostFriends").IsSelected.Should().BeFalse();
                operationsPage.OnPageLeavingAsync(null).Wait();

                var serviceInstance = wizard.GetFinishedServiceInstanceAsync().Result as ODataConnectedServiceInstance;
                var config = serviceInstance?.ServiceConfig as ServiceConfigurationV4;

                config?.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string> { "GetPersonWithMostFriends" });
                config?.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>
                {
                    "GetFavoriteAirline(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetFriendsTrips(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "GetInvolvedPeople(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip)",
                    "ShareTrip(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)",
                    "UpdatePersonLastName(Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person)"
                });
                config?.ExcludedSchemaTypes.ShouldBeEquivalentTo(new List<string>
                {
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Employee",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Person",
                    "Microsoft.OData.Service.Sample.TrippinInMemory.Models.Trip"
                });
            }
        }

        [Fact]
        public void UnsupportedFeaturesAreDisabledOrHidden_WhenServiceIsV3OrLess()
        {
            var context = new TestConnectedServiceProviderContext();
            using(var wizard = new ODataConnectedServiceWizard(context))
            {
                var endpointPage = wizard.ConfigODataEndpointViewModel;
                endpointPage.UserSettings.Endpoint = MetadataPathV3;
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
