// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.OData.Design.Scaffolding.UI;
using System.Web.OData.Design.Scaffolding.VisualStudio;
using EnvDTE;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.EntityFramework;

namespace System.Web.OData.Design.Scaffolding
{
    public class MvcViewScaffolderModel : ScaffolderModel, IScaffoldingSettings
    {
        private const string DefaultViewNameKey = "DefaultViewName";
        private const string ControllerFolderNameKey = "ControllerFolderName";

        public MvcViewScaffolderModel(CodeGenerationContext context)
            : base(context)
        {
            // These are the defaults for these settings, they may be overridden by saved settings
            IsLayoutPageSelected = true;
            IsPartialViewSelected = false;
            IsReferenceScriptLibrariesSelected = true;

            // For Views and Controllers, the Area name can be inferred from where in the tree the
            // scaffolder was launched. This will also work in the case that the 'Add View' context
            // menu item is invoked, because the folder containing the controller will be the ActiveProjectItem
            AreaName = GetAreaNameFromSelection(context.ActiveProjectItem);

            ViewTemplates = Enumerable.Empty<ViewTemplate>();

            ModelTypes = ServiceProvider.GetService<ICodeTypeService>().GetAllCodeTypes(ActiveProject)
                                                                 .Where(codeType => codeType.IsValidWebProjectEntityType())
                                                                 .Select(ct => new ModelType(ct));

            DataContextTypes = ServiceProvider.GetService<ICodeTypeService>().GetAllCodeTypes(ActiveProject)
                                                                 .Where(codeType => codeType.IsValidDbContextType())
                                                                 .Select(ct => new ModelType(ct));

            // This is true for the view scaffolder - the controller scaffolder model sets this to
            // false by default. This will be removed when we split the controller model (which currently
            // inherits from this class).
            IsModelClassSupported = true;

            // When invoked from the 'Add View' context menu in the editor, we can pre-populate the output
            // path and view name based on the controller and action that invoked it.
            string value;
            if (context.Items.TryGetProperty<string>(DefaultViewNameKey, out value))
            {
                ViewName = value;
            }
            else
            {
                ViewName = GetGeneratedName(MvcProjectUtil.ViewName, ViewFileExtension);
            }

            if (context.Items.TryGetProperty<string>(ControllerFolderNameKey, out value) && value != null)
            {
                SelectionRelativePath = Path.Combine(AreaRelativePath, CommonFolderNames.Views, value);
            }
        }

        public bool IsModelClassSupported { get; set; }

        public bool IsDataContextSupported { get; set; }

        public override string OutputFileFullPath
        {
            get { return ViewFileFullPath; }
        }

        public bool IsLayoutPageSelected { get; set; }

        public bool IsReferenceScriptLibrariesSelected { get; set; }

        public string LayoutPageFile { get; set; }

        public ModelType ModelType { get; set; }

        public IEnumerable<ModelType> ModelTypes { get; private set; }

        public ModelType DataContextType { get; set; }

        public string DataContextName { get; set; }

        public IEnumerable<ModelType> DataContextTypes { get; private set; }

        public bool IsPartialViewSelected { get; set; }

        /// <summary>
        /// Gets or sets the View Name (used in the generated filename)
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Gets or sets the list of available templates.
        /// </summary>
        public IEnumerable<ViewTemplate> ViewTemplates { get; set; }

        public string ViewFileExtension
        {
            get
            {
                ProjectLanguage language = ActiveProject.GetCodeLanguage();
                return MvcProjectUtil.GetViewFileExtension(language);
            }
        }

        /// <summary>
        /// Gets the full path of the code file containing the generated View, or null if the ViewName is null.
        /// </summary>
        /// <remarks>
        /// The MvcControllerScaffolderModel inherits from this class, but doesn't make use of the ViewName property.
        /// </remarks>
        public string ViewFileFullPath
        {
            get
            {
                if (ViewName == null)
                {
                    return null;
                }
                else
                {
                    return GetFileFullPath(ViewName, ViewFileExtension);
                }
            }
        }

        public virtual void LoadSettings(IProjectSettings settings)
        {
            Contract.Assert(settings != null);

            bool boolValue;
            if (settings.TryGetBool(SavedSettingsKeys.IsLayoutPageSelectedKey, out boolValue))
            {
                IsLayoutPageSelected = boolValue;
            }

            if (settings.TryGetBool(SavedSettingsKeys.IsPartialViewSelectedKey, out boolValue))
            {
                IsPartialViewSelected = boolValue;
            }

            if (settings.TryGetBool(SavedSettingsKeys.IsReferencingScriptLibrariesSelectedKey, out boolValue))
            {
                IsReferenceScriptLibrariesSelected = boolValue;
            }

            string stringValue;
            if (settings.TryGetString(SavedSettingsKeys.LayoutPageFileKey, out stringValue))
            {
                LayoutPageFile = stringValue;
            }

            if (IsModelClassSupported && settings.TryGetString(SavedSettingsKeys.DbContextTypeFullNameKey, out stringValue))
            {
                DataContextType = DataContextTypes.Where(t => String.Equals(t.TypeName, stringValue, StringComparison.Ordinal)).FirstOrDefault();
            }
        }

