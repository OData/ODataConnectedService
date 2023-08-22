//-----------------------------------------------------------------------------
// <copyright file="CliConnectedServiceJsonFileData.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using Microsoft.OData.CodeGen.Models;

namespace Microsoft.OData.Cli.Models
{
    /// <summary>
    /// Represents OData Connected Service Config File<br />
    /// See <seealso cref="ConnectedServiceJsonFileData"/> in<br />
    /// ODataConnectedService.Shared/Common/ExtensionsHelper.cs
    /// </summary>
    public class CliConnectedServiceJsonFileData
    {
        public string ProviderId { get; set; }

        public UserSettings ExtendedData { get; set; }
    }
}
