// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.VisualStudio
{
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    [Export(typeof(IVisualStudioIntegration))]
    internal class VisualStudioIntegration : IVisualStudioIntegration
    {
        public VisualStudioIntegration()
        {
            Editor = new EditorIntegration(this);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "This is used by MEF.")]
        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public IOleServiceProvider OleServiceProvider
        {
            get
            {
                return (IOleServiceProvider)ServiceProvider.GetService(typeof(SDTE));
            }
        }

        public IComponentModel ComponentModel
        {
            get
            {
                return (IComponentModel)ServiceProvider.GetService(typeof(SComponentModel));
            }
        }

        public IEditorIntegration Editor
        {
            get;
            private set;
        }

        /// <remarks>
        /// Not all project systems will implement IVsBuildPropertyStorage. If we're dealing with a project
        /// that doesn't implement it, we'll return null.
        /// </remarks>
        public IProjectSettings GetProjectSettings(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            IVsBuildPropertyStorage storage = GetBuildPropertyStorage(project);
            return storage == null ? null : new ProjectSettings(storage);
        }

        private IVsBuildPropertyStorage GetBuildPropertyStorage(Project project)
        {
            IVsSolution solution = (IVsSolution)ServiceProvider.GetService(typeof(SVsSolution));

            IVsHierarchy hierarchy;
            int hr = solution.GetProjectOfUniqueName(project.FullName, out hierarchy);
            Marshal.ThrowExceptionForHR(hr);

            return hierarchy as IVsBuildPropertyStorage;
        }

        public void ShowErrorMessage(string caption, string message)
        {
            if (caption == null)
            {
                throw new ArgumentNullException("caption");
            }

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                message,
                caption,
                OLEMSGICON.OLEMSGICON_CRITICAL,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
