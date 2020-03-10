// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.OData.ConnectedService.Views
{

    public delegate void OpenGeneratedFilesInIDE(object sender, RoutedEventArgs e);
    
    /// <summary>
    /// Interaction logic for ConfigODataEndpoint.xaml
    /// </summary>
    public partial class ConfigODataEndpoint : UserControl
    {
        public event OpenGeneratedFilesInIDE Checked;
        public ConfigODataEndpoint()
        {
            InitializeComponent();
            Checked += (obj, ev) => { };
        }
    }
}
