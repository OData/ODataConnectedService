// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace System.Web.OData.Design.Scaffolding.UI
{
    /// <summary>
    /// An attached behavior that allow text selection via regex when a TextBox is focused.
    /// 
    /// This behavior can work in a few different ways, configurable by the value of the SelectOnFocus property.
    /// - empty-string - All text is selected.
    /// - regex without groups - The first match of the pattern is selected.
    /// - regex with groups - The first group is selected if it matches, else the whole pattern is selected.
    /// </summary>
    internal static class SelectOnFocusBehavior
    {
        public static readonly DependencyProperty SelectOnFocusProperty = DependencyProperty.RegisterAttached(
            "SelectOnFocus",
            typeof(string),
            typeof(SelectOnFocusBehavior),
            new UIPropertyMetadata(null, OnSelectOnFocusPropertyChanged));

        public static string GetSelectOnFocus(DependencyObject obj)
        {
            return (string)obj.GetValue(SelectOnFocusProperty);
        }

        public static void SetSelectOnFocus(DependencyObject obj, string value)
        {
            obj.SetValue(SelectOnFocusProperty, value);
        }

        private static void OnSelectOnFocusPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (e.NewValue is string)
                {
                    textBox.GotKeyboardFocus += OnTextBoxGotKeyboardFocus;
                    textBox.TextChanged += OnTextBoxGotKeyboardFocus;
                }
            }
        }

        private static void OnTextBoxGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            if (!Object.ReferenceEquals(sender, e.OriginalSource))
            {
                return;
            }

            TextBox textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                string actualText = textBox.Text;
                string pattern = GetSelectOnFocus(textBox);

                if (String.IsNullOrEmpty(actualText))
                {
                    return;
                }

                if (String.IsNullOrEmpty(pattern))
                {
                    Action action = () => { textBox.SelectAll(); };
                    textBox.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
                }
                else
                {
                    Match match = Regex.Match(actualText, pattern);
                    if (match.Success)
                    {
                        // If the regex uses groups then select the first matching group, otherwise
                        // select the whole match.

                        int start;
                        int length;

                        // The 0th group is the whole match, we don't want that.
                        if (match.Groups.Count > 1 && match.Groups[1].Success)
                        {
                            Group group = match.Groups[1];
                            start = group.Index;
                            length = group.Length;
                        }
                        else
                        {
                            start = match.Index;
                            length = match.Length;
                        }

                        Action action = () => { textBox.Select(start, length); };
                        textBox.Dispatcher.BeginInvoke(action, DispatcherPriority.ContextIdle);
                    }
                }

                textBox.TextChanged -= OnTextBoxGotKeyboardFocus;
            }
        }
    }
}
