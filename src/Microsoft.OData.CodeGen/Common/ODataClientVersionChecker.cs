//-----------------------------------------------------------------------------
// <copyright file="ODataClientVersionChecker.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;

namespace Microsoft.OData.CodeGen.Common
{
    /// <summary>
    /// Evaluates capabilities provided by Microsoft.OData.Client versions.
    /// </summary>
    public static class ODataClientVersionChecker
    {
        /// <summary>
        /// The first Microsoft.OData.Client version that supports System.DateOnly and System.TimeOnly.
        /// </summary>
        public static readonly Version NativeDateTimeTypesMinimumVersion = new Version(9, 0, 0);

        /// <summary>
        /// Determines whether a client version supports native date and time types.
        /// </summary>
        /// <param name="version">The Microsoft.OData.Client version.</param>
        /// <returns>True when native date and time types are supported; otherwise, false.</returns>
        public static bool SupportsNativeDateTimeTypes(Version version)
        {
            return version != null && version >= NativeDateTimeTypesMinimumVersion;
        }

        /// <summary>
        /// Parses a NuGet or assembly version, ignoring prerelease and build metadata.
        /// </summary>
        /// <param name="version">The version string.</param>
        /// <param name="parsedVersion">The parsed version.</param>
        /// <returns>True when the version can be parsed; otherwise, false.</returns>
        public static bool TryParseVersion(string version, out Version parsedVersion)
        {
            parsedVersion = null;
            if (string.IsNullOrWhiteSpace(version))
            {
                return false;
            }

            int suffixIndex = version.IndexOfAny(new[] { '-', '+' });
            string normalizedVersion = suffixIndex >= 0 ? version.Substring(0, suffixIndex) : version;
            return Version.TryParse(normalizedVersion, out parsedVersion);
        }
    }
}
