// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using EnvDTE;
    using Microsoft.AspNet.Scaffolding;

    internal static class ProjectItemExtensions
    {
        // TODO: This is a good candidate to get into Core, since this seems like a common thing to do
        public static string GetProjectRelativePath(this ProjectItem projectItem)
        {
            Project project = projectItem.ContainingProject;
            string projRelativePath = null;

            string rootProjectDir = project.GetFullPath();
            rootProjectDir = MvcProjectUtil.EnsureTrailingBackSlash(rootProjectDir);
            string fullPath = projectItem.GetFullPath();

            if (!String.IsNullOrEmpty(rootProjectDir) && !String.IsNullOrEmpty(fullPath))
            {
                projRelativePath = CoreScaffoldingUtil.MakeRelativePath(fullPath, rootProjectDir);
            }

            return projRelativePath;
        }
    }
}
