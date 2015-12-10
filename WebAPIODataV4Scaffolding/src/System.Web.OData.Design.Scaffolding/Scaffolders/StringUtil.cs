// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding
{
    internal static class StringUtil
    {
        internal static string ToLowerInvariantFirstChar(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (String.IsNullOrEmpty(input))
            {
                return input;
            }

            return input.Substring(0, length: 1).ToLowerInvariant() + input.Substring(1);
        }
    }
}
