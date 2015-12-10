// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.UI
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.Restier.Scaffolding.VisualStudio;

    /// <summary>
    /// Contains common view model functionality.
    /// </summary>
    public abstract class ViewModel : ValidatingViewModel
    {
        private double _dialogWidth;

        public ViewModel(ScaffolderModel model)
        {
            Context = model.Context;
            LoadDialogSettings();
            if (DialogWidth == default(double) || DialogWidth > SystemParameters.PrimaryScreenWidth)
            {
                DialogWidth = DefaultDialogSize.DialogWidth;
            }
        }

        protected CodeGenerationContext Context { get; private set; }

        public double DialogWidth
        {
            get
            {
                return _dialogWidth;
            }
            set
            {
                if (OnPropertyChanged(ref _dialogWidth, value))
                {
                    SaveDialogSettings();
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to fail scaffolding if we can't read settings.")]
        protected void LoadDialogSettings()
        {
            // Some models persist settings, this is an optional feature
            IDialogSettings modelSettings = this as IDialogSettings;
            IVisualStudioIntegration visualStudioIntegration = Context.Items.GetProperty<IVisualStudioIntegration>(typeof(IVisualStudioIntegration));
            if (modelSettings != null && visualStudioIntegration != null)
            {
                // The project settings will be null if the project doesn't implement settings (project systems are
                // extensible).
                IProjectSettings projectSettings = visualStudioIntegration.GetProjectSettings(Context.ActiveProject);
                if (projectSettings != null)
                {
                    try
                    {
                        modelSettings.LoadDialogSettings(projectSettings);
                    }
                    catch (Exception ex)
                    {
                        // We don't want to make it a blocking issue if we're unable to load settings.
                        Debug.Fail("Failed to load settings\r\n" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// This function is responsible for saving the dialog settings. We need a seperate function for this because we need to
        /// save the state of the dialog irrespective of the scaffolding result. This function is called from the view models after
        /// setting the dialog size in the view models.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to fail scaffolding if we can't save settings.")]
        protected void SaveDialogSettings()
        {
            IDialogSettings modelSettings = this as IDialogSettings;
            IVisualStudioIntegration visualStudioIntegration = Context.Items.GetProperty<IVisualStudioIntegration>(typeof(IVisualStudioIntegration));
            if (modelSettings != null && visualStudioIntegration != null)
            {
                // The project settings will be null if the project doesn't implement settings (project systems are
                // extensible).
                IProjectSettings projectSettings = visualStudioIntegration.GetProjectSettings(Context.ActiveProject);
                if (projectSettings != null)
                {
                    try
                    {
                        modelSettings.SaveDialogSettings(projectSettings);
                    }
                    catch (Exception ex)
                    {
                        // We don't want to make it a blocking issue if we're unable to save settings.
                        Debug.Fail("Failed to save settings", ex.Message);
                    }
                }
            }
        }
    }
}
