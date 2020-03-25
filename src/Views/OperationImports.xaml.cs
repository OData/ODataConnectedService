using Microsoft.OData.ConnectedService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ObjectSelection.xaml
    /// </summary>
    public partial class OperationImports : UserControl
    {
        public OperationImports()
        {
            InitializeComponent();
        }

        private void UnselectAll_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as OperationImportsViewModel).UnselectAll();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as OperationImportsViewModel).SelectAll();
        }
    }
}
