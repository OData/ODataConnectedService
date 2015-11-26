// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace System.Web.OData.Design.Scaffolding.UI
{
    internal partial class ControllerScaffolderDialog : ValidatingDialogWindow
    {
        public ControllerScaffolderDialog()
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
