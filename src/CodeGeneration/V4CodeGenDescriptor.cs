// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Services.Design;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Templates;
using Microsoft.VisualStudio.ConnectedServices;
using VSLangProj;

namespace Microsoft.OData.ConnectedService.CodeGeneration
{
    internal class V4CodeGenDescriptor : BaseCodeGenDescriptor
    {
        public V4CodeGenDescriptor(string metadataUri, ConnectedServiceHandlerContext context, Project project, IODataT4CodeGeneratorFactory codeGeneratorFactory)
            : base(metadataUri, context, project)
        {
            ClientNuGetPackageName = Common.Constants.V4ClientNuGetPackage;
            ClientDocUri = Common.Constants.V4DocUri;
            ServiceConfiguration = base.ServiceConfiguration as ServiceConfigurationV4;
            CodeGeneratorFactory = codeGeneratorFactory;
        }

        private IODataT4CodeGeneratorFactory CodeGeneratorFactory { get; set; }

        private new ServiceConfigurationV4 ServiceConfiguration { get; set; }

        public override async Task AddNugetPackagesAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages...");

            foreach (var nugetPackage in Common.Constants.V4NuGetPackages)
                await CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage);

            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages were installed.");
        }

        public override async Task AddGeneratedClientCodeAsync()
        {
            if (this.ServiceConfiguration.IncludeT4File)
            {
                await AddT4FileAsync();
            }
            else
            {
                await AddGeneratedCodeAsync();
            }

            this.ServiceConfiguration.CustomHttpHeaders = null;

            // Since all the code is generated make sure we don't write the username and password for the network credentials
            this.ServiceConfiguration.WebProxyNetworkCredentialsUsername = null;
            this.ServiceConfiguration.WebProxyNetworkCredentialsPassword = null;

        }

        private async Task AddT4FileAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding T4 files for OData V4...");

            var tempFile = Path.GetTempFileName();
            var t4Folder = Path.Combine(this.CurrentAssemblyPath, "Templates");

            var referenceFolder = GetReferenceFileFolder();

            // generate .ttinclude
            using (StreamWriter writer = File.CreateText(tempFile))
            {
                var ttIncludeText = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.ttinclude"));
                if (this.TargetProjectLanguage == LanguageOption.GenerateVBCode)
                    ttIncludeText = Regex.Replace(ttIncludeText, "(output extension=)\".cs\"", "$1\".vb\"");
                await writer.WriteAsync(ttIncludeText);
                await writer.FlushAsync();
            }

            await this.Context.HandlerHelper.AddFileAsync(tempFile, Path.Combine(referenceFolder, this.GeneratedFileNamePrefix + ".ttinclude"));
            await this.Context.HandlerHelper.AddFileAsync(Path.Combine(t4Folder, "ODataT4CodeGenFilesManager.ttinclude"), Path.Combine(referenceFolder, "ODataT4CodeGenFilesManager.ttinclude"));

            tempFile = Path.GetTempFileName();

            var metadataFile = Path.Combine(referenceFolder, Common.Constants.CsdlFileName);
            await this.Context.HandlerHelper.AddFileAsync(tempFile, metadataFile, new AddFileOptions() { SuppressOverwritePrompt = true });
            var projectItem = this.GetCsdlFileProjectItem(Common.Constants.CsdlFileName);
            projectItem.Properties.Item("BuildAction").Value = prjBuildAction.prjBuildActionEmbeddedResource;

            using (StreamWriter writer = File.CreateText(tempFile))
            {
                var text = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.tt"));

                text = Regex.Replace(text, "ODataT4CodeGenerator(\\.ttinclude)", this.GeneratedFileNamePrefix + "$1");
                text = Regex.Replace(text, "(public const string MetadataDocumentUri = )\"\";", "$1\"" + ServiceConfiguration.Endpoint + "\";");
                text = Regex.Replace(text, "(public const bool UseDataServiceCollection = ).*;", "$1" + ServiceConfiguration.UseDataServiceCollection.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const string NamespacePrefix = )\"\\$rootnamespace\\$\";", "$1\"" + ServiceConfiguration.NamespacePrefix + "\";");
                if (this.TargetProjectLanguage == LanguageOption.GenerateCSharpCode)
                {
                    text = Regex.Replace(text, "(public const string TargetLanguage = )\"OutputLanguage\";",
                        "$1\"CSharp\";");
                }
                else if (this.TargetProjectLanguage == LanguageOption.GenerateVBCode)
                {
                    text = Regex.Replace(text, "(public const string TargetLanguage = )\"OutputLanguage\";",
                        "$1\"VB\";");
                }
                text = Regex.Replace(text, "(public const bool EnableNamingAlias = )true;", "$1" + this.ServiceConfiguration.EnableNamingAlias.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool IgnoreUnexpectedElementsAndAttributes = )true;", "$1" + this.ServiceConfiguration.IgnoreUnexpectedElementsAndAttributes.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool MakeTypesInternal = )false;", "$1" + ServiceConfiguration.MakeTypesInternal.ToString().ToLower(CultureInfo.InvariantCulture) + ";");
                var customHeaders = ServiceConfiguration.CustomHttpHeaders ?? "";
                text = Regex.Replace(text, "(public const string CustomHttpHeaders = )\"\";", "$1@\"" + customHeaders + "\";");
                text = Regex.Replace(text, "(public const string MetadataFilePath = )\"\";", "$1@\"" + metadataFile + "\";");
                text = Regex.Replace(text, "(public const string MetadataFileRelativePath = )\"\";", "$1@\"" + Common.Constants.CsdlFileName + "\";");
                await writer.WriteAsync(text);
                await writer.FlushAsync();
            }

            await this.Context.HandlerHelper.AddFileAsync(tempFile, Path.Combine(referenceFolder, this.GeneratedFileNamePrefix + ".tt"));

            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "T4 files for OData V4 were added.");
        }

        private async Task AddGeneratedCodeAsync()
        {
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy for OData V4...");

            ODataT4CodeGenerator t4CodeGenerator = CodeGeneratorFactory.Create();
            t4CodeGenerator.MetadataDocumentUri = MetadataUri;
            t4CodeGenerator.UseDataServiceCollection = this.ServiceConfiguration.UseDataServiceCollection;
            t4CodeGenerator.TargetLanguage =
                this.TargetProjectLanguage == LanguageOption.GenerateCSharpCode
                    ? ODataT4CodeGenerator.LanguageOption.CSharp
                    : ODataT4CodeGenerator.LanguageOption.VB;
            t4CodeGenerator.IgnoreUnexpectedElementsAndAttributes = this.ServiceConfiguration.IgnoreUnexpectedElementsAndAttributes;
            t4CodeGenerator.EnableNamingAlias = this.ServiceConfiguration.EnableNamingAlias;
            t4CodeGenerator.NamespacePrefix = this.ServiceConfiguration.NamespacePrefix;
            t4CodeGenerator.MakeTypesInternal = ServiceConfiguration.MakeTypesInternal;
            t4CodeGenerator.GenerateMultipleFiles = ServiceConfiguration.GenerateMultipleFiles;
            t4CodeGenerator.ExcludedOperationImports = ServiceConfiguration.ExcludedOperationImports;
            t4CodeGenerator.ExcludedSchemaTypes = ServiceConfiguration.ExcludedSchemaTypes;
            var headers = new List<string>();
            if (this.ServiceConfiguration.CustomHttpHeaders !=null)
            {
                var headerElements = this.ServiceConfiguration.CustomHttpHeaders.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var headerElement in headerElements)
                {
                    // Trim header for empty spaces
                    var header = headerElement.Trim();
                    headers.Add(header);
                }
            }
            t4CodeGenerator.CustomHttpHeaders = headers;
            t4CodeGenerator.IncludeWebProxy = ServiceConfiguration.IncludeWebProxy;
            t4CodeGenerator.WebProxyHost = ServiceConfiguration.WebProxyHost;
            t4CodeGenerator.IncludeWebProxyNetworkCredentials = ServiceConfiguration.IncludeWebProxyNetworkCredentials;
            t4CodeGenerator.WebProxyNetworkCredentialsUsername = ServiceConfiguration.WebProxyNetworkCredentialsUsername;
            t4CodeGenerator.WebProxyNetworkCredentialsPassword = ServiceConfiguration.WebProxyNetworkCredentialsPassword;
            t4CodeGenerator.WebProxyNetworkCredentialsDomain = ServiceConfiguration.WebProxyNetworkCredentialsDomain;

            var tempFile = Path.GetTempFileName();
            var referenceFolder = GetReferenceFileFolder();

            var metadataFile = Path.Combine(referenceFolder, Common.Constants.CsdlFileName);
            await this.Context.HandlerHelper.AddFileAsync(tempFile, metadataFile, new AddFileOptions() { SuppressOverwritePrompt = true });

            // Hack!
            // Tests were failing since the test project cannot access ProjectItems
            // dte == null when running test cases
            var dte = VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            if(dte != null)
            {
                var projectItem = this.GetCsdlFileProjectItem(Common.Constants.CsdlFileName);
                projectItem.Properties.Item("BuildAction").Value = prjBuildAction.prjBuildActionEmbeddedResource;
            }

            t4CodeGenerator.MetadataFilePath = metadataFile;
            t4CodeGenerator.MetadataFileRelativePath = Common.Constants.CsdlFileName;

            using (StreamWriter writer = File.CreateText(tempFile))
            {
                await writer.WriteAsync(t4CodeGenerator.TransformText());
                await writer.FlushAsync();
                if (t4CodeGenerator.Errors != null && t4CodeGenerator.Errors.Count > 0)
                {
                    foreach (var err in t4CodeGenerator.Errors)
                    {
                        await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.ToString());
                    }
                }
            }

            var outputFile = Path.Combine(referenceFolder, $"{this.GeneratedFileNamePrefix}{(this.TargetProjectLanguage == LanguageOption.GenerateCSharpCode ? ".cs" : ".vb")}");
            await this.Context.HandlerHelper.AddFileAsync(tempFile, outputFile, new AddFileOptions { OpenOnComplete = this.ServiceConfiguration.OpenGeneratedFilesInIDE });
            t4CodeGenerator.MultipleFilesManager?.GenerateFiles(ServiceConfiguration.GenerateMultipleFiles, this.Context.HandlerHelper, this.Context.Logger, referenceFolder, true, this.ServiceConfiguration.OpenGeneratedFilesInIDE);
            await this.Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OData V4 was generated.");
        }

        private ProjectItem GetCsdlFileProjectItem(string fileName)
        {
            return this.Project.ProjectItems.Item("Connected Services").ProjectItems.Item(ServiceConfiguration.ServiceName).ProjectItems.Item(fileName);
        }
    }
}