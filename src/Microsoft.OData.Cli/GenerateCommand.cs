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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Evaluation;
using Microsoft.OData.Cli.Models;
using Microsoft.OData.CodeGen.CodeGeneration;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Newtonsoft.Json;

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
                Description = "The URI of the metadata document. The value must be set to a valid service document URI or a local file path. Optional if config file is specified. If specified, it will take precedence over the value in config file.",
                IsRequired = false
            };

            this.AddOption(metadataUri);

            Option configFile = new Option<string>(new[] { "--config-file", "-c" })
            {
                Name = "config-file",
                Description = "Path to the OData Connected Service config file.",
                IsRequired = false
            };

            this.AddOption(configFile);

            this.AddValidator((commandResult) =>
            {
                var metadataUriValue = commandResult.GetValueForOption(metadataUri) as string;
                var configFileValue = commandResult.GetValueForOption(configFile) as string;
                if (string.IsNullOrWhiteSpace(metadataUriValue) && string.IsNullOrWhiteSpace(configFileValue))
                {
                    commandResult.ErrorMessage = $"Either of '{metadataUri.Name}' or '{configFile.Name}' options must be specified.";
                }
            });

            Option fileName = new Option<string>(new[] { "--file-name", "-fn" })
            {
                Name = "file-name",
                Description = "The name of the generated file. If not provided, the default name 'Reference.cs/.vb' is used.",
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

            Option enableTracking = new Option<bool?>(new[] { "--enable-tracking", "-et" })
            {
                Name = "enable-tracking",
                Description = "Enable entity and property tracking."
            };

            enableTracking.SetDefaultValue(null);
            this.AddOption(enableTracking);

            Option upperCamelCase = new Option<bool?>(new[] { "--upper-camel-case", "-ucc" })
            {
                Name = "upper-camel-case",
                Description = "Disables upper camel casing."
            };

            upperCamelCase.SetDefaultValue(null);
            this.AddOption(upperCamelCase);

            Option internalModifier = new Option<bool?>(new[] { "--enable-internal", "-i" })
            {
                Name = "enable-internal",
                Description = "Apply the \"internal\" class modifier on generated classes instead of \"public\" thereby making them invisible outside the assembly."
            };

            this.AddOption(internalModifier);

            Option omitVersioningInfo = new Option<bool?>(new[] { "--omit-versioning-info", "-vi" })
            {
                Name = "omit-versioning-info",
                Description = "Omit runtime version and code generation timestamp from the generated files.",
            };

            noTimestamp.SetDefaultValue(null);
            this.AddOption(noTimestamp);

            Option multipleFiles = new Option<bool?>(new[] { "--multiple-files" })
            {
                Name = "multiple-files",
                Description = "Split the generated classes into separate files instead of generating all the code in a single file."
            };

            multipleFiles.SetDefaultValue(null);
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

            Option ignoreUnexpectedElements = new Option<bool?>(new[] { "--ignore-unexpected-elements", "-iue" })
            {
                Name = "ignore-unexpected-elements",
                Description = "This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any."
            };
            
            ignoreUnexpectedElements.SetDefaultValue(null);
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

                ServiceConfiguration config = GetServiceConfiguration<ServiceConfiguration>(options);
                MetadataReader.ProcessServiceMetadata(config, out Version version);
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
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Get a <see cref="ServiceConfiguration"/> object from <paramref name="generateOptions"/>
        /// </summary>
        /// <typeparam name="TServiceConfig">Type of <see cref="ServiceConfiguration"/> to return</typeparam>
        /// <param name="generateOptions">Source options used to populate the service configuration</param>
        /// <returns>New <typeparamref name="TServiceConfig"/> from <paramref name="generateOptions"/></returns>
        private TServiceConfig GetServiceConfiguration<TServiceConfig>(GenerateOptions generateOptions)
            where TServiceConfig : ServiceConfiguration, new()
        {
            TServiceConfig serviceConfig = null;
            BaseUserSettings configUserSettings = null;
            if (!string.IsNullOrWhiteSpace(generateOptions.ConfigFile))
            {
                var configFileData = ReadConfigFile(generateOptions.ConfigFile);
                configUserSettings = configFileData?.ExtendedData;
            }

            var namespacePrefix = string.IsNullOrEmpty(generateOptions.NamespacePrefix) ? configUserSettings?.NamespacePrefix : generateOptions.NamespacePrefix;
            var excludedSchemaTypes = generateOptions.ExcludedSchemaTypes?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(type => type.Trim()).ToList();
            var excludedBoundOperations = generateOptions.ExcludedBoundOperations?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(operation => operation.Trim()).ToList();
            var excludedOperationImports = generateOptions.ExcludedOperationImports?.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(import => import.Trim()).ToList();

            serviceConfig = new TServiceConfig
            {
                Endpoint = string.IsNullOrEmpty(generateOptions.MetadataUri) ? configUserSettings?.Endpoint : generateOptions.MetadataUri,
                ServiceName = string.IsNullOrEmpty(configUserSettings?.ServiceName) ? Constants.DefaultServiceName : configUserSettings?.ServiceName,
                GeneratedFileNamePrefix = string.IsNullOrEmpty(generateOptions.FileName) ? configUserSettings?.GeneratedFileNamePrefix : generateOptions.FileName,
                CustomHttpHeaders = string.IsNullOrEmpty(generateOptions.CustomHeaders) ? configUserSettings?.CustomHttpHeaders : generateOptions.CustomHeaders,
                WebProxyHost = string.IsNullOrEmpty(generateOptions.WebProxyHost) ? configUserSettings?.WebProxyHost : generateOptions.WebProxyHost,
                IncludeWebProxy = generateOptions.IncludeWebProxy || (configUserSettings?.IncludeWebProxy ?? false),
                IncludeWebProxyNetworkCredentials = generateOptions.IncludeWebProxyNetworkCredentials
                    || (configUserSettings?.IncludeWebProxyNetworkCredentials ?? false),
                WebProxyNetworkCredentialsUsername = string.IsNullOrEmpty(generateOptions.WebProxyNetworkCredentialsUsername) ? configUserSettings?.WebProxyNetworkCredentialsUsername : generateOptions.WebProxyNetworkCredentialsUsername,
                WebProxyNetworkCredentialsPassword = string.IsNullOrEmpty(generateOptions.WebProxyNetworkCredentialsDomain) ? configUserSettings?.WebProxyNetworkCredentialsPassword : generateOptions.WebProxyNetworkCredentialsDomain,
                WebProxyNetworkCredentialsDomain = string.IsNullOrEmpty(generateOptions.WebProxyNetworkCredentialsPassword) ? configUserSettings?.WebProxyNetworkCredentialsDomain : generateOptions.WebProxyNetworkCredentialsPassword,
                NamespacePrefix = namespacePrefix,
                UseNamespacePrefix = (configUserSettings?.UseNamespacePrefix ?? false) || (!string.IsNullOrWhiteSpace(namespacePrefix)),
                UseDataServiceCollection = (generateOptions.EnableTracking == null) ? (configUserSettings?.UseDataServiceCollection ?? false) : generateOptions.EnableTracking.Value,
                MakeTypesInternal = (generateOptions.EnableInternal == null) ? (configUserSettings?.MakeTypesInternal ?? false) : generateOptions.EnableInternal.Value,
                GenerateMultipleFiles = (generateOptions.MultipleFiles == null) ? (configUserSettings?.GenerateMultipleFiles ?? false) : generateOptions.MultipleFiles.Value,
                ExcludedSchemaTypes = excludedSchemaTypes?.Count > 0 ? excludedSchemaTypes : configUserSettings?.ExcludedSchemaTypes,
            };

            if (serviceConfig is ServiceConfigurationV4 serviceConfigurationV4)
            {
                // Add additional V4 properties
                serviceConfigurationV4.EnableNamingAlias = (generateOptions.UpperCamelCase == null) ? (configUserSettings?.EnableNamingAlias ?? false) : generateOptions.UpperCamelCase.Value;
                serviceConfigurationV4.IgnoreUnexpectedElementsAndAttributes = (generateOptions.IgnoreUnexpectedElements == null) ? (configUserSettings?.IgnoreUnexpectedElementsAndAttributes ?? false) : generateOptions.IgnoreUnexpectedElements.Value;
                serviceConfigurationV4.IncludeT4File = configUserSettings?.IncludeT4File ?? false;
                serviceConfigurationV4.ExcludedOperationImports = excludedOperationImports?.Count > 0 ? excludedOperationImports : configUserSettings?.ExcludedOperationImports;
                serviceConfigurationV4.ExcludedBoundOperations = excludedBoundOperations?.Count > 0 ? excludedBoundOperations : configUserSettings?.ExcludedBoundOperations;
                serviceConfigurationV4.OmitVersioningInfo = (generateOptions.OmitVersioningInfo == null) ? (configUserSettings?.OmitVersioningInfo ?? false) : generateOptions.OmitVersioningInfo.Value;
            }

            return serviceConfig;
        }

        /// <summary>
        /// Read and deserialize <paramref name="fileName"/> into <see cref="ConfigJsonFile"/>
        /// </summary>
        /// <param name="fileName">Name of config file to read</param>
        /// <returns><see cref="ConnectedServiceFileData"/> read from <paramref name="fileName"/></returns>
        /// <exception cref="Exception">Thrown on deserialization errors</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileName"/> is null/empty or does not exist</exception>
        private ConnectedServiceFileData ReadConfigFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException($"{nameof(fileName)} cannot be null/empty", nameof(fileName));
            }

            if (!File.Exists(fileName))
            {
                throw new ArgumentException($"Specified config file does not exist: '{fileName}'", nameof(fileName));
            }

            string configFileText;
            try
            {
                configFileText = File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load configuration file '{fileName}': {ex.Message}", ex);
            }

            if (string.IsNullOrWhiteSpace(configFileText))
            {
                throw new Exception($"Config file '{fileName}' is empty.");
            }

            try
            {
                return JsonConvert.DeserializeObject<ConnectedServiceFileData>(configFileText);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Contents of the config file ('{fileName}') could not be deserialized: {ex.Message}", ex);
            }
        }

        private async Task GenerateCodeForV4Clients(GenerateOptions generateOptions, IConsole console)
        {
            var serviceConfiguration = GetServiceConfiguration<ServiceConfigurationV4>(generateOptions);
            Project project = ProjectHelper.CreateProjectInstance(generateOptions.OutputDir);
            BaseCodeGenDescriptor codeGenDescriptor = new CodeGenDescriptorFactory().Create(
                Constants.EdmxVersion4,
                new ODataCliFileHandler(new ODataCliMessageLogger(console), project),
                new ODataCliMessageLogger(console),
                new ODataCliPackageInstaller(project, new ODataCliMessageLogger(console)));
            await codeGenDescriptor.AddNugetPackagesAsync().ConfigureAwait(false);
            await codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfiguration.Endpoint, generateOptions.OutputDir, LanguageOption.GenerateCSharpCode, serviceConfiguration).ConfigureAwait(false);
        }

        private async Task GenerateCodeForV3Clients(GenerateOptions generateOptions, IConsole console)
        {
            var serviceConfiguration = GetServiceConfiguration<ServiceConfiguration>(generateOptions);
            Project project = ProjectHelper.CreateProjectInstance(generateOptions.OutputDir);
            BaseCodeGenDescriptor codeGenDescriptor = new CodeGenDescriptorFactory().Create(
                Constants.EdmxVersion3,
                new ODataCliFileHandler(new ODataCliMessageLogger(console), project),
                new ODataCliMessageLogger(console),
                new ODataCliPackageInstaller(project,
                new ODataCliMessageLogger(console)));
            await codeGenDescriptor.AddNugetPackagesAsync().ConfigureAwait(false);
            await codeGenDescriptor.AddGeneratedClientCodeAsync(serviceConfiguration.Endpoint, generateOptions.OutputDir, LanguageOption.GenerateCSharpCode, serviceConfiguration).ConfigureAwait(false);
        }
    }
}
