//---------------------------------------------------------------------------------
// <copyright file="GeneratedCodeHelpers.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Data.Services.Client;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.CSharp;
using Microsoft.OData;
using Microsoft.OData.CodeGen.Templates;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ODataConnectedService.Tests.TestHelpers
{
    public static class GeneratedCodeHelpers
    {
        static Assembly Assembly = Assembly.GetExecutingAssembly();
        const string ReferenceResourcePrefix = "ODataConnectedService.Tests.CodeGenReferences.";
        private const string T4Version = "#VersionNumber#";

        public static string LoadReferenceContent(string name)
        {
            var fullName = $"{ReferenceResourcePrefix}{name}";
            using (var stream = Assembly.GetManifestResourceStream(fullName))
            {
                if (stream == null)
                {
                    throw new Exception($"Embedded resource '{name}' not found.");
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void VerifyGeneratedCode(string expectedCode, string actualCode)
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

        public static void VerifyGeneratedCodeNoTimestamp(string expectedCode, string actualCode)
        {
            var normalizedExpected = NormalizeGeneratedCode(expectedCode);
            var normalizedActual = NormalizeGeneratedCodeKeepGenerationDate(actualCode);
            Assert.AreEqual(normalizedExpected, normalizedActual);

            var normalizedActualWithoutTimestamp = NormalizeGeneratedCode(actualCode);
            Assert.AreEqual(normalizedActualWithoutTimestamp, normalizedActual);
        }

        public static string NormalizeGeneratedCodeKeepGenerationDate(string code)
        {
            var normalized = Regex.Replace(code, "//     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "'     Runtime Version:.*", string.Empty, RegexOptions.Multiline);
            normalized = Regex.Replace(normalized,
                "global::System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
                "global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
                RegexOptions.Multiline);

            //Remove the spaces from the string to avoid indentation change errors
            normalized = Regex.Replace(normalized, @"\s+", "");

            return normalized;
        }

        public static void VerifyGeneratedCodeCompiles(string source, bool isCSharp)
        {
            var results = CompileCode(source, isCSharp);
            results.Errors.Should().BeEmpty();
        }

        public static void VerifyGeneratedCodeCompiles(string source, ODataT4CodeGenerator.LanguageOption lang)
        {
            var results = CompileCode(source, lang);
            results.Errors.Should().BeEmpty();
        }

        private static CompilerResults CompileCode(string source, ODataT4CodeGenerator.LanguageOption lang)
        {
            bool isCSharp = lang == ODataT4CodeGenerator.LanguageOption.CSharp;
            return CompileCode(source, isCSharp);
        }

        private static CompilerResults CompileCode(string source, bool isCSharp)
        {
            var compilerOptions = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                TreatWarningsAsErrors = false,
                WarningLevel = 0, // TODO: Switch on warning levels once we resolve why warnings are still being treated as errors.
                ReferencedAssemblies =
                {
                    typeof(DataServiceContext).Assembly.Location,
                    typeof(Microsoft.OData.Client.DataServiceContext).Assembly.Location,
                    typeof(IEdmModel).Assembly.Location,
                    typeof(GeographyPoint).Assembly.Location,
                    typeof(ODataVersion).Assembly.Location,
                    AssemblyRef.SystemRuntime,
                    AssemblyRef.SystemXmlReaderWriter,
                    AssemblyRef.SystemIO,
                    AssemblyRef.System,
                    AssemblyRef.SystemCore,
                    AssemblyRef.SystemXml,
                    typeof(RequiredAttribute).Assembly.Location
                }
            };

            CodeDomProvider codeProvider = null;
            if (isCSharp)
            {
                using (codeProvider = new CSharpCodeProvider())
                {
                }
            }
            else
            {
                using (codeProvider = new VBCodeProvider())
                {
                }
            }

            CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerOptions, source);
            return results;
        }
    }
}
