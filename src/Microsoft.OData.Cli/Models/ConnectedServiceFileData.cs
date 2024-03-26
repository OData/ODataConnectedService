//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceFileData.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using Microsoft.OData.CodeGen.Models;

namespace Microsoft.OData.Cli.Models
{
    /// <summary>
    /// Represents OData Connected Service config file
    /// </summary>
    public class ConnectedServiceFileData
    {
        /// <summary>
        /// Gets or sets the unique Provider ID for the provider/handler. This is used to match a provider with its handlers.
        /// </summary>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the OData Connected Service user settings extracted from ConnectedService.json.
        /// </summary>
        public BaseUserSettings ExtendedData { get; set; }
    }
}
