// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.IO;

    internal static class MvcProjectUtil
    {
        public const string ControllerSuffix = "Controller";
        public const string DataContextSuffix = "Context";
        public const string ConfigName = "WebApiConfig";

        public const string DefaultNamespace = "DefaultNamespace";
        public static readonly string PathSeparator = Path.DirectorySeparatorChar.ToString();

        /// <summary>
        /// Regex for selecting the 'Controller' portion of a name.
        /// </summary>
        /// <remarks>
        /// This isn't super-precise, but should do an adequate job matching common identifiers. 
        /// 
        /// Here's a description. \b makes the match start at a word boundary, so for names with
        /// namespaces, this starts the match after that. The group ([_\d\w]*) matches any number
        /// of underscore, number or letter characters and stores them in a group. Lastly the literal
        /// text Controller, and the $ matches end of string.
        /// 
        /// Technically some other characters besides _\d\w are allowed, but this should handled the 99.9% case.
        /// </remarks>
        public const string ControllerNameRegex = @"\b([_\d\w]*)" + ControllerSuffix + "$";

        /// <summary>
        /// Regex for selecting the 'Controller' portion of a name. See the remarks on ControllerNameRegex.
        /// </summary>
        public const string DataContextNameRegex = @"\b([_\d\w]*)" + DataContextSuffix + "$";

        public static string EnsureTrailingBackSlash(string str)
        {
            if (str != null && !str.EndsWith(PathSeparator, StringComparison.Ordinal))
            {
                str += PathSeparator;
            }
            return str;
        }
    }
}
