using Microsoft.OData.ConnectedService.ViewModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.OData.ConnectedService.Views
{
    /// <summary>
    /// Interaction logic for ObjectSelection.xaml
    /// </summary>
    public partial class OperationImports : UserControl
    {
        private int currentPage = 1;
        private readonly int itemsPerPage = 50;
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

        /// <summary>
        /// Paginates data for schema types to allow the UI to render only one page of items at a time making the UI more responsive.
        /// </summary>
        /// <param name="pageNumber">The page to view indexes start from 1.</param>
        public void DisplayPage(int pageNumber)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            int startIndex = (pageNumber - 1) * itemsPerPage;
            int endIndex = startIndex + itemsPerPage;

            var items = (DataContext as OperationImportsViewModel)?.FilteredOperationImports;
            endIndex = endIndex > items.Count() ? items.Count() : endIndex;

            // Get the items for the current page
            var pageItems = items.Skip(startIndex).Take(endIndex - startIndex);

            OperationImportsList.ItemsSource = pageItems;

            // Update the page info
            PageInfoTextBlock.Text = $"Page {pageNumber} of {Math.Ceiling((double)items.Count() / itemsPerPage)}";
        }

        /// <summary>
        /// Event handler that moves to the next page while ensuring to check bounds.
        /// </summary>
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var items = (DataContext as OperationImportsViewModel)?.FilteredOperationImports;

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
