// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.IO;

    internal static class MvcProjectUtil
    {
        public static readonly string ControllerSuffix = "Controller";
        public static readonly string DataContextSuffix = "Context";
        public static readonly string ConfigName = "WebApiConfig";

        public static readonly string DefaultNamespace = "DefaultNamespace";
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
        public static readonly string ControllerNameRegex = @"\b([_\d\w]*)" + ControllerSuffix + "$";

        /// <summary>
        /// Regex for selecting the 'Controller' portion of a name. See the remarks on ControllerNameRegex.
        /// </summary>
        public static readonly string DataContextNameRegex = @"\b([_\d\w]*)" + DataContextSuffix + "$";

        /// <summary>
        /// This method extracts the root name of the controller from the class name of
        /// the controller.
        /// </summary>
        /// <param name="fullConfigName">Full class name of the controller</param>
        /// <returns>The stripped string if the suffix was found, or the same string as the parameter if
        /// no 'Controller' suffix was found</returns>
        public static string StripConfigName(string fullConfigName)
        {
            if (String.IsNullOrEmpty(fullConfigName))
            {
                return fullConfigName;
            }

            return EndsWithConfig(fullConfigName) ? fullConfigName.Substring(0, fullConfigName.Length - ControllerSuffix.Length)
                                                          : fullConfigName;
        }

        /// <summary>
        /// This method checks to see if a controller name follows the MVC convention of ending the
        /// class name with the suffix 'Controller'.
        /// </summary>
        /// <param name="name">Name of the controller class</param>
        /// <returns>true if the name ends with the correct suffix, false otherwise</returns>
        public static bool EndsWithConfig(string name)
        {
            return name.EndsWith(ControllerSuffix, StringComparison.Ordinal);
        }

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
