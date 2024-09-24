using System;
using System.Collections.Generic;
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
        private int currentPage = 1;
        private int itemsPerPage = 50;

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

        /// <summary>
        /// Paginates data for schema types to allow the UI to render only one page of items at a time making the UI more responsive.
        /// </summary>
        /// <param name="pageNumber">The page to view indexes start from 1.</param>
        public void DisplayPage(int pageNumber)
        {
            int startIndex = (pageNumber - 1) * itemsPerPage;
            int endIndex = startIndex + itemsPerPage;

            var items = (DataContext as SchemaTypesViewModel)?.FilteredSchemaTypes;
            endIndex = endIndex > items.Count() ? items.Count() : endIndex;

            // Get the items for the current page
            var pageItems = items.Skip(startIndex).Take(endIndex - startIndex);

            // Bind the items to the ListBox
            SchemaTypesTreeView.ItemsSource = pageItems;

            // Update the page info
            PageInfoTextBlock.Text = $"Page {pageNumber} of {Math.Ceiling((double)items.Count() / itemsPerPage)}";
        }

        /// <summary>
        /// Event handler that moves to the next page while ensuring to check bounds.
        /// </summary>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var items = (DataContext as SchemaTypesViewModel)?.FilteredSchemaTypes;

            if (currentPage * itemsPerPage < items.Count())
            {
                currentPage++;
                DisplayPage(currentPage);
            }
        }

        /// <summary>
        /// Event handler that moves to the previous page while ensuring to check bounds.
        /// </summary>
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayPage(currentPage);
            }
        }
    }
}