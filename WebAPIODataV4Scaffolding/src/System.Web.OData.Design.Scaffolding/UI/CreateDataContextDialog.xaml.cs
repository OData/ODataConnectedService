// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace System.Web.OData.Design.Scaffolding.UI
{
    // Interaction logic for CreateDataContextDialog.xaml
    internal partial class CreateDataContextDialog : ValidatingDialogWindow
    {
        public CreateDataContextDialog()
        {
            InitializeComponent();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is called in xaml.")]
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            TryClose();
        }
    }
}