        public virtual void SaveSettings(IProjectSettings settings)
        {
            settings.SetBool(SavedSettingsKeys.IsLayoutPageSelectedKey, IsLayoutPageSelected);
            settings.SetBool(SavedSettingsKeys.IsPartialViewSelectedKey, IsPartialViewSelected);
            settings.SetBool(SavedSettingsKeys.IsReferencingScriptLibrariesSelectedKey, IsReferenceScriptLibrariesSelected);

            if (IsLayoutPageSelected)
            {
                settings[SavedSettingsKeys.LayoutPageFileKey] = LayoutPageFile;
            }
            else
            {
                settings[SavedSettingsKeys.LayoutPageFileKey] = null;
            }

            if (IsModelClassSupported && DataContextType != null)
            {
                settings[SavedSettingsKeys.DbContextTypeFullNameKey] = DataContextType.TypeName;
            }
        }

        /// <summary>
        /// This function is used for generating distinct view-name and controller-name. This function is
        /// called by View Scaffolder and Controller Scaffolder for setting the model view-name and model 
        /// controller-name.
        /// </summary>
        public string GetGeneratedName(string resourceName, string fileExtension)
        {
            int i = 1;
            string fileName = String.Format(CultureInfo.InvariantCulture, resourceName, String.Empty);
            while (true)
            {
                Debug.Assert(fileName != resourceName);
                if (!File.Exists(Path.Combine(SelectionFullPath, fileName + "." + fileExtension)))
                {
                    return fileName;
                }

                fileName = String.Format(CultureInfo.InvariantCulture, resourceName, i++);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "We want a consistent API for these validation methods.")]
        public string ValidateModelType(ModelType modelType)
        {
            // TODO: this is not a complete validation
            if (modelType == null)
            {
                return Resources.EmptyModelName;
            }

            return null;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "We want a consistent API for these validation methods.")]
        public string ValidateDataContextType(ModelType dataContextType)
        {
            // TODO: this is not a complete validation
            if (dataContextType == null)
            {
                return Resources.EmptyDbContextName;
            }

            return null;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "We want a consistent API for these validation methods.")]
        public string ValidateDbContextName(string dbContextName)
        {
            if (String.IsNullOrWhiteSpace(dbContextName))
            {
                return Resources.EmptyDbContextName;
            }

            return null;
        }

        /// <summary>
        /// This function attempts to determine the Area name based on the user selection. The Area name is
        /// used as a parameter to Controller generation.
        /// </summary>
        /// <remarks>
        //  User selection:
        //  \ --> "" 
        //  Cool\Awesome --> ""
        //  Areas --> ""
        //  Areasabc --> ""
        //  Areas51  --> ""
        //  Areas\Admin --> Admin
        //  Areas\Admin\Views --> Admin 
        /// </remarks>
        private static string GetAreaNameFromSelection(ProjectItem projectItem)
        {
            if (projectItem == null)
            {
                return String.Empty;
            }

            string selectionRelativePath = projectItem.GetProjectRelativePath();

            // check the first 6 characters to see if it matches with 'Areas\' string.
            string areasPath = CommonFolderNames.Areas + MvcProjectUtil.PathSeparator;
            bool isInArea = selectionRelativePath.StartsWith(areasPath, StringComparison.OrdinalIgnoreCase);

            if (isInArea)
            {
                string stringWithoutAreas = selectionRelativePath.Remove(0, areasPath.Length);
                int lastPosition = stringWithoutAreas.IndexOf(MvcProjectUtil.PathSeparator, StringComparison.OrdinalIgnoreCase);

                if (lastPosition != -1)
                {
                    return stringWithoutAreas.Substring(0, lastPosition);
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Function generates the default DataContext name.
        /// </summary>
        public string GenerateDefaultDataContextTypeName()
        {
            Project project = ActiveProject;
            string modelsNamespace = MvcProjectUtil.GetDefaultModelsNamespace(project.GetDefaultNamespace());
            CodeDomProvider provider = ValidationUtil.GenerateCodeDomProvider(project.GetCodeLanguage());
            // CreateValidIdentifier considers dot as a valid identifier hence replacing the dot explicitly.
            string projectName = provider.CreateValidIdentifier(project.GetDefaultNamespace().Replace(".", String.Empty));
            DataContextName = modelsNamespace + "." + projectName + MvcProjectUtil.DataContextSuffix;

            return DataContextName;
        }
    }
}
