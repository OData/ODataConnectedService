// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace System.Web.OData.Design.Scaffolding.Interop
{
    [ComImport]
    [Guid("EDDE1B36-C493-4cbe-B75C-762947CFE068")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    internal interface IProjectItemSelector
    {
        [PreserveSig]
        [DispId(1)]
        int SelectItem(
            [In] [MarshalAs(UnmanagedType.Interface)] IVsHierarchy hierarchy, // The project hierarchy 
            [In] [MarshalAs(UnmanagedType.U4)] uint itemID, // Itemid of the document making the call. Only used if the file is in a misc files project.
            [In] [MarshalAs(UnmanagedType.LPWStr)] string filters, // can be NULL. Format: "All Files (*.*)|*.*|Next Filter (*.ext1,*.ext2)|*.ext1,*.ext2";
            [In] [MarshalAs(UnmanagedType.LPWStr)] string dlgTitle, // Can be NULL. Dialog title
            [In] [MarshalAs(UnmanagedType.U4)] ProjectItemSelectorFlags flags, // Controls operation
            [In] [MarshalAs(UnmanagedType.LPWStr)] string relUrlToAnchor, // can be NULL. Relative URL to start enumeration (relative to project root)
            [In] [MarshalAs(UnmanagedType.LPWStr)] string relUrlToSelect, // Can be NULL. Relative URL of the item to select on launch.
            [In] [MarshalAs(UnmanagedType.LPWStr)] string baseUrl, // Must be set if PSIF_ReturnDocRelativeUrls is Set. Otherwise it is ignored.
            [Out] [MarshalAs(UnmanagedType.BStr)] out string relUrlOfSelectedItem, // return value is the rel url (in "~/a/b" form) the user selected
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool canceled);

        // TRUE if user cancel
    }
}
