//-----------------------------------------------------------------------------
// <copyright file="GenerateCommand.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.OData.CodeGen.CodeGeneration;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;

namespace Microsoft.OData.Cli
{
    /// <summary>
    /// The generate command class handler.
    /// </summary>
    public class GenerateCommand : Command
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GenerateCommand"/> class.
        /// </summary>
        public GenerateCommand()
           : base("generate", "Command to generate proxy classes for OData endpoints.")
        {
            Option metadataUri = new Option<string>(new[] { "--metadata-uri", "-m" })
            {
                Name = "metadata-uri",
                Description = "The URI of the metadata document. The value must be set to a valid service document URI or a local file path.",
                IsRequired = true
            };

            this.AddOption(metadataUri);

            Option fileName = new Option<string>(new[] { "--file-name", "-fn" })
            {
                Name = "file-name",
                Description = "The name of the generated file. If not provided then the default name 'Reference.cs/.vb' is used",
            };

            this.AddOption(fileName);

            Option customHeaders = new Option<string>(new[] { "--custom-headers", "-h" })
            {
                Name = "custom-headers",
                Description = "Headers that will get sent along with the request when fetching the metadata document from the service. Format: Header1:HeaderValue, Header2:HeaderValue."
            };

            this.AddOption(customHeaders);

            Option proxy = new Option<string>(new[] { "--proxy", "-p" })
            {
                Name = "proxy",
                Description = "Proxy settings. Format: domain\\user:password@SERVER:PORT."
            };

            this.AddOption(proxy);

            Option ns = new Option<string>(new[] { "--namespace-prefix", "-ns" })
            {
                Name = "namespace-prefix",
                Description = "The namespace of the client code generated. Example:ODataCliCodeGeneratorSample.NorthWindModel or ODataCliCodeGeneratorSample or it could be a name related to the OData endpoint."
            };

            this.AddOption(ns);

            Option upperCamelCase = new Option<bool>(new[] { "--upper-camel-case", "-ucc" })
            {
                Name = "upper-camel-case",
                Description = "Disables upper camel casing."
            };

            this.AddOption(upperCamelCase);

            Option internalModifier = new Option<bool>(new[] { "--internal", "-i" })
            {
                Name = "internal",
                Description = "Apply the \"internal\" class modifier on generated classes instead of \"public\" thereby making them invisible outside the assembly."
            };

            this.AddOption(internalModifier);

            Option multipleFiles = new Option<bool>(new[] { "--multiple-files" })
            {
                Name = "multiple-files",
                Description = "Split the generated classes into separate files instead of generating all the code in a single file."
            };

            this.AddOption(multipleFiles);

            Option excludedOperationImports = new Option<string>(new[] { "--excluded-operation-imports", "-eoi" })
            {
                Name = "excluded-operation-imports",
                Description = "Comma-separated list of the names of operation imports to exclude from the generated code. Example: ExcludedOperationImport1,ExcludedOperationImport2."
            };

            this.AddOption(excludedOperationImports);

            Option excludedBoundOperations = new Option<string>(new[] { "--excluded-bound-operations", "-ebo" })
            {
                Name = "excluded-bound-operations",
                Description = "Comma-separated list of the names of bound operations to exclude from the generated code. Example: BoundOperation1,BoundOperation2."
            };

            this.AddOption(excludedBoundOperations);

            Option excludedSchemaTypes = new Option<string>(new[] { "--excluded-schema-types", "-est" })
            {
                Name = "excluded-schema-types",
                Description = "Comma-separated list of the names of entity types to exclude from the generated code. Example: EntityType1,EntityType2,EntityType3."
            };

            this.AddOption(excludedSchemaTypes);

            Option ignoreUnexpectedElements = new Option<bool>(new[] { "--ignore-unexpected-elements", "-iue" })
            {
                Name = "ignore-unexpected-elements",
                Description = "This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any."
            };

            this.AddOption(ignoreUnexpectedElements);

            Option outputDir = new Option<string>(new[] { "--outputdir", "-o" })
            {
                Name = "outputdir",
                Description = "Full path to output directory.",
                IsRequired = true
            };

            this.AddOption(outputDir);

            this.Handler = CommandHandler.Create(
                (
                    GenerateOptions options,
                    IConsole console
                ) 
                => HandleGenerateCommand(options, console));
        }

