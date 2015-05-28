// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Windows;
using System.Windows.Controls;

namespace System.Web.OData.Design.Scaffolding.UI
{
    internal partial class AddCancelButtonControl : UserControl
    {
        public event RoutedEventHandler AddButtonClick;

        public AddCancelButtonControl()
        {
            InitializeComponent();
        }

        public void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddButtonClick != null)
            {
                AddButtonClick(sender, e);
            }
        }
    }
}
