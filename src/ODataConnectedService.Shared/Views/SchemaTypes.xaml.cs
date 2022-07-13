using System.Linq;
using Microsoft.OData.ConnectedService.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.OData.CodeGen.Models;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ObjectSelection.xaml
    /// </summary>
    public partial class SchemaTypes : UserControl
    {
        public SchemaTypes()
        {
            InitializeComponent();
        }

        private void DeselectAllSchemaTypes_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SchemaTypesViewModel)?.DeselectAllSchemaTypes();
        }

        private void SelectAllSchemaTypes_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SchemaTypesViewModel)?.SelectAllSchemaTypes();
        }

        //public static RoutedCommand SelectAllBoundOperationsForSchemaTypeCmd = new RoutedCommand();

        private void SelectAllBoundOperationsForSchemaTypeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SchemaTypeModel schemaTypeModel)
            {
                (DataContext as SchemaTypesViewModel)?.SelectAllBoundOperationsForSchemaType(schemaTypeModel);
            }
        }

        private void SelectAllBoundOperationsForSchemaTypeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SchemaTypeModel schemaTypeModel &&
                           schemaTypeModel.BoundOperations.Any(x => !x.IsSelected);
        }

        //public static RoutedCommand DeselectAllBoundOperationsForSchemaTypeCmd = new RoutedCommand();

        private void DeselectAllBoundOperationsForSchemaTypeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is SchemaTypeModel schemaTypeModel)
            {
                (DataContext as SchemaTypesViewModel)?.DeselectAllBoundOperationsForSchemaType(schemaTypeModel);
            }
        }

        private void DeselectAllBoundOperationsForSchemaTypeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter is SchemaTypeModel schemaTypeModel &&
                           schemaTypeModel.BoundOperations.Any(x => x.IsSelected);
        }

        private void SelectAllBoundOperations_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SchemaTypesViewModel)?.SelectAllBoundOperations();
        }

        private void DeselectAllBoundOperations_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as SchemaTypesViewModel)?.DeselectAllBoundOperations();
        }
    }
}
