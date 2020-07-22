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
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    internal class V3CodeGenDescriptor : BaseCodeGenDescriptor
    {
        public V3CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project)
            : base(metadataUri, context, project)
        {
            this.ClientNuGetPackageName = Common.Constants.V3ClientNuGetPackage;
            this.ClientDocUri = Common.Constants.V3DocUri;
        }

        public override async Task AddNugetPackagesAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages...").ConfigureAwait(false);

            foreach (var nugetPackage in Common.Constants.V3NuGetPackages)
                await CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage).ConfigureAwait(false);

            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages were installed.").ConfigureAwait(false);
        }

        public override async Task AddGeneratedClientCodeAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy ...").ConfigureAwait(false);

            var generator = new EntityClassGenerator(this.TargetProjectLanguage)
            {
                UseDataServiceCollection = this.ServiceConfiguration.UseDataServiceCollection,
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

            using (var reader = XmlReader.Create(this.MetadataUri, settings))
            {
                var tempFile = Path.GetTempFileName();
                var noErrors = true;

                using (StreamWriter writer = File.CreateText(tempFile))
                {
                    var errors = generator.GenerateCode(reader, writer, this.ServiceConfiguration.NamespacePrefix);
                    await writer.FlushAsync().ConfigureAwait(false);
                    if (errors != null && errors.Any())
                    {
                        noErrors = false;

                        foreach (var err in errors)
                        {
                            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.Message).ConfigureAwait(false);
                        }

                        await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, "Client Proxy for OData V3 was not generated.").ConfigureAwait(false);
                    }
                }

                if (noErrors)
                {
                    var ext = this.TargetProjectLanguage == LanguageOption.GenerateCSharpCode
                        ? ".cs"
                        : ".vb";

                    var outputFile = Path.Combine(GetReferenceFileFolder(), this.GeneratedFileNamePrefix + ext);
                    await Context.HandlerHelper.AddFileAsync(tempFile, outputFile, new AddFileOptions { OpenOnComplete = ServiceConfiguration.OpenGeneratedFilesInIDE }).ConfigureAwait(false);

                    await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OData V3 was generated.").ConfigureAwait(false);
                }
            }
        }
    }
}