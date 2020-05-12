//---------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceWizard.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

        public SchemaTypesViewModel SchemaTypesViewModel { get; set; }

        public AdvancedSettingsViewModel AdvancedSettingsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        public ODataConnectedServiceInstance ServiceInstance => this.serviceInstance ?? (this.serviceInstance = new ODataConnectedServiceInstance());

        public Version EdmxVersion => this.ConfigODataEndpointViewModel.EdmxVersion;

        public UserSettings UserSettings { get; }

        private readonly ServiceConfigurationV4 _serviceConfig;

        public ServiceConfigurationV4 ServiceConfig { get { return _serviceConfig; } }

        internal string ProcessedEndpointForOperationImports;

        internal string ProcessedEndpointForSchemaTypes;

        public ODataConnectedServiceWizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this.UserSettings = UserSettings.Load(context?.Logger);

            // Since ServiceConfigurationV4 is a derived type of ServiceConfiguration. So we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
            this._serviceConfig = this.Context?.GetExtendedDesignerData<ServiceConfigurationV4>();

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(this.UserSettings, this);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel(this.UserSettings);
            SchemaTypesViewModel = new SchemaTypesViewModel(this.UserSettings);
            OperationImportsViewModel = new OperationImportsViewModel(this.UserSettings);

            OperationImportsViewModel.PageEntering += OperationImportsViewModel_PageEntering;

            SchemaTypesViewModel.PageEntering += SchemaTypeSelectionViewModel_PageEntering;
            if (this.Context != null && this.Context.IsUpdating)
            {
                ConfigODataEndpointViewModel.Endpoint = this._serviceConfig.Endpoint;
                ConfigODataEndpointViewModel.EdmxVersion = this._serviceConfig.EdmxVersion;
                ConfigODataEndpointViewModel.ServiceName = this._serviceConfig.ServiceName;
                ConfigODataEndpointViewModel.CustomHttpHeaders = this._serviceConfig.CustomHttpHeaders;

                // Restore the main settings to UI elements.
                ConfigODataEndpointViewModel.PageEntering += ConfigODataEndpointViewModel_PageEntering;

                // The ViewModel should always be filled otherwise if the wizard is completed without visiting this page the generated code becomes wrong
                AdvancedSettingsViewModel_PageEntering(AdvancedSettingsViewModel, EventArgs.Empty);

                // Restore the advanced settings to UI elements.
                AdvancedSettingsViewModel.PageEntering += AdvancedSettingsViewModel_PageEntering;
            }

            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(SchemaTypesViewModel);
            this.Pages.Add(OperationImportsViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);
            this.IsFinishEnabled = true;
        }

        public override async Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            // ensure that the data has been loaded from wizard pages and saved to UserSettings
            if (this.Context.IsUpdating)
            {
                if (!this.OperationImportsViewModel.IsEntered)
                {
                    await this.OperationImportsViewModel.OnPageEnteringAsync(null);
                    await this.OperationImportsViewModel.OnPageLeavingAsync(null);
                }

                if (!this.SchemaTypesViewModel.IsEntered)
                {
                    await this.SchemaTypesViewModel.OnPageEnteringAsync(null);
                    await this.SchemaTypesViewModel.OnPageLeavingAsync(null);
                }

                if (!this.AdvancedSettingsViewModel.IsEntered)
                {
                    await this.AdvancedSettingsViewModel.OnPageEnteringAsync(null);
                    await this.AdvancedSettingsViewModel.OnPageLeavingAsync(null);
                }
            }

            this.UserSettings.Save();
            this.ServiceInstance.InstanceId = AdvancedSettingsViewModel.GeneratedFileNamePrefix;
            this.ServiceInstance.Name = ConfigODataEndpointViewModel.ServiceName;
            this.ServiceInstance.MetadataTempFilePath = ConfigODataEndpointViewModel.MetadataTempPath;
            this.ServiceInstance.ServiceConfig = this.CreateServiceConfiguration();

            return await Task.FromResult<ConnectedServiceInstance>(this.ServiceInstance);
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
                var serviceConfigurationV4 = new ServiceConfigurationV4
                {
                    ExcludedOperationImports = OperationImportsViewModel.ExcludedOperationImportsNames.ToList(),
                    IgnoreUnexpectedElementsAndAttributes =
                        AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes,
                    EnableNamingAlias = AdvancedSettingsViewModel.EnableNamingAlias,
                    IncludeT4File = AdvancedSettingsViewModel.IncludeT4File,
                    OpenGeneratedFilesInIDE = AdvancedSettingsViewModel.OpenGeneratedFilesInIDE
                };
                serviceConfiguration = serviceConfigurationV4;
            }
            else
            {
                serviceConfiguration = new ServiceConfiguration();
            }

            serviceConfiguration.ExcludedSchemaTypes = SchemaTypesViewModel.ExcludedSchemaTypeNames.ToList();
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
            serviceConfiguration.GeneratedFileNamePrefix = AdvancedSettingsViewModel.GeneratedFileNamePrefix;
            serviceConfiguration.UseNamespacePrefix = AdvancedSettingsViewModel.UseNamespacePrefix;
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

        public void ConfigODataEndpointViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is ConfigODataEndpointViewModel configOdataViewModel)
            {
                if (configOdataViewModel.View is ConfigODataEndpoint configOdataView)
                {
                    configOdataView.Endpoint.IsEnabled = false;
                    configOdataView.OpenConnectedServiceJsonFileButton.IsEnabled = false;
                    configOdataView.OpenEndpointFileButton.IsEnabled = !this._serviceConfig.Endpoint.StartsWith("http");
                    configOdataView.ServiceName.IsEnabled = false;
                }

                configOdataViewModel.IncludeCustomHeaders = this._serviceConfig.IncludeCustomHeaders;
                configOdataViewModel.IncludeWebProxy = this._serviceConfig.IncludeWebProxy;
                configOdataViewModel.WebProxyHost = this._serviceConfig.WebProxyHost;
                configOdataViewModel.IncludeWebProxyNetworkCredentials = this._serviceConfig.IncludeWebProxyNetworkCredentials;
                configOdataViewModel.WebProxyNetworkCredentialsDomain = this._serviceConfig.WebProxyNetworkCredentialsDomain;

                // don't accept any credentials from the restored settings
                configOdataViewModel.WebProxyNetworkCredentialsUsername = null;
                configOdataViewModel.WebProxyNetworkCredentialsPassword = null;
            }
        }

        public void AdvancedSettingsViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
            {
                if (advancedSettingsViewModel.View is AdvancedSettings advancedSettings)
                {
                    advancedSettingsViewModel.GeneratedFileNamePrefix = this._serviceConfig.GeneratedFileNamePrefix;
                    advancedSettings.ReferenceFileName.IsEnabled = !this.Context.IsUpdating;
                    advancedSettingsViewModel.UseNamespacePrefix = this._serviceConfig.UseNamespacePrefix;
                    advancedSettingsViewModel.NamespacePrefix = this._serviceConfig.NamespacePrefix ?? Common.Constants.DefaultReferenceFileName;
                    advancedSettingsViewModel.UseDataServiceCollection = this._serviceConfig.UseDataServiceCollection;
                    advancedSettingsViewModel.OpenGeneratedFilesInIDE = this._serviceConfig.OpenGeneratedFilesInIDE;
                    advancedSettingsViewModel.GenerateMultipleFiles = this._serviceConfig.GenerateMultipleFiles;
                    advancedSettings.GenerateMultipleFiles.IsEnabled = !this.Context.IsUpdating;

                    if (this._serviceConfig.EdmxVersion == Common.Constants.EdmxVersion4)
                    {
                        advancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes = this._serviceConfig.IgnoreUnexpectedElementsAndAttributes;
                        advancedSettingsViewModel.EnableNamingAlias = this._serviceConfig.EnableNamingAlias;
                        advancedSettingsViewModel.IncludeT4File = this._serviceConfig.IncludeT4File;
                        advancedSettingsViewModel.MakeTypesInternal = this._serviceConfig.MakeTypesInternal;
                        advancedSettings.IncludeT4File.IsEnabled = !this.Context.IsUpdating;
                    }
                }
            }
        }

        public void OperationImportsViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is OperationImportsViewModel operationImportsViewModel)
            {
                if (this.ProcessedEndpointForOperationImports != ConfigODataEndpointViewModel.Endpoint)
                {
                    if (ConfigODataEndpointViewModel.EdmxVersion != Constants.EdmxVersion4)
                    {
                        operationImportsViewModel.View.IsEnabled = false;
                        operationImportsViewModel.IsSupportedODataVersion = false;
                        return;
                    }
                    var model = EdmHelper.GetEdmModelFromFile(ConfigODataEndpointViewModel.MetadataTempPath);
                    var operations = EdmHelper.GetOperationImports(model);
                    OperationImportsViewModel.LoadOperationImports(operations, new HashSet<string>(SchemaTypesViewModel.ExcludedSchemaTypeNames), SchemaTypesViewModel.SchemaTypeModelMap);

                    if (Context.IsUpdating)
                    {
                        operationImportsViewModel.ExcludeOperationImports(this._serviceConfig?.ExcludedOperationImports ?? Enumerable.Empty<string>());
                    }
                }

                this.ProcessedEndpointForOperationImports = ConfigODataEndpointViewModel.Endpoint;
            }
        }

        public void SchemaTypeSelectionViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is SchemaTypesViewModel entityTypeViewModel)
            {
                if (this.ProcessedEndpointForSchemaTypes != ConfigODataEndpointViewModel.Endpoint)
                {
                    var model = EdmHelper.GetEdmModelFromFile(ConfigODataEndpointViewModel.MetadataTempPath);
                    var entityTypes = EdmHelper.GetSchemaTypes(model);
                    var boundOperations = EdmHelper.GetBoundOperations(model);
                    SchemaTypesViewModel.LoadSchemaTypes(entityTypes, boundOperations);

                    if (Context.IsUpdating)
                    {
                        entityTypeViewModel.ExcludeSchemaTypes(this._serviceConfig?.ExcludedSchemaTypes ?? Enumerable.Empty<string>());
                    }
                }

                this.ProcessedEndpointForSchemaTypes = ConfigODataEndpointViewModel.Endpoint;
            }
        }

        #endregion

        #region Disposing

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

                    if (this.SchemaTypesViewModel != null)
                    {
                        this.SchemaTypesViewModel.Dispose();
                        this.SchemaTypesViewModel = null;
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

        #endregion
    }
}
