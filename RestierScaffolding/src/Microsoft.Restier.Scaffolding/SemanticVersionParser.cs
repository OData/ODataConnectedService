// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Text.RegularExpressions;

    internal static class SemanticVersionParser
    {
        private const RegexOptions Flags = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        private static readonly Regex _semanticVersionRegex = new Regex(@"^(?<Version>\d+(\s*\.\s*\d+){0,3})(?<Release>-[a-z][0-9a-z-]*)?$", Flags);

        /// <summary>
        /// Helper method to parse a Version from a semantic version string.
        /// This ignores any special version in the semantic version string and
        /// just returns the version component in the out variable for a successful parse.
        /// Otherwise returns false.
        /// </summary>
        /// <param name="versionString"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        internal static bool TryParse(string versionString, out Version version)
        {
            version = null;
            if (String.IsNullOrWhiteSpace(versionString))
            {
                return false;
            }

            var match = _semanticVersionRegex.Match(versionString.Trim());
            if (!match.Success || !Version.TryParse(match.Groups["Version"].Value, out version))
            {
                return false;
            }

            return true;
        }
    }
}
