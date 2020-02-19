// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    interface ICodeGenDescriptorFactory
    {
        BaseCodeGenDescriptor Create(Version edmxVersion, string metadataUri, ConnectedServiceHandlerContext context, Project project);
    }
}
