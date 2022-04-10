//---------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceWizard.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.OData.ConnectedService.Common;

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

        public ODataConnectedServiceInstance ServiceInstance => serviceInstance ?? (serviceInstance = new ODataConnectedServiceInstance());

        public Version EdmxVersion => ConfigODataEndpointViewModel.EdmxVersion;

        public UserSettings UserSettings { get; internal set; }

        public ServiceConfigurationV4 ServiceConfig { get; private set; }

        internal string ProcessedEndpointForOperationImports;

        internal string ProcessedEndpointForSchemaTypes;

        public ODataConnectedServiceWizard(ConnectedServiceProviderContext context)
        {
            Context = context;
            // We only use most recently used endpoints from the config file saved in user's isolated storage
            // The UserSettings constructor will load those endpoints
            UserSettings = new UserSettings(context?.Logger);

            // Since ServiceConfigurationV4 is a derived type of ServiceConfiguration,
            // we can deserialize a ServiceConfiguration into a ServiceConfigurationV4.
            ServiceConfig = Context?.GetExtendedDesignerData<ServiceConfigurationV4>();

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(UserSettings);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel(UserSettings);
            SchemaTypesViewModel = new SchemaTypesViewModel(UserSettings);
            OperationImportsViewModel = new OperationImportsViewModel(UserSettings);

            OperationImportsViewModel.PageEntering += OperationImportsViewModel_PageEntering;
            SchemaTypesViewModel.PageEntering += SchemaTypeSelectionViewModel_PageEntering;
            SchemaTypesViewModel.PageLeaving += SchemaTypeSelectionViewModel_PageLeaving;

            if (Context != null && Context.IsUpdating)
            {
                LoadUserSettingsFromServiceConfig();
                ConfigODataEndpointViewModel.EdmxVersion = ServiceConfig?.EdmxVersion;

                // Restore the main settings to UI elements.
                ConfigODataEndpointViewModel.PageEntering += ConfigODataEndpointViewModel_PageEntering;
                // The ViewModel should always be filled otherwise if the wizard is completed without visiting this page the generated code becomes wrong
                AdvancedSettingsViewModel_PageEntering(AdvancedSettingsViewModel, EventArgs.Empty);
                // Restore the advanced settings to UI elements.
                AdvancedSettingsViewModel.PageEntering += AdvancedSettingsViewModel_PageEntering;
            }

            Pages.Add(ConfigODataEndpointViewModel);
            Pages.Add(SchemaTypesViewModel);
            Pages.Add(OperationImportsViewModel);
            Pages.Add(AdvancedSettingsViewModel);
            IsFinishEnabled = true;
        }

        public override async Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            // ensure that the data has been loaded from wizard pages and saved to UserSettings
            if (Context.IsUpdating)
            {
                if (!OperationImportsViewModel.IsEntered)
                {
                    await OperationImportsViewModel.OnPageEnteringAsync(null).ConfigureAwait(false);
                    await OperationImportsViewModel.OnPageLeavingAsync(null).ConfigureAwait(false);
                }

                if (!SchemaTypesViewModel.IsEntered)
                {
                    await SchemaTypesViewModel.OnPageEnteringAsync(null).ConfigureAwait(false);
                    await SchemaTypesViewModel.OnPageLeavingAsync(null).ConfigureAwait(false);
                }

                if (!AdvancedSettingsViewModel.IsEntered)
                {
                    await AdvancedSettingsViewModel.OnPageEnteringAsync(null).ConfigureAwait(false);
                    await AdvancedSettingsViewModel.OnPageLeavingAsync(null).ConfigureAwait(false);
                }
            }

            UserSettings.Save();
            ServiceInstance.InstanceId = UserSettings.GeneratedFileNamePrefix;
            ServiceInstance.Name = UserSettings.ServiceName;
            ServiceInstance.MetadataTempFilePath = ConfigODataEndpointViewModel.MetadataTempPath;
            ServiceInstance.ServiceConfig = CreateServiceConfiguration();

            return await Task.FromResult<ConnectedServiceInstance>(ServiceInstance).ConfigureAwait(false);
        }

        /// <summary>
        /// Create the service configuration according to the edmx version.
        /// </summary>
        /// <returns>If the edm version is less than 4.0, returns a ServiceConfiguration, else, returns ServiceConfigurationV4</returns>
        private ServiceConfiguration CreateServiceConfiguration()
        {
            ServiceConfiguration serviceConfiguration;

            if (ConfigODataEndpointViewModel.EdmxVersion == Constants.EdmxVersion4)
            {
                var serviceConfigurationV4 = new ServiceConfigurationV4();
                serviceConfigurationV4.CopyPropertiesFrom(UserSettings);

                serviceConfigurationV4.ExcludedOperationImports = OperationImportsViewModel.ExcludedOperationImportsNames.ToList();
                serviceConfigurationV4.ExcludedBoundOperations = SchemaTypesViewModel.ExcludedBoundOperationsNames.ToList();

                serviceConfiguration = serviceConfigurationV4;
            }
            else
            {
                serviceConfiguration = new ServiceConfiguration();

                serviceConfiguration.CopyPropertiesFrom(UserSettings);
            }

            serviceConfiguration.ExcludedSchemaTypes = SchemaTypesViewModel.ExcludedSchemaTypeNames.ToList();
            serviceConfiguration.EdmxVersion = ConfigODataEndpointViewModel.EdmxVersion;

            return serviceConfiguration;
        }

        private void LoadUserSettingsFromServiceConfig()
        {
            if (ServiceConfig == null)
            {
                return;
            }

            UserSettings.CopyPropertiesFrom(ServiceConfig);
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
                    configOdataView.OpenEndpointFileButton.IsEnabled = !ServiceConfig.Endpoint.StartsWith("http", StringComparison.OrdinalIgnoreCase);
                    configOdataView.ServiceName.IsEnabled = false;
                }
            }
        }

        public void AdvancedSettingsViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is AdvancedSettingsViewModel advancedSettingsViewModel)
            {
                if (advancedSettingsViewModel.View is AdvancedSettings advancedSettings)
                {
                    advancedSettings.ReferenceFileName.IsEnabled = !Context.IsUpdating;
                    advancedSettings.GenerateMultipleFiles.IsEnabled = !Context.IsUpdating;

                    if (ServiceConfig.EdmxVersion == Constants.EdmxVersion4)
                    {
                        advancedSettings.IncludeT4File.IsEnabled = !Context.IsUpdating;
                    }
                }
            }
        }

        public void OperationImportsViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is OperationImportsViewModel operationImportsViewModel)
            {
                if (ProcessedEndpointForOperationImports != UserSettings.Endpoint)
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
                        operationImportsViewModel.ExcludeOperationImports(ServiceConfig?.ExcludedOperationImports ?? Enumerable.Empty<string>());
                    }
                }

                ProcessedEndpointForOperationImports = UserSettings.Endpoint;
            }
        }

        public void SchemaTypeSelectionViewModel_PageEntering(object sender, EventArgs args)
        {
            if (sender is SchemaTypesViewModel entityTypeViewModel)
            {
                if (ProcessedEndpointForSchemaTypes != UserSettings.Endpoint)
                {
                    var model = EdmHelper.GetEdmModelFromFile(ConfigODataEndpointViewModel.MetadataTempPath);
                    var entityTypes = EdmHelper.GetSchemaTypes(model);
                    var boundOperations = EdmHelper.GetBoundOperations(model);
                    SchemaTypesViewModel.LoadSchemaTypes(entityTypes, boundOperations);

                    if (Context.IsUpdating)
                    {
                        entityTypeViewModel.ExcludeSchemaTypes(
                            ServiceConfig?.ExcludedSchemaTypes ?? Enumerable.Empty<string>(),
                            ServiceConfig?.ExcludedBoundOperations ?? Enumerable.Empty<string>());
                    }
                }

                ProcessedEndpointForSchemaTypes = UserSettings.Endpoint;
            }
        }

        public void SchemaTypeSelectionViewModel_PageLeaving(object sender, EventArgs args)
        {
            var model = EdmHelper.GetEdmModelFromFile(ConfigODataEndpointViewModel.MetadataTempPath);

            // exclude related operation imports for excluded types
            var operations = EdmHelper.GetOperationImports(model);
            var operationsToExclude = operations.Where(x => !OperationImportsViewModel.IsOperationImportIncluded(x,
                SchemaTypesViewModel.ExcludedSchemaTypeNames.ToList())).ToList();
            foreach (var operationImport in OperationImportsViewModel.OperationImports)
            {
                if (operationsToExclude.Any(x => x.Name == operationImport.Name))
                {
                    operationImport.IsSelected = false;
                }
            }

            // exclude bound operations for excluded types
            var boundOperations = EdmHelper.GetBoundOperations(model);
            var boundOperationsToExclude = boundOperations.SelectMany(x => x.Value)
                .Where(x => !SchemaTypesViewModel.IsBoundOperationIncluded(x,
                    SchemaTypesViewModel.ExcludedSchemaTypeNames.ToList())).ToList();
            foreach (var boundOperation in SchemaTypesViewModel.SchemaTypes.SelectMany(x => x.BoundOperations))
            {
                if (boundOperationsToExclude.Any(x => $"{x.Name}({x.Parameters.First().Type.Definition.FullTypeName()})" == boundOperation.Name))
                {
                    boundOperation.IsSelected = false;
                }
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
                    if (AdvancedSettingsViewModel != null)
                    {
                        AdvancedSettingsViewModel.Dispose();
                        AdvancedSettingsViewModel = null;
                    }

                    if (OperationImportsViewModel != null)
                    {
                        OperationImportsViewModel.Dispose();
                        OperationImportsViewModel = null;
                    }

                    if (SchemaTypesViewModel != null)
                    {
                        SchemaTypesViewModel.Dispose();
                        SchemaTypesViewModel = null;
                    }

                    if (ConfigODataEndpointViewModel != null)
                    {
                        ConfigODataEndpointViewModel.Dispose();
                        ConfigODataEndpointViewModel = null;
                    }

                    if (serviceInstance != null)
                    {
                        serviceInstance.Dispose();
                        serviceInstance = null;
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
