// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.OData2OpenApi.ConsoleApp.Abstracts;
using Microsoft.OpenApi.OData;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    internal class ConvertSettingsProvider : IConvertSettingsProvider
    {
        private Configuration Config { get; }

        public ConvertSettingsProvider(Configuration config)
        {
            Config = config;
        }

        public virtual OpenApiConvertSettings GetConvertSettings()
        {
            var settings = new OpenApiConvertSettings();

            settings.EnableKeyAsSegment = Config.KeyAsSegment;
            settings.EnableNavigationPropertyPath = Config.NavigationPropertyPath;
            settings.EnableOperationPath = Config.OperationPath;
            settings.EnableOperationId = Config.OperationId;
            settings.PrefixEntityTypeNameBeforeKey = Config.PrefixTypeBeforeKey;
            settings.EnableOperationImportPath = Config.OperationImportPath;
            settings.EnableUnqualifiedCall = Config.UnqualifiedCall;

            return settings;
        }
    }
}