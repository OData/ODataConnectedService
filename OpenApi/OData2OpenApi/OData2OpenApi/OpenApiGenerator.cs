// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using Microsoft.OData.Edm;
using Microsoft.OData2OpenApi.ConsoleApp.Abstracts;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    /// <summary>
    /// Open Api generator.
    /// </summary>
    internal static class OpenApiGenerator
    {
        /// <summary>
        /// Generate the Open Api.
        /// </summary>
        public static bool Run(Configuration config)
        {
            IEdmModelProvider modelProvider = new EdmModelProvider(config.InputCsdl, config.IsLocalFile);
            IEdmModel edmModel = modelProvider.GetEdmModel();

            IConvertSettingsProvider settingsProvider = new ConvertSettingsProvider(config);
            OpenApiConvertSettings settings = settingsProvider.GetConvertSettings();

            using (FileStream fs = File.Create(config.OutputFileName))
            {
                OpenApiDocument document = edmModel.ConvertToOpenApi(settings);
                document.Serialize(fs, OpenApi.OpenApiSpecVersion.OpenApi3_0, config.Format);
                fs.Flush();
            }

            return true;
        }
    }
}
