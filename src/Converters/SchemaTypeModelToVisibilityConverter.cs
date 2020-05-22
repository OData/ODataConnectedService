using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.OData.ConnectedService.Models;

namespace Microsoft.OData.ConnectedService.Converters
{
    public class SchemaTypeModelToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SchemaTypeModel schemaTypeModel && schemaTypeModel.BoundOperations.Any())
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
