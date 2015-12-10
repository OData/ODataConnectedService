// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using Microsoft.Restier.Scaffolding.VisualStudio;

    public interface IScaffoldingSettings
    {
        void LoadSettings(IProjectSettings settings);

        void SaveSettings(IProjectSettings settings);
    }
}
