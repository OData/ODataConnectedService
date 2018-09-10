// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.OpenApi;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    /// <summary>
    /// The global configuration.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets/sets the input CSDL.
        /// </summary>
        [CommandOption("--csdl=[value] : Input the OData CSDL, file or Url.")]
        public string InputCsdl { get; set; }

        /// <summary>
        /// Gets/sets the output file name.
        /// </summary>
        [CommandOption("--output=[value] : Set the output file, with extension as 'yaml' or 'json'.")]
        public string OutputFileName { get; set; }

        /// <summary>
        /// Gets/set the option for Edm operation path.
        /// </summary>
        [CommandOption("--OperationPath=[true/false] : Enable Edm operation path or not.")]
        public string OperationPath { get; set; }

        /// <summary>
        /// Gets/set the option for Edm operation import path.
        /// </summary>
        [CommandOption("--OperationImportPath=[true/false] : Enable Edm operation import path or not.")]
        public string OperationImportPath { get; set; }

        /// <summary>
        /// Gets/set the option for Edm operation import path.
        /// </summary>
        [CommandOption("--NavigationPath=[true/false] : Enable Edm navigation property path or not.")]
        public string NavigationPropertyPath { get; set; }

        /// <summary>
        /// Gets/set the option for Validate Edm model.
        /// </summary>
        [CommandOption("--Validate=[true/false] : Enable validate Edm model or not.")]
        public string ValidateModel { get; set; }

        /// <summary>
        /// Gets the boolean value indicating whether the input is local file or not.
        /// </summary>
        public bool IsLocalFile { get; private set; }

        /// <summary>
        /// Gets the output format.
        /// </summary>
        public OpenApiFormat Format { get; private set; } = OpenApiFormat.Json;

        /// <summary>
        /// Validate the configuration.
        /// </summary>
        /// <returns></returns>
        public void InitializeAndValidate()
        {
            if (String.IsNullOrWhiteSpace(InputCsdl))
            {
                throw new Exception($"'--csdl=[value]' is required.");
            }

            IsLocalFile = IsLocalPath(InputCsdl);
            if (IsLocalFile)
            {
                if (!File.Exists(InputCsdl))
                {
                    throw new Exception($"File '{ InputCsdl }' is not existed.\n");
                }
            }

            if (String.IsNullOrWhiteSpace(OutputFileName))
            {
                throw new Exception($"'--output=[value]' is required.");
            }

            string extension = Path.GetExtension(OutputFileName);
            if (!String.IsNullOrWhiteSpace(extension))
            {
                Format = extension.ToLower() == "yaml" ? OpenApiFormat.Yaml : OpenApiFormat.Json;
            }
        }

        private static bool IsLocalPath(string path)
        {
            bool ret = true;
            try
            {
                ret = new Uri(path).IsFile;
            }
            catch
            {
                if (path.StartsWith("http://") ||
                    path.StartsWith(@"http:\\") ||
                    path.StartsWith("https://") ||
                    path.StartsWith(@"https:\\"))
                {
                    return false;
                }
            }

            return ret;
        }
    }
}
