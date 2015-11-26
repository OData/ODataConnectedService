// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Globalization;
using System.Windows.Data;

namespace System.Web.OData.Design.Scaffolding.UI
{
    internal class MultiAndBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool returnValue = true;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] is bool)
                {
                    returnValue &= (bool)values[i];
                }
                else
                {
                    throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Values must be boolean!. Value {0} had a type of {1}", i, values[i].GetType().ToString()));
                }
            }

            return returnValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
