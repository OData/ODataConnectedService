// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.UI
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;

    // Interaction logic for ConfigScaffolderDialog.xaml
    internal partial class ConfigScaffolderDialog : ValidatingDialogWindow
    {
        public ConfigScaffolderDialog()
        {
            InitializeComponent();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is called in xaml.")]
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            TryClose();
        }
    }
}
