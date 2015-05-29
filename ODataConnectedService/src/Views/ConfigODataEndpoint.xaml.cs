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

        private void Endpoint_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= Endpoint_GotFocus;
        }
    }
}
