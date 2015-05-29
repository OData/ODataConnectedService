using System.Windows;
using System.Windows.Controls;
using Microsoft.OData.ConnectedService.ViewModels;

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
        }

        internal AdvancedSettingsViewModel AdvancedSettingsViewModel
        {
            get { return (AdvancedSettingsViewModel)this.DataContext; }
        }


        private void settings_Click(object sender, RoutedEventArgs e)
        {
            this.AdvancedSettingsViewModel.ConfigAdvancedSettings();
        }
    }
}
