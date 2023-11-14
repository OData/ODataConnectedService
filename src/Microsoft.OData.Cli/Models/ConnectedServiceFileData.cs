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
        public string ProviderId { get; set; }

        public BaseUserSettings ExtendedData { get; set; }
    }
}
