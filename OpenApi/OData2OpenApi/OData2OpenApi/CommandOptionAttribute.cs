// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    /// <summary>
    /// Command option attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandOptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOptionAttribute"/> class.
        /// </summary>
        /// <param name="command">The command option.</param>
        public CommandOptionAttribute(string command)
        {
            if (String.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (!command.StartsWith("--"))
            {
                throw new ArgumentException($"Invalid input argument {command}, argument should start '--'.");
            }

            string[] pieces = command.Split('=');
            if (pieces.Length != 2)
            {
                throw new ArgumentException($"Invalid input argument {command}, argument should like '--key=value'.");
            }

            Raw = command;
            Key = pieces[0];
            Description = pieces[1];
        }

        /// <summary>
        /// Gets the option key name.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets the option description name.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the raw command option string.
        /// </summary>
        public string Raw { get; }
    }
}
