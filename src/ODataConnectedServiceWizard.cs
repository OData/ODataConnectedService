// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    internal class ODataConnectedServiceWizard : ConnectedServiceWizard
    {
        private ODataConnectedServiceInstance serviceInstance;

        public ConfigODataEndpointViewModel ConfigODataEndpointViewModel { get; set; }

        public OperationImportsViewModel OperationImportsViewModel { get; set; }

        public AdvancedSettingsViewModel AdvancedSettingsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        public ODataConnectedServiceInstance ServiceInstance => this.serviceInstance ?? (this.serviceInstance = new ODataConnectedServiceInstance());


        public Version EdmxVersion => this.ConfigODataEndpointViewModel.EdmxVersion;

        public UserSettings UserSettings { get; }

        public ODataConnectedServiceWizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this.UserSettings = UserSettings.Load(context.Logger);

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(this.UserSettings);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel(this.UserSettings);
            OperationImportsViewModel = new OperationImportsViewModel();

            ServiceConfigurationV4 serviceConfig = null;

            OperationImportsViewModel.PageEntering += ObjectSelectionViewModel_PageEntering;

            if (this.Context.IsUpdating)
            {
                //Since ServiceConfigurationV4 is a derived type of ServiceConfiguration. So we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
                serviceConfig = this.Context.GetExtendedDesignerData<ServiceConfigurationV4>();
                ConfigODataEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigODataEndpointViewModel.EdmxVersion = serviceConfig.EdmxVersion;
                ConfigODataEndpointViewModel.ServiceName = serviceConfig.ServiceName;
                ConfigODataEndpointViewModel.CustomHttpHeaders = serviceConfig.CustomHttpHeaders;



                // The ViewModel should always be filled otherwise if the wizard is completed without visiting this page the generated code becomes wrong
                AdvancedSettingsViewModel.UseNamespacePrefix = serviceConfig.UseNameSpacePrefix;
                AdvancedSettingsViewModel.NamespacePrefix = serviceConfig.NamespacePrefix;
                AdvancedSettingsViewModel.UseDataServiceCollection = serviceConfig.UseDataServiceCollection;

                if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                {
                    AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes =
                        serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                    AdvancedSettingsViewModel.EnableNamingAlias = serviceConfig.EnableNamingAlias;
                    AdvancedSettingsViewModel.IncludeT4File = serviceConfig.IncludeT4File;
                    AdvancedSettingsViewModel.MakeTypesInternal = serviceConfig.MakeTypesInternal;
                }


                ConfigODataEndpointViewModel.PageEntering += (sender, args) =>
                {

                    var configOdataViewModel = sender as ConfigODataEndpointViewModel;
                    if (configOdataViewModel != null)
                    {
                        var configOdataView = configOdataViewModel.View as ConfigODataEndpoint;
                        configOdataView.Endpoint.IsEnabled = false;
                        configOdataView.OpenEndpointFileButton.IsEnabled = !serviceConfig.Endpoint.StartsWith("http");
                        configOdataView.ServiceName.IsEnabled = false;
                        configOdataViewModel.IncludeCustomHeaders = serviceConfig.IncludeCustomHeaders;
                        configOdataViewModel.IncludeWebProxy = serviceConfig.IncludeWebProxy;
                        configOdataViewModel.WebProxyHost = serviceConfig.WebProxyHost;

                        configOdataViewModel.IncludeWebProxyNetworkCredentials = serviceConfig.IncludeWebProxyNetworkCredentials;
                        configOdataViewModel.WebProxyNetworkCredentialsDomain = serviceConfig.WebProxyNetworkCredentialsDomain;

                       // don't accept any credentials from the restored settings
                        configOdataViewModel.WebProxyNetworkCredentialsUsername = null;
                        configOdataViewModel.WebProxyNetworkCredentialsPassword = null;
                    }
                };


                //Restore the advanced settings to UI elements.
                AdvancedSettingsViewModel.PageEntering += (sender, args) =>
                {
                    if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
                    {
                        if (advancedSettingsViewModel.View is AdvancedSettings advancedSettings)
                        {
                            advancedSettingsViewModel.GeneratedFileName = serviceConfig.GeneratedFileNamePrefix;
                            advancedSettings.ReferenceFileName.IsEnabled = false;
                            advancedSettingsViewModel.UseNamespacePrefix = serviceConfig.UseNameSpacePrefix;
                            advancedSettingsViewModel.NamespacePrefix = serviceConfig.NamespacePrefix;
                            advancedSettingsViewModel.UseDataServiceCollection = serviceConfig.UseDataServiceCollection;
                            advancedSettingsViewModel.GenerateMultipleFiles = serviceConfig.GenerateMultipleFiles;
                            advancedSettings.GenerateMultipleFiles.IsEnabled = false;

                            if (serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                            {
                                advancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes = serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                                advancedSettingsViewModel.EnableNamingAlias = serviceConfig.EnableNamingAlias;
                                advancedSettingsViewModel.IncludeT4File = serviceConfig.IncludeT4File;
                                advancedSettingsViewModel.MakeTypesInternal = serviceConfig.MakeTypesInternal;
                                advancedSettings.IncludeT4File.IsEnabled = false;
                            }
                        }
                    }
                };
            }

            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(OperationImportsViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);
            this.IsFinishEnabled = true;
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            this.UserSettings.Save();

            this.ServiceInstance.InstanceId = AdvancedSettingsViewModel.GeneratedFileName;
            this.ServiceInstance.Name = ConfigODataEndpointViewModel.ServiceName;
            this.ServiceInstance.MetadataTempFilePath = ConfigODataEndpointViewModel.MetadataTempPath;
            this.ServiceInstance.ServiceConfig = this.CreateServiceConfiguration();

            return Task.FromResult<ConnectedServiceInstance>(this.ServiceInstance);
        }

        /// <summary>
        /// Create the service configuration according to the edmx version.
        /// </summary>
        /// <returns>If the edm version is less than 4.0, returns a ServiceConfiguration, else, returns ServiceConfigurationV4</returns>
        private ServiceConfiguration CreateServiceConfiguration()
        {
            ServiceConfiguration serviceConfiguration;
            if (ConfigODataEndpointViewModel.EdmxVersion == Common.Constants.EdmxVersion4)
            {
                var ServiceConfigurationV4 = new ServiceConfigurationV4();
                ServiceConfigurationV4.ExcludedOperationImports = OperationImportsViewModel.ExcludedOperationImportsNames.ToList();
                ServiceConfigurationV4.IgnoreUnexpectedElementsAndAttributes = AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes;
                ServiceConfigurationV4.EnableNamingAlias = AdvancedSettingsViewModel.EnableNamingAlias;
                ServiceConfigurationV4.IncludeT4File = AdvancedSettingsViewModel.IncludeT4File;
                ServiceConfigurationV4.OpenGeneratedFilesInIDE = AdvancedSettingsViewModel.OpenGeneratedFilesInIDE;
                serviceConfiguration = ServiceConfigurationV4;
            }
            else
            {
                serviceConfiguration = new ServiceConfiguration();
            }

            serviceConfiguration.ServiceName = ConfigODataEndpointViewModel.ServiceName;
            serviceConfiguration.Endpoint = ConfigODataEndpointViewModel.Endpoint;
            serviceConfiguration.EdmxVersion = ConfigODataEndpointViewModel.EdmxVersion;
            serviceConfiguration.CustomHttpHeaders = ConfigODataEndpointViewModel.CustomHttpHeaders;
            serviceConfiguration.IncludeWebProxy = ConfigODataEndpointViewModel.IncludeWebProxy;
            serviceConfiguration.WebProxyHost = ConfigODataEndpointViewModel.WebProxyHost;
            serviceConfiguration.IncludeWebProxyNetworkCredentials = ConfigODataEndpointViewModel.IncludeWebProxyNetworkCredentials;
            serviceConfiguration.WebProxyNetworkCredentialsUsername = ConfigODataEndpointViewModel.WebProxyNetworkCredentialsUsername;
            serviceConfiguration.WebProxyNetworkCredentialsPassword = ConfigODataEndpointViewModel.WebProxyNetworkCredentialsPassword;
            serviceConfiguration.WebProxyNetworkCredentialsDomain = ConfigODataEndpointViewModel.WebProxyNetworkCredentialsDomain;

            serviceConfiguration.IncludeCustomHeaders = ConfigODataEndpointViewModel.IncludeCustomHeaders;

            serviceConfiguration.UseDataServiceCollection = AdvancedSettingsViewModel.UseDataServiceCollection;
            serviceConfiguration.GeneratedFileNamePrefix = AdvancedSettingsViewModel.GeneratedFileName;
            serviceConfiguration.UseNameSpacePrefix = AdvancedSettingsViewModel.UseNamespacePrefix;
            serviceConfiguration.MakeTypesInternal = AdvancedSettingsViewModel.MakeTypesInternal;
            serviceConfiguration.OpenGeneratedFilesInIDE = AdvancedSettingsViewModel.OpenGeneratedFilesInIDE;
            serviceConfiguration.GenerateMultipleFiles = AdvancedSettingsViewModel.GenerateMultipleFiles;

            if (AdvancedSettingsViewModel.UseNamespacePrefix && !string.IsNullOrEmpty(AdvancedSettingsViewModel.NamespacePrefix))
            {
                serviceConfiguration.NamespacePrefix = AdvancedSettingsViewModel.NamespacePrefix;
            }

            return serviceConfiguration;
        }

        #region "Event Handlers"

        public void ObjectSelectionViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is OperationImportsViewModel objectSelectionViewModel)
            {
                if (ConfigODataEndpointViewModel.EdmxVersion != Constants.EdmxVersion4)
                {
                    objectSelectionViewModel.View.IsEnabled = false;
                    objectSelectionViewModel.IsSupportedODataVersion = false;
                    return;
                }
                var model = EdmHelper.GetEdmModelFromFile(ConfigODataEndpointViewModel.MetadataTempPath);
                var operations = EdmHelper.GetOperationImports(model);
                OperationImportsViewModel.LoadOperationImports(operations);

                if (Context.IsUpdating)
                {
                    var serviceConfig = Context.GetExtendedDesignerData<ServiceConfigurationV4>();
                    objectSelectionViewModel.ExcludeOperationImports(serviceConfig?.ExcludedOperationImports ?? Enumerable.Empty<string>());
                }
            }
        }

        #endregion

        /// <summary>
        /// Cleanup object references
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (this.AdvancedSettingsViewModel != null)
                    {
                        this.AdvancedSettingsViewModel.Dispose();
                        this.AdvancedSettingsViewModel = null;
                    }

                    if (this.OperationImportsViewModel != null)
                    {
                        this.OperationImportsViewModel.Dispose();
                        OperationImportsViewModel = null;
                    }

                    if (this.ConfigODataEndpointViewModel != null)
                    {
                        this.ConfigODataEndpointViewModel.Dispose();
                        this.ConfigODataEndpointViewModel = null;
                    }

                    if (this.serviceInstance != null)
                    {
                        this.serviceInstance.Dispose();
                        this.serviceInstance = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}