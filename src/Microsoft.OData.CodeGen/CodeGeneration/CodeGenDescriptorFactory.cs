//-----------------------------------------------------------------------------
// <copyright file="CodeGenDescriptorFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Globalization;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.PackageInstallation;
using Microsoft.OData.CodeGen.Templates;

namespace Microsoft.OData.CodeGen.CodeGeneration
{
    public class CodeGenDescriptorFactory : ICodeGenDescriptorFactory
    {
        public BaseCodeGenDescriptor Create(Version edmxVersion, IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller)
        {
            if (edmxVersion == Common.Constants.EdmxVersion1
                || edmxVersion == Common.Constants.EdmxVersion2
                || edmxVersion == Common.Constants.EdmxVersion3)
            {
                return CreateV3CodeGenDescriptor(fileHandler, logger, packageInstaller);
            }
            else if (edmxVersion == Common.Constants.EdmxVersion4)
            {
                return CreateV4CodeGenDescriptor(fileHandler, logger, packageInstaller);
            }
            throw new Exception(string.Format(CultureInfo.InvariantCulture, "Not supported Edmx Version {0}", edmxVersion.ToString()));
        }

        protected virtual BaseCodeGenDescriptor CreateV3CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller)
        {
            return new V3CodeGenDescriptor(fileHandler, logger, packageInstaller);
        }

        protected virtual BaseCodeGenDescriptor CreateV4CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller)
        {
            return new V4CodeGenDescriptor(fileHandler, logger, packageInstaller, new ODataT4CodeGeneratorFactory());
        }
    }
}