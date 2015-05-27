// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding.VisualStudio
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
