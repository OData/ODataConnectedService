//-----------------------------------------------------------------------------
// <copyright file="SchemaTypeModelToVisibilityConverter.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.OData.CodeGen.Models;

namespace Microsoft.OData.ConnectedService.Converters
{
    public class SchemaTypeModelToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Visibility))
            {
                if (value is SchemaTypeModel schemaTypeModel && schemaTypeModel.BoundOperations.Any())
                    return Visibility.Visible;

                return Visibility.Collapsed;
            }

            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
