// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using System.Web.OData.Design.Scaffolding.ReadMe;
using System.Web.OData.Design.Scaffolding.VisualStudio;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding
{
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
            object isODataAssemblyReferenced;
            Context.Items.TryGetProperty(ContextKeys.IsODataAssemblyReferencedKey, out isODataAssemblyReferenced);

            object isWebApiAssemblyReferenced;
            Context.Items.TryGetProperty(ContextKeys.IsWebApiAssemblyReferencedKey, out isWebApiAssemblyReferenced);

            bool showODataReadme = !(bool)isODataAssemblyReferenced;
            bool showWebApiReadme = !(bool)isWebApiAssemblyReferenced;
            bool isNewGlobalAsaxCreated = TryCreateGlobalAsax();

            if (isNewGlobalAsaxCreated && showODataReadme)
            {
                return FrameworkDependencyStatus.FromReadme(ODataReadMe.CreateReadMeText());
            }
            else if (!isNewGlobalAsaxCreated)
            {
                string readmeText = String.Empty;

                if (showODataReadme)
                {
                    readmeText += ODataReadMe.CreateReadMeText();
                }

                if (showWebApiReadme)
                {
                    // WebApiReadMe readMeGenerator = new WebApiReadMe(Context.ActiveProject.GetCodeLanguage(), Context.ActiveProject.Name, AppStartFileNames);
                    // readmeText += readMeGenerator.CreateReadMeText();
                }

                if (!String.IsNullOrEmpty(readmeText))
                {
                    return FrameworkDependencyStatus.FromReadme(readmeText);
                }
            }

            return FrameworkDependencyStatus.InstallSuccessful;
        }
    }
}
