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
using Microsoft.OData.CodeGen.FileHandling;
using Microsoft.OData.CodeGen.Logging;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.CodeGen.PackageInstallation;
using Microsoft.OData.CodeGen.Templates;

namespace Microsoft.OData.CodeGen.CodeGeneration
{
    public class V4CodeGenDescriptor : BaseCodeGenDescriptor
    {
        public V4CodeGenDescriptor(IFileHandler fileHandler, IMessageLogger logger, IPackageInstaller packageInstaller, IODataT4CodeGeneratorFactory codeGeneratorFactory)
            : base(fileHandler, logger, packageInstaller)
        {
            ClientNuGetPackageName = Common.Constants.V4ClientNuGetPackage;
            ClientDocUri = Common.Constants.V4DocUri;
          //  ServiceConfiguration = base.ServiceConfiguration as ServiceConfigurationV4;
            CodeGeneratorFactory = codeGeneratorFactory;
        }

        private IODataT4CodeGeneratorFactory CodeGeneratorFactory { get; set; }

        public override async Task AddNugetPackagesAsync()
        {
            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Adding Nuget Packages...").ConfigureAwait(false);


            foreach (var nugetPackage in Common.Constants.V4NuGetPackages)
                await PackageInstaller.CheckAndInstallNuGetPackageAsync(Common.Constants.NuGetOnlineRepository, nugetPackage).ConfigureAwait(false);

            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Nuget Packages were installed.").ConfigureAwait(false);
        }

