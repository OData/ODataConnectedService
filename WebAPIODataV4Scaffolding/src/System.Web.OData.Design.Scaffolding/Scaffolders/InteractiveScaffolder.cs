// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web.OData.Design.Scaffolding.Telemetry;
using System.Web.OData.Design.Scaffolding.UI;
using System.Web.OData.Design.Scaffolding.VisualStudio;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.NuGet;

namespace System.Web.OData.Design.Scaffolding
{
    /// <summary>
    /// This class is for internal use only.
    /// 
    /// A base class which defines the pattern for Scaffolders that show UI and gather user input.
    /// </summary>
    /// <typeparam name="TModel">
    /// The type of the model - a data object storing the state of what code assets will be generated.
    /// </typeparam>
    /// <typeparam name="TFramework">
    /// The type of the framework - an IFrameworkDependency implementation that can configure the project.
    /// </typeparam>
    /// <remarks>
    /// This class provides access to the CodeGenerationContext and services, and a standard pattern for 
    /// displaying UI and launching code generation.
    /// 
    /// This class is generic with respect to the _type_ of asset being generated. Subclasses are responsible
    /// for creating the UI objects to be shown, and will specialize based on the type of asset being generated
    /// (eg: controllers).
    /// </remarks>
    public abstract class InteractiveScaffolder<TModel, TFramework> : CodeGenerator
        where TFramework : IFrameworkDependency
        where TModel : ScaffolderModel
    {
        private TModel _model;
        private object _viewModel;

        protected InteractiveScaffolder(CodeGenerationContext context, CodeGeneratorInformation information)
            : base(context, information)
        {
            Framework = context.Items.GetProperty<TFramework>(typeof(TFramework));
            Repository = context.Items.GetProperty<INuGetRepository>(typeof(INuGetRepository));
            VisualStudioIntegration = context.Items.GetProperty<IVisualStudioIntegration>(typeof(IVisualStudioIntegration));
        }

        protected TFramework Framework
        {
            get;
            private set;
        }

        protected INuGetRepository Repository
        {
            get;
            private set;
        }

        protected IVisualStudioIntegration VisualStudioIntegration
        {
            get;
            private set;
        }

        // Exposed for Apex test access
        public TModel Model
        {
            get
            {
                if (_model == null)
                {
                    _model = CreateModel();
                    Contract.Assert(_model != null);

                    LoadSettings(_model);
                }

                return _model;
            }
        }

        private object ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = CreateViewModel(Model);
                    Contract.Assert(_viewModel != null);
                }

