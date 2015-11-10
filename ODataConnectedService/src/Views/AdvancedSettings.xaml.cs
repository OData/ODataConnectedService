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
            this.AdvancedSettingsPanel.Margin = new Thickness(10, -60, 0, 0);
            this.AdvancedSettingsPanel.Visibility = Visibility.Visible;

            var wizard = (ODataConnectedServiceWizard)this.AdvancedSettingsViewModel.Wizard;
            if (wizard.EdmxVersion == Common.Constants.EdmxVersion4)
            {
                this.EnableCamelCase.Visibility = Visibility.Visible;
                this.EnableCamelCase.IsEnabled = true;
                this.IgnoreUnknownAttributeOrElement.Visibility = Visibility.Visible;
                this.IgnoreUnknownAttributeOrElement.IsEnabled = true;
            }

            this.TextBlock.Visibility = Visibility.Hidden;
            this.Label.Visibility = Visibility.Hidden;
        }
    }
}
