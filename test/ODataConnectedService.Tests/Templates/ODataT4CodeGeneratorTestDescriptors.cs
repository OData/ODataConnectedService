//---------------------------------------------------------------------------------
// <copyright file="ODataT4CodeGeneratorTestDescriptors.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using System.Net;

namespace ODataConnectedService.Tests
{
    public static partial class ODataT4CodeGeneratorTestDescriptors
    {
        private const string ExpectedCSharpUseDSC = "ExpectedCSharpUseDSC";
        private const string ExpectedCSharp = "ExpectedCSharp";
        private const string ExpectedVBUseDSC = "ExpectedVBUseDSC";
        private const string ExpectedVB = "ExpectedVB";
        private const string T4Version = "#VersionNumber#";

        internal class ODataT4CodeGeneratorTestsDescriptor
        {
            /// <summary>
            /// Edmx Metadata to generate code from.
            /// </summary>
            public string Metadata { get; set; }

            /// <summary>
            /// Gets or Sets the func for getting the referenced model's stream.
            /// </summary>
            public Func<Uri, WebProxy, IList<string>, XmlReader> GetReferencedModelReaderFunc { get; set; }

            /// <summary>
            /// Dictionary of expected CSharp/VB code generation results.
            /// </summary>
            public Dictionary<string, string> ExpectedResults { get; set; }

            /// <summary>
            /// A custom verification action to perform. Takes in the generated code and runs asserts that the code was generated properly. A verification function provided here should be valid for both CodeGen using the Design DLL and T4.
            /// </summary>
            public Action<string, bool, bool> Verify { get; set; }
        }

        internal static void ValidateXMLFile(string tempFilePath)
        {
            var doc = new XmlDocument { XmlResolver = null };
            using (var reader = XmlReader.Create(tempFilePath, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit }))
            {
                doc.Load(reader);
            }
        }

