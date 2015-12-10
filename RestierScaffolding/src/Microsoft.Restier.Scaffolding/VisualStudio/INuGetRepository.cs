// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding.VisualStudio
{
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.AspNet.Scaffolding.NuGet;

    public interface INuGetRepository
    {
        NuGetPackage GetPackage(CodeGenerationContext context, string id);
        string GetPackageVersion(CodeGenerationContext context, string id);
    }
}
