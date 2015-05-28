// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding
{
    internal static class TemplateSearchDirectories
    {
        public static string InstalledTemplateRoot
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(typeof(TemplateSearchDirectories).Assembly.Location), "Templates");
            }
        }

        public static string GetProjectTemplateRoot(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            return Path.Combine(project.GetFullPath(), "CodeTemplates");
        }
    }
}
