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

        public ObjectSelectionViewModel ObjectSelectionViewModel { get; set; }

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
            ObjectSelectionViewModel = new ObjectSelectionViewModel();

            ServiceConfigurationV4 serviceConfig = null;

            ConfigODataEndpointViewModel.PageLeaving += ConfigODataEndpointViewModel_PageLeaving;
            ObjectSelectionViewModel.PageEntering += ObjectSelectionViewModel_PageEntering;

            if (this.Context.IsUpdating)
            {
                //Since ServiceConfigurationV4 is a derived type of ServiceConfiguration. So we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
                serviceConfig = this.Context.GetExtendedDesignerData<ServiceConfigurationV4>();
                ConfigODataEndpointViewModel.Endpoint = serviceConfig.Endpoint;
                ConfigODataEndpointViewModel.EdmxVersion = serviceConfig.EdmxVersion;
                ConfigODataEndpointViewModel.ServiceName = serviceConfig.ServiceName;
                ConfigODataEndpointViewModel.CustomHttpHeaders = serviceConfig.CustomHttpHeaders;

                if (ConfigODataEndpointViewModel.View is ConfigODataEndpoint configODataEndpoint)
                    configODataEndpoint.IsEnabled = false;

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

        public void AddObjectSelectionPage()
        {
            if (!Pages.Contains(ObjectSelectionViewModel))
            {
                Pages.Add(ObjectSelectionViewModel);
            }
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
                ServiceConfigurationV4.ExcludedOperationImports = ObjectSelectionViewModel.ExcludedOperationImportsNames.ToList();
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

        public void ConfigODataEndpointViewModel_PageLeaving(object sender, EventArgs args)
        {
            if (ConfigODataEndpointViewModel.EdmxVersion == Constants.EdmxVersion4)
            {
                AddObjectSelectionPage();
            }
        }

        public void ObjectSelectionViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is ObjectSelectionViewModel objectSelectionViewModel)
            {
                var model = EdmHelper.GetEdmModelFromFile(ConfigODataEndpointViewModel.MetadataTempPath);
                var operations = EdmHelper.GetOperationImports(model);
                ObjectSelectionViewModel.LoadOperationImports(operations);

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

                    if (this.ObjectSelectionViewModel != null)
                    {
                        this.ObjectSelectionViewModel.Dispose();
                        ObjectSelectionViewModel = null;
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