        public override async Task AddGeneratedClientCodeAsync(string metadata, string outputDirectory, LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
        {
            if ((serviceConfiguration as ServiceConfigurationV4).IncludeT4File)
            {
                await AddT4FileAsync(metadata, outputDirectory, languageOption, serviceConfiguration);
            }
            else
            {
                await AddGeneratedCodeAsync(metadata, outputDirectory, languageOption, serviceConfiguration);
            }
        }

        private async Task AddT4FileAsync(string metadata, string outputDirectory, LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
        {
            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Adding T4 files for OData V4...");

            var t4IncludeTempFile = Path.GetTempFileName();
            var t4Folder = Path.Combine(this.CurrentAssemblyPath, "Templates");

            var referenceFolder = outputDirectory;
            var servConfiguration = serviceConfiguration as ServiceConfigurationV4;

            // generate .ttinclude
            using (StreamWriter writer = File.CreateText(t4IncludeTempFile))
            {
                var ttIncludeText = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.ttinclude"));
                if (languageOption == LanguageOption.GenerateVBCode)
                    ttIncludeText = Regex.Replace(ttIncludeText, "(output extension=)\".cs\"", "$1\".vb\"");
                await writer.WriteAsync(ttIncludeText);
                await writer.FlushAsync();
            }

            await FileHandler.AddFileAsync(t4IncludeTempFile, Path.Combine(referenceFolder, this.GeneratedFileNamePrefix(servConfiguration.GeneratedFileNamePrefix) + ".ttinclude"));
            await FileHandler.AddFileAsync(Path.Combine(t4Folder, "ODataT4CodeGenFilesManager.ttinclude"), Path.Combine(referenceFolder, "ODataT4CodeGenFilesManager.ttinclude"));

            var csdlTempFile = Path.GetTempFileName();

            // Csdl file name is this format [ServiceName]Csdl.xml
            var csdlFileName = string.Concat(servConfiguration.ServiceName, Common.Constants.CsdlFileNameSuffix);
            var metadataFile = Path.Combine(referenceFolder, csdlFileName);

            // When the T4 file is added to the target project, the proxy and metadata files 
            // are not automatically generated. To avoid ending up with an empty metadata file with 
            // warnings, we pre-populate it with the root element. The content will later be overwritten with the actual metadata when T4 template is run by the user.
            using (StreamWriter writer = File.CreateText(csdlTempFile))
            {
                await writer.WriteLineAsync("<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">");
                await writer.WriteLineAsync("</edmx:Edmx>");
            }

            await FileHandler.AddFileAsync(csdlTempFile, metadataFile, new ODataFileOptions { SuppressOverwritePrompt = true});

            FileHandler.SetFileAsEmbeddedResource(csdlFileName);

            var t4TempFile = Path.GetTempFileName();

            using (StreamWriter writer = File.CreateText(t4TempFile))
            {
                var text = File.ReadAllText(Path.Combine(t4Folder, "ODataT4CodeGenerator.tt"));

                text = Regex.Replace(text, "ODataT4CodeGenerator(\\.ttinclude)", this.GeneratedFileNamePrefix(servConfiguration.GeneratedFileNamePrefix) + "$1");
                text = Regex.Replace(text, "(public const string MetadataDocumentUri = )\"\";", "$1@\"" + servConfiguration.Endpoint + "\";");
                text = Regex.Replace(text, "(public const bool UseDataServiceCollection = ).*;", "$1" + servConfiguration.UseDataServiceCollection.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const string NamespacePrefix = )\"\\$rootnamespace\\$\";", "$1\"" + servConfiguration.NamespacePrefix + "\";");
                if (languageOption == LanguageOption.GenerateCSharpCode)
                {
                    text = Regex.Replace(text, "(public const string TargetLanguage = )\"OutputLanguage\";",
                        "$1\"CSharp\";");
                }
                else if (languageOption == LanguageOption.GenerateVBCode)
                {
                    text = Regex.Replace(text, "(public const string TargetLanguage = )\"OutputLanguage\";",
                        "$1\"VB\";");
                }
                text = Regex.Replace(text, "(public const bool EnableNamingAlias = )true;", "$1" + servConfiguration.EnableNamingAlias.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool IgnoreUnexpectedElementsAndAttributes = )true;", "$1" + servConfiguration.IgnoreUnexpectedElementsAndAttributes.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool MakeTypesInternal = )false;", "$1" + servConfiguration.MakeTypesInternal.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                text = Regex.Replace(text, "(public const bool GenerateMultipleFiles = )false;", "$1" + servConfiguration.GenerateMultipleFiles.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture) + ";");
                var customHeaders = servConfiguration.CustomHttpHeaders ?? "";
                text = Regex.Replace(text, "(public const string CustomHttpHeaders = )\"\";", "$1@\"" + customHeaders + "\";");
                text = Regex.Replace(text, "(public const string MetadataFilePath = )\"\";", "$1@\"" + metadataFile + "\";");
                text = Regex.Replace(text, "(public const string MetadataFileRelativePath = )\"\";", "$1@\"" + csdlFileName + "\";");
                if (servConfiguration.ExcludedOperationImports?.Any() == true)
                {
                    text = Regex.Replace(text, "(public const string ExcludedOperationImports = )\"\";", "$1\"" + string.Join(",", servConfiguration.ExcludedOperationImports) + "\";");
                }
                if (servConfiguration.ExcludedBoundOperations?.Any() == true)
                {
                    text = Regex.Replace(text, "(public const string ExcludedBoundOperations = )\"\";", "$1\"" + string.Join(",", servConfiguration.ExcludedBoundOperations) + "\";");
                }
                if (servConfiguration.ExcludedSchemaTypes?.Any() == true)
                {
                    text = Regex.Replace(text, "(public const string ExcludedSchemaTypes = )\"\";", "$1\"" + string.Join(",", servConfiguration.ExcludedSchemaTypes) + "\";");
                }
                await writer.WriteAsync(text);
                await writer.FlushAsync();
            }

            await FileHandler.AddFileAsync(t4TempFile, Path.Combine(referenceFolder, this.GeneratedFileNamePrefix(servConfiguration.GeneratedFileNamePrefix) + ".tt"));

            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "T4 files for OData V4 were added.");
        }

