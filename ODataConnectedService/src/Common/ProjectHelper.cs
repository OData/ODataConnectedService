using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.OData.ConnectedService.Common
{
    /// <summary>
    /// A utility class for working with VS projects (e.g. retrieving settings from).
    /// </summary>
    internal static class ProjectHelper
    {
        public const uint VSConstants_VSITEMID_ROOT = 4294967294;
        public const int __VSHPROPID_VSHPROPID_ExtObject = -2027;
        public const int VSConstans_S_OK = 0;

        public static Project GetProjectFromHierarchy(IVsHierarchy projectHierarchy)
        {
            object projectObject;
            int result = projectHierarchy.GetProperty(
                VSConstants_VSITEMID_ROOT /* VSConstants.VSITEMID_ROOT */,
                __VSHPROPID_VSHPROPID_ExtObject /* (int)__VSHPROPID.VSHPROPID_ExtObject */,
                out projectObject);
            if (result != VSConstans_S_OK)
            {
                throw new Exception("Cannot find the project from VsHierarchy");
            }

            return (Project)projectObject;
        }

        public static string GetProjectNamespace(Project project)
        {
            return project.Properties.Item("DefaultNamespace").Value.ToString();
        }

        public static string GetProjectFullPath(Project project)
        {
            return project.Properties.Item("FullPath").Value.ToString();
        }
    }
}

