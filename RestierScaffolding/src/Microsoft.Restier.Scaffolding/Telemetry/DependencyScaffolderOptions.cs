// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.Telemetry
{
    internal enum DependencyScaffolderOptions : uint
    {
        None = 0u,
        AlreadyInstalled = 1u,
        MvcMinimal = 2u,
        MvcFull = 3u,
        WebApi = 4u
    }
}
