// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.OData2OpenApi.ConsoleApp.Abstracts
{
    /// <summary>
    /// Interfaced for the command option.
    /// </summary>
    public interface ICommandOptionManager
    {
        /// <summary>
        /// List all the options.
        /// </summary>
        /// <returns>The options.</returns>
        IEnumerable<CommandOptionAttribute> ListOptions();

        /// <summary>
        /// Get the options property.
        /// </summary>
        /// <param name="key">The option key.</param>
        /// <returns>The option property info.</returns>
        PropertyInfo GetOption(string key);
    }
}
