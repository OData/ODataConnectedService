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
    internal static class ExtensionMethods
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


        /// <summary>
        /// Ensures that the Uri conforms to the expected "$metadata" format
        /// </summary>
        /// <param name="uri">Uri to clean</param>
        /// <returns>Cleaned uri</returns>
        public static Uri CleanMetadataUri(this Uri uri)
        {
            if (uri.Scheme == "http" || uri.Scheme == "https")
            {
                UriBuilder uriBuilder;

                /// Evaluates to true if Query and Fragment properties are present in the Uri 
                bool preserveQueryAndFragment = true;

                if (uri.Segments.Last().StartsWith("$metadata", StringComparison.InvariantCultureIgnoreCase))
                {
                    preserveQueryAndFragment = !uri.AbsolutePath.EndsWith("/", StringComparison.Ordinal);
                    Uri absolutePathUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath.TrimEnd('/')).Uri;
                    uriBuilder = new UriBuilder(absolutePathUri);
                }
                else
                {
                    var absolutePathUri = new UriBuilder(uri.Scheme, uri.Host, uri.Port, uri.AbsolutePath.TrimEnd('/') + "/").Uri;
                    uriBuilder = new UriBuilder(new Uri(absolutePathUri, "$metadata"));
                }

                if (preserveQueryAndFragment)
                {
                    uriBuilder.Query = uri.Query.TrimStart('?');
                    uriBuilder.Fragment = uri.Fragment.TrimStart('#');
                }
                uriBuilder.UserName = uri.UserInfo;

                return new Uri(uriBuilder.Uri.AbsoluteUri);
            }
            return new Uri(uri.AbsoluteUri);
        }
    }
}
