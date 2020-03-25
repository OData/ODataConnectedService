// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ConfigODataEndpoint.xaml
    /// </summary>
    public partial class ConfigODataEndpoint : UserControl
    {
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
    }
}
