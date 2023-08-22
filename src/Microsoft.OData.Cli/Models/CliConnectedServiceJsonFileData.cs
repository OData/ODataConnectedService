//-----------------------------------------------------------------------------
// <copyright file="UserSettings.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

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

        public CliUserSettings ExtendedData { get; set; }
    }
}
