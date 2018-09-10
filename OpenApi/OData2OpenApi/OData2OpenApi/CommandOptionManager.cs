// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OData2OpenApi.ConsoleApp.Abstracts;

namespace Microsoft.OData2OpenApi.ConsoleApp
{
    internal class CommandOptionManager<T> : ICommandOptionManager where T: class
    {
        private Lazy<IDictionary<string, CommandOptionItem>> _commands
             = new Lazy<IDictionary<string, CommandOptionItem>>(() => LoadCommands(), isThreadSafe: false);

        /// <summary>
        /// Gets the command by the key
        /// </summary>
        /// <param name="key">The command key.</param>
        /// <returns>The command parser or null.</returns>
        public PropertyInfo GetOption(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            if (_commands.Value.TryGetValue(key.ToLower(), out CommandOptionItem value))
            {
                return value.Property;
            }

            return null;
        }

        /// <summary>
        /// List the all command options.
        /// </summary>
        /// <returns>The command options.</returns>
        public IEnumerable<CommandOptionAttribute> ListOptions()
        {
            return _commands.Value.Values.Select(c => c.Option);
        }

        private static IDictionary<string, CommandOptionItem> LoadCommands()
        {
            IDictionary<string, CommandOptionItem> commands = new Dictionary<string, CommandOptionItem>();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<CommandOptionAttribute>();
                if (attr == null)
                {
                    continue;
                }

                commands[attr.Key] = new CommandOptionItem(property, attr);
            }

            return commands;
        }

        private class CommandOptionItem
        {
            public CommandOptionItem(PropertyInfo propertyInfo, CommandOptionAttribute option)
            {
                Property = propertyInfo;
                Option = option;
            }

            public PropertyInfo Property { get; set; }

            public CommandOptionAttribute Option { get; set; }
        }
    }
}
