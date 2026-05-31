//-----------------------------------------------------------------------------------
// <copyright file="MSBuildRegistration.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Microsoft.Build.Locator;

namespace Microsoft.OData.Cli.Tests
{
    /// <summary>
    /// Registers the MSBuild assemblies from the running .NET SDK before any test exercises the
    /// <c>Microsoft.Build.Evaluation</c> APIs (as the CLI does via <c>ProjectHelper</c> when handling
    /// the <c>generate</c> command). This mirrors the registration performed in <c>Program.Main</c>.
    /// Without it the test host has no MSBuild instance registered and the code-generation tests fail
    /// with "No instances of MSBuild could be detected".
    /// </summary>
    internal static class MSBuildRegistration
    {
        [ModuleInitializer]
        internal static void Register()
        {
            if (!MSBuildLocator.IsRegistered)
            {
                MSBuildLocator.RegisterDefaults();
            }
        }
    }
}
