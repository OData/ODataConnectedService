//-----------------------------------------------------------------------------
// <copyright file="CodeGenDescriptorFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Globalization;
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.OData.ConnectedService.Templates;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    class CodeGenDescriptorFactory : ICodeGenDescriptorFactory
    {
        public BaseCodeGenDescriptor Create(Version edmxVersion, string metadataUri, ConnectedServiceHandlerContext context, Project project)
        {
            if (edmxVersion == Common.Constants.EdmxVersion1
                || edmxVersion == Common.Constants.EdmxVersion2
                || edmxVersion == Common.Constants.EdmxVersion3)
            {
                return CreateV3CodeGenDescriptor(metadataUri, context, project);
            }
            else if (edmxVersion == Common.Constants.EdmxVersion4)
            {
                return CreateV4CodeGenDescriptor(metadataUri, context, project);
            }
            throw new Exception(string.Format(CultureInfo.InvariantCulture, "Not supported Edmx Version {0}", edmxVersion.ToString()));
        }

        protected virtual BaseCodeGenDescriptor CreateV3CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
        {
            return new V3CodeGenDescriptor(metadataUri, context, project);
        }

        protected virtual BaseCodeGenDescriptor CreateV4CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
        {
            return new V4CodeGenDescriptor(metadataUri, context, project, new ODataT4CodeGeneratorFactory());
        }
    }
}