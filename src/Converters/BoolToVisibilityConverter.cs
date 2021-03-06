//-----------------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.OData.ConnectedService.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                if (value is bool boolValue)
                {
                    if (parameter is bool isInversed && isInversed)
                    {
                        return !boolValue ? Visibility.Visible : Visibility.Collapsed;
                    }

                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
                }

                throw new NotSupportedException();
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
