// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using EnvDTE;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.Restier.Scaffolding.VisualStudio;

    /// <summary>
    /// A base class for classes that install and configure a framework.
    /// </summary>
    public abstract class DependencyInstaller
    {
        private const string GlobalAsaxClassName = "Global";
        private const string GlobalAsaxMvcClassName = "MvcApplication";
        private const string GlobalAsaxWebApiClassName = "WebApiApplication";

        private const string JQueryBundleSearchText = "ScriptBundle(\"~/bundles/jquery\")";

        protected DependencyInstaller(CodeGenerationContext context, IVisualStudioIntegration visualStudioIntegration)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (visualStudioIntegration == null)
            {
                throw new ArgumentNullException("visualStudioIntegration");
            }

            Context = context;
            VisualStudioIntegration = visualStudioIntegration;

            ActionsService = context.ServiceProvider.GetService<ICodeGeneratorActionsService>();
            FilesLocatorService = context.ServiceProvider.GetService<ICodeGeneratorFilesLocator>();

            AppStartFileNames = new Dictionary<string, string>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "This is an internal API.")]
        protected abstract string[] SearchFolders
        {
            get;
        }

        protected Dictionary<string, string> AppStartFileNames
        {
            get;
            private set;
        }

        protected ICodeGeneratorActionsService ActionsService
        {
            get;
            private set;
        }

        protected CodeGenerationContext Context
        {
            get;
            private set;
        }

        protected ICodeGeneratorFilesLocator FilesLocatorService
        {
            get;
            private set;
        }

        protected IVisualStudioIntegration VisualStudioIntegration
        {
            get;
            private set;
        }

        protected ICodeTypeService CodeTypeService
        {
            get
            {
                return Context.ServiceProvider.GetService<ICodeTypeService>();
            }
        }

        /// <summary>
        /// This function is responsible for installing the product packages, creating the required files and folders and running
        /// the t4 template files.
        /// </summary>
        public FrameworkDependencyStatus Install()
        {
            CreateStaticFilesAndFolders();
            GenerateFiles();
            return GenerateConfiguration();
        }

        /// <summary>
        /// Implement this method to generate configuration for the project, and return a status indicating
        /// whether or not further configuration on the part of the user is needed.
        /// </summary>
        /// <returns></returns>
        protected virtual FrameworkDependencyStatus GenerateConfiguration()
        {
            return FrameworkDependencyStatus.InstallSuccessful;
        }

        protected virtual void CreateStaticFilesAndFolders()
        {
            ActionsService.CreateAppDataFolder(Context.ActiveProject);
            ActionsService.AddFolder(Context.ActiveProject, CommonFolderNames.AppStart);
        }

        protected virtual void GenerateFiles()
        {
            // intentionally empty.
        }

        protected void CreateAppStartFiles(string configFileName, string productNamespace)
        {
            string templateName = configFileName;
            int fileNameSuffixCounter = 2;

            while (true)
            {
                Project activeProject = Context.ActiveProject;
                CodeType matchingConfigFile = CodeTypeService.GetCodeType(activeProject, activeProject.GetDefaultNamespace() + "." + configFileName);
                string configFileNameWithExtension = configFileName + "." + Context.ActiveProject.GetCodeLanguage().CodeFileExtension;
                string configFilePath = Path.Combine(Context.ActiveProject.GetFullPath(), CommonFolderNames.AppStart, configFileNameWithExtension);
                if (!File.Exists(Path.Combine(Context.ActiveProject.GetFullPath(), CommonFolderNames.AppStart, configFileNameWithExtension)) && matchingConfigFile == null)
                {
                    AppStartFileNames.Add(templateName, configFileName);
                    GenerateT4File(templateName, configFileName, CommonFolderNames.AppStart);
                    return;
                }
                else if (matchingConfigFile != null &&
                        !matchingConfigFile.Name.StartsWith(CommonFilenames.BundleConfig, StringComparison.OrdinalIgnoreCase) &&
                        CodeTypeFilter.IsProductNamespaceImported(matchingConfigFile, productNamespace))
                {
                    // For all App_Start files except BundleConfig, the code verifies if the namespace import is present.
                    AppStartFileNames.Add(templateName, configFileName);
                    return;
                }
                else if (matchingConfigFile != null &&
                        matchingConfigFile.Name.StartsWith(CommonFilenames.BundleConfig, StringComparison.OrdinalIgnoreCase) &&
                        AddDependencyUtil.IsSearchTextPresent(configFilePath, JQueryBundleSearchText))
                {
                    // For BundleConfig, the code verifies if the jquery bundling is present. This is because the 
                    // BundleConfig does not import product namespace like other App_Start config files.
                    AppStartFileNames.Add(templateName, configFileName);
                    return;
                }
                else
                {
                    configFileName = templateName + fileNameSuffixCounter;
                    fileNameSuffixCounter++;
                }
            }
        }

        protected bool TryCreateGlobalAsax()
        {
            if (!IsGlobalAsaxPresent())
            {
                // HACK - we intentionally do the code behind file second to avoid a bug in rollback support.
                // Since the project system treats the code behind as a child item of the .asax, removing the .asax
                // will remove the code behind and break rollback.
                //
                // See #744741

                // Add Global.asax if file is not present.
                GenerateT4File(CommonFilenames.GlobalAsax, CommonFilenames.GlobalAsax, String.Empty);

                // Add Global.asax.cs if file is not present.
                GenerateT4File(CommonFilenames.GlobalAsaxCodeBehind, CommonFilenames.GlobalAsaxCodeBehind, String.Empty);

                return true;
            }

            return false;
        }

        private void GenerateT4File(string templateName, string outputFileName, string path)
        {
            IDictionary<string, object> templateParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            templateParameters.Add("Namespace", Context.ActiveProject.GetDefaultNamespace());

            Contract.Assert(AppStartFileNames != null);
            foreach (KeyValuePair<string, string> appStartFileNames in AppStartFileNames)
            {
                templateParameters.Add(appStartFileNames.Key, appStartFileNames.Value);
            }

            string codeFileExtension = Context.ActiveProject.GetCodeLanguage().CodeFileExtension;
            string outputPath = Path.Combine(path, outputFileName);
            string templatePath = FilesLocatorService.GetTextTemplatePath(templateName, SearchFolders, codeFileExtension);
            ActionsService.AddFileFromTemplate(
                Context.ActiveProject,
                outputPath,
                templatePath,
                templateParameters,
                skipIfExists: true);
        }

        /// <summary>
        /// Attempts to find a Global.asax file or a Global.asax.cs (or .vb) for the current project,
        /// including some variations based on the names that Scaffolding could use when generating it.
        /// </summary>
        /// <returns>True if found, otherwise false.</returns>
        private bool IsGlobalAsaxPresent()
        {
            bool foundGlobalAsaxClass =
                CodeTypeService.GetCodeType(Context.ActiveProject, GlobalAsaxClassName) != null ||
                CodeTypeService.GetCodeType(Context.ActiveProject, GlobalAsaxMvcClassName) != null ||
                CodeTypeService.GetCodeType(Context.ActiveProject, GlobalAsaxWebApiClassName) != null;

            if (foundGlobalAsaxClass)
            {
                return true;
            }

            return File.Exists(Path.Combine(Context.ActiveProject.GetFullPath(), String.Empty, CommonFilenames.GlobalAsaxCodeBehind));
        }
    }
}
