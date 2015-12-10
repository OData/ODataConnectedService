// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding.Interop
{
    [Flags]
    internal enum ProjectItemSelectorFlags : int
    {
        PISF_ReturnAppRelativeUrls = 0, // Default. Dialog returns application (~/) urls
        PSIF_ReturnDocRelativeUrls = 1, // Dialog returns document relative URLS. Requires pszBaseUrl to be set
    }
}
