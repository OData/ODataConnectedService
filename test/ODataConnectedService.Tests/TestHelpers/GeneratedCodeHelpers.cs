using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ODataConnectedService.Tests.TestHelpers
{
    public class GeneratedCodeHelpers
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
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
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
    }
}
