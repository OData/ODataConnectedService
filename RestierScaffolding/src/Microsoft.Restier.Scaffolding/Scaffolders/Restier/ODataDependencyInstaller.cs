// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.IO;
    using System.Runtime.Remoting.Contexts;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.Restier.Scaffolding.ReadMe;
    using Microsoft.Restier.Scaffolding.VisualStudio;

    public class ODataDependencyInstaller : DependencyInstaller
    {
        public ODataDependencyInstaller(CodeGenerationContext context, IVisualStudioIntegration visualStudioIntegration)
            : base(context, visualStudioIntegration)
        {
        }

        protected override string[] SearchFolders
        {
            get
            {
                // TODO: this is temporarily here until we reorganize these dependency folders
                return new string[] { Path.Combine(TemplateSearchDirectories.InstalledTemplateRoot, "ODataDependencyCodeGenerator") };
            }
        }

        protected override FrameworkDependencyStatus GenerateConfiguration()
        {
            return FrameworkDependencyStatus.FromReadme(ODataReadMe.CreateReadMeText());
        }
    }
}
