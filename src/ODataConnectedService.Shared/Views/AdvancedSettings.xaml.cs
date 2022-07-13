// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Windows;
using System.Windows.Controls;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.OData.CodeGen.Common;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : UserControl
    {
        public AdvancedSettings()
        {
            InitializeComponent();
            this.AdvancedSettingsPanel.Visibility = Visibility.Hidden;
        }

        internal ODataConnectedServiceWizard ODataConnectedServiceWizard => ((AdvancedSettingsViewModel)this.DataContext).Wizard as ODataConnectedServiceWizard;

        private void settings_Click(object sender, RoutedEventArgs e)
        {
            this.AdvancedSettingsHyperLinkPanel.Visibility = Visibility.Hidden;

            this.AdvancedSettingsPanel.Margin = new Thickness(10, -125, 0, 0);
            this.AdvancedSettingsPanel.Visibility = Visibility.Visible;

            this.AdvancedSettingsForv4.Visibility = this.ODataConnectedServiceWizard.EdmxVersion == Constants.EdmxVersion4
                ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
