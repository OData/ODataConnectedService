//-----------------------------------------------------------------------------
// <copyright file="V3CodeGenDescriptor.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Data.Services.Design;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen.PackageInstallation;
using Common = Microsoft.OData.CodeGen.Common;

namespace Microsoft.OData.CodeGen.CodeGeneration
{
    public class V3CodeGenDescriptor : BaseCodeGenDescriptor
    {
        public V3CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller)
            : base(fileHandler, logger, packageInstaller)
        {
            this.ClientNuGetPackageName = Common.Constants.V3ClientNuGetPackage;
            this.ClientDocUri = Common.Constants.V3DocUri;
        }

        public override async Task AddNugetPackagesAsync()
        {
            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Adding Nuget Packages...").ConfigureAwait(false);

            foreach (var nugetPackage in Common.Constants.V3NuGetPackages)
                await PackageInstaller.CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage).ConfigureAwait(false);

            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Nuget Packages were installed.").ConfigureAwait(false);
        }

        public override async Task AddGeneratedClientCodeAsync(string metadataUri, string outputDirectory, Common.LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
        {
            await AddGeneratedCodeAsync(metadataUri, outputDirectory, languageOption, serviceConfiguration);
        }

        private async Task AddGeneratedCodeAsync(string metadataUri, string outputDirectory, Common.LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
        {
            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Generating Client Proxy ...");

            var generator = new EntityClassGenerator((System.Data.Services.Design.LanguageOption)languageOption)
            {
                UseDataServiceCollection = serviceConfiguration.UseDataServiceCollection,
                Version = DataServiceCodeVersion.V3
            };

            // Set up XML secure resolver
            var xmlUrlResolver = new XmlUrlResolver
            {
                Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
            };

            var permissionSet = new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);

            var settings = new XmlReaderSettings
            {
                XmlResolver = new XmlSecureResolver(xmlUrlResolver, permissionSet)
            };

            using (var reader = XmlReader.Create(metadataUri, settings))
            {
                var tempFile = Path.GetTempFileName();
                var noErrors = true;

                using (StreamWriter writer = File.CreateText(tempFile))
                {
                    var errors = generator.GenerateCode(reader, writer, serviceConfiguration.NamespacePrefix);
                    await writer.FlushAsync();
                    if (errors != null && errors.Any())
                    {
                        noErrors = false;

                        foreach (var err in errors)
                        {
                            await MessageLogger.WriteMessageAsync(LogMessageCategory.Warning, err.Message);
                        }

                        await MessageLogger.WriteMessageAsync(LogMessageCategory.Warning, "Client Proxy for OData V3 was not generated.");
                    }
                }

                if (noErrors)
                {
                    var ext = languageOption == Common.LanguageOption.GenerateCSharpCode
                        ? ".cs"
                        : ".vb";

                    var outputFile = Path.Combine(outputDirectory, this.GeneratedFileNamePrefix(serviceConfiguration.GeneratedFileNamePrefix) + ext);
                    await FileHandler.AddFileAsync(tempFile, outputFile, new ODataFileOptions { OpenOnComplete = serviceConfiguration.OpenGeneratedFilesInIDE });

                    await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Client Proxy for OData V3 was generated.");
                }
            }
        }
    }
}