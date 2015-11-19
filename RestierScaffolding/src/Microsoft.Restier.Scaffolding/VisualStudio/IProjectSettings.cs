// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.VisualStudio
{
    public interface IProjectSettings
    {
        string this[string key]
        {
            get;
            set;
        }
    }
}
