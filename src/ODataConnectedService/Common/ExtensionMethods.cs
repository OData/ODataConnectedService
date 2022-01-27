//-----------------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.OData.ConnectedService.Common
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Copies public properties with matching name and type from one object to the other.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom(this object target, object source)
        {
            if (source == null)
            {
                return;
            }

            var fromProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(d => d.GetGetMethod() != null);
            var toProperties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(d => d.GetSetMethod() != null);

            foreach(var toProperty in toProperties)
            {
                var fromProperty = fromProperties.SingleOrDefault(d => 
                    d.Name.Equals(toProperty.Name, StringComparison.Ordinal) 
                    && toProperty.PropertyType.IsAssignableFrom(d.PropertyType));

                if (fromProperty != null)
                {
                    toProperty.SetValue(target, fromProperty.GetValue(source));
                }
            }
        }
    }
}
