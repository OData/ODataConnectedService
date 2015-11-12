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
            this.AdvancedSettingsPanel.Visibility = Visibility.Hidden;
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
