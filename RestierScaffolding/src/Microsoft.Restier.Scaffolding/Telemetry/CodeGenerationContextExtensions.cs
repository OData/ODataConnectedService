// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.Telemetry
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNet.Scaffolding;

    internal static class CodeGenerationContextExtensions
    {
        public static void AddTelemetryData(this CodeGenerationContext context, string key, object value)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Dictionary<string, object> mvcTelemetryItems;
            if (!context.Items.TryGetProperty<Dictionary<string, object>>(TelemetrySharedKeys.MvcTelemetryItems, out mvcTelemetryItems))
            {
                mvcTelemetryItems = new Dictionary<string, object>();
                context.Items.AddProperty(TelemetrySharedKeys.MvcTelemetryItems, mvcTelemetryItems);
            }

            mvcTelemetryItems[key] = value;
        }
    }
}
