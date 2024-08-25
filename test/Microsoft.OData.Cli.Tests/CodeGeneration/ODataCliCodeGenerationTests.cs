//-----------------------------------------------------------------------------------
// <copyright file="ODataCliCodeGenerationTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;
using Microsoft.OData.CodeGen.Common;

namespace Microsoft.OData.Cli.Tests.CodeGeneration
{
    public class ODataCliCodeGenerationTests : IDisposable
    {
        private readonly GenerateCommand generateCommand;
        private readonly string metadataUri = Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4.xml");
        private readonly string lowerCamelCaseMetadataUri = Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4LowerCamelCase.xml");
        private readonly string outputDir;

        public ODataCliCodeGenerationTests()
        {
            this.generateCommand = new GenerateCommand();
            this.outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

        [Fact]
        public void TestCodeGeneratedForRequiredOptions()
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4Proxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForNamespacePrefixOptionTestData()
        {
            return
                [
                    [
                        $"--namespace-prefix Client"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4NamespacePrefixConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForNamespacePrefixOptionTestData))]
        public void TestCodeGeneratedForNamespacePrefixOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4NamespacePrefixProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForEnableTrackingOptionTestData()
        {
            return
                [
                    [
                        $"--enable-tracking"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4EnableTrackingConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForEnableTrackingOptionTestData))]
        public void TestCodeGeneratedForEnableTrackingOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4EnableTrackingProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForUpperCamelCaseOptionTestData()
        {
            return
                [
                    [
                        $"--upper-camel-case"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4UpperCamelCaseConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForUpperCamelCaseOptionTestData))]
        public void TestCodeGeneratedForUpperCamelCaseOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.lowerCamelCaseMetadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4UpperCamelCaseProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForEnableInternalOptionTestData()
        {
            return
                [
                    [
                        $"--enable-internal"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4EnableInternalConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForEnableInternalOptionTestData))]
        public void TestCodeGeneratedForEnableInternalOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4EnableInternalProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForOmitVersioningInfoOptionTestData()
        {
            return
                [
                    [
                        $"--omit-versioning-info"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4OmitVersioningInfoConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForOmitVersioningInfoOptionTestData))]
        public void TestCodeGeneratedForOmitVersioningInfoOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4Proxy.cs");

            CodeVerificationHelper.VerifyGeneratedCodeOmitVersioningInfo(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForMultipleFilesOptionTestData()
        {
            return
                [
                    [
                        $"--multiple-files"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4MultipleFilesConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForMultipleFilesOptionTestData))]
        public void TestCodeGeneratedForMultipleFilesOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var defaultExtensionMethodsProxyFile = Assert.Single(Directory.GetFiles(outputDir, "SampleServiceV4.Default.ExtensionMethods.cs"));
            var modelsExtensionMethodsProxyFile = Assert.Single(Directory.GetFiles(outputDir, "SampleServiceV4.Models.ExtensionMethods.cs"));
            var customerProxyFile = Assert.Single(Directory.GetFiles(outputDir, "Customer.cs"));
            var orderProxyFile = Assert.Single(Directory.GetFiles(outputDir, "Order.cs"));
            var addressProxyFile = Assert.Single(Directory.GetFiles(outputDir, "Address.cs"));
            var cityProxyFile = Assert.Single(Directory.GetFiles(outputDir, "City.cs"));

            var defaultExtensionMethodsProxyGeneratedCode = File.ReadAllText(defaultExtensionMethodsProxyFile);
            var defaultExtensionMethodsProxyExpectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleFilesProxy.Default.ExtensionMethods.cs");
            CodeVerificationHelper.VerifyGeneratedCode(defaultExtensionMethodsProxyExpectedCode, defaultExtensionMethodsProxyGeneratedCode);

            var modelsExtensionMethodsProxyGeneratedCode = File.ReadAllText(modelsExtensionMethodsProxyFile);
            var modelsExtensionMethodsProxyExpectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleFilesProxy.Models.ExtensionMethods.cs");
            CodeVerificationHelper.VerifyGeneratedCode(modelsExtensionMethodsProxyExpectedCode, modelsExtensionMethodsProxyGeneratedCode);

            var customerProxyGeneratedCode = File.ReadAllText(customerProxyFile);
            var customerProxyExpectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleFilesProxy.Customer.cs");
            CodeVerificationHelper.VerifyGeneratedCode(customerProxyExpectedCode, customerProxyGeneratedCode);

            var orderProxyGeneratedCode = File.ReadAllText(orderProxyFile);
            var orderProxyExpectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleFilesProxy.Order.cs");
            CodeVerificationHelper.VerifyGeneratedCode(orderProxyExpectedCode, orderProxyGeneratedCode);

            var addressProxyGeneratedCode = File.ReadAllText(addressProxyFile);
            var addressProxyExpectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleFilesProxy.Address.cs");
            CodeVerificationHelper.VerifyGeneratedCode(addressProxyExpectedCode, addressProxyGeneratedCode);

            var cityProxyGeneratedCode = File.ReadAllText(cityProxyFile);
            var cityProxyExpectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleFilesProxy.City.cs");
            CodeVerificationHelper.VerifyGeneratedCode(cityProxyExpectedCode, cityProxyGeneratedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForExcludedOperationImportsOptionTestData()
        {
            return
                [
                    [
                        $"--excluded-operation-imports BackupDataSource,RepairDataSource"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ExcludedOperationImportsConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForExcludedOperationImportsOptionTestData))]
        public void TestCodeGeneratedForExcludedOperationImportsOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4ExcludedOperationImportsProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForExcludedBoundOperationsOptionTestData()
        {
            return
                [
                    [
                        $"--excluded-bound-operations RateCustomer(SampleServiceV4.Models.Customer),ProcessOrder(SampleServiceV4.Models.Order)"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ExcludedBoundOperationsConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForExcludedBoundOperationsOptionTestData))]
        public void TestCodeGeneratedForExcludedBoundOperationsOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4ExcludedBoundOperationsProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForExcludedSchemaTypesOptionTestData()
        {
            return
                [
                    [
                        $"--excluded-schema-types SampleServiceV4.Models.Address,SampleServiceV4.Models.City"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ExcludedSchemaTypesConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForExcludedSchemaTypesOptionTestData))]
        public void TestCodeGeneratedForExcludedSchemaTypesOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4ExcludedSchemaTypesProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }



        public static IEnumerable<object[]> GetCommandLineOptionTakesPrecedenceOverConfigFileTestData()
        {
            return
                [
                    [
                        $"--namespace-prefix \"\" --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4NamespacePrefixConnectedService.json")}"
                    ],
                    [
                        $"--enable-tracking false --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4EnableTrackingConnectedService.json")}"
                    ],
                    [
                        $"--upper-camel-case false --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4UpperCamelCaseConnectedService.json")}"
                    ],
                    [
                        $"--enable-internal false --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4EnableInternalConnectedService.json")}"
                    ],
                    [
                        $"--omit-versioning-info false --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4OmitVersioningInfoConnectedService.json")}"
                    ],
                    [
                        $"--multiple-files false --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4MultipleFilesConnectedService.json")}"
                    ],
                    [
                        $"--excluded-operation-imports \"\" --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ExcludedOperationImportsConnectedService.json")}"
                    ],
                    [
                        $"--excluded-bound-operations \"\" --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ExcludedBoundOperationsConnectedService.json")}"
                    ],
                    [
                        $"--excluded-schema-types \"\" --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ExcludedSchemaTypesConnectedService.json")}"
                    ],
                    [
                        $"--file-name \"Reference\" --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4FileNameConnectedService.json")}"
                    ],
                    [
                        $"--service-name \"{Constants.DefaultServiceName}\" --config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ServiceNameConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCommandLineOptionTakesPrecedenceOverConfigFileTestData))]
        public void TestCommandLineOptionTakesPrecedenceOverConfigFileOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4Proxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForMultipleOptionsTestData()
        {
            return
                [
                    [
                        $"--namespace-prefix \"Client\" " +
                        $"--enable-tracking " +
                        $"--enable-internal " +
                        $"--omit-versioning-info " +
                        $"--excluded-operation-imports BackupDataSource,RepairDataSource " +
                        $"--excluded-bound-operations RateCustomer(SampleServiceV4.Models.Customer),ProcessOrder(SampleServiceV4.Models.Order) " +
                        $"--excluded-schema-types SampleServiceV4.Models.Address,SampleServiceV4.Models.City"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4MultipleOptionsConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForMultipleOptionsTestData))]
        public void TestCodeGeneratedForMultipleOptions(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var referenceProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var generatedCode = File.ReadAllText(referenceProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4MultipleOptionsProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForFileNameOptionTestData()
        {
            return
                [
                    [
                        $"--file-name SampleServiceV4Proxy"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4FileNameConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForFileNameOptionTestData))]
        public void TestCodeGeneratedForFileNameOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var sampleServiceV4ProxyFile = Assert.Single(Directory.GetFiles(outputDir, "SampleServiceV4Proxy.cs"));

            var generatedCode = File.ReadAllText(sampleServiceV4ProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4Proxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
        }

        public static IEnumerable<object[]> GetCodeGeneratedForServiceNameOptionTestData()
        {
            return
                [
                    [
                        $"--service-name SampleServiceV4"
                    ],
                    [
                        $"--config-file {Path.Combine(Environment.CurrentDirectory, "CodeGeneration\\Artifacts\\SampleServiceV4ServiceNameConnectedService.json")}"
                    ]
                ];
        }

        [Theory]
        [MemberData(nameof(GetCodeGeneratedForServiceNameOptionTestData))]
        public void TestCodeGeneratedForServiceNameOption(string commandLine)
        {
            var parseResult = this.generateCommand.Parse($"--metadata-uri {this.metadataUri} --outputdir {this.outputDir} {commandLine}");

            parseResult.Invoke();

            var outputCsdlFile = Assert.Single(Directory.GetFiles(outputDir, "SampleServiceV4Csdl.xml"));
            var sampleServiceV4ProxyFile = Assert.Single(Directory.GetFiles(outputDir, $"{Constants.DefaultReferenceFileName}.cs"));

            var inputCsdlContent = File.ReadAllText(metadataUri);
            var outputCsdlContent = File.ReadAllText(outputCsdlFile);

            var generatedCode = File.ReadAllText(sampleServiceV4ProxyFile);
            var expectedCode = CodeVerificationHelper.LoadReferenceContent("SampleServiceV4ServiceNameProxy.cs");

            CodeVerificationHelper.VerifyGeneratedCode(expectedCode, generatedCode);
            // Output CSDL file doesn't contain the XML declaration header
            Assert.EndsWith(
                Regex.Replace(outputCsdlContent, @"\s+", ""),
                Regex.Replace(inputCsdlContent, @"\s+", ""));
        }

        public void Dispose()
        {
            try
            {
                // Delete the temp directory if possible
                if (Directory.Exists(outputDir))
                {
                    Directory.Delete(outputDir, true);
                }
            }
            catch
            {
                // Ignore - Temporary files are eventually clean up
            }
        }
    }
}
