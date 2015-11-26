// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.OData.Design.Scaffolding.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace System.Web.OData.Design.Scaffolding.UI
{
    internal static class ProjectItemSelector
    {
        public static bool TrySelectItem(IVsHierarchy hierarchy, string title, string filter, string preselectedItem, out string relativePath)
        {
            if (hierarchy == null)
            {
                throw new ArgumentNullException("hierarchy");
            }
            if (title == null)
            {
                throw new ArgumentNullException("title");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            bool isCancelled;
            int hr = SelectItem(hierarchy, filter, title, preselectedItem, out relativePath, out isCancelled);

            return NativeMethods.Succeeded(hr) && !isCancelled;
        }

        private static int SelectItem(IVsHierarchy hierarchy, string filter, string title, string preselectedItem, out string appRelUrlOfSelectedItem, out bool canceled)
        {
            appRelUrlOfSelectedItem = null;
            canceled = false;
            int hr = NativeMethods.E_FAIL;

            if (hierarchy != null)
            {
                IOleServiceProvider site = null;
                hr = hierarchy.GetSite(out site);
                if (NativeMethods.Succeeded(hr) && site != null)
                {
                    IProjectItemSelector selector = site.CreateSitedInstance<IProjectItemSelector>(typeof(IProjectItemSelector_Class).GUID);
                    if (selector != null)
                    {
                        hr = selector.SelectItem(
                            hierarchy,
                            VSConstants.VSITEMID_NIL,
                            filter,
                            title,
                            ProjectItemSelectorFlags.PISF_ReturnAppRelativeUrls,
                            null,
                            preselectedItem,
                            null,
                            out appRelUrlOfSelectedItem,
                            out canceled);
                    }
                }
            }

            return hr;
        }
    }
}
