// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.OData.Design.Scaffolding.VisualStudio;

namespace System.Web.OData.Design.Scaffolding.UI
{
    /// <summary>
    /// An interface for Scaffolder Models that read and write dialog settings.
    /// </summary>
    public interface IDialogSettings
    {
        void LoadDialogSettings(IProjectSettings settings);

        void SaveDialogSettings(IProjectSettings settings);
    }
}
