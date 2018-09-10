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
    internal class OpenApiGenerator
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public Configuration Config { get; }

        public IEdmModelProvider ModelProvider { get; }

        public IConvertSettingsProvider SettingsProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiGenerator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public OpenApiGenerator(Configuration config)
        {
            Config = config;
            ModelProvider = new EdmModelProvider(config.InputCsdl, config.IsLocalFile);
            SettingsProvider = new ConvertSettingsProvider(config);
        }

        /// <summary>
        /// Generate the Open Api.
        /// </summary>
        public bool Run()
        {
            IEdmModel edmModel = ModelProvider.GetEdmModel();

            OpenApiConvertSettings settings = SettingsProvider.GetConvertSettings();

            using (FileStream fs = File.Create(Config.OutputFileName))
            {
                OpenApiDocument document = edmModel.ConvertToOpenApi(settings);
                document.Serialize(fs, OpenApi.OpenApiSpecVersion.OpenApi3_0, Config.Format);
                fs.Flush();
            }

            return true;
        }
    }
}
