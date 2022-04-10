//------------------------------------------------------------------------------
// <copyright file="ICodeGenDescriptorFactory.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Data.Services.Design;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen.PackageInstallation;


namespace Microsoft.OData.CodeGen.CodeGeneration
{
    public interface ICodeGenDescriptorFactory
    {
        BaseCodeGenDescriptor Create(Version edmxVersion, IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller);
    }
}
