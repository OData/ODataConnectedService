// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.CodeDom.Compiler;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding
{
    internal static class ValidationUtil
    {
        private static readonly char[] _invalidCharacters = new[] 
                                                            {
                                                            '.',
                                                            '-',
                                                            '@',
                                                            '+',
                                                            }
                                                            .Concat(Path.GetInvalidFileNameChars())
                                                            .ToArray();

        //For unit tests.
        internal static char[] DisplayInvalidCharacters
        {
            get
            {
                return _invalidCharacters.Where(c => c != '\0').ToArray();
            }
        }

        /// <summary>
        /// This function is used to verify if the specified text is a valid identifier in the specified language.
        /// </summary>
        /// <param name="text">The text to be validated.</param>
        /// <param name="projectLanguage">The project language.</param>
        /// <returns><see langword="null"/> if the specified text is a valid identifier in the specified language; otherwise, 
        /// an error message.</returns>
        public static string GetErrorIfInvalidIdentifier(string text, ProjectLanguage projectLanguage)
        {
            if (String.IsNullOrEmpty(text))
            {
                return Resources.InvalidIdentifierEmpty;
            }

            if (text.IndexOfAny(_invalidCharacters) >= 0)
            {
                return String.Format(CultureInfo.CurrentCulture, Resources.InvalidIdentifierCharacters, String.Join(" ", DisplayInvalidCharacters));
            }

            if (text.Any(c => (Char.IsWhiteSpace(c))))
            {
                return Resources.InvalidIdentifierWhitespaces;
            }

            CodeDomProvider provider = GenerateCodeDomProvider(projectLanguage);
            Contract.Assert(provider != null);
            if (!provider.IsValidIdentifier(text))
            {
                return Resources.InvalidIdentifierReservedName;
            }

            return null;
        }

        public static CodeDomProvider GenerateCodeDomProvider(ProjectLanguage projectLanguage)
        {
            if (projectLanguage == null)
            {
                throw new ArgumentNullException("projectLanguage");
            }

            if (!CodeDomProvider.IsDefinedLanguage(projectLanguage.ToString()))
            {
                throw new InvalidOperationException(Resources.ScaffoldLanguageNotSupported);
            }

            return CodeDomProvider.CreateProvider(projectLanguage.ToString());
        }
    }
}
