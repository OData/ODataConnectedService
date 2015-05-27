// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.NuGet;

namespace System.Web.OData.Design.Scaffolding.VisualStudio
{
    public interface INuGetRepository
    {
        NuGetPackage GetPackage(CodeGenerationContext context, string id);
        string GetPackageVersion(CodeGenerationContext context, string id);
    }
}
