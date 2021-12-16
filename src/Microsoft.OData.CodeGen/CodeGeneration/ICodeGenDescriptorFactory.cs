//------------------------------------------------------------------------------
// <copyright file="ICodeGenDescriptorFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.CodeGen.CodeGeneration
{
    public interface ICodeGenDescriptorFactory
    {
        BaseCodeGenDescriptor Create(Version edmxVersion, string metadataUri, ConnectedServiceHandlerContext context, Project project);
    }
}
