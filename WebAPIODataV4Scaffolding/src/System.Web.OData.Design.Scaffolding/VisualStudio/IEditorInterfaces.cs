// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;

namespace System.Web.OData.Design.Scaffolding.VisualStudio
{
    public interface IEditorInterfaces
    {
        ITextBuffer TextBuffer
        {
            get;
        }

        ITextDocument TextDocument
        {
            get;
        }

        IVsTextBuffer VsTextBuffer
        {
            get;
        }
    }
}
