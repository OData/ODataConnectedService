//-----------------------------------------------------------------------------------
// <copyright file="CodeVerificationHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using Microsoft.OData.Client;
using Microsoft.OData.CodeGen.Templates;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Microsoft.VisualBasic;

namespace Microsoft.OData.Cli.Tests.CodeGeneration
{
    internal static class CodeVerificationHelper
    {
        static Assembly Assembly = Assembly.GetExecutingAssembly();
        const string ReferenceResourcePrefix = "Microsoft.OData.Cli.Tests.CodeGeneration.Artifacts.";
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
            Assert.Equal(normalizedExpected, normalizedActual);
        }

        public static string NormalizeGeneratedCode(string code)
        {
            var normalized = Regex.Replace(code, "// Generation date:.*", "// Generation date:", RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "'Generation date:.*", "'Generation date:", RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "//     Runtime Version:.*", "//     Runtime Version:", RegexOptions.Multiline);
            normalized = Regex.Replace(normalized, "'     Runtime Version:.*", "'     Runtime Version:", RegexOptions.Multiline);
            normalized = Regex.Replace(normalized,
               "global::System.CodeDom.Compiler.GeneratedCodeAttribute\\(.*\\)",
               "global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"Microsoft.OData.Client.Design.T4\", \"" + T4Version + "\")",
               RegexOptions.Multiline);

            //Remove the spaces from the string to avoid indentation change errors
            normalized = Regex.Replace(normalized, @"\s+", "");

            return normalized;
        }

        public static void VerifyGeneratedCodeOmitVersioningInfo(string expectedCode, string actualCode)
        {
            var normalizedExpected = NormalizeGeneratedCodeOmitVersionInfo(expectedCode);
            var normalizedActual = NormalizeGeneratedCode(actualCode);
            Assert.Equal(normalizedExpected, normalizedActual);
        }

        public static string NormalizeGeneratedCodeOmitVersionInfo(string code)
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

        public static void VerifyGeneratedCodeCompiles(string source, bool isCSharp)
        {
            var results = CompileCode(source, isCSharp);
            Assert.Empty(results.Errors);
        }

        public static void VerifyGeneratedCodeCompiles(string source, ODataT4CodeGenerator.LanguageOption lang)
        {
            var results = CompileCode(source, lang);
            Assert.Empty(results.Errors);
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
                WarningLevel = 0,
                ReferencedAssemblies =
                {
                    typeof(DataServiceContext).Assembly.Location,
                    typeof(DataServiceContext).Assembly.Location,
                    typeof(IEdmModel).Assembly.Location,
                    typeof(GeographyPoint).Assembly.Location,
                    typeof(ODataVersion).Assembly.Location,
                    "System.Runtime.dll",
                    "System.Xml.ReaderWriter.dll",
                    "System.IO.dll",
                    "System.dll",
                    "System.Core.dll",
                    "System.Xml.dll",
                    typeof(RequiredAttribute).Assembly.Location
                }
            };

            CodeDomProvider codeProvider;
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
