// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;

namespace System.Web.OData.Design.Scaffolding.UI
{
    public static class FocusFirstElementBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(FocusFirstElementBehavior),
            new PropertyMetadata(OnIsEnabledChanged));

        public static ResizeMode GetIsEnabled(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            return (ResizeMode)dependencyObject.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject dependencyObject, bool value)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            dependencyObject.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Window window = sender as Window;
            if (window == null)
            {
                // I'd like to have an assert here, but it causes errors in the designer.
                return;
            }

            if ((bool)e.NewValue)
            {
                if (!window.IsLoaded)
                {
                    window.ContentRendered += Content_Rendered;
                }
            }
            else
            {
                window.ContentRendered -= Content_Rendered;
            }
        }

        private static void Content_Rendered(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window == null)
            {
                Contract.Assert(false, "Only window is supported by this behavior.");
                return;
            }

            window.ContentRendered -= Content_Rendered;

            window.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}
