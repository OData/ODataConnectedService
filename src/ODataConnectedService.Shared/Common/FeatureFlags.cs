//---------------------------------------------------------------------------------
// <copyright file="FeatureFlags.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;

namespace Microsoft.OData.ConnectedService.Common
{
    internal static class FeatureFlags
    {
        internal const string ShowLegacyPersistenceOptions = "Microsoft.OData.ConnectedService.ShowLegacyPersistenceOptions";

        internal static bool IsEnabled(string switchName)
        {
            return AppContext.TryGetSwitch(switchName, out bool isEnabled) && isEnabled;
        }
    }
}
