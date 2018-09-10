// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.OData2OpenApi.ConsoleApp.Abstracts;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    /// <summary>
    /// Command line arguments processer.
    /// </summary>
    internal class CommandLineParser
    {
        private ICommandOptionManager _commandManager = new CommandOptionManager<Configuration>();

        /// <summary>
        /// Gets the command options.
        /// </summary>
        public IEnumerable<CommandOptionAttribute> Options => _commandManager.ListOptions();

        /// <summary>
        /// Process the arguments.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        /// <returns>The configuration instance.</returns>
        public Configuration Parse(string [] args)
        {
            Configuration config = new Configuration();

            // a command option is a string like "--key=value"
            foreach (string arg in args)
            {
                // check the option key start with '--'
                if (!arg.StartsWith("--"))
                {
                    throw new Exception($"Invalid input argument {arg}, argument should start '--'.");
                }

                string[] pieces = arg.Split(new[] { '=' }, 2);
                if (pieces.Length != 2)
                {
                    throw new Exception($"Invalid input argument {arg}, argument should like '--key=[value]'.");
                }

                if (String.IsNullOrWhiteSpace(pieces[1]))
                {
                    throw new Exception($"'{pieces[0]}' command should have a value assigned.\n");
                }

                var propertyInfo = _commandManager.GetOption(pieces[0]);
                if (propertyInfo == null)
                {
                    throw new Exception($"Unknown input argument {pieces[0]}.");
                }

                /* 
                if (propertyInfo.GetValue(config) != null)
                {
                    throw new ODataOpenApiException($"Multiple '{pieces[0]}' are not allowed.\n");
                }*/

                // The last option will win.
                propertyInfo.SetValue(config, pieces[1]);
            }

            return config;
        }
    }
}