        private async Task AddGeneratedCodeAsync(string metadata, string outputDirectory, LanguageOption languageOption, ServiceConfiguration serviceConfiguration)
        {
            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Generating Client Proxy for OData V4...");
            ServiceConfigurationV4 servConfiguration = serviceConfiguration as ServiceConfigurationV4;
            ODataT4CodeGenerator t4CodeGenerator = CodeGeneratorFactory.Create();
            t4CodeGenerator.MetadataDocumentUri = metadata;
            t4CodeGenerator.UseDataServiceCollection = servConfiguration.UseDataServiceCollection;
            t4CodeGenerator.TargetLanguage =
                languageOption == LanguageOption.GenerateCSharpCode
                    ? ODataT4CodeGenerator.LanguageOption.CSharp
                    : ODataT4CodeGenerator.LanguageOption.VB;
            t4CodeGenerator.IgnoreUnexpectedElementsAndAttributes = servConfiguration.IgnoreUnexpectedElementsAndAttributes;
            t4CodeGenerator.EnableNamingAlias = servConfiguration.EnableNamingAlias;
            t4CodeGenerator.NamespacePrefix = servConfiguration.NamespacePrefix;
            t4CodeGenerator.MakeTypesInternal = servConfiguration.MakeTypesInternal;
            t4CodeGenerator.GenerateMultipleFiles = servConfiguration.GenerateMultipleFiles;
            t4CodeGenerator.ExcludedOperationImports = servConfiguration.ExcludedOperationImports;
            t4CodeGenerator.ExcludedBoundOperations = servConfiguration.ExcludedBoundOperations;
            t4CodeGenerator.ExcludedSchemaTypes = servConfiguration.ExcludedSchemaTypes;
            var headers = new List<string>();
            if (servConfiguration.CustomHttpHeaders !=null)
            {
                var headerElements = servConfiguration.CustomHttpHeaders.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var headerElement in headerElements)
                {
                    // Trim header for empty spaces
                    var header = headerElement.Trim();
                    headers.Add(header);
                }
            }
            t4CodeGenerator.CustomHttpHeaders = headers;
            t4CodeGenerator.IncludeWebProxy = servConfiguration.IncludeWebProxy;
            t4CodeGenerator.WebProxyHost = servConfiguration.WebProxyHost;
            t4CodeGenerator.IncludeWebProxyNetworkCredentials = servConfiguration.IncludeWebProxyNetworkCredentials;
            t4CodeGenerator.WebProxyNetworkCredentialsUsername = servConfiguration.WebProxyNetworkCredentialsUsername;
            t4CodeGenerator.WebProxyNetworkCredentialsPassword = servConfiguration.WebProxyNetworkCredentialsPassword;
            t4CodeGenerator.WebProxyNetworkCredentialsDomain = servConfiguration.WebProxyNetworkCredentialsDomain;

            var tempFile = Path.GetTempFileName();
            var referenceFolder = outputDirectory;

            // Csdl file name is this format [ServiceName]Csdl.xml
            var csdlFileName = string.Concat(servConfiguration.ServiceName, Common.Constants.CsdlFileNameSuffix);
            var metadataFile = Path.Combine(referenceFolder, csdlFileName);
            await FileHandler.AddFileAsync(tempFile, metadataFile, new ODataFileOptions { SuppressOverwritePrompt = true});

            FileHandler.SetFileAsEmbeddedResource(csdlFileName);
            t4CodeGenerator.EmitContainerPropertyAttribute = FileHandler.EmitContainerPropertyAttribute();

            t4CodeGenerator.MetadataFilePath = metadataFile;
            t4CodeGenerator.MetadataFileRelativePath = csdlFileName;

            using (StreamWriter writer = File.CreateText(tempFile))
            {
                await writer.WriteAsync(t4CodeGenerator.TransformText());
                await writer.FlushAsync();
                if (t4CodeGenerator.Errors != null && t4CodeGenerator.Errors.Count > 0)
                {
                    foreach (var err in t4CodeGenerator.Errors)
                    {
                        await MessageLogger.WriteMessageAsync(LogMessageCategory.Warning, err.ToString()).ConfigureAwait(false);
                    }
                }
            }

            var outputFile = Path.Combine(referenceFolder, $"{this.GeneratedFileNamePrefix(servConfiguration.GeneratedFileNamePrefix)}{(languageOption == LanguageOption.GenerateCSharpCode ? ".cs" : ".vb")}");
            await FileHandler.AddFileAsync(tempFile, outputFile, new ODataFileOptions { SuppressOverwritePrompt = true});
            t4CodeGenerator.MultipleFilesManager?.GenerateFiles(servConfiguration.GenerateMultipleFiles, FileHandler, MessageLogger, referenceFolder, true, servConfiguration.OpenGeneratedFilesInIDE);
            await MessageLogger.WriteMessageAsync(LogMessageCategory.Information, "Client Proxy for OData V4 was generated.");
        }
    }
}
