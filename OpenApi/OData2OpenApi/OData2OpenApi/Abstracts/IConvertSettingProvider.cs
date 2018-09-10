// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.OpenApi.OData;

namespace Microsoft.OData2OpenApi.ConsoleApp.Abstracts
{
    /// <summary>
    /// Interface to <see cref="OpenApiConvertSettings"/>.
    /// </summary>
    public interface IConvertSettingsProvider
    {
        /// <summary>
        /// Gets the <see cref="OpenApiConvertSettings"/>.
        /// </summary>
        /// <returns>The settings.</returns>
        OpenApiConvertSettings GetConvertSettings();
    }
}