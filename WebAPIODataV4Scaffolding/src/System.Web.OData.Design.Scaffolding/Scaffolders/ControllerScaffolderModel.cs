// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.OData.Design.Scaffolding.VisualStudio;
using Microsoft.AspNet.Scaffolding;
using Microsoft.AspNet.Scaffolding.EntityFramework;


namespace System.Web.OData.Design.Scaffolding
{
    public class ControllerScaffolderModel : MvcViewScaffolderModel
    {
        public ControllerScaffolderModel(CodeGenerationContext context)
            : base(context)
        {
            IsModelClassSupported = false;
            IsAsyncSupported = IsUsingEntityFrameworkWithAsyncSupport();
        }

        public override string OutputFileFullPath
        {
            get { return ControllerFileFullPath; }
        }

        public string ControllerName { get; set; }

        public bool IsViewGenerationSupported { get; set; }

        public bool IsViewGenerationSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Views/[Controller Name] folder should be created
        /// during scaffolding.
        /// </summary>
        public bool IsViewFolderRequired { get; set; }

        public bool IsAsyncSelected { get; set; }

        public bool IsAsyncSupported { get; set; }

        /// <summary>
        /// Gets the full path of the code file containing the generated Controller, or null if the ControllerName is null.
        /// </summary>
        public string ControllerFileFullPath
        {
            get
            {
                if (ControllerName == null)
                {
                    return null;
                }
                else
                {
                    return GetFileFullPath(ControllerName, CodeFileExtension);
                }
            }
        }

        /// <summary>
        /// If the user selects the root project and adds the controller for the first time when the Controllers folder is created
        /// then the namespace will be the appended with controller folder name, else Project namespace will be returned. If the user
        /// selects anywhere else Project Item's namespace will be returned.
        /// </summary>
        public string ControllerNamespace
        {
            get
            {
                if (ActiveProjectItem == null)
                {
                    if (IsControllersFolderCreated())
                    {
                        return ActiveProject.GetDefaultNamespace() + "." + CommonFolderNames.Controllers;
                    }

                    return ActiveProject.GetDefaultNamespace();
                }
                else
                {
                    return ActiveProjectItem.GetDefaultNamespace();
                }
            }
        }

        public string ControllerRootName
        {
            get
            {
                return MvcProjectUtil.StripControllerName(ControllerName);
            }
        }

        /// <summary>
        /// Selection relative path can be only set when the project item is not selected and Controllers folders is
        /// created for the first time.
        /// </summary>
        public new string SelectionRelativePath
        {
            get
            {
                return base.SelectionRelativePath;
            }

            set
            {
                if (ActiveProjectItem == null && IsControllersFolderCreated())
                {
                    base.SelectionRelativePath = value;
                }
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "We want a consistent API for these validation methods.")]
        public string ValidateControllerName(string controllerName)
        {
            if (String.IsNullOrWhiteSpace(controllerName))
            {
                return Resources.EmptyControllerName;
            }

            if (String.Equals(controllerName, MvcProjectUtil.ControllerSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return Resources.InvalidIdentifierReservedName;
            }

            return null;
        }

        public override void LoadSettings(VisualStudio.IProjectSettings settings)
        {
            // TODO - Right now this class inherits from the view scaffolder model, so we need to call into the
            // base class to read those settings as well. We want to remove this inheritance at some point
            // and this method will have to change when we do.
            base.LoadSettings(settings);

            bool value;

            if (IsViewGenerationSupported)
            {
                if (settings.TryGetBool(SavedSettingsKeys.IsViewGenerationSelectedKey, out value))
                {
                    IsViewGenerationSelected = value;
                }
                else
                {
                    // default to true if views are supported and we have no saved state
                    IsViewGenerationSelected = true;
                }
            }

            if (IsAsyncSupported)
            {
                if (settings.TryGetBool(SavedSettingsKeys.IsAsyncSelectedKey, out value))
                {
                    IsAsyncSelected = value;
                }
            }
        }

        public override void SaveSettings(VisualStudio.IProjectSettings settings)
        {
            // TODO - Right now this class inherits from the view scaffolder model, so we need to call into the
            // base class to save those settings as well. We want to remove this inheritance at some point
            // and this method will have to change when we do.
            base.SaveSettings(settings);

            // We only want to save this value if this scaffolder uses views.
            //
            // Ex: Using the 'empty controller' scaffolder and then going back to 'Controller + EF + Views' should
            // not uncheck the box for you.
            if (IsViewGenerationSupported)
            {
                settings.SetBool(SavedSettingsKeys.IsViewGenerationSelectedKey, IsViewGenerationSelected);
            }

            if (IsAsyncSupported)
            {
                settings.SetBool(SavedSettingsKeys.IsAsyncSelectedKey, IsAsyncSelected);
            }
        }

        /// <summary>
        /// Determines whether the Controller exists.
        /// </summary>
        /// <param name="controllerName">The controller name to check.</param>
        /// <returns><see langword="true" /> if the path contains the name of an existing file;
        /// otherwise, <see langword="false" />. This method also returns <see langword="false" />
        /// if path is <see langword="null" />, an invalid path, or a zero-length string.
        /// </returns>
        public bool ControllerExists(string controllerName)
        {
            return File.Exists(GetFileFullPath(controllerName, CodeFileExtension));
        }

        private bool IsUsingEntityFrameworkWithAsyncSupport()
        {
            foreach (string assemblyPath in ActiveProject.GetAssemblyReferences())
            {
                AssemblyName assembly = AssemblyName.GetAssemblyName(assemblyPath);
                if (assembly.Name.Equals(AssemblyVersions.AsyncEntityFrameworkAssemblyName))
                {
                    return assembly.Version >= AssemblyVersions.AsyncEntityFrameworkMinVersion;
                }
            }   
            // Since no reference to Entity Framework exists, the correct version will be added later by the system.
            return true;
        }

        public string GenerateControllerName(string modelClassName)
        {
            if (String.IsNullOrWhiteSpace(modelClassName))
            {
                return null;
            }

            IEntityFrameworkService efService = Context.ServiceProvider.GetService<IEntityFrameworkService>();
            string controllerRootName = efService.GetPluralizedWord(modelClassName, CultureInfo.GetCultureInfo(1033));

            if (controllerRootName == null)
            {
                controllerRootName = modelClassName;
            }

            return GetGeneratedName(controllerRootName + "{0}" + MvcProjectUtil.ControllerSuffix, CodeFileExtension);
        }

        private bool IsControllersFolderCreated()
        {
            object isControllersFolderCreated;
            Context.Items.TryGetProperty(ContextKeys.IsControllersFolderCreated, out isControllersFolderCreated);

            return isControllersFolderCreated == null ? false : (bool)isControllersFolderCreated;
        }
    }
}
