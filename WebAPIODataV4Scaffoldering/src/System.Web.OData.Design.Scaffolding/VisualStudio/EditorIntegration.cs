// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace System.Web.OData.Design.Scaffolding.VisualStudio
{
    internal class EditorIntegration : IEditorIntegration
    {
        public EditorIntegration(VisualStudioIntegration visualStudio)
        {
            VisualStudio = visualStudio;
        }

        private VisualStudioIntegration VisualStudio
        {
            get;
            set;
        }

        public void FormatDocument(string filePath)
        {
            IVsTextView vsTextView;
            IVsUIHierarchy uiHierarchy;
            uint itemID;
            IVsWindowFrame windowFrame;
            if (VsShellUtilities.IsDocumentOpen(
                VisualStudio.ServiceProvider,
                filePath,
                Guid.Empty,
                out uiHierarchy,
                out itemID,
                out windowFrame))
            {
                vsTextView = VsShellUtilities.GetTextView(windowFrame);
            }
            else
            {
                Contract.Assert(false, "Failed to get the IVsTextView, is the document open in VS?");
                return;
            }

            IOleCommandTarget commandTarget = (IOleCommandTarget)vsTextView;

            Guid guid = typeof(VSConstants.VSStd2KCmdID).GUID;
            OLECMD[] commandStatus = new OLECMD[]
            { 
                new OLECMD() { cmdID = (uint)VSConstants.VSStd2KCmdID.FORMATDOCUMENT } 
            };

            int hr = commandTarget.QueryStatus(
                ref guid,
                1,
                commandStatus,
                IntPtr.Zero);

            Marshal.ThrowExceptionForHR(hr);

            if (commandStatus[0].cmdf == (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED))
            {
                hr = commandTarget.Exec(
                ref guid,
                (uint)VSConstants.VSStd2KCmdID.FORMATDOCUMENT,
                0u,
                IntPtr.Zero,
                IntPtr.Zero);

                Marshal.ThrowExceptionForHR(hr);
            }
            else
            {
                Contract.Assert(false, "The format command can't be executed right now, we don't expect this to happen.");
            }
        }

        public IEditorInterfaces GetOrOpenDocument(string path)
        {
            OpenFileInEditor(path);
            IVsRunningDocumentTable runningDocumentTable = (IVsRunningDocumentTable)VisualStudio.ServiceProvider.GetService(typeof(SVsRunningDocumentTable));

            IVsHierarchy hierarchy;
            uint itemId;
            IntPtr documentData = IntPtr.Zero;
            uint cookie;

            try
            {
                int hr = runningDocumentTable.FindAndLockDocument(
                    (uint)_VSRDTFLAGS.RDT_NoLock,
                    path,
                    out hierarchy,
                    out itemId,
                    out documentData,
                    out cookie);

                if (hr == NativeMethods.S_OK && documentData != IntPtr.Zero)
                {
                    IVsTextBuffer vsTextBuffer = Marshal.GetObjectForIUnknown(documentData) as IVsTextBuffer;
                    if (vsTextBuffer != null)
                    {
                        IVsEditorAdaptersFactoryService editorAdaptersFactory = VisualStudio.ComponentModel.GetService<IVsEditorAdaptersFactoryService>();
                        ITextBuffer buffer = editorAdaptersFactory.GetDocumentBuffer(vsTextBuffer);
                        ITextDocument document = buffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));

                        return new EditorInterfaces(buffer, document, vsTextBuffer);
                    }
                }

                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, Resources.FailedToOpenFile, path));
            }
            finally
            {
                if (documentData != IntPtr.Zero)
                {
                    Marshal.Release(documentData);
                }
            }
        }

        public IVsWindowFrame CreateAndOpenReadme(string text)
        {
            string tempFilename = GetTempFilename("readme", "txt");

            // We leave this file around right now in temp, this simplifies picking a unique name. If we re-use the file
            // path of an open 'readme' file, then it will no-op trying to open it in the editor.
            File.WriteAllText(tempFilename, text);

            IVsUIShellOpenDocument openDocument = (IVsUIShellOpenDocument)VisualStudio.ServiceProvider.GetService(typeof(SVsUIShellOpenDocument));

            Guid logicalView = VSConstants.LOGVIEWID.TextView_guid;
            IOleServiceProvider oleServiceProvider;
            IVsUIHierarchy hierarchy;
            uint itemId;
            IVsWindowFrame window;
            int hr = openDocument.OpenDocumentViaProject(
                tempFilename,
                ref logicalView,
                out oleServiceProvider,
                out hierarchy,
                out itemId,
                out window);

            if (NativeMethods.Succeeded(hr))
            {
                // This will ask the user to choose a name if they try to save the file. We're not overly worried
                // if this fails to set (hence ignoring the HResult).
                hr = hierarchy.SetProperty(itemId, (int)__VSHPROPID.VSHPROPID_IsNewUnsavedItem, true);

                hr = window.Show();
                if (NativeMethods.Succeeded(hr))
                {
                    return window;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void OpenFileInEditor(string filePath)
        {
            DTE dte = (DTE)VisualStudio.ServiceProvider.GetService(typeof(SDTE));
            if (File.Exists(filePath))
            {
                if (!dte.ItemOperations.IsFileOpen(filePath))
                {
                    // We have a possible timing issue when opening a file in the editor if the file was just edited.
                    // The project system is still tracking the fact that it changed, even though we're opening it after 
                    // we finished writing to it.
                    using (SuppressChangeNotifications(filePath))
                    {
                    }

                    dte.ItemOperations.OpenFile(filePath);
                }
            }
            else
            {
                Contract.Assert(false, "The file should have been written to disk by now.");
            }
        }

        /// <summary>
        /// Creates a unique but still readable name for a temp file.
        /// </summary>
        private static string GetTempFilename(string baseName, string extension)
        {
            int i = 1;
            string tempDirectory = Path.GetTempPath();

            string filename = Path.Combine(tempDirectory, baseName + "." + extension);
            while (true)
            {
                if (!File.Exists(filename))
                {
                    return filename;
                }

                filename = Path.Combine(tempDirectory, baseName + i++ + "." + extension);
            }
        }

        public IDisposable SuppressChangeNotifications(string filePath)
        {
            // In general we want to turn off notifications for a file if NuGet is changing it externally, this will avoid the 
            // 'file has changed, would you like to reload' dialog.
            //
            // See: http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivsfilechangeex.syncfile(v=vs.110).aspx
            // for an example.
            IVsFileChangeEx fileChangeService = (IVsFileChangeEx)VisualStudio.ServiceProvider.GetService(typeof(IVsFileChangeEx));

            // This is a COM interface that wants the Win32 TRUE, which is why the third value is 1.
            int hr = fileChangeService.IgnoreFile(0u, filePath, 1);
            Marshal.ThrowExceptionForHR(hr);

            return new NotificationActivator(this, filePath);
        }

        private void ResumeChangeNotifications(string filePath)
        {
            IVsFileChangeEx fileChangeService = (IVsFileChangeEx)VisualStudio.ServiceProvider.GetService(typeof(IVsFileChangeEx));

            int hr;
            try
            {
                // We need to call sync to 'flush' events that have piled up for the file, this prevents a notification
                // from being delivered after we turn notifications back on.
                hr = fileChangeService.SyncFile(filePath);
                Marshal.ThrowExceptionForHR(hr);
            }
            finally
            {
                // We want to turn notifications back on if the sync fails for some reason
                //
                // This is a COM interface that wants the Win32 FALSE, which is why the third value is 0.
                hr = fileChangeService.IgnoreFile(0u, filePath, 0);
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        private class NotificationActivator : IDisposable
        {
            private readonly EditorIntegration _editorIntegration;
            private readonly string _filePath;

            public NotificationActivator(EditorIntegration editorIntegration, string filePath)
            {
                Contract.Assert(editorIntegration != null);
                Contract.Assert(filePath != null);

                _editorIntegration = editorIntegration;
                _filePath = filePath;
            }

            public void Dispose()
            {
                _editorIntegration.ResumeChangeNotifications(_filePath);
            }
        }

        private class EditorInterfaces : IEditorInterfaces
        {
            public EditorInterfaces(ITextBuffer textBuffer, ITextDocument textDocument, IVsTextBuffer vsTextBuffer)
            {
                Contract.Assert(textBuffer != null);
                Contract.Assert(textDocument != null);
                Contract.Assert(vsTextBuffer != null);

                TextBuffer = textBuffer;
                TextDocument = textDocument;
                VsTextBuffer = vsTextBuffer;
            }

            public ITextBuffer TextBuffer
            {
                get;
                private set;
            }

            public ITextDocument TextDocument
            {
                get;
                private set;
            }

            public IVsTextBuffer VsTextBuffer
            {
                get;
                private set;
            }
        }
    }
}
