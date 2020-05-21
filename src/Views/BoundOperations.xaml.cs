using Microsoft.OData.ConnectedService.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ObjectSelection.xaml
    /// </summary>
    public partial class BoundOperations : UserControl
    {
        public BoundOperations()
        {
            InitializeComponent();
        }

        private void UnselectAll_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as BoundOperationsViewModel)?.DeselectAll();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as BoundOperationsViewModel)?.SelectAll();
        }
    }
}
