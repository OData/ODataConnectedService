// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding.UI
{
    // Avoid referencing the assembly of "Microsoft.VisualStudio.shell.<version>.dll" in xaml,
    // the same xaml source will be used by all versions of visual studio we ship on.
    public class VSPlatformDialogWindow : Microsoft.VisualStudio.PlatformUI.DialogWindow
    {
    }
}
