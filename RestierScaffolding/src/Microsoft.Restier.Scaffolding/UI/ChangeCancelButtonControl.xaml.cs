// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.UI
{
    using System.Windows;
    using System.Windows.Controls;

    // Interaction logic for ChangeCancelButtonControl.xaml
    internal partial class ChangeCancelButtonControl : UserControl
    {
        public event RoutedEventHandler ChangeButtonClick;

        public ChangeCancelButtonControl()
        {
            InitializeComponent();
        }

        public void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeButtonClick != null)
            {
                ChangeButtonClick(sender, e);
            }
        }
    }
}
