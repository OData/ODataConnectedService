// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace System.Web.OData.Design.Scaffolding.UI
{
    /// <summary>
    /// A comparer implementation for the DataContext menu, that puts the "Add New DataContext"
    /// item at the top, and then sorts by type name.
    /// </summary>
    internal class DataContextModelTypeComparer : Comparer<ModelType>
    {
        public override int Compare(ModelType x, ModelType y)
        {
            if (x == null && y == null)
            {
                return 0;
            }
            else if (x == null)
            {
                return -1;
            }
            else if (y == null)
            {
                return 1;
            }
            else
            {
                return StringComparer.CurrentCulture.Compare(x.ShortTypeName, y.ShortTypeName);
            }
        }
    }
}
