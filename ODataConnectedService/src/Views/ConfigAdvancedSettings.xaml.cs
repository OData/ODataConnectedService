using System.Windows;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ConfigAdvancedSettings.xaml
    /// </summary>
    public partial class ConfigAdvancedSettings : Window
    {
        public ConfigAdvancedSettings()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
