// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.ComponentModel;
using System.Windows;
using EnvDTE;

namespace System.Web.OData.Design.Scaffolding.UI
{
    public interface IDialogHost
    {
        /// <summary>
        /// Fired when the dialog is closing with a successful result. This is not fired when the user
        /// clicks cancel.
        /// </summary>
        /// <remarks>
        /// Use this handler to force expensive validation, or show a confirmation.
        /// </remarks>
        event EventHandler<CancelEventArgs> Closing;

        /// <summary>
        /// Shows a modal Message Box for interactive confirmation.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="caption">The caption for the Message Box, provide null to use the caption of the window.</param>
        /// <returns>MessageBoxResult.Yes or MessageBoxResult.No</returns>
        MessageBoxResult RequestConfirmation(string message, string caption);

        /// <summary>
        /// Shows a modal Message Box with an error message.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="caption">The caption for the Message Box, provide null to use the caption of the window.</param>
        void ShowErrorMessage(string message, string caption);

        /// <summary>
        /// Shows a dialog promptiong the user to interactively select a file from the project.
        /// </summary>
        /// <param name="project">The project within which to select.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="filter">The file filter to use.</param>
        /// <param name="storageKey">A string key used to pre-select the last-used file. Set to null to avoid this behavior.</param>
        /// <param name="file">The project relative path of the file, preceded with a tilde '~'.</param>
        /// <returns>True if a file is selected</returns>
        bool TrySelectFile(Project project, string title, string filter, string storageKey, out string file);
    }
}
