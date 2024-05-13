//-----------------------------------------------------------------------------------
// <copyright file="GenerateCommandTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

namespace Microsoft.OData.Cli.Tests.Command
{
    public class GenerateCommandTests
    {
        private readonly GenerateCommand generateCommand;
        private readonly string metadataUri;
        private readonly string outputDir;

        public GenerateCommandTests()
        {
            generateCommand = new GenerateCommand();
            metadataUri = metadataUri = Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4.xml");
            outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        [Fact]
        public void TestEitherOfMetadataUriOrConfigFileCommandLineOptionIsRequired()
        {
            var parseResult = this.generateCommand.Parse($"--outputdir {this.outputDir}");

            var parseError = Assert.Single(parseResult.Errors);
            Assert.Equal("Either of 'metadata-uri' or 'config-file' options must be specified.", parseError.Message);
        }

        [Fact]
        public void TestOutputDirCommandLineOptionIsRequired()
        {           
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri}");

            var parseError = Assert.Single(parseResult.Errors);
            Assert.Equal("Option '--outputdir' is required.", parseError.Message);
        }

        [Theory]
        [InlineData("--metadata-uri", "--outputdir")]
        [InlineData("-m", "-o")]
        public void TestRequiredCommandLineOptions(string metadataUriOption, string outputDirOption)
        {
            var parseResult = this.generateCommand.Parse($"{metadataUriOption} {this.metadataUri} {outputDirOption} {this.outputDir}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(this.metadataUri, generateOptions.MetadataUri);
                Assert.Equal(this.outputDir, generateOptions.OutputDir);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("--config-file SampleServiceV4.json")]
        [InlineData("-c SampleServiceV4.json")]
        public void TestConfigFileCommandLineOption(string configFileOption)
        {
            var parseResult = this.generateCommand.Parse($"{configFileOption} --outputdir {this.outputDir}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal("SampleServiceV4.json", generateOptions.ConfigFile);
                Assert.Null(generateOptions.MetadataUri);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--file-name SampleServiceV4Proxy", "SampleServiceV4Proxy")]
        [InlineData("-fn SampleServiceV4Proxy", "SampleServiceV4Proxy")]
        public void TestFileNameCommandLineOption(string fileNameOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {fileNameOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.FileName);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--custom-headers foo:bar", "foo:bar")]
        [InlineData("-h foo:bar", "foo:bar")]
        public void TestCustomHeaderCommandLineOption(string customHeadersOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {customHeadersOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.CustomHeaders);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--proxy domain\\user:password@SERVER:PORT", "domain\\user:password@SERVER:PORT")]
        [InlineData("-p domain\\user:password@SERVER:PORT", "domain\\user:password@SERVER:PORT")]
        public void TestProxyCommandLineOption(string proxyOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {proxyOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.Proxy);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--namespace-prefix NS.Models", "NS.Models")]
        [InlineData("-ns NS.Models", "NS.Models")]
        public void TestNamespacePrefixCommandLineOption(string namespacePrefixOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {namespacePrefixOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.NamespacePrefix);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--enable-tracking", true)]
        [InlineData("--enable-tracking true", true)]
        [InlineData("--enable-tracking false", false)]
        [InlineData("-et", true)]
        [InlineData("-et true", true)]
        [InlineData("-et false", false)]
        public void TestEnableTrackingCommandLineOption(string enableTrackingOption, bool? expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {enableTrackingOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.EnableTracking);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--upper-camel-case", true)]
        [InlineData("--upper-camel-case true", true)]
        [InlineData("--upper-camel-case false", false)]
        [InlineData("-ucc", true)]
        [InlineData("-ucc true", true)]
        [InlineData("-ucc false", false)]
        public void TestUpperCamelCaseCommandLineOption(string upperCamelCaseOption, bool? expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {upperCamelCaseOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.UpperCamelCase);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--enable-internal", true)]
        [InlineData("--enable-internal true", true)]
        [InlineData("--enable-internal false", false)]
        [InlineData("-i", true)]
        [InlineData("-i true", true)]
        [InlineData("-i false", false)]
        public void TestEnableInternalCommandLineOption(string enableInternalOption, bool? expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {enableInternalOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.EnableInternal);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--omit-versioning-info", true)]
        [InlineData("--omit-versioning-info true", true)]
        [InlineData("--omit-versioning-info false", false)]
        [InlineData("-vi", true)]
        [InlineData("-vi true", true)]
        [InlineData("-vi false", false)]
        public void TestOmitVersioningInfoCommandLineOption(string omitVersioningInfoOption, bool? expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {omitVersioningInfoOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.OmitVersioningInfo);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--multiple-files", true)]
        [InlineData("--multiple-files true", true)]
        [InlineData("--multiple-files false", false)]
        public void TestMultipleFilesCommandLineOption(string multipleFilesOption, bool? expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {multipleFilesOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.MultipleFiles);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--excluded-operation-imports BackupDataSource,RepairDataSource", "BackupDataSource,RepairDataSource")]
        [InlineData("-eoi BackupDataSource,RepairDataSource", "BackupDataSource,RepairDataSource")]
        public void TestExcludedOperationImportsCommandLineOption(string excludedOperationImportsOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {excludedOperationImportsOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.ExcludedOperationImports);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--excluded-bound-operations RateCustomer(SampleServiceV4.Models.Customer),ProcessOrder(SampleServiceV4.Models.Order)", "RateCustomer(SampleServiceV4.Models.Customer),ProcessOrder(SampleServiceV4.Models.Order)")]
        [InlineData("-ebo RateCustomer(SampleServiceV4.Models.Customer),ProcessOrder(SampleServiceV4.Models.Order)", "RateCustomer(SampleServiceV4.Models.Customer),ProcessOrder(SampleServiceV4.Models.Order)")]
        public void TestExcludedBoundOperationsCommandLineOption(string excludedOperationImportsOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {excludedOperationImportsOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.ExcludedBoundOperations);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--excluded-schema-types SampleServiceV4.Models.Address,SampleServiceV4.Models.City", "SampleServiceV4.Models.Address,SampleServiceV4.Models.City")]
        [InlineData("-est SampleServiceV4.Models.Address,SampleServiceV4.Models.City", "SampleServiceV4.Models.Address,SampleServiceV4.Models.City")]
        public void TestExcludedSchemaTypesCommandLineOption(string excludedOperationImportsOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {excludedOperationImportsOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.ExcludedSchemaTypes);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("--ignore-unexpected-elements", true)]
        [InlineData("--ignore-unexpected-elements true", true)]
        [InlineData("--ignore-unexpected-elements false", false)]
        [InlineData("-iue", true)]
        [InlineData("-iue true", true)]
        [InlineData("-iue false", false)]
        public void TestIgnoreUnexpectedElementsCommandLineOption(string ignoreUnexpectedElementsOption, bool? expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {ignoreUnexpectedElementsOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.IgnoreUnexpectedElements);
            });

            parseResult.Invoke();
        }

        [Fact]
        public void TestBothMetadataUriAndConfigFileCommandLineOptionsSpecified()
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} --config-file SampleServiceV4ConnectedService.json");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(this.metadataUri, generateOptions.MetadataUri);
                Assert.Equal("SampleServiceV4ConnectedService.json", generateOptions.ConfigFile);
            });

            parseResult.Invoke();
        }

        [Theory]
        [InlineData("", null)]
        [InlineData($"--service-name SampleServiceV4", "SampleServiceV4")]
        [InlineData("-s SampleServiceV4", "SampleServiceV4")]
        public void TestServiceNameCommandLineOption(string serviceNameOption, string expectedArgumentValue)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {serviceNameOption}");
            this.generateCommand.Handler = CommandHandler.Create((GenerateOptions generateOptions, IConsole _) =>
            {
                Assert.Equal(expectedArgumentValue, generateOptions.ServiceName);
            });

            parseResult.Invoke();
        }
    }
}
