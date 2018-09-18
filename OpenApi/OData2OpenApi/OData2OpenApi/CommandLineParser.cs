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
    internal static class CommandLineParser
    {
        private static ICommandOptionManager _commandManager = new CommandOptionManager<Configuration>();

        /// <summary>
        /// Gets the command options.
        /// </summary>
        public static IEnumerable<CommandOptionAttribute> Options => _commandManager.ListOptions();

        /// <summary>
        /// Process the arguments.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        /// <returns>The configuration instance.</returns>
        public static Configuration Parse(string [] args)
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

                // The last option will win.
                if (propertyInfo.PropertyType == typeof(bool))
                {
                    if (!Boolean.TryParse(pieces[1], out bool boolValue))
                    {
                        throw new Exception($"Invalid input value '{pieces[1]}' for argument '{pieces[0]}'.");
                    }

                    propertyInfo.SetValue(config, boolValue);
                }
                else
                {
                    propertyInfo.SetValue(config, pieces[1]);
                }
            }

            return config;
        }
    }
}
