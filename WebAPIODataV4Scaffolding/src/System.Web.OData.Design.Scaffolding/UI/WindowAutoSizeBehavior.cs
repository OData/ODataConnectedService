// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Windows;

namespace System.Web.OData.Design.Scaffolding.UI
{
    /// <summary>
    /// An attached behavior to help auto-size a resizable window.
    /// </summary>
    /// <remarks>
    /// WPF Windows have a nice feature called SizeToContent that allows a Window to choose an appropriate size based on
    /// its contents. SizeToContent setting can be problematic when the contents of a Window change based on user input,
    /// as the Window will continue to resize as it is used, which can be distracting. 
    /// 
    /// This behavior supports a dual-mode layout, where SizeToContent is used to determine an optimal initial size while
    /// the Window is loading, and then the behavior switches to fixed-size. The initial size chosen by SizeToContent becomes
    /// 'sticky' and remains the size of the Window once the user begins to enter input.
    /// 
    /// To make use of this behavior, create a Window where one or more dimensions are controlled by SizeToContent. When
    /// the Window is loaded, the layout system will choose a size for those dimensions, and then this behavior will store
    /// that size as a fixed size. 
    /// 
    /// MinHeight/MaxHeight/MinWidth/MaxWidth (if specified) will continue to work as expected.
    /// 
    /// Ex (tags omitted):
    /// 
    /// Window
    ///   MinHeight=200
    ///   Height=600
    ///   SizeToContent=Width
    ///   ResizeMode=AllowResize
    ///   ui:WindowAutoSizeBehavior.IsEnabled=true
    ///   
    /// This will produce a resizable Window that defaults to Nx600, where N is the width chosen by the layout system.
    /// Resizing will allow a range of N+x200-600.
    /// </remarks>
    public static class WindowAutoSizeBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(WindowAutoSizeBehavior),
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
                    window.ContentRendered += Window_ContentRendered;
                }
            }
            else
            {
                window.ContentRendered -= Window_ContentRendered;
            }
        }

        private static void Window_ContentRendered(object sender, EventArgs e)
        {
            Window window = sender as Window;
            if (window == null)
            {
                Contract.Assert(false, "Only window is supported by this behavior.");
                return;
            }

            window.ContentRendered -= Window_ContentRendered;

            // We need to set an explicit value for each dimension, and provide a min/max-value for each dimension 
            // that has SizeToContent turned on. We don't want to override any user-provided min/max-values, so we
            // check if the valus was set on the control directly.

            window.Height = window.ActualHeight;
            if (window.SizeToContent.HasFlag(SizeToContent.Height) &&
                DependencyPropertyHelper.GetValueSource(window, FrameworkElement.MinHeightProperty).BaseValueSource != BaseValueSource.Local)
            {
                window.MinHeight = window.ActualHeight;
            }
            if (window.SizeToContent.HasFlag(SizeToContent.Height) &&
                DependencyPropertyHelper.GetValueSource(window, FrameworkElement.MaxHeightProperty).BaseValueSource != BaseValueSource.Local)
            {
                window.MaxHeight = window.ActualHeight;
            }

            window.Width = window.ActualWidth;
            if (window.SizeToContent.HasFlag(SizeToContent.Width) &&
                DependencyPropertyHelper.GetValueSource(window, FrameworkElement.MinWidthProperty).BaseValueSource != BaseValueSource.Local)
            {
                window.MinWidth = window.ActualWidth;
            }
            if (window.SizeToContent.HasFlag(SizeToContent.Width) &&
                DependencyPropertyHelper.GetValueSource(window, FrameworkElement.MaxWidthProperty).BaseValueSource != BaseValueSource.Local)
            {
                window.MaxWidth = window.ActualHeight;
            }

            // Setting this to manual ensures that the Window will not continue to adjust as its content changes.
            window.SizeToContent = SizeToContent.Manual;
        }
    }
}
