// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.VisualStudio
{
    using System;
    using Microsoft.VisualStudio.Shell.Interop;

    public interface IEditorIntegration
    {
        IEditorInterfaces GetOrOpenDocument(string path);

        IVsWindowFrame CreateAndOpenReadme(string text);

        void OpenFileInEditor(string filePath);

        void FormatDocument(string filePath);

        IDisposable SuppressChangeNotifications(string filePath);
    }
}
