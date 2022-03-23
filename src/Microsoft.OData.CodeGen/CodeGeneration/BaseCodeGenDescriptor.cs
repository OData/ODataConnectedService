//-----------------------------------------------------------------------------
// <copyright file="BaseCodeGenDescriptor.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen.PackageInstallation;

namespace Microsoft.OData.CodeGen.CodeGeneration
{
    public abstract class BaseCodeGenDescriptor
    {
        public IPackageInstaller PackageInstaller { get; protected set; }
        public IFileHandler FileHandler { get; private set; }
        public IMessageLogger MessageLogger { get; private set; }
        public string ClientNuGetPackageName { get; set; }
        public string ClientDocUri { get; set; }

        protected string CurrentAssemblyPath => Path.GetDirectoryName(this.GetType().Assembly.Location);

        protected BaseCodeGenDescriptor(IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller)
        {
            this.PackageInstaller = packageInstaller;
            this.FileHandler = fileHandler;
            this.MessageLogger = logger;
        }

        public abstract Task AddNugetPackagesAsync();
        public abstract Task AddGeneratedClientCodeAsync(string metadata, string outputDirectory, LanguageOption languageOption, ServiceConfiguration serviceConfiguration);

        public string GeneratedFileNamePrefix(string generatedFileNamePrefix)
        {
            if (string.IsNullOrWhiteSpace(generatedFileNamePrefix))
            {
                return Common.Constants.DefaultReferenceFileName;
            }
            else
            {
                return generatedFileNamePrefix;
            }
        }
    }
}