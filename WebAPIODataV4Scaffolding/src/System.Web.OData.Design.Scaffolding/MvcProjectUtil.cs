// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding
{
    internal static class MvcProjectUtil
    {
        public const string AreaRegistration = "AreaRegistration";
        public const string ControllerSuffix = "Controller";
        public const string DataContextSuffix = "Context";
        public const string ControllerName = "Default{0}Controller";
        public const string PartialViewName = "Partial{0}";
        public const string ViewName = "View{0}";

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

        /// <summary>
        /// This method extracts the root name of the controller from the class name of
        /// the controller.
        /// </summary>
        /// <param name="fullControllerName">Full class name of the controller</param>
        /// <returns>The stripped string if the suffix was found, or the same string as the parameter if
        /// no 'Controller' suffix was found</returns>
        public static string StripControllerName(string fullControllerName)
        {
            if (String.IsNullOrEmpty(fullControllerName))
            {
                return fullControllerName;
            }

            return EndsWithController(fullControllerName) ? fullControllerName.Substring(0, fullControllerName.Length - ControllerSuffix.Length)
                                                          : fullControllerName;
        }

        /// <summary>
        /// This method checks to see if a controller name follows the MVC convention of ending the
        /// class name with the suffix 'Controller'.
        /// </summary>
        /// <param name="name">Name of the controller class</param>
        /// <returns>true if the name ends with the correct suffix, false otherwise</returns>
        public static bool EndsWithController(string name)
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

        public static string GetDefaultModelsNamespace(string projectDefaultNamespace)
        {
            if (String.IsNullOrEmpty(projectDefaultNamespace))
            {
                return CommonFolderNames.Models;
            }
            else
            {
                return projectDefaultNamespace + "." + CommonFolderNames.Models;
            }
        }

        internal static string GetViewFileExtension(ProjectLanguage language)
        {
            if (language == null)
            {
                throw new ArgumentNullException("language");
            }

            if (language == ProjectLanguage.CSharp)
            {
                return "cshtml";
            }
            else if (language == ProjectLanguage.VisualBasic)
            {
                return "vbhtml";
            }
            else
            {
                throw new InvalidOperationException(Resources.ScaffoldLanguageNotSupported);
            }
        }
    }
}
