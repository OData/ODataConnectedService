// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.ConnectedService.Templates;
using ODataConnectedService.Tests.TestHelpers;

namespace Microsoft.OData.ConnectedService.Tests.Templates
{
    [TestClass]
    public class ODataT4CodeGeneratorTest
    {

        
        [TestMethod]
        public void TestEntitiesComplexTypesEnumsFunctions()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = generator.TransformText();
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestEntitiesComplexTypesEnumsFunctionsDSC()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsDSC.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true
            };
            var output = generator.TransformText();
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestEntitiesComplexTypesEnumFunctionsWithInternalTypes()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsWithInternalTypes.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                MakeTypesInternal = true

            };
            var output = generator.TransformText();
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestEntitiesComplexTypesEnumFunctionsDSCWithInternalTypes()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsDSCWithInternalTypes.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true,
                MakeTypesInternal = true

            };
            var output = generator.TransformText();
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestTypeDefinitionsParamsConvertedToUnderlyingType()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("TypeDefinitions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("TypeDefinitionsParamsConvertedToUnderlyingType.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = generator.TransformText();
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public void TestExcludedOperationImportsNotIncludeInGeneratedCode()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsDSCExcludeOperationImports.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                UseDataServiceCollection = true,
                ExcludedOperationImports = new string[] { "GetPersonWithMostFriends", "ResetDataSource" },
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = generator.TransformText();
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }
    }
}
