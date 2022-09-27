// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using Microsoft.OData.ConnectedService.Common;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ConfigODataEndpoint.xaml
    /// </summary>
    public partial class ConfigODataEndpoint : UserControl
    {
        internal UserSettings UserSettings { get; set; }

        public ConfigODataEndpoint()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ODataConnectedServiceWizard connectedServiceWizard = GetODataConnectedServiceWizard();
            UserSettings = connectedServiceWizard.UserSettings;

            if (Endpoint.Items.Count > 0 && !connectedServiceWizard.Context.IsUpdating)
            {
                Endpoint.SelectedItem = Endpoint.Items[0];
            }
        }

        private void OpenMetadataFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "OData Metadata Files (.xml)|*.xml",
                Title = "Open OData Metadata File",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Endpoint.Text = openFileDialog.FileName;
            }
        }

        private void OpenConnectedServiceJsonFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialogTitle = "Open OData Connected Service Config File";

            var openFileDialog = new Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON File (.json)|*.json",
                Title = fileDialogTitle
            };

            if (!(openFileDialog.ShowDialog() == true)) // Result of ShowDialog() call is bool?
            {
                return;
            }

            if (!File.Exists(openFileDialog.FileName))
            {
               MessageBox.Show(
                   $"File \"{openFileDialog.FileName}\" does not exists.",
                   string.Format(CultureInfo.InvariantCulture, fileDialogTitle),
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning);
               return;
            }

            var jsonFileText = File.ReadAllText(openFileDialog.FileName);
            if (string.IsNullOrWhiteSpace(jsonFileText))
            {
               MessageBox.Show("Config file is empty.",
                   string.Format(CultureInfo.InvariantCulture, fileDialogTitle),
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning);
               return;
            }

            ConnectedServiceJsonFileData connectedServiceData;

            try
            {
                connectedServiceData = JsonConvert.DeserializeObject<ConnectedServiceJsonFileData>(jsonFileText);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.Assert(ex != null);
                MessageBox.Show(
                    "Contents of the config file could not be deserialized.",
                    string.Format(CultureInfo.InvariantCulture, fileDialogTitle),
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // connectedServiceData not expected to be null at this point
            if (connectedServiceData.ExtendedData != null)
            {
                this.UserSettings.CopyPropertiesFrom(connectedServiceData.ExtendedData);
            }

            ODataConnectedServiceWizard connectedServiceWizard = GetODataConnectedServiceWizard();

            // get Operation Imports and bound operations from metadata for excluding ExcludedOperationImports and ExcludedBoundOperations
            try
            {
                var serviceConfiguration = GetServiceConfiguration();
                connectedServiceWizard.ConfigODataEndpointViewModel.MetadataTempPath = CodeGen.Common.MetadataReader.ProcessServiceMetadata(serviceConfiguration, out var version);
                connectedServiceWizard.ConfigODataEndpointViewModel.EdmxVersion = version;
                if (version == Constants.EdmxVersion4)
                {
                    Edm.IEdmModel model = EdmHelper.GetEdmModelFromFile(connectedServiceWizard.ConfigODataEndpointViewModel.MetadataTempPath);

                    IEnumerable<Edm.IEdmSchemaType> entityTypes = EdmHelper.GetSchemaTypes(model);
                    IDictionary<Edm.IEdmType, List<Edm.IEdmOperation>> boundOperations = EdmHelper.GetBoundOperations(model);
                    connectedServiceWizard.SchemaTypesViewModel.LoadSchemaTypes(entityTypes, boundOperations);
                    connectedServiceWizard.ProcessedEndpointForSchemaTypes = this.UserSettings.Endpoint;
                    connectedServiceWizard.SchemaTypesViewModel.LoadFromUserSettings();

                    IEnumerable<Edm.IEdmOperationImport> operations = EdmHelper.GetOperationImports(model);
                    connectedServiceWizard.OperationImportsViewModel.LoadOperationImports(operations, new HashSet<string>(), new Dictionary<string, SchemaTypeModel>());
                    connectedServiceWizard.ProcessedEndpointForOperationImports = this.UserSettings.Endpoint;
                    connectedServiceWizard.OperationImportsViewModel.LoadFromUserSettings();
                }
            }
            catch
            {
                // ignored
            }
        }

        private ODataConnectedServiceWizard GetODataConnectedServiceWizard()
        {
            return (ODataConnectedServiceWizard)((ConfigODataEndpointViewModel)this.DataContext).Wizard;
        }

        private ServiceConfiguration GetServiceConfiguration()
        {
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
            serviceConfiguration.Endpoint = this.UserSettings.Endpoint;
            serviceConfiguration.CustomHttpHeaders = this.UserSettings.CustomHttpHeaders;
            serviceConfiguration.WebProxyHost = this.UserSettings.WebProxyHost;
            serviceConfiguration.IncludeWebProxy = this.UserSettings.IncludeWebProxy;
            serviceConfiguration.IncludeWebProxyNetworkCredentials = this.UserSettings.IncludeWebProxyNetworkCredentials;
            serviceConfiguration.WebProxyNetworkCredentialsUsername = this.UserSettings.WebProxyNetworkCredentialsUsername;
            serviceConfiguration.WebProxyNetworkCredentialsPassword = this.UserSettings.WebProxyNetworkCredentialsPassword;
            serviceConfiguration.WebProxyNetworkCredentialsDomain = this.UserSettings.WebProxyNetworkCredentialsDomain;

            return serviceConfiguration;
        }
    }
}
