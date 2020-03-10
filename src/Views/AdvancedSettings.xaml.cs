// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.OData.ConnectedService.ViewModels;

namespace Microsoft.OData.ConnectedService.Views
{
    //public delegate void OpenGeneratedFilesInIDE(object sender, RoutedEventArgs e);
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : UserControl
    {
        //public event OpenGeneratedFilesInIDE Checked;
        public AdvancedSettings()
        {
            InitializeComponent();
            this.AdvancedSettingsPanel.Visibility = Visibility.Hidden;
            //Checked += (obj, ev) => { };

        }

        internal ODataConnectedServiceWizard ODataConnectedServiceWizard
        {
            get { return ((AdvancedSettingsViewModel)this.DataContext).Wizard as ODataConnectedServiceWizard; }
        }

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            this.SettingsPanel.Visibility = Visibility.Hidden;

            this.AdvancedSettingsPanel.Margin = new Thickness(10, -125, 0, 0);
            this.AdvancedSettingsPanel.Visibility = Visibility.Visible;

            this.AdvancedSettingsForv4.Visibility = this.ODataConnectedServiceWizard.EdmxVersion == Common.Constants.EdmxVersion4
                ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