        private async Task<int> HandleGenerateCommand(GenerateOptions options, IConsole console)
        {
            try
            {
                if (!Directory.Exists(options.OutputDir))
                    Directory.CreateDirectory(options.OutputDir);

                if (options.Proxy != null)
                {
                    string[] proxyParts = options.Proxy.Split('@');
                    string server = proxyParts.Length > 0 ? proxyParts[0] : string.Empty;
                    string username = proxyParts.Length > 1 ? proxyParts[1] : string.Empty;

                    if (!string.IsNullOrWhiteSpace(server))
                    {
                        options.WebProxyHost = server;
                        if (!string.IsNullOrWhiteSpace(username))
                        {
                            string[] usernameParts = username.Split(':');
                            options.WebProxyNetworkCredentialsPassword = usernameParts.Length > 1 ? usernameParts[1] : string.Empty;

                            string[] userParts = usernameParts[0].Split('\\', '/');
                            options.WebProxyNetworkCredentialsDomain = userParts.Length > 0 ? userParts[0] : string.Empty;
                            options.WebProxyNetworkCredentialsUsername = userParts.Length > 1 ? userParts[1] : string.Empty;
                            options.IncludeWebProxyNetworkCredentials = true;
                        }
                        options.IncludeWebProxy = true;
                    }
                }

                Version version = GetMetadataVersion(options);
                if (version == Constants.EdmxVersion4)
                {
                    await GenerateCodeForV4Clients(options, console).ConfigureAwait(false);
                }
                else if (version == Constants.EdmxVersion3)
                {
                    await GenerateCodeForV3Clients(options, console).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                console.Error.Write(ex.Message);
                return 0;
            }

            return 1;
        }

        private Version GetMetadataVersion(GenerateOptions generateOptions)
        {
            Version version = null;
            var serviceConfiguration = new ServiceConfiguration();
            serviceConfiguration.Endpoint = generateOptions.MetadataUri;
            serviceConfiguration.CustomHttpHeaders = generateOptions.CustomHeaders;
            serviceConfiguration.WebProxyHost = generateOptions.WebProxyHost;
            serviceConfiguration.IncludeWebProxy = generateOptions.IncludeWebProxy;
            serviceConfiguration.IncludeWebProxyNetworkCredentials = generateOptions.IncludeWebProxyNetworkCredentials;
            serviceConfiguration.WebProxyNetworkCredentialsUsername = generateOptions.WebProxyNetworkCredentialsUsername;
            serviceConfiguration.WebProxyNetworkCredentialsPassword = generateOptions.WebProxyNetworkCredentialsPassword;
            serviceConfiguration.WebProxyNetworkCredentialsDomain = generateOptions.WebProxyNetworkCredentialsDomain;
            
            version = MetadataReader.GetMetadataVersion(serviceConfiguration);
            return version;
        }

        private async Task GenerateCodeForV4Clients(GenerateOptions generateOptions, IConsole console)
        {
            var serviceConfigurationV4 = new ServiceConfigurationV4();
            serviceConfigurationV4.Endpoint = generateOptions.MetadataUri;
            serviceConfigurationV4.ServiceName = Constants.DefaultServiceName;
            serviceConfigurationV4.GeneratedFileNamePrefix = generateOptions.FileName;
            serviceConfigurationV4.CustomHttpHeaders = generateOptions.CustomHeaders;
            serviceConfigurationV4.WebProxyHost = generateOptions.WebProxyHost;
            serviceConfigurationV4.IncludeWebProxy = generateOptions.IncludeWebProxy;
            serviceConfigurationV4.IncludeWebProxyNetworkCredentials = generateOptions.IncludeWebProxyNetworkCredentials;
            serviceConfigurationV4.WebProxyNetworkCredentialsUsername = generateOptions.WebProxyNetworkCredentialsUsername;
            serviceConfigurationV4.WebProxyNetworkCredentialsPassword = generateOptions.WebProxyNetworkCredentialsPassword;
            serviceConfigurationV4.WebProxyNetworkCredentialsDomain = generateOptions.WebProxyNetworkCredentialsDomain;
            serviceConfigurationV4.NamespacePrefix = generateOptions.NamespacePrefix;
            serviceConfigurationV4.MakeTypesInternal = generateOptions.EnableInternal;
            serviceConfigurationV4.GenerateMultipleFiles = generateOptions.MultipleFiles;
            serviceConfigurationV4.ExcludedSchemaTypes = generateOptions.ExcludedSchemaTypes;
            serviceConfigurationV4.ExcludedBoundOperations = generateOptions.ExcludedBoundOperations;
            serviceConfigurationV4.ExcludedOperationImports = generateOptions.ExcludedOperationImports;
            serviceConfigurationV4.IgnoreUnexpectedElementsAndAttributes = generateOptions.IgnoreUnexpectedElements;
            serviceConfigurationV4.EnableNamingAlias = generateOptions.UpperCamelCase;

            Project project = ProjectHelper.CreateProjectInstance(generateOptions.OutputDir);
            BaseCodeGenDescriptor codeGenDescriptor = new CodeGenDescriptorFactory().Create(
                Constants.EdmxVersion4, 
                new ODataCliFileHandler(new ODataCliMessageLogger(console), project),
                new ODataCliMessageLogger(console),
                new ODataCliPackageInstaller(project, new ODataCliMessageLogger(console)));
            await codeGenDescriptor.AddNugetPackagesAsync().ConfigureAwait(false);
            await codeGenDescriptor.AddGeneratedClientCodeAsync(generateOptions.MetadataUri, generateOptions.OutputDir, LanguageOption.GenerateCSharpCode, serviceConfigurationV4).ConfigureAwait(false);
        }

        private async Task GenerateCodeForV3Clients(GenerateOptions generateOptions, IConsole console)
        {
            var serviceConfiguration = new ServiceConfiguration();
            serviceConfiguration.Endpoint = generateOptions.MetadataUri;
            serviceConfiguration.ServiceName = Constants.DefaultServiceName;
            serviceConfiguration.GeneratedFileNamePrefix = generateOptions.FileName;
            serviceConfiguration.CustomHttpHeaders = generateOptions.CustomHeaders;
            serviceConfiguration.WebProxyHost = generateOptions.WebProxyHost;
            serviceConfiguration.IncludeWebProxy = generateOptions.IncludeWebProxy;
            serviceConfiguration.IncludeWebProxyNetworkCredentials = generateOptions.IncludeWebProxyNetworkCredentials;
            serviceConfiguration.WebProxyNetworkCredentialsUsername = generateOptions.WebProxyNetworkCredentialsUsername;
            serviceConfiguration.WebProxyNetworkCredentialsPassword = generateOptions.WebProxyNetworkCredentialsPassword;
            serviceConfiguration.WebProxyNetworkCredentialsDomain = generateOptions.WebProxyNetworkCredentialsDomain;
            serviceConfiguration.NamespacePrefix = generateOptions.NamespacePrefix;

            Project project = ProjectHelper.CreateProjectInstance(generateOptions.OutputDir);
            BaseCodeGenDescriptor codeGenDescriptor = new CodeGenDescriptorFactory().Create(
                Constants.EdmxVersion3,
                new ODataCliFileHandler(new ODataCliMessageLogger(console), project),
                new ODataCliMessageLogger(console),
                new ODataCliPackageInstaller(project,
                new ODataCliMessageLogger(console)));
            await codeGenDescriptor.AddNugetPackagesAsync().ConfigureAwait(false);
            await codeGenDescriptor.AddGeneratedClientCodeAsync(generateOptions.MetadataUri, generateOptions.OutputDir, LanguageOption.GenerateCSharpCode, serviceConfiguration).ConfigureAwait(false);
        }
    }
}
