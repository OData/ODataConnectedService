//---------------------------------------------------------------------------------
// <copyright file="ODataConnectedServiceInstance.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using Microsoft.OData.CodeGen.Models;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    public class ODataConnectedServiceInstance : ConnectedServiceInstance
    {
        public ServiceConfiguration ServiceConfig { get; set; }
        public string MetadataTempFilePath { get; set; }
    }
}
