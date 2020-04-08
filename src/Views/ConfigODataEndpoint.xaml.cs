// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".json",
                Filter = "JSON File (.json)|*.json",
                Title = "Open OData Connected Service Config File"
            };

            var result = openFileDialog.ShowDialog();
            if (result == false)
                return;
            if (!File.Exists(openFileDialog.FileName))
            {
               MessageBox.Show($"File \"{openFileDialog.FileName}\" does not exists.", "Open OData Connected Service json-file", MessageBoxButton.OK, MessageBoxImage.Warning);
               return;
            }

            var jsonFileText = File.ReadAllText(openFileDialog.FileName);
            if (string.IsNullOrWhiteSpace(jsonFileText))
            {
              MessageBox.Show("File have not content.", "Open OData Connected Service json-file", MessageBoxButton.OK, MessageBoxImage.Warning);
               return;
            }
            if (JObject.Parse(jsonFileText) == null)
            {
               MessageBox.Show("Can't convert file content to JObject.", "Open OData Connected Service json-file", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var microsoftConnectedServiceData = JsonConvert.DeserializeObject<ConnectedServiceJsonFileData>(jsonFileText);
            if (microsoftConnectedServiceData != null)
            {
                this.UserSettings = new UserSettings();
                this.UserSettings.Endpoint = microsoftConnectedServiceData.ExtendedData?.Endpoint ?? this.UserSettings.Endpoint;
                this.UserSettings.ServiceName = microsoftConnectedServiceData.ExtendedData?.ServiceName ?? this.UserSettings.ServiceName;
                this.UserSettings.GeneratedFileNamePrefix = microsoftConnectedServiceData.ExtendedData?.GeneratedFileNamePrefix ?? this.UserSettings.GeneratedFileNamePrefix;
                this.UserSettings.OpenGeneratedFilesInIDE = microsoftConnectedServiceData.ExtendedData?.OpenGeneratedFilesInIDE ?? this.UserSettings.OpenGeneratedFilesInIDE;
                this.UserSettings.MakeTypesInternal = microsoftConnectedServiceData.ExtendedData?.MakeTypesInternal ?? this.UserSettings.MakeTypesInternal;
                this.UserSettings.NamespacePrefix = microsoftConnectedServiceData.ExtendedData?.NamespacePrefix ?? this.UserSettings.NamespacePrefix;
                this.UserSettings.UseDataServiceCollection = microsoftConnectedServiceData.ExtendedData?.UseDataServiceCollection ?? this.UserSettings.UseDataServiceCollection;
                this.UserSettings.UseNamespacePrefix = microsoftConnectedServiceData.ExtendedData?.UseNamespacePrefix ?? this.UserSettings.UseNamespacePrefix;
                this.UserSettings.IncludeT4File = microsoftConnectedServiceData.ExtendedData?.IncludeT4File ?? this.UserSettings.IncludeT4File;
                this.UserSettings.IgnoreUnexpectedElementsAndAttributes = microsoftConnectedServiceData.ExtendedData?.IgnoreUnexpectedElementsAndAttributes ?? this.UserSettings.IgnoreUnexpectedElementsAndAttributes;
                this.UserSettings.GenerateMultipleFiles = microsoftConnectedServiceData.ExtendedData?.GenerateMultipleFiles ?? this.UserSettings.GenerateMultipleFiles;
                this.UserSettings.EnableNamingAlias = microsoftConnectedServiceData.ExtendedData?.EnableNamingAlias ?? this.UserSettings.EnableNamingAlias;
                this.UserSettings.CustomHttpHeaders = microsoftConnectedServiceData.ExtendedData?.CustomHttpHeaders ?? this.UserSettings.CustomHttpHeaders;
                this.UserSettings.IncludeCustomHeaders = microsoftConnectedServiceData.ExtendedData?.IncludeCustomHeaders ?? this.UserSettings.IncludeCustomHeaders;
                this.UserSettings.ExcludedOperationImports = microsoftConnectedServiceData.ExtendedData?.ExcludedOperationImports ?? new List<string>();
                this.UserSettings.WebProxyHost = microsoftConnectedServiceData.ExtendedData?.WebProxyHost ?? this.UserSettings.WebProxyHost;
                this.UserSettings.IncludeWebProxy = microsoftConnectedServiceData.ExtendedData?.IncludeWebProxy ?? this.UserSettings.IncludeWebProxy;
                this.UserSettings.IncludeWebProxyNetworkCredentials = microsoftConnectedServiceData.ExtendedData?.IncludeWebProxyNetworkCredentials ?? this.UserSettings.IncludeWebProxyNetworkCredentials;
                this.UserSettings.WebProxyNetworkCredentialsDomain = microsoftConnectedServiceData.ExtendedData?.WebProxyNetworkCredentialsDomain ?? this.UserSettings.WebProxyNetworkCredentialsDomain;
                this.UserSettings.WebProxyNetworkCredentialsPassword = microsoftConnectedServiceData.ExtendedData?.WebProxyNetworkCredentialsPassword ?? this.UserSettings.WebProxyNetworkCredentialsPassword;
                this.UserSettings.WebProxyNetworkCredentialsUsername = microsoftConnectedServiceData.ExtendedData?.WebProxyNetworkCredentialsUsername ?? this.UserSettings.WebProxyNetworkCredentialsUsername;
                ODataConnectedServiceWizard ServiceWizard = ((ConfigODataEndpointViewModel)this.DataContext).ServiceWizard;
                this.UserSettings.MruEndpoints = UserSettings.Load(ServiceWizard.Context.Logger)?.MruEndpoints;

                ServiceWizard.ConfigODataEndpointViewModel.UserSettings = this.UserSettings;
                ServiceWizard.ConfigODataEndpointViewModel.LoadFromUserSettings();

                ServiceWizard.AdvancedSettingsViewModel.UserSettings = this.UserSettings;
                ServiceWizard.AdvancedSettingsViewModel.LoadFromUserSettings();

                ServiceWizard.OperationImportsViewModel.UserSettings = this.UserSettings;

                // get Operation Imports from metadata for excluding ExcludedOperationImports
                try
                {
                    ServiceWizard.ConfigODataEndpointViewModel.MetadataTempPath = ServiceWizard.ConfigODataEndpointViewModel.GetMetadata(out var version);
                    ServiceWizard.ConfigODataEndpointViewModel.EdmxVersion = version;
                    if (version == Constants.EdmxVersion4)
                    {
                        var model = EdmHelper.GetEdmModelFromFile(ServiceWizard.ConfigODataEndpointViewModel.MetadataTempPath);
                        var operations = EdmHelper.GetOperationImports(model);
                        ServiceWizard.OperationImportsViewModel.LoadOperationImports(operations);
                        ServiceWizard.ProcessedEndpointForOperationImports = this.UserSettings.Endpoint;
                        ServiceWizard.OperationImportsViewModel.LoadFromUserSettings();
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
