//---------------------------------------------------------------------------------
// <copyright file="ODataT4CodeGeneratorUnitTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.OData.CodeGen.Templates;
using System.Collections.Generic;

namespace ODataConnectedService.Tests
{

    [TestClass]
    public class ODataT4CodeGeneratorUnitTest
    {
        private ODataT4CodeGenerator codeGenerator;
        private const string MetadataUri = "http://services.odata.org/Experimental/OData/OData.svc/$metadata";

        [TestInitialize]
        public void Init()
        {
            codeGenerator = new ODataT4CodeGenerator();
        }

        [TestMethod]
        public void ValidateAndSetUseDataServiceCollectionFromStringShouldSetUseDataServiceCollectionToTrue()
        {
            codeGenerator.ValidateAndSetUseDataServiceCollectionFromString("true");
            codeGenerator.UseDataServiceCollection.Should().BeTrue();
        }

        [TestMethod]
        public void ValidateAndSetUseDataServiceCollectionFromStringShouldSetUseDataServiceCollectionToFalse()
        {
            codeGenerator.ValidateAndSetUseDataServiceCollectionFromString("FALSE");
            codeGenerator.UseDataServiceCollection.Should().BeFalse();
        }

        [TestMethod]
        public void ValidateAndSetUseDataServiceCollectionFromStringShouldThrowException()
        {
            Action setUseDataServiceCollection = () => codeGenerator.ValidateAndSetUseDataServiceCollectionFromString("NotTrue");
            setUseDataServiceCollection.ShouldThrow<ArgumentException>().WithMessage("The value \"NotTrue\" cannot be assigned to the UseDataServiceCollection parameter because it is not a valid boolean value.");
        }

        [TestMethod]
        public void ValidateAndSetExcludedOperationImportsFromStringShouldReturnListOfstring()
        {
            codeGenerator.ValidateAndSetExcludedOperationImportsFromString("Func1, Func2,      Func3, ,Func4  ");
            codeGenerator.ExcludedOperationImports.ShouldBeEquivalentTo(new List<string>() { "Func1", "Func2", "Func3", "Func4" });
        }

        [TestMethod]
        public void ValidateAndSetExcludedBoundOperationsFromStringShouldReturnListOfstring()
        {
            codeGenerator.ValidateAndSetExcludedBoundOperationsFromString("Func1(Type1), Func2(Type2),      Func3(Type3), ,Func4(Type4)  ");
            codeGenerator.ExcludedBoundOperations.ShouldBeEquivalentTo(new List<string>() { "Func1(Type1)", "Func2(Type2)", "Func3(Type3)", "Func4(Type4)" });
        }

        [TestMethod]
        public void ValidateAndSetTargetLanguageFromStringShouldSetTargetLanguageToCSharp()
        {
            codeGenerator.ValidateAndSetTargetLanguageFromString("CSharp");
            codeGenerator.TargetLanguage.Should().Be(ODataT4CodeGenerator.LanguageOption.CSharp);
        }

        [TestMethod]
        public void ValidateAndSetTargetLanguageFromStringShouldSetTargetLanguageToCSharpIgnoringCase()
        {
            codeGenerator.ValidateAndSetTargetLanguageFromString("csharp");
            codeGenerator.TargetLanguage.Should().Be(ODataT4CodeGenerator.LanguageOption.CSharp);
        }

        [TestMethod]
        public void ValidateAndSetTargetLanguageFromStringShouldSetTargetLanguageToVB()
        {
            codeGenerator.ValidateAndSetTargetLanguageFromString("VB");
            codeGenerator.TargetLanguage.Should().Be(ODataT4CodeGenerator.LanguageOption.VB);
        }

        [TestMethod]
        public void ValidateAndSetTargetLanguageFromStringShouldSetTargetLanguageToVBIgnoringCase()
        {
            codeGenerator.ValidateAndSetTargetLanguageFromString("vb");
            codeGenerator.TargetLanguage.Should().Be(ODataT4CodeGenerator.LanguageOption.VB);
        }

        [TestMethod]
        public void ValidateAndSetTargetLanguageFromStringShouldThrowException()
        {
            Action setTargetLanguage = () => codeGenerator.ValidateAndSetTargetLanguageFromString("cplusplus");
            setTargetLanguage.ShouldThrow<ArgumentException>().WithMessage("The value \"cplusplus\" cannot be assigned to the TargetLanguage parameter because it is not a valid LanguageOption. The supported LanguageOptions are \"CSharp\" and \"VB\".");
        }

        [TestMethod]
        public void GetMetadataDocumentUriShouldAddSlashAndMetadataSuffix()
        {
            codeGenerator.MetadataDocumentUri = "http://services.odata.org/Experimental/OData/OData.svc";
            codeGenerator.MetadataDocumentUri.Should().Be(MetadataUri);
        }

        [TestMethod]
        public void GetMetadataDocumentUriShouldAddMetadataSuffix()
        {
            codeGenerator.MetadataDocumentUri = "http://services.odata.org/Experimental/OData/OData.svc/";
            codeGenerator.MetadataDocumentUri.Should().Be(MetadataUri);
        }

        [TestMethod]
        public void GetMetadataDocumentUriShouldNotAddMetadataSuffix()
        {
            codeGenerator.MetadataDocumentUri = "http://services.odata.org/Experimental/OData/OData.svc/$metadata";
            codeGenerator.MetadataDocumentUri.Should().Be(MetadataUri);
        }

        [TestMethod]
        public void GetMetadataDocumentUriWithSlashShouldNotAddMetadataSuffix()
        {
            codeGenerator.MetadataDocumentUri = "http://services.odata.org/Experimental/OData/OData.svc/$metadata/";
            codeGenerator.MetadataDocumentUri.Should().Be(MetadataUri);
        }

        [TestMethod]
        public void GetMetadataDocumentUriWithSlashShouldIgnoreQueries()
        {
            codeGenerator.MetadataDocumentUri = "http://services.odata.org/Experimental/OData/OData.svc/$metadata/?query=0";
            codeGenerator.MetadataDocumentUri.Should().Be(MetadataUri);
        }

        [TestMethod]
        public void GetMetadataDocumentUriWithLocalhostShouldContainPortNumber()
        {
            codeGenerator.MetadataDocumentUri = "http://localhost:8080/Aruba.svc/?query=0";
            codeGenerator.MetadataDocumentUri.Should().Be("http://localhost:8080/Aruba.svc/$metadata?query=0");
        }

        [TestMethod]
        public void GetMetadataDocumentUriShouldNotAddMetadataSuffixForFilePath()
        {
            codeGenerator.MetadataDocumentUri = "file:///C://Odata//edmx";
            codeGenerator.MetadataDocumentUri.Should().Be("file:///C://Odata//edmx");
        }
    }
}
