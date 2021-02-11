//-----------------------------------------------------------------------------
// <copyright file="V4CodeGenDescriptor.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Services.Design;
using System.Globalization;
using System.IO;
using System.Linq;
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
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding Nuget Packages...").ConfigureAwait(false);

            foreach (var nugetPackage in Common.Constants.V4NuGetPackages)
                await CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage).ConfigureAwait(false);

            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Nuget Packages were installed.").ConfigureAwait(false);
        }

        public override async Task AddGeneratedClientCodeAsync()
        {
            if (this.ServiceConfiguration.IncludeT4File)
            {
                await AddT4FileAsync().ConfigureAwait(true);
            }
            else
            {
                await AddGeneratedCodeAsync().ConfigureAwait(true);
            }

        }

        private async Task AddT4FileAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Adding T4 files for OData V4...").ConfigureAwait(true);

            var tempFile = Path.GetTempFileName();
            var t4Folder = Path.Combine(this.CurrentAssemblyPath, "Templates");

            var referenceFolder = this.GetReferenceFileFolder();

            // generate .ttinclude
            using (StreamWriter writer = File.CreateText(tempFile))
            {
                var ttIncludeText = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.ttinclude"));
                if (this.TargetProjectLanguage == LanguageOption.GenerateVBCode)
                    ttIncludeText = Regex.Replace(ttIncludeText, "(output extension=)\".cs\"", "$1\".vb\"");
                await writer.WriteAsync(ttIncludeText).ConfigureAwait(true);
                await writer.FlushAsync().ConfigureAwait(true);
            }

            await Context.HandlerHelper.AddFileAsync(tempFile, Path.Combine(referenceFolder, GeneratedFileNamePrefix + ".ttinclude")).ConfigureAwait(true);
            await Context.HandlerHelper.AddFileAsync(Path.Combine(t4Folder, "ODataT4CodeGenFilesManager.ttinclude"), Path.Combine(referenceFolder, "ODataT4CodeGenFilesManager.ttinclude")).ConfigureAwait(true);

            tempFile = Path.GetTempFileName();

            // Csdl file name is this format [ServiceName]Csdl.xml
            var csdlFileName = string.Concat(ServiceConfiguration.ServiceName, Common.Constants.CsdlFileNameSuffix);
            var metadataFile = Path.Combine(referenceFolder, csdlFileName);

            // When the T4 file is added to the target project, the proxy and metadata files 
            // are not automatically generated. To avoid ending up with an empty metadata file with 
            // warnings, we pre-populate it with the root element. The content will later be overwritten with the actual metadata when T4 template is run by the user.
            using (StreamWriter writer = File.CreateText(tempFile))
            {
                await writer.WriteLineAsync("<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">").ConfigureAwait(true);
                await writer.WriteLineAsync("</edmx:Edmx>").ConfigureAwait(true);
            }

            await Context.HandlerHelper.AddFileAsync(tempFile, metadataFile, new AddFileOptions() { SuppressOverwritePrompt = true }).ConfigureAwait(true);

            // Hack!
            // Tests were failing since the test project cannot access ProjectItems
            // dte == null when running test cases
            var dte = VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte != null)
            {
                var projectItem = this.GetCsdlFileProjectItem(csdlFileName);
                projectItem.Properties.Item("BuildAction").Value = prjBuildAction.prjBuildActionEmbeddedResource;
            }

            tempFile = Path.GetTempFileName();

            using (StreamWriter writer = File.CreateText(tempFile))
            {
                var text = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.tt"));

                text = Regex.Replace(text, "ODataT4CodeGenerator(\\.ttinclude)", this.GeneratedFileNamePrefix + "$1");
                text = Regex.Replace(text, "(public const string MetadataDocumentUri = )\"\";", "$1@\"" + ServiceConfiguration.Endpoint + "\";");
                text = Regex.Replace(text, "(public const bool UseDataServiceCollection = ).*;", "$1" + ServiceConfiguration.UseDataServiceCollection.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
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
                text = Regex.Replace(text, "(public const bool EnableNamingAlias = )true;", "$1" + this.ServiceConfiguration.EnableNamingAlias.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool IgnoreUnexpectedElementsAndAttributes = )true;", "$1" + this.ServiceConfiguration.IgnoreUnexpectedElementsAndAttributes.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool MakeTypesInternal = )false;", "$1" + ServiceConfiguration.MakeTypesInternal.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool GenerateMultipleFiles = )false;", "$1" + ServiceConfiguration.GenerateMultipleFiles.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                var customHeaders = ServiceConfiguration.CustomHttpHeaders ?? "";
                text = Regex.Replace(text, "(public const string CustomHttpHeaders = )\"\";", "$1@\"" + customHeaders + "\";");
                text = Regex.Replace(text, "(public const string MetadataFilePath = )\"\";", "$1@\"" + metadataFile + "\";");
                text = Regex.Replace(text, "(public const string MetadataFileRelativePath = )\"\";", "$1@\"" + csdlFileName + "\";");
                if (ServiceConfiguration.ExcludedOperationImports?.Any() == true)
                {
                    text = Regex.Replace(text, "(public const string ExcludedOperationImports = )\"\";", "$1\"" + string.Join(",", ServiceConfiguration.ExcludedOperationImports) + "\";");
                }
                if (ServiceConfiguration.ExcludedBoundOperations?.Any() == true)
                {
                    text = Regex.Replace(text, "(public const string ExcludedBoundOperations = )\"\";", "$1\"" + string.Join(",", ServiceConfiguration.ExcludedBoundOperations) + "\";");
                }
                if (ServiceConfiguration.ExcludedSchemaTypes?.Any() == true)
                {
                    text = Regex.Replace(text, "(public const string ExcludedSchemaTypes = )\"\";", "$1\"" + string.Join(",", ServiceConfiguration.ExcludedSchemaTypes) + "\";");
                }
                await writer.WriteAsync(text).ConfigureAwait(true);
                await writer.FlushAsync().ConfigureAwait(true);
            }

            await Context.HandlerHelper.AddFileAsync(tempFile, Path.Combine(referenceFolder, GeneratedFileNamePrefix + ".tt")).ConfigureAwait(true);

            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "T4 files for OData V4 were added.").ConfigureAwait(true);
        }

        private async Task AddGeneratedCodeAsync()
        {
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Generating Client Proxy for OData V4...").ConfigureAwait(true);

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
            t4CodeGenerator.ExcludedBoundOperations = ServiceConfiguration.ExcludedBoundOperations;
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
            var referenceFolder = this.GetReferenceFileFolder();

            // Csdl file name is this format [ServiceName]Csdl.xml
            var csdlFileName = string.Concat(ServiceConfiguration.ServiceName, Common.Constants.CsdlFileNameSuffix);
            var metadataFile = Path.Combine(referenceFolder, csdlFileName);
            await Context.HandlerHelper.AddFileAsync(tempFile, metadataFile, new AddFileOptions() { SuppressOverwritePrompt = true }).ConfigureAwait(true);

            // Hack!
            // Tests were failing since the test project cannot access ProjectItems
            // dte == null when running test cases
            var dte = VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte != null)
            {
                var projectItem = this.GetCsdlFileProjectItem(csdlFileName);
                projectItem.Properties.Item("BuildAction").Value = prjBuildAction.prjBuildActionEmbeddedResource;
                t4CodeGenerator.EmitContainerPropertyAttribute = EmitContainerPropertyAttribute();
            }

            t4CodeGenerator.MetadataFilePath = metadataFile;
            t4CodeGenerator.MetadataFileRelativePath = csdlFileName;

            using (StreamWriter writer = File.CreateText(tempFile))
            {
                await writer.WriteAsync(t4CodeGenerator.TransformText()).ConfigureAwait(true);
                await writer.FlushAsync().ConfigureAwait(true);
                if (t4CodeGenerator.Errors != null && t4CodeGenerator.Errors.Count > 0)
                {
                    foreach (var err in t4CodeGenerator.Errors)
                    {
                        await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Warning, err.ToString()).ConfigureAwait(false);
                    }
                }
            }

            var outputFile = Path.Combine(referenceFolder, $"{this.GeneratedFileNamePrefix}{(this.TargetProjectLanguage == LanguageOption.GenerateCSharpCode ? ".cs" : ".vb")}");
            await Context.HandlerHelper.AddFileAsync(tempFile, outputFile, new AddFileOptions { OpenOnComplete = ServiceConfiguration.OpenGeneratedFilesInIDE }).ConfigureAwait(true);
            t4CodeGenerator.MultipleFilesManager?.GenerateFiles(ServiceConfiguration.GenerateMultipleFiles, this.Context.HandlerHelper, this.Context.Logger, referenceFolder, true, this.ServiceConfiguration.OpenGeneratedFilesInIDE);
            await Context.Logger.WriteMessageAsync(LoggerMessageCategory.Information, "Client Proxy for OData V4 was generated.").ConfigureAwait(true);
        }

        private ProjectItem GetCsdlFileProjectItem(string fileName)
        {
            return this.Project.ProjectItems.Item("Connected Services").ProjectItems.Item(ServiceConfiguration.ServiceName).ProjectItems.Item(fileName);
        }

        private bool EmitContainerPropertyAttribute()
        {
            var vsProject = this.Project.Object as VSProject;

            foreach(Reference reference in vsProject.References)
            {
                if (reference.SourceProject == null)
                {
                    // Assembly reference (For project reference, SourceProject != null)
                    if (reference.Name.Equals("Microsoft.OData.Client", StringComparison.Ordinal) && string.Compare(reference.Version, "7.6.4.0", StringComparison.Ordinal) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
