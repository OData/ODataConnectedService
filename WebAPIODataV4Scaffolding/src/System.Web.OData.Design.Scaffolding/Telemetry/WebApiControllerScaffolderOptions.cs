// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace System.Web.OData.Design.Scaffolding.Telemetry
{
    [Flags]
    internal enum WebApiControllerScaffolderOptions : uint
    {
        None = 0u,
        CreatedController = 1u,
        IsAsyncSelected = 2u,
    }
}
