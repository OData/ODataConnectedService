//-----------------------------------------------------------------------------
// <copyright file="ODataT4CodeGeneratorFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

namespace Microsoft.OData.ConnectedService.Templates
{
    class ODataT4CodeGeneratorFactory: IODataT4CodeGeneratorFactory
    {
        public ODataT4CodeGenerator Create()
        {
            return new ODataT4CodeGenerator();
        }
    }
}
