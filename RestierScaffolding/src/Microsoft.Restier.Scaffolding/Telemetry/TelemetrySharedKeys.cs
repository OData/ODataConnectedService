// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.Telemetry
{
    /// <summary>
    /// Shared keys for storing SQM data points in the CodeGenerationContext. These values are shared with
    /// the Core scaffolding implementation, do not change them.
    /// </summary>
    internal static class TelemetrySharedKeys
    {
        /// <summary>
        /// This is the key used to store the dictionary of MVC/Web API specific sqm data in the context.
        /// </summary>
        public const string MvcTelemetryItems = "MSInternal_MvcInfo";
        public const string DependencyScaffolderOptions = "DependencyScaffolderOptions";
        public const string WebApiControllerScaffolderOptions = "WebApiControllerScaffolderOptions";
    }
}
