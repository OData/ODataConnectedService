// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.VisualStudio
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    internal static class ProjectSettingsExtensions
    {
        public static bool TryGetString(this IProjectSettings settings, string key, out string value)
        {
            Contract.Assert(settings != null);
            Contract.Assert(key != null);

            value = settings[key];
            return value != null;
        }

        public static bool TryGetDouble(this IProjectSettings settings, string key, out double value)
        {
            Contract.Assert(settings != null);
            Contract.Assert(key != null);

            string storedValue = settings[key];
            double parsedValue;
            if (storedValue != null && Double.TryParse(storedValue, out parsedValue))
            {
                value = parsedValue;
                return true;
            }
            else
            {
                value = default(double);
                return false;
            }
        }
    }
}
