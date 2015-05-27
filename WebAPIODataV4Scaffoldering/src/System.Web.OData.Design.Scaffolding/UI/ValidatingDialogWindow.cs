// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Web.OData.Design.Scaffolding.VisualStudio;

namespace System.Web.OData.Design.Scaffolding.UI
{
    public class ValidatingDialogWindow : VSPlatformDialogWindow
    {
        // This has to be read-write because WPF does not allow a OneWayToSource binding
        // on a readonly property. We still don't want to let this get set to null.
        private static readonly DependencyProperty DialogHostProperty = DependencyProperty.Register(
            "DialogHost",
            typeof(IDialogHost),
            typeof(ValidatingDialogWindow),
            new PropertyMetadata());

        public ValidatingDialogWindow()
        {
            Loaded += Dialog_Loaded;
        }

        public IDialogHost DialogHost
        {
            get
            {
                return (IDialogHost)GetValue(DialogHostProperty);
            }
            set
            {
                SetValue(DialogHostProperty, value);
            }
        }

        /// <summary>
        /// Gets a reference to the Global VS Service Provider
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "We prefer this design, it will be easier to work with in the future if we want to write tests.")]
        private IServiceProvider ServiceProvider
        {
            get
            {
                return Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider;
            }
        }

        protected bool TryClose()
        {
            ValidatingDialogHost dialogHost = (ValidatingDialogHost)DialogHost ?? new ValidatingDialogHost(this);

            IDataErrorInfo validatingDataContext = (IDataErrorInfo)DataContext;
            if (validatingDataContext != null)
            {
                string error = validatingDataContext.Error;
                if (error != null)
                {
                    dialogHost.ShowErrorMessage(error, caption: null);
                    return false;
                }
            }

            if (dialogHost.OnClosing())
            {
                DialogResult = true;
                return true;
            }
            else
            {
                // A confirmation or validation issue is causing the dialog to stay open.
                return false;
            }
        }

        private void Dialog_Loaded(object sender, EventArgs e)
        {
            Loaded -= Dialog_Loaded;

            // This needs to be set in the loaded event, because a call to InitializeComponent
            // from a child class runs _after_ our constructor, and will set the dialog host to null.
            DialogHost = new ValidatingDialogHost(this);
        }

        private class ValidatingDialogHost : IDialogHost
        {
            public event EventHandler<CancelEventArgs> Closing;

            public ValidatingDialogHost(ValidatingDialogWindow dialog)
            {
                Contract.Assert(dialog != null);
                Dialog = dialog;
            }

            private ValidatingDialogWindow Dialog { get; set; }

            public MessageBoxResult RequestConfirmation(string message, string caption)
            {
                Contract.Assert(message != null);

                IVsUIShell uiShell = (IVsUIShell)Dialog.ServiceProvider.GetService(typeof(SVsUIShell));

                bool result = VsShellUtilities.PromptYesNo(
                    message,
                    caption ?? Dialog.Title,
                    OLEMSGICON.OLEMSGICON_QUERY,
                    uiShell);

                return result ? MessageBoxResult.Yes : MessageBoxResult.No;
            }

            public void ShowErrorMessage(string message, string caption)
            {
                Contract.Assert(message != null);

                VsShellUtilities.ShowMessageBox(
                    Dialog.ServiceProvider,
                    message,
                    caption ?? Dialog.Title,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }

            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to crash if using settings fails.")]
            public bool TrySelectFile(Project project, string title, string filter, string storageKey, out string file)
            {
                IVsSolution solution = (IVsSolution)Dialog.ServiceProvider.GetService(typeof(SVsSolution));

                IVsHierarchy hierarchy;
                if (!NativeMethods.Succeeded(solution.GetProjectOfUniqueName(project.FullName, out hierarchy)))
                {
                    file = null;
                    return false;
                }

                // We want to read/persist the last directory location that was used if a key is provided.
                string lastSelectedFile = null;
                ProjectSettings settings = null;
                IVsBuildPropertyStorage storage = null;
                if (storageKey != null)
                {
                    storage = hierarchy as IVsBuildPropertyStorage;
                }

                if (storage != null)
                {
                    try
                    {
                        settings = new ProjectSettings(storage);
                        lastSelectedFile = settings[storageKey];
                    }
                    catch
                    {
                        // We don't want to fail scaffolding/selection if we have a settings issue. We'll just
                        // ignore the settings entirely.
                        settings = null;
                    }
                }

                if (ProjectItemSelector.TrySelectItem(hierarchy, title, filter, lastSelectedFile, out file))
                {
                    if (settings != null)
                    {
                        try
                        {
                            settings[storageKey] = file;
                        }
                        catch
                        {
                            // We don't want to fail scaffolding/selection if we have a settings issue. We'll just
                            // ignore the settings entirely.
                            settings = null;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }

            public bool OnClosing()
            {
                EventHandler<CancelEventArgs> handler = Closing;
                if (handler == null)
                {
                    return true;
                }
                else
                {
                    CancelEventArgs e = new CancelEventArgs(cancel: false);
                    handler(this, e);

                    return !e.Cancel;
                }
            }
        }
    }
}
