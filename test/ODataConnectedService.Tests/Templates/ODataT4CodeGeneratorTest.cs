//---------------------------------------------------------------------------------
// <copyright file="ODataT4CodeGeneratorTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Templates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataConnectedService.Tests.TestHelpers;

namespace Microsoft.OData.ConnectedService.Tests.Templates
{
    [TestClass]
    public class ODataT4CodeGeneratorTest
    {
        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumsFunctionsAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumsFunctionsDSCAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsDSC.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true
            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumFunctionsWithInternalTypesAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsWithInternalTypes.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                MakeTypesInternal = true

            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }


        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumFunctionsWithNoVersioningInfoAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                OmitVersioningInfo = true,
            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCodeOmitVersioningInfo(expected, output);
        }

        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumFunctionsDSCWithInternalTypesAsync()
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
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumFunctionsDSCWithNoVersioningInfoAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsDSC.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true,
                OmitVersioningInfo = true,
            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCodeOmitVersioningInfo(expected, output);
        }

        [TestMethod]
        public async Task TestEntitiesComplexTypesEnumFunctionsDSCWithInternalTypesWithNoVersioningInfoAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("EntitiesEnumsFunctionsDSCWithInternalTypes.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp,
                UseDataServiceCollection = true,
                MakeTypesInternal = true,
                OmitVersioningInfo = true,
            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCodeOmitVersioningInfo(expected, output);
        }

        [TestMethod]
        public async Task TestTypeDefinitionsParamsConvertedToUnderlyingTypeAsync()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("TypeDefinitions.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent("TypeDefinitionsParamsConvertedToUnderlyingType.cs");
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = ODataT4CodeGenerator.LanguageOption.CSharp
            };
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        public async Task TestExcludedOperationImportsNotIncludeInGeneratedCodeAsync()
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
            var output = await generator.TransformTextAsync().ConfigureAwait(false);
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
        }

        [TestMethod]
        [DataRow(ODataT4CodeGenerator.LanguageOption.CSharp, "EntityPropertiesWithDefaultValues.cs")]
        [DataRow(ODataT4CodeGenerator.LanguageOption.VB, "EntityPropertiesWithDefaultValues.vb")]
        public async Task TestPropertyInitializersGeneratedForDefaultValuesAsync(ODataT4CodeGenerator.LanguageOption lang, string expectedCodeFile)
        {
            // Arrange
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("EntityPropertiesWithDefaultValues.xml");
            string expected = GeneratedCodeHelpers.LoadReferenceContent(expectedCodeFile);
            var generator = new ODataT4CodeGenerator()
            {
                Edmx = edmx,
                TargetLanguage = lang
            };

            // Act
            string output = await generator.TransformTextAsync().ConfigureAwait(false);

            // Assert
            GeneratedCodeHelpers.VerifyGeneratedCode(expected, output);
            GeneratedCodeHelpers.VerifyGeneratedCodeCompiles(output, lang);
        }
    }
}
