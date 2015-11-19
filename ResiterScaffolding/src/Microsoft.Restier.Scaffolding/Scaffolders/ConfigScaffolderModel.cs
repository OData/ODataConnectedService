// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Globalization;

namespace Microsoft.Restier.Scaffolding
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNet.Scaffolding;
    using Microsoft.AspNet.Scaffolding.EntityFramework;
    using Microsoft.Restier.Scaffolding.UI;
    using Microsoft.Restier.Scaffolding.VisualStudio;

    public class ConfigScaffolderModel : ScaffolderModel, IScaffoldingSettings
    {
        public ConfigScaffolderModel(CodeGenerationContext context)
            : base(context)
        {
            DataContextTypes = ServiceProvider.GetService<ICodeTypeService>().GetAllCodeTypes(ActiveProject)
                .Where(codeType => codeType.IsValidDbContextType())
                .Select(ct => new ModelType(ct));


            ConfigTypes = ServiceProvider.GetService<ICodeTypeService>().GetAllCodeTypes(ActiveProject)
                .Where(codeType => codeType.IsValidConfigType())
                .Select(ct => new ModelType(ct));

            if (DataContextTypes.FirstOrDefault() == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.InvalidResource, "context class"));
            }

            if (ConfigTypes.FirstOrDefault() == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Resources.InvalidResource, "config class"));
            }
        }

        public bool IsDataContextSupported { get; set; }

        public ModelType DataContextType { get; set; }

        public string DataContextName { get; set; }

        public IEnumerable<ModelType> DataContextTypes { get; private set; }

        public ModelType ConfigType { get; set; }

        public IEnumerable<ModelType> ConfigTypes { get; private set; }

        public string ConfigFileFullPath
        {
            get
            {
                if (ConfigType == null)
                {
                    return null;
                }
                else
                {
                    return GetFileFullPath(ConfigType.ShortTypeName, CodeFileExtension);
                }
            }
        }

        public override string OutputFileFullPath
        {
            get { return ConfigFileFullPath; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "We want a consistent API for these validation methods.")]
        public string ValidateConfigName(string controllerName)
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

        public void LoadSettings(VisualStudio.IProjectSettings settings)
        {
            // TODO - Right now this class inherits from the view scaffolder model, so we need to call into the
            // base class to read those settings as well. We want to remove this inheritance at some point
            // and this method will have to change when we do.
            Contract.Assert(settings != null);

            string stringValue;

            if (settings.TryGetString(SavedSettingsKeys.DbContextTypeFullNameKey, out stringValue))
            {
                DataContextType = DataContextTypes.Where(t => String.Equals(t.TypeName, stringValue, StringComparison.Ordinal)).FirstOrDefault();
            }

            if (settings.TryGetString(SavedSettingsKeys.ConfigTypeFullNameKey, out stringValue))
            {
                ConfigType = ConfigTypes.Where(t => String.Equals(t.TypeName, stringValue, StringComparison.Ordinal)).FirstOrDefault();
            }
        }

        public void SaveSettings(VisualStudio.IProjectSettings settings)
        {
            // TODO - Right now this class inherits from the view scaffolder model, so we need to call into the
            // base class to save those settings as well. We want to remove this inheritance at some point
            // and this method will have to change when we do.

            if (DataContextType != null)
            {
                settings[SavedSettingsKeys.DbContextTypeFullNameKey] = DataContextType.TypeName;
            }

            if (ConfigType != null)
            {
                settings[SavedSettingsKeys.ConfigTypeFullNameKey] = ConfigType.TypeName;
            }
        }

        /// <summary>
        /// Determines whether the Controller exists.
        /// </summary>
        /// <param name="configName">The controller name to check.</param>
        /// <returns><see langword="true" /> if the path contains the name of an existing file;
        /// otherwise, <see langword="false" />. This method also returns <see langword="false" />
        /// if path is <see langword="null" />, an invalid path, or a zero-length string.
        /// </returns>
        public bool ConfigExists(string configName)
        {
            return File.Exists(GetFileFullPath(configName, CodeFileExtension));
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
    }
}