        internal static void ValidateEdmx(string tempFilePath)
        {
            var edmx = File.ReadAllText(tempFilePath);
            using (var stringReader = new StringReader(edmx))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    IEdmModel edmModel = null;
                    IEnumerable<EdmError> edmErrors = null;
                    CsdlReader.TryParse(xmlReader, out edmModel, out edmErrors).Should().BeTrue();
                    edmErrors.Should().BeEmpty();
                };
            };
        }

        private static void VerifyGeneratedCode(string actualCode, Dictionary<string, string> expectedCode, bool isCSharp, bool useDSC, string key = null)
        {
            string expected;
            if (isCSharp && useDSC)
            {
                expected = expectedCode[ExpectedCSharpUseDSC];
            }
            else if (isCSharp && !useDSC)
            {
                expected = expectedCode[ExpectedCSharp];
            }
            else if (!isCSharp && useDSC)
            {
                expected = expectedCode[ExpectedVBUseDSC];
            }
            else
            {
                expected = expectedCode[ExpectedVB];
            }

            string actualBak = actualCode;
            var normalizedExpectedCode = Regex.Replace(expected, "// Generation date:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode, "'Generation date:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode, "//     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode, "'     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode,
                "global::System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
                "global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
                RegexOptions.Multiline);
            normalizedExpectedCode = Regex.Replace(normalizedExpectedCode,
                "Global.System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
                "Global.System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
                RegexOptions.Multiline);

            //Remove the spaces from the string to avoid indentation change errors
            var rawExpectedCode = Regex.Replace(normalizedExpectedCode, @"\s+", "");
            actualCode = Regex.Replace(actualCode, "// Generation date:.*", string.Empty);
            actualCode = Regex.Replace(actualCode, "'Generation date:.*", string.Empty);
            actualCode = Regex.Replace(actualCode, "//     Runtime Version:.*", string.Empty);
            actualCode = Regex.Replace(actualCode, "'     Runtime Version:.*", string.Empty);
            //Remove the spaces from the string to avoid indentation change errors
            var rawActualCode = Regex.Replace(actualCode, @"\s+", "");

            if (key == null)
            {
                rawActualCode.Should().Be(rawExpectedCode);
            }
            else
            {
                var equal = rawExpectedCode == rawActualCode;

                if (!equal)
                {
                    var filename = key + (useDSC ? "DSC" : "") + (isCSharp ? ".cs" : ".vb");
                    var currentFolder = Directory.GetCurrentDirectory();
                    var path = Path.Combine(currentFolder, filename);
                    File.WriteAllText(path, actualBak);
                    var basePath = Path.Combine(currentFolder, "Expected" + filename);
                    File.WriteAllText(basePath, expected);
                    equal.Should().Be(true, "Baseline not equal.\n " +
                        "To diff run: \n" +
                        "odd \"{0}\" \"{1}\"\n" +
                        "To update run: \n" +
                        "copy /y \"{1}\" \"{0}\"\n" +
                        "\n", basePath, path);
                }
            }
        }

        internal static string GetFileContent(string fileName)
        {
            return LoadContentFromBaseline(fileName);
        }

        internal static string GetAbsoluteUriOfFile(string fileName)
        {
            var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            var filePath = Path.Combine(outPutDirectory, $"CodeGenReferences\\{fileName}");
            return filePath;
        }

        private const string BaseName = "ODataConnectedService.Tests.CodeGenReferences.";
        private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

        private static string LoadContentFromBaseline(string key)
        {
            Stream stream = null;
            try
            {
                stream = Assembly.GetManifestResourceStream(BaseName + key);
                if (stream == null)
                {
                    throw new ApplicationException("Baseline [" + key + "] not found.");
                }

                using (var sr = new StreamReader(stream))
                {
                    stream = null;
                    return sr.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }
        }

        #region EntityBooleanPropertyWithDefaultValue
        private static string EdmxEntityBooleanPropertyWithDefaultValue = LoadContentFromBaseline("EntityBooleanPropertyWithDefaultValue.xml");
        private static string EntityBooleanPropertyWithDefaultValueCSharp = LoadContentFromBaseline("EntityBooleanPropertyWithDefaultValue.cs");
        private static string EntityBooleanPropertyWithDefaultValueCSharpUseDSC = LoadContentFromBaseline("EntityBooleanPropertyWithDefaultValueDSC.cs");
        private static string EntityBooleanPropertyWithDefaultValueVB = LoadContentFromBaseline("EntityBooleanPropertyWithDefaultValue.vb");
        private static string EntityBooleanPropertyWithDefaultValueVBUseDSC = LoadContentFromBaseline("EntityBooleanPropertyWithDefaultValueDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor EntityBooleanPropertyWithDefaultValue = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEntityBooleanPropertyWithDefaultValue,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, EntityBooleanPropertyWithDefaultValueCSharp },
                { ExpectedCSharpUseDSC, EntityBooleanPropertyWithDefaultValueCSharpUseDSC },
                { ExpectedVB, EntityBooleanPropertyWithDefaultValueVB },
                { ExpectedVBUseDSC, EntityBooleanPropertyWithDefaultValueVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EntityBooleanPropertyWithDefaultValue.ExpectedResults, isCSharp, useDSC, "EntityBooleanPropertyWithDefaultValue"),
        };
        #endregion

        #region EntityHierarchyWithIDAndId

        private static string EdmxEntityHierarchyWithIDAndId = LoadContentFromBaseline("EntityHierarchyWithIDAndId.xml");
        private static string EntityHierarchyWithIDAndIdCSharpUseDSC = LoadContentFromBaseline("EntityHierarchyWithIDAndIdDSC.cs");
        private static string EntityHierarchyWithIDAndIdVBUseDSC = LoadContentFromBaseline("EntityHierarchyWithIDAndIdDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor EntityHierarchyWithIDAndId = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEntityHierarchyWithIDAndId,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharpUseDSC, EntityHierarchyWithIDAndIdCSharpUseDSC }, { ExpectedVBUseDSC, EntityHierarchyWithIDAndIdVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EntityHierarchyWithIDAndId.ExpectedResults, isCSharp, useDSC, "EntityHierarchyWithIDAndId"),
        };

        #endregion

        #region Simple

        private static string EdmxSimple = LoadContentFromBaseline("Simple.xml");
        private static string SimpleCSharp = LoadContentFromBaseline("Simple.cs");
        private static string SimpleCSharpUseDSC = LoadContentFromBaseline("SimpleDSC.cs");
        private static string SimpleVB = LoadContentFromBaseline("Simple.vb");
        private static string SimpleVBUseDSC = LoadContentFromBaseline("SimpleDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor Simple = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxSimple,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharp, SimpleCSharp }, { ExpectedCSharpUseDSC, SimpleCSharpUseDSC }, { ExpectedVB, SimpleVB }, { ExpectedVBUseDSC, SimpleVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, Simple.ExpectedResults, isCSharp, useDSC, "Simple"),
        };
        #endregion

        #region MaxLength

        private static string EdmxMaxLength = LoadContentFromBaseline("MaxLength.xml");
        private static string MaxLengthCSharp = LoadContentFromBaseline("MaxLength.cs");
        private static string MaxLengthCSharpUseDSC = LoadContentFromBaseline("MaxLengthDSC.cs");
        private static string MaxLengthVB = LoadContentFromBaseline("MaxLength.vb");
        private static string MaxLengthVBUseDSC = LoadContentFromBaseline("MaxLengthDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor MaxLength = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxMaxLength,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharp, MaxLengthCSharp }, { ExpectedCSharpUseDSC, MaxLengthCSharpUseDSC }, { ExpectedVB, MaxLengthVB }, { ExpectedVBUseDSC, MaxLengthVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, MaxLength.ExpectedResults, isCSharp, useDSC, "MaxLength"),
        };

        #endregion

        #region SimpleMultipleFiles
        private static string EdmxSimpleMultipleFiles = LoadContentFromBaseline("SimpleMultipleFiles.xml");
        private static string SimpleMultipleFilesCSharp = LoadContentFromBaseline("SimpleMultipleFilesMain.cs");
        internal static ODataT4CodeGeneratorTestsDescriptor SimpleMultipleFiles = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxSimpleMultipleFiles,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharp, SimpleMultipleFilesCSharp }, { ExpectedCSharpUseDSC, SimpleCSharpUseDSC }, { ExpectedVB, SimpleVB }, { ExpectedVBUseDSC, SimpleVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, SimpleMultipleFiles.ExpectedResults, isCSharp, useDSC, "SimpleMultipleFiles"),
        };
        #endregion

        #region SameNamedEntityMultipleFiles
        private static string EdmxSameNamedEntityMultipleFiles = LoadContentFromBaseline("SameNamedEntityMultipleFiles.xml");
        private static string SameNamedEntityMultipleFilesNamespace1CSharp = LoadContentFromBaseline("SameNamedEntityMultipleFilesNamespace1Main.cs");
        internal static ODataT4CodeGeneratorTestsDescriptor SameNamedEntityMultipleFiles = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxSameNamedEntityMultipleFiles,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharp, SameNamedEntityMultipleFilesNamespace1CSharp } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, SameNamedEntityMultipleFiles.ExpectedResults, isCSharp, useDSC, "SameNamedEntityMultipleFiles"),
        };
        #endregion

        #region NamespacePrefix

        private static string EdmxNamespacePrefixWithSingleNamespace = LoadContentFromBaseline("NamespacePrefixWithSingleNamespace.xml");
        private static string NamespacePrefixWithSingleNamespaceCSharp = LoadContentFromBaseline("NamespacePrefixWithSingleNamespace.cs");
        private static string NamespacePrefixWithSingleNamespaceVB = LoadContentFromBaseline("NamespacePrefixWithSingleNamespace.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixWithSingleNamespace = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixWithSingleNamespace,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, NamespacePrefixWithSingleNamespaceCSharp },
                { ExpectedVB, NamespacePrefixWithSingleNamespaceVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixWithSingleNamespace.ExpectedResults, isCSharp, useDSC, "NamespacePrefixWithSingleNamespace"),
        };

        private static string EdmxNamespacePrefixWithDoubleNamespaces = LoadContentFromBaseline("NamespacePrefixWithDoubleNamespaces.xml");
        private static string NamespacePrefixWithDoubleNamespacesCSharp = LoadContentFromBaseline("NamespacePrefixWithDoubleNamespaces.cs");
        private static string NamespacePrefixWithDoubleNamespacesVB = LoadContentFromBaseline("NamespacePrefixWithDoubleNamespaces.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixWithDoubleNamespaces = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixWithDoubleNamespaces,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, NamespacePrefixWithDoubleNamespacesCSharp },
                { ExpectedVB, NamespacePrefixWithDoubleNamespacesVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixWithDoubleNamespaces.ExpectedResults, isCSharp, useDSC, "NamespacePrefixWithDoubleNamespaces"),
        };

        private static string EdmxNamespacePrefixWithInheritence = LoadContentFromBaseline("NamespacePrefixWithInheritence.xml");
        private static string NamespacePrefixWithInheritenceCSharp = LoadContentFromBaseline("NamespacePrefixWithInheritence.cs");
        private static string NamespacePrefixWithInheritenceVB = LoadContentFromBaseline("NamespacePrefixWithInheritence.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixWithInheritence = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixWithInheritence,
            ExpectedResults = new Dictionary<string, string>()
        {
                { ExpectedCSharp, NamespacePrefixWithInheritenceCSharp },
                { ExpectedVB, NamespacePrefixWithInheritenceVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixWithInheritence.ExpectedResults, isCSharp, useDSC, "NamespacePrefixWithInheritence"),
        };

        #endregion

        #region NamespacePrefixRepeatWithSchemaNameSpace

        private static string EdmxNamespacePrefixRepeatWithSchemaNameSpace = LoadContentFromBaseline("NamespacePrefixRepeatWithSchemaNameSpace.xml");
        private static string NamespacePrefixRepeatWithSchemaNameSpaceCSharp = LoadContentFromBaseline("NamespacePrefixRepeatWithSchemaNameSpace.cs");

        internal static ODataT4CodeGeneratorTestsDescriptor NamespacePrefixRepeatWithSchemaNameSpace = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespacePrefixRepeatWithSchemaNameSpace,
            ExpectedResults = new Dictionary<string, string>()
            {
                {ExpectedCSharp, NamespacePrefixRepeatWithSchemaNameSpaceCSharp},
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespacePrefixRepeatWithSchemaNameSpace.ExpectedResults, isCSharp, useDSC, "NamespacePrefixRepeatWithSchemaNameSpace"),
        };

        #endregion

        #region KeywordsAsNames

        private static string EdmxKeywordsAsNames = LoadContentFromBaseline("KeywordsAsNames.xml");
        private static string KeywordsAsNamesCSharp = LoadContentFromBaseline("KeywordsAsNames.cs");
        private static string KeywordsAsNamesVB = LoadContentFromBaseline("KeywordsAsNames.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor KeywordsAsNames = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxKeywordsAsNames,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, KeywordsAsNamesCSharp },
                { ExpectedVB, KeywordsAsNamesVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, KeywordsAsNames.ExpectedResults, isCSharp, useDSC, "KeywordsAsNames"),
        };

        #endregion

        #region MergedFunctionalTest

        private static string EdmxMergedFunctionalTest = LoadContentFromBaseline("MergedFunctionalTest.xml");
        private static string MergedFunctionalTestCSharp = LoadContentFromBaseline("MergedFunctionalTest.cs");
        private static string MergedFunctionalTestVB = LoadContentFromBaseline("MergedFunctionalTest.vb");
        private static string MergedFunctionalTestCSharpUseDSC = LoadContentFromBaseline("MergedFunctionalTestDSC.cs");
        private static string MergedFunctionalTestVBUseDSC = LoadContentFromBaseline("MergedFunctionalTestDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor MergedFunctionalTest = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxMergedFunctionalTest,
            ExpectedResults = new Dictionary<string, string>()
            {
                {ExpectedCSharp, MergedFunctionalTestCSharp},
                {ExpectedCSharpUseDSC, MergedFunctionalTestCSharpUseDSC},
                {ExpectedVB, MergedFunctionalTestVB},
                {ExpectedVBUseDSC, MergedFunctionalTestVBUseDSC}
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, MergedFunctionalTest.ExpectedResults, isCSharp, useDSC, "MergedFunctionalTest"),
        };

        #endregion

        #region Multiplicity

        private static string EdmxMultiplicity = LoadContentFromBaseline("Multiplicity.xml");
        private static string MultiplicityCSharp = LoadContentFromBaseline("Multiplicity.cs");
        private static string MultiplicityVB = LoadContentFromBaseline("Multiplicity.vb");
        private static string MultiplicityCSharpUseDSC = LoadContentFromBaseline("MultiplicityDSC.cs");
        private static string MultiplicityVBUseDSC = LoadContentFromBaseline("MultiplicityDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor Multiplicity = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxMultiplicity,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, MultiplicityCSharp },
                { ExpectedCSharpUseDSC, MultiplicityCSharpUseDSC },
                { ExpectedVB, MultiplicityVB },
                { ExpectedVBUseDSC, MultiplicityVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, Multiplicity.ExpectedResults, isCSharp, useDSC, "MultiplicityDSC"),
        };

        #endregion

        #region EmptySchema

        private static string EdmxEmptySchema = LoadContentFromBaseline("EmptySchema.xml");
        private static string EmptySchemaCSharp = LoadContentFromBaseline("EmptySchema.cs");
        private static string EmptySchemaVBUseDSC = LoadContentFromBaseline("EmptySchemaDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor EmptySchema = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEmptySchema,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, EmptySchemaCSharp },
                { ExpectedVBUseDSC, EmptySchemaVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EmptySchema.ExpectedResults, isCSharp, useDSC, "EmptySchema"),
        };

        #endregion

        #region NamespaceInKeywords

        private static string EdmxNamespaceInKeywords = LoadContentFromBaseline("NamespaceInKeywords.xml");
        private static string NamespaceInKeywordsCSharp = LoadContentFromBaseline("NamespaceInKeywords.cs");
        private static string NamespaceInKeywordsVB = LoadContentFromBaseline("NamespaceInKeywords.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor NamespaceInKeywords = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespaceInKeywords,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, NamespaceInKeywordsCSharp },
                { ExpectedVB, NamespaceInKeywordsVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespaceInKeywords.ExpectedResults, isCSharp, useDSC, "NamespaceInKeywords"),
        };

        #endregion

        #region NamespaceInKeywordsWithRefModel

        private static string EdmxNamespaceInKeywordsWithRefModel = LoadContentFromBaseline("NamespaceInKeywordsWithRefModel.xml");
        private static string EdmxNamespaceInKeywordsWithRefModelReferencedEdmx = LoadContentFromBaseline("NamespaceInKeywordsWithRefModelReferenced.xml");
        private static string NamespaceInKeywordsWithRefModelCSharp = LoadContentFromBaseline("NamespaceInKeywordsWithRefModel.cs");
        private static string NamespaceInKeywordsWithRefModelVB = LoadContentFromBaseline("NamespaceInKeywordsWithRefModel.vb");
        private static string NamespaceInKeywordsWithRefModelCSharpUseDSC = LoadContentFromBaseline("NamespaceInKeywordsWithRefModelDSC.cs");
        private static string NamespaceInKeywordsWithRefModelVBUseDSC = LoadContentFromBaseline("NamespaceInKeywordsWithRefModelDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor NamespaceInKeywordsWithRefModel = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxNamespaceInKeywordsWithRefModel,
            GetReferencedModelReaderFunc = (url,proxy,headers) => XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(EdmxNamespaceInKeywordsWithRefModelReferencedEdmx))),
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, NamespaceInKeywordsWithRefModelCSharp },
                { ExpectedVB, NamespaceInKeywordsWithRefModelVB },
                { ExpectedCSharpUseDSC, NamespaceInKeywordsWithRefModelCSharpUseDSC },
                { ExpectedVBUseDSC, NamespaceInKeywordsWithRefModelVBUseDSC },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, NamespaceInKeywordsWithRefModel.ExpectedResults, isCSharp, useDSC, "NamespaceInKeywordsWithRefModel"),
        };

        #endregion

        #region MultiReferenceModel

        private static string EdmxWithMultiReferenceModel = LoadContentFromBaseline("MultiReferenceModel.xml");
        private static string MultiReferenceModelCoreTermsEdmx = LoadContentFromBaseline("MultiReferenceModelCoreTerms.xml");
        private static string MultiReferenceModelDeviceModelTermsEdmx = LoadContentFromBaseline("MultiReferenceModelDeviceModelTerms.xml");
        private static string MultiReferenceModelGPSEdmx = LoadContentFromBaseline("MultiReferenceModelGPS.xml");
        private static string MultiReferenceModelLocationEdmx = LoadContentFromBaseline("MultiReferenceModelLocation.xml");
        private static string MultiReferenceModelMapEdmx = LoadContentFromBaseline("MultiReferenceModelMap.xml");

        private static string MultiReferenceModelCSharp = LoadContentFromBaseline("MultiReferenceModel.cs");
        private static string MultiReferenceModelVB = LoadContentFromBaseline("MultiReferenceModel.vb");
        private static string MultiReferenceModelCSharpUseDSC = LoadContentFromBaseline("MultiReferenceModelDSC.cs");
        private static string MultiReferenceModelVBUseDSC = LoadContentFromBaseline("MultiReferenceModelDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor MultiReferenceModel = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxWithMultiReferenceModel,
            GetReferencedModelReaderFunc = (url,proxy,headers) =>
            {
                string text;
                var urlStr = url.OriginalString;
                if (urlStr.EndsWith("CoreTerms.csdl", StringComparison.Ordinal))
                {
                    text = MultiReferenceModelCoreTermsEdmx;
                }
                else if (urlStr.EndsWith("DeviceModelTerms.csdl", StringComparison.Ordinal))
                {
                    text = MultiReferenceModelDeviceModelTermsEdmx;
                }
                else if (urlStr.EndsWith("GPS.csdl", StringComparison.Ordinal))
                {
                    text = MultiReferenceModelGPSEdmx;
                }
                else if (urlStr.EndsWith("Location.csdl", StringComparison.Ordinal))
                {
                    text = MultiReferenceModelLocationEdmx;
                }
                else // (urlStr.EndsWith("Map.csdl"))
                {
                    text = MultiReferenceModelMapEdmx;
                }

                var setting = new XmlReaderSettings()
                {
                    IgnoreWhitespace = true
                };

                return XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(text)), setting);
            },
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, MultiReferenceModelCSharp },
                { ExpectedVB, MultiReferenceModelVB },
                { ExpectedCSharpUseDSC, MultiReferenceModelCSharpUseDSC },
                { ExpectedVBUseDSC, MultiReferenceModelVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, MultiReferenceModel.ExpectedResults, isCSharp, useDSC, "MultiReferenceModel"),
        };

        #endregion

        #region MultiReferenceModelRelativeUri

        internal static string EdmxWithMultiReferenceModelRelativeUriFilePath = GetAbsoluteUriOfFile("MultiReferenceModelRelativeUri.xml");

        private static string MultiReferenceModelRelativeUriCSharp = LoadContentFromBaseline("MultiReferenceModelRelativeUri.cs");
        private static string MultiReferenceModelRelativeUriVB = LoadContentFromBaseline("MultiReferenceModelRelativeUri.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor MultiReferenceModelRelativeUri = new ODataT4CodeGeneratorTestsDescriptor()
        {
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, MultiReferenceModelRelativeUriCSharp },
                { ExpectedVB, MultiReferenceModelRelativeUriVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, MultiReferenceModelRelativeUri.ExpectedResults, isCSharp, useDSC, "MultiReferenceModelRelativeUri"),
        };

        #endregion

        #region UpperCamelCaseWithNamespacePrefix

        private static string EdmxUpperCamelCaseWithNamespacePrefix = LoadContentFromBaseline("UpperCamelCaseWithNamespacePrefix.xml");
        private static string UpperCamelCaseWithNamespacePrefixCSharp = LoadContentFromBaseline("UpperCamelCaseWithNamespacePrefix.cs");
        private static string UpperCamelCaseWithNamespacePrefixVB = LoadContentFromBaseline("UpperCamelCaseWithNamespacePrefix.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor UpperCamelCaseWithNamespacePrefix = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxUpperCamelCaseWithNamespacePrefix,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, UpperCamelCaseWithNamespacePrefixCSharp },
                { ExpectedVB, UpperCamelCaseWithNamespacePrefixVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, UpperCamelCaseWithNamespacePrefix.ExpectedResults, isCSharp, useDSC, "UpperCamelCaseWithNamespacePrefix"),
        };

        #endregion

        #region UpperCamelCaseWithoutNamespacePrefix

        private static string EdmxUpperCamelCaseWithoutNamespacePrefix = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefix.xml");
        private static string UpperCamelCaseWithoutNamespacePrefixCSharp = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefix.cs");
        private static string UpperCamelCaseWithoutNamespacePrefixVB = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefix.vb");
        private static string UpperCamelCaseWithoutNamespacePrefixCSharpUseDSC = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefixDSC.cs");
        private static string UpperCamelCaseWithoutNamespacePrefixVBUseDSC = LoadContentFromBaseline("UpperCamelCaseWithoutNamespacePrefixDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor UpperCamelCaseWithoutNamespacePrefix = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxUpperCamelCaseWithoutNamespacePrefix,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, UpperCamelCaseWithoutNamespacePrefixCSharp },
                { ExpectedVB, UpperCamelCaseWithoutNamespacePrefixVB },
                { ExpectedCSharpUseDSC, UpperCamelCaseWithoutNamespacePrefixCSharpUseDSC },
                { ExpectedVBUseDSC, UpperCamelCaseWithoutNamespacePrefixVBUseDSC }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, UpperCamelCaseWithoutNamespacePrefix.ExpectedResults, isCSharp, useDSC, "UpperCamelCaseWithoutNamespacePrefix"),
        };

        #endregion

        #region IgnoreUnexpectedElementsAndAttributes

        private static string EdmxUnexpectedElementsAndAttributes = LoadContentFromBaseline("UnexpectedElementsAndAttributes.xml");
        private static string UnexpectedElementsAndAttributesCSharp = LoadContentFromBaseline("UnexpectedElementsAndAttributes.cs");
        private static string UnexpectedElementsAndAttributesVB = LoadContentFromBaseline("UnexpectedElementsAndAttributes.vb");
        internal static ODataT4CodeGeneratorTestsDescriptor IgnoreUnexpectedElementsAndAttributes = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxUnexpectedElementsAndAttributes,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, UnexpectedElementsAndAttributesCSharp },
                { ExpectedVB, UnexpectedElementsAndAttributesVB }
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, IgnoreUnexpectedElementsAndAttributes.ExpectedResults, isCSharp, useDSC, "UnexpectedElementsAndAttributes"),
        };

        #endregion

        #region PrefixConflict
        private static string EdmxPrefixConflict = LoadContentFromBaseline("PrefixConflict.xml");
        private static string PrefixConflictCSharp = LoadContentFromBaseline("PrefixConflict.cs");
        private static string PrefixConflictVBUseDSC = LoadContentFromBaseline("PrefixConflictDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor PrefixConflict = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxPrefixConflict,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, PrefixConflictCSharp },
                { ExpectedVBUseDSC, PrefixConflictVBUseDSC },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, PrefixConflict.ExpectedResults, isCSharp, useDSC, "PrefixConflict"),
        };
        #endregion

        #region DupNames

        private static string EdmxDupNames = LoadContentFromBaseline("DupNames.xml");
        private static string DupNamesCSharp = LoadContentFromBaseline("DupNames.cs");
        private static string DupNamesVBUseDSC = LoadContentFromBaseline("DupNamesDSC.vb");
        private static string DupNamesWithCamelCaseCSharpUseDSC = LoadContentFromBaseline("DupNamesWithCamelCaseDSC.cs");
        private static string DupNamesWithCamelCaseVB = LoadContentFromBaseline("DupNamesWithCamelCase.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor DupNames = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxDupNames,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, DupNamesCSharp },
                { ExpectedVBUseDSC, DupNamesVBUseDSC },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, DupNames.ExpectedResults, isCSharp, useDSC, "DupNames"),
        };

        internal static ODataT4CodeGeneratorTestsDescriptor DupNamesWithCamelCase = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxDupNames,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharpUseDSC, DupNamesWithCamelCaseCSharpUseDSC },
                { ExpectedVB, DupNamesWithCamelCaseVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, DupNamesWithCamelCase.ExpectedResults, isCSharp, useDSC, "DupNamesWithCamelCase"),
        };
        #endregion

        #region OverrideOperations

        private static string EdmxOverrideOperations = LoadContentFromBaseline("OverrideOperations.xml");
        private static string OverrideOperationsCSharpUseDSC = LoadContentFromBaseline("OverrideOperationsDSC.cs");
        private static string OverrideOperationsVB = LoadContentFromBaseline("OverrideOperations.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor OverrideOperations = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxOverrideOperations,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharpUseDSC, OverrideOperationsCSharpUseDSC },
                { ExpectedVB, OverrideOperationsVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, OverrideOperations.ExpectedResults, isCSharp, useDSC, "OverrideOperations"),
        };
        #endregion

        #region AbstractEntityTypeWithoutKey
        private static string EdmxAbstractEntityTypeWithoutKey = LoadContentFromBaseline("AbstractEntityTypeWithoutKey.xml");
        private static string AbstractEntityTypeWithoutKeyCSharpUseDSC = LoadContentFromBaseline("AbstractEntityTypeWithoutKeyDSC.cs");
        private static string AbstractEntityTypeWithoutKeyVB = LoadContentFromBaseline("AbstractEntityTypeWithoutKey.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor AbstractEntityTypeWithoutKey = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxAbstractEntityTypeWithoutKey,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharpUseDSC, AbstractEntityTypeWithoutKeyCSharpUseDSC },
                { ExpectedVB, AbstractEntityTypeWithoutKeyVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, AbstractEntityTypeWithoutKey.ExpectedResults, isCSharp, useDSC, "AbstractEntityTypeWithoutKey"),
        };
        #endregion

        #region EntitiesEnumsFunctionsSelectTypes
        private static string EdmxEntitiesEnumsFunctionsSelectTypes = LoadContentFromBaseline("EntitiesEnumsFunctions.xml");
        private static string EntitiesEnumsFunctionsSelectTypesCSharp = LoadContentFromBaseline("EntitiesEnumsFunctionsSelectTypes.cs");
        private static string EntitiesEnumsFunctionsSelectTypesVB = LoadContentFromBaseline("EntitiesEnumsFunctionsSelectTypes.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor EntitiesEnumsFunctionsSelectTypes = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEntitiesEnumsFunctionsSelectTypes,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, EntitiesEnumsFunctionsSelectTypesCSharp },
                { ExpectedVB, EntitiesEnumsFunctionsSelectTypesVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EntitiesEnumsFunctionsSelectTypes.ExpectedResults, isCSharp, useDSC, "EntitiesEnumsFunctionsSelectTypes"),
        };
        #endregion

        #region SourceParameterOrKeysProperty

        private static string EdmxSourceParameterOrKeysProperty = LoadContentFromBaseline("SourceParameterOrKeysProperty.xml");
        private static string SourceParameterOrKeysPropertyCSharp = LoadContentFromBaseline("SourceParameterOrKeysProperty.cs");
        private static string SourceParameterOrKeysPropertyCSharpUseDSC = LoadContentFromBaseline("SourceParameterOrKeysPropertyDSC.cs");
        private static string SourceParameterOrKeysPropertyVB = LoadContentFromBaseline("SourceParameterOrKeysProperty.vb");
        private static string SourceParameterOrKeysPropertyVBUseDSC = LoadContentFromBaseline("SourceParameterOrKeysPropertyDSC.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor SourceParameterOrKeysProperty = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxSourceParameterOrKeysProperty,
            ExpectedResults = new Dictionary<string, string>() { { ExpectedCSharp, SourceParameterOrKeysPropertyCSharp }, { ExpectedCSharpUseDSC, SourceParameterOrKeysPropertyCSharpUseDSC }, { ExpectedVB, SourceParameterOrKeysPropertyVB }, { ExpectedVBUseDSC, SourceParameterOrKeysPropertyVBUseDSC } },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, SourceParameterOrKeysProperty.ExpectedResults, isCSharp, useDSC, nameof(SourceParameterOrKeysProperty)),
        };
        #endregion

        #region EntityTypeMarkedObsolete
        private static string EdmxEntityTypeMarkedObsolete = LoadContentFromBaseline("EntityTypeMarkedObsolete.xml");
        private static string EntityTypeMarkedObsoleteCSharp = LoadContentFromBaseline("EntityTypeMarkedObsolete.cs");
        private static string EntityTypeMarkedObsoleteVB = LoadContentFromBaseline("EntityTypeMarkedObsolete.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor EntityTypeMarkedObsolete = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEntityTypeMarkedObsolete,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, EntityTypeMarkedObsoleteCSharp },
                { ExpectedVB, EntityTypeMarkedObsoleteVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EntityTypeMarkedObsolete.ExpectedResults, isCSharp, useDSC, "EntityTypeMarkedObsolete"),
        };
        #endregion

        #region EntitySetMarkedObsolete
        private static string EdmxEntitySetMarkedObsolete = LoadContentFromBaseline("EntitySetMarkedObsolete.xml");
        private static string EntitySetMarkedObsoleteCSharp = LoadContentFromBaseline("EntitySetMarkedObsolete.cs");
        private static string EntitySetMarkedObsoleteVB = LoadContentFromBaseline("EntitySetMarkedObsolete.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor EntitySetMarkedObsolete = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxEntitySetMarkedObsolete,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, EntitySetMarkedObsoleteCSharp },
                { ExpectedVB, EntitySetMarkedObsoleteVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, EntitySetMarkedObsolete.ExpectedResults, isCSharp, useDSC, "EntitySetMarkedObsolete"),
        };
        #endregion

        #region BoundActionsAndFunctionsMarkedObsolete
        private static string EdmxBoundActionsAndFunctionsMarkedObsolete = LoadContentFromBaseline("BoundActionsAndFunctionsMarkedObsolete.xml");
        private static string BoundActionsAndFunctionsMarkedObsoleteCSharp = LoadContentFromBaseline("BoundActionsAndFunctionsMarkedObsolete.cs");
        private static string BoundActionsAndFunctionsMarkedObsoleteVB = LoadContentFromBaseline("BoundActionsAndFunctionsMarkedObsolete.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor BoundActionsAndFunctionsMarkedObsolete = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxBoundActionsAndFunctionsMarkedObsolete,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, BoundActionsAndFunctionsMarkedObsoleteCSharp },
                { ExpectedVB, BoundActionsAndFunctionsMarkedObsoleteVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, BoundActionsAndFunctionsMarkedObsolete.ExpectedResults, isCSharp, useDSC, "BoundActionsAndFunctionsMarkedObsolete"),
        };
        #endregion

        #region EntityAndNavPropertiesMarkedObsolete
        private static string EdmxPropertyAndNavPropertiesMarkedObsolete = LoadContentFromBaseline("PropertyAndNavPropertiesMarkedObsolete.xml");
        private static string PropertyAndNavPropertiesMarkedObsoleteCSharp = LoadContentFromBaseline("PropertyAndNavPropertiesMarkedObsolete.cs");
        private static string PropertyAndNavPropertiesMarkedObsoleteVB = LoadContentFromBaseline("PropertyAndNavPropertiesMarkedObsolete.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor PropertyAndNavPropertiesMarkedObsolete = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxPropertyAndNavPropertiesMarkedObsolete,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, PropertyAndNavPropertiesMarkedObsoleteCSharp },
                { ExpectedVB, PropertyAndNavPropertiesMarkedObsoleteVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, PropertyAndNavPropertiesMarkedObsolete.ExpectedResults, isCSharp, useDSC, "PropertyAndNavPropertiesMarkedObsolete"),
        };
        #endregion

        #region FunctionsAndActionImportsMarkedObsolete
        private static string EdmxFunctionsAndActionImportsMarkedObsolete = LoadContentFromBaseline("FunctionsAndActionImportsMarkedObsolete.xml");
        private static string FunctionsAndActionImportsMarkedObsoleteCSharp = LoadContentFromBaseline("FunctionsAndActionImportsMarkedObsolete.cs");
        private static string FunctionsAndActionImportsMarkedObsoleteVB = LoadContentFromBaseline("FunctionsAndActionImportsMarkedObsolete.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor FunctionsAndActionImportsMarkedObsolete = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxFunctionsAndActionImportsMarkedObsolete,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, FunctionsAndActionImportsMarkedObsoleteCSharp },
                { ExpectedVB, FunctionsAndActionImportsMarkedObsoleteVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, FunctionsAndActionImportsMarkedObsolete.ExpectedResults, isCSharp, useDSC, "FunctionsAndActionImportsMarkedObsolete"),
        };
        #endregion

        #region SingletonsMarkedObsolete
        private static string EdmxSingletonsMarkedObsolete = LoadContentFromBaseline("SingletonsMarkedObsolete.xml");
        private static string SingletonsMarkedObsoleteCSharp = LoadContentFromBaseline("SingletonsMarkedObsolete.cs");
        private static string SingletonsMarkedObsoleteVB = LoadContentFromBaseline("SingletonsMarkedObsolete.vb");

        internal static ODataT4CodeGeneratorTestsDescriptor SingletonsMarkedObsolete = new ODataT4CodeGeneratorTestsDescriptor()
        {
            Metadata = EdmxSingletonsMarkedObsolete,
            ExpectedResults = new Dictionary<string, string>()
            {
                { ExpectedCSharp, SingletonsMarkedObsoleteCSharp },
                { ExpectedVB, SingletonsMarkedObsoleteVB },
            },
            Verify = (code, isCSharp, useDSC) => VerifyGeneratedCode(code, SingletonsMarkedObsolete.ExpectedResults, isCSharp, useDSC, "SingletonsMarkedObsolete"),
        };
        #endregion

        #region RevisionsAnnotationMissingProperties
        internal static string EdmxRevisionsAnnotationMissingRevisionKind = LoadContentFromBaseline("RevisionsAnnotationMissingRevisionKind.xml");
        internal static string EdmxRevisionsAnnotationMissingDescription = LoadContentFromBaseline("RevisionsAnnotationMissingDescription.xml");
        #endregion
    }
}