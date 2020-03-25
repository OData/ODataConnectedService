// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

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
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages...");

            foreach (var nugetPackage in Common.Constants.V3NuGetPackages)
                await CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage);

            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages were installed.");
        }

        public override async Task AddGeneratedClientCodeAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy ...");

            EntityClassGenerator generator = new EntityClassGenerator(this.TargetProjectLanguage)
            {
                UseDataServiceCollection = this.ServiceConfiguration.UseDataServiceCollection,
                Version = DataServiceCodeVersion.V3
            };

            // Set up XML secure resolver
            XmlUrlResolver xmlUrlResolver = new XmlUrlResolver
            {
                Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
            };

            PermissionSet permissionSet = new PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);

            XmlReaderSettings settings = new XmlReaderSettings
            {
                XmlResolver = new XmlSecureResolver(xmlUrlResolver, permissionSet)
            };

            using (XmlReader reader = XmlReader.Create(this.MetadataUri, settings))
            {
                string tempFile = Path.GetTempFileName();
                bool noErrors = true;

                using (StreamWriter writer = File.CreateText(tempFile))
                {
                    var errors = generator.GenerateCode(reader, writer, this.ServiceConfiguration.NamespacePrefix);
                    await writer.FlushAsync();
                    if (errors != null && errors.Any())
                    {
                        noErrors = false;

                        foreach (var err in errors)
                        {
                            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.Message);
                        }

                        await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, "Client Proxy for OData V3 was not generated.");
                    }
                }

                if (noErrors)
                {
                    var ext = this.TargetProjectLanguage == LanguageOption.GenerateCSharpCode
                        ? ".cs"
                        : ".vb";

                    string outputFile = Path.Combine(GetReferenceFileFolder(), this.GeneratedFileNamePrefix + ext);
                    await this.Context.HandlerHelper.AddFileAsync(tempFile, outputFile, new AddFileOptions { OpenOnComplete = this.ServiceConfiguration.OpenGeneratedFilesInIDE });

                    await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OData V3 was generated.");
                }
            }
        }
    }
}