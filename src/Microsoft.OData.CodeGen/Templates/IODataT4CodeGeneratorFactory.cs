//-----------------------------------------------------------------------------
// <copyright file="IODataT4CodeGeneratorFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

namespace Microsoft.OData.CodeGen.Templates
{
    public interface IODataT4CodeGeneratorFactory
    {
        ODataT4CodeGenerator Create();
    }
}
