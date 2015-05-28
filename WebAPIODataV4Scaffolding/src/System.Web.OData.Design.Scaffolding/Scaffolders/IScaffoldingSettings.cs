// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Web.OData.Design.Scaffolding.VisualStudio;

namespace System.Web.OData.Design.Scaffolding
{
    public interface IScaffoldingSettings
    {
        void LoadSettings(IProjectSettings settings);

        void SaveSettings(IProjectSettings settings);
    }
}
