// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.ConnectedService.Templates;

namespace Microsoft.OData.ConnectedService.Tests.Templates
{
    [TestClass]
    public class ODataT4CodeGeneratorTest
    {

        private const string T4Version = "#VersionNumber#";
        [TestMethod]
        public void TestEntitiesComplexTypesEnumsFunctions()
        {
            string edmx = LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = LoadReferenceContent("EntitiesEnumsFunctions.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = generator.TransformText();
            VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestEntitiesComplexTypesEnumsFunctionsDSC()
        {
            string edmx = LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = LoadReferenceContent("EntitiesEnumsFunctionsDSC.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true
            };
            var output = generator.TransformText();
            VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestEntitiesComplexTypesEnumFunctionsWithInternalTypes()
        {
            string edmx = LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = LoadReferenceContent("EntitiesEnumsFunctionsWithInternalTypes.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                MakeTypesInternal = true

            };
            var output = generator.TransformText();
            VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestEntitiesComplexTypesEnumFunctionsDSCWithInternalTypes()
        {
            string edmx = LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = LoadReferenceContent("EntitiesEnumsFunctionsDSCWithInternalTypes.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true,
                MakeTypesInternal = true

            };
            var output = generator.TransformText();
            VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestTypeDefinitionsParamsConvertedToUnderlyingType()
        {
            string edmx = LoadReferenceContent("TypeDefinitions.xml");
            string expected = LoadReferenceContent("TypeDefinitionsParamsConvertedToUnderlyingType.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = generator.TransformText();
            VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestExcludedOperationImportsNotIncludeInGeneratedCode()
        {
            string edmx = LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = LoadReferenceContent("EntitiesEnumsFunctionsDSCExcludeOperationImports.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                UseDataServiceCollection = true,
                ExcludedOperationImports = new string[] { "GetPersonWithMostFriends", "ResetDataSource" },
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = generator.TransformText();
            VerifyGeneratedCode(expected, output);
        }


        static Assembly Assembly = Assembly.GetExecutingAssembly();
        const string ReferenceResourcePrefix = "ODataConnectedService.Tests.CodeGenReferences.";

        static string LoadReferenceContent(string name)
        {
            var fullName = $"{ReferenceResourcePrefix}{name}";
            using (var stream = Assembly.GetManifestResourceStream(fullName))
            {
                if (stream == null)
                {
                    throw new Exception($"Embedded resource '{name}' not found.");
                }
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        static void VerifyGeneratedCode(string expectedCode, string actualCode)
        {
            var normalizedExpected = NormalizeGeneratedCode(expectedCode);
            var normalizedActual = NormalizeGeneratedCode(actualCode);
            Assert.AreEqual(normalizedExpected, normalizedActual);
        }

        public static string NormalizeGeneratedCode(string code)
        {
            var normalized = Regex.Replace(code, "// Generation date:.*", string.Empty, RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "'Generation date:.*", string.Empty, RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "//     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "'     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalized = Regex.Replace(normalized,
               "global::System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
               "global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
               RegexOptions.Multiline);
            //Remove the spaces from the string to avoid indentation change errors
            normalized = Regex.Replace(normalized, @"\s+", "");
            return normalized;
        }
    }
}