                return _viewModel;
            }
        }

        protected abstract ValidatingDialogWindow CreateDialog();

        protected abstract TModel CreateModel();

        protected abstract object CreateViewModel(TModel model);

        // Exposed for unit tests
        protected internal abstract void Scaffold();

        /// <summary>
        /// The dependencies added to the package list in this function will be installed before the actual scaffolding operation.
        /// This list should contain all the packages required for succesfully running the scaffolding operation.
        /// </summary>
        /// <param name="packages">List of packages to be installed before the scaffolding operation.</param>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This is an internal API.")]
        protected internal virtual void AddScaffoldDependencies(List<NuGetPackage> packages)
        {
            // intentionally empty
        }

        /// <summary>
        /// The dependencies added to the package list in this function will be installed after the actual scaffolding operation.
        /// This list should contain all the packages required at runtime to run the product code successfully.
        /// </summary>
        /// <param name="packages">List of packages to be installed after the scaffolding operation.</param>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "This is an internal API.")]
        protected internal virtual void AddRuntimePackages(List<NuGetPackage> packages)
        {
            // intentionally empty
        }

        protected string[] GetSearchFolders(string templateFolderName)
        {
            if (templateFolderName == null)
            {
                throw new ArgumentNullException("templateFolderName");
            }

            return (new string[]
            {
                Path.Combine(TemplateSearchDirectories.GetProjectTemplateRoot(Context.ActiveProject), templateFolderName),
                Path.Combine(TemplateSearchDirectories.InstalledTemplateRoot, templateFolderName),
            }).Where(folder => Directory.Exists(folder)).ToArray();
        }

        public sealed override bool ShowUIAndValidate()
        {
            // The dialogs that inherit from VsPlatformDialog cannot be re-used, they have to be created anew each time.
            ValidatingDialogWindow dialog = CreateDialog();
            dialog.DataContext = ViewModel;

            bool? isOk = dialog.ShowModal();

            // We need to unhook the view model to prevent binding from conflicts/contention between multiple
            // dialogs.
            dialog.DataContext = null;

            return isOk == true;
        }

        public sealed override IEnumerable<NuGetPackage> Dependencies
        {
            get
            {
                List<NuGetPackage> packages = new List<NuGetPackage>();
                AddScaffoldDependencies(packages);
                return packages;
            }
        }

        private IEnumerable<NuGetPackage> RuntimePackages
        {
            get
            {
                List<NuGetPackage> packages = new List<NuGetPackage>();

                // The full set of packages is the set required by the framework dependency + any additional
                // dependencies required by the scaffolder.
                //
                // The most common cases are:
                // 1. Everything is already installed
                // 2. The framework is not installed
                // 3. The framework is installed, but some extras are needed (usually by views/EF)

                if (!Framework.IsDependencyInstalled(Context))
                {
                    // Install is needed, add all of the dependency packages
                    packages.AddRange(Framework.GetRequiredPackages(Context));
                }

                AddRuntimePackages(packages);
                return packages;
            }
        }

        public sealed override void GenerateCode()
        {
            FrameworkDependencyStatus dependencyStatus = FrameworkDependencyStatus.InstallNotNeeded;
            if (Framework.IsDependencyInstalled(Context))
            {
                Context.AddTelemetryData(TelemetrySharedKeys.DependencyScaffolderOptions, (uint)DependencyScaffolderOptions.AlreadyInstalled);
            }
            else
            {
                dependencyStatus = Framework.EnsureDependencyInstalled(Context);
            }

            Scaffold();

            if (dependencyStatus.IsNewDependencyInstall)
            {
                Framework.UpdateConfiguration(Context);
            }

            if (Model.OutputFileFullPath != null)
            {
                VisualStudioIntegration.Editor.OpenFileInEditor(Model.OutputFileFullPath);
            }

            // The readme is opened after the output file if needed, we want it to stay on top
            if (dependencyStatus.IsReadmeRequired)
            {
                VisualStudioIntegration.Editor.CreateAndOpenReadme(dependencyStatus.ReadmeText);
            }

            // Settings are only saved if scaffolding is successful
            SaveSettings(Model);

            // Adding the list of all the runtime dependency packages to the Context. Scaffolding core will install these
            // dependencies after the GenerateCode function is executed.
            foreach (NuGetPackage package in RuntimePackages)
            {
                Context.Packages.Add(package);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to fail scaffolding if we can't read settings.")]
        private void LoadSettings(TModel model)
        {
            Contract.Assert(model != null);

            // Some models persist settings, this is an optional feature
            IScaffoldingSettings modelSettings = model as IScaffoldingSettings;
            if (modelSettings != null && VisualStudioIntegration != null)
            {
                // The project settings will be null if the project doesn't implement settings (project systems are
                // extensible).
                IProjectSettings projectSettings = VisualStudioIntegration.GetProjectSettings(Context.ActiveProject);
                if (projectSettings != null)
                {
                    try
                    {
                        modelSettings.LoadSettings(projectSettings);
                    }
                    catch (Exception ex)
                    {
                        // We don't want to make it a blocking issue if we're unable to load settings.
                        Debug.Fail("Failed to load settings\r\n" + ex.Message);
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We don't want to fail scaffolding if we can't save settings.")]
        private void SaveSettings(TModel model)
        {
            Contract.Assert(model != null);

            IScaffoldingSettings modelSettings = model as IScaffoldingSettings;
            if (modelSettings != null && VisualStudioIntegration != null)
            {
                // The project settings will be null if the project doesn't implement settings (project systems are
                // extensible).
                IProjectSettings projectSettings = VisualStudioIntegration.GetProjectSettings(Context.ActiveProject);
                if (projectSettings != null)
                {
                    try
                    {
                        modelSettings.SaveSettings(projectSettings);
                    }
                    catch (Exception ex)
                    {
                        // We don't want to make it a blocking issue if we're unable to save settings.
                        Debug.Fail("Failed to save settings", ex.Message);
                    }
                }
            }
        }
    }
}
