//---------------------------------------------------------------------------------
// <copyright file="OperationImportsViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class OperationImportsViewModel: ConnectedServiceWizardPage
    {
        private bool _isSupportedVersion;

        public UserSettings UserSettings { get; set; }

        internal bool IsEntered;

        public OperationImportsViewModel(UserSettings userSettings = null) : base()
        {
            Title = "Function/Action Imports";
            Description = "Select function and action imports to include in the generated code.";
            Legend = "Function/Action Imports Selection";
            OperationImports = new List<OperationImportModel>();
            IsSupportedODataVersion = true;
            this.UserSettings = userSettings;
        }

        public IEnumerable<OperationImportModel> OperationImports { get; set; }
        /// <summary>
        /// Whether the connected service supports operation selection feature for the current OData version, default is true
        /// </summary>
        public bool IsSupportedODataVersion
        {
            get
            {
                return _isSupportedVersion;
            }
            set
            {
                _isSupportedVersion = value;
                OnPropertyChanged("VersionWarningVisibility");
            }
        }

        public Visibility VersionWarningVisibility
        {
            get
            {
                return IsSupportedODataVersion ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public IEnumerable<string> ExcludedOperationImportsNames
        {
            get
            {
                return OperationImports.Where(o => !o.IsSelected).Select(o => o.Name);
            }
        }

        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            this.IsEntered = true;
            await base.OnPageEnteringAsync(args);
            this.View = new OperationImports { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            SaveToUserSettings();
            return await base.OnPageLeavingAsync(args);
        }

        /// <summary> 
        /// Loads operation imports except the ones that require a type that is excluded
        /// </summary>
        /// <param name="operationImports">a list of all the operation imports.</param>
        /// <param name="excludedSchemaTypes">A collection of schema types that will be excluded from generated code.</param>
        /// <param name="schemaTypeModels">a dictionary of schema type and the associated schematypemodel.</param>
        public void LoadOperationImports(IEnumerable<IEdmOperationImport> operationImports, ICollection<string> excludedSchemaTypes, IDictionary<string, SchemaTypeModel> schemaTypeModels)
        {
            var toLoad = new List<OperationImportModel>();
            var alreadyAdded = new HashSet<string>();

            foreach (var operation in operationImports)
            {
                if (!alreadyAdded.Contains(operation.Name))
                {
                    var operationImportModel = new OperationImportModel()
                    {
                        Name = operation.Name,
                        IsSelected = IsOperationImportIncluded(operation, excludedSchemaTypes)
                    };

                    operationImportModel.PropertyChanged += (s, args) =>
                    {
                        IEnumerable<IEdmOperationParameter> parameters = operation.Operation.Parameters;

                        foreach (var parameter in parameters)
                        {
                            if (schemaTypeModels.TryGetValue(parameter.Type.FullName(), out SchemaTypeModel model) && !model.IsSelected)
                            {
                                model.IsSelected = (s as OperationImportModel).IsSelected;
                            }
                        }

                        string returnTypeName = operation.Operation.ReturnType?.FullName();

                        if(returnTypeName != null && schemaTypeModels.TryGetValue(returnTypeName, out SchemaTypeModel schemaTypeModel) && !schemaTypeModel.IsSelected)
                        {
                            schemaTypeModel.IsSelected = (s as OperationImportModel).IsSelected;
                        }
                    };
                    toLoad.Add(operationImportModel);

                    alreadyAdded.Add(operation.Name);
                }
            }

            OperationImports = toLoad.OrderBy(o => o.Name).ToList();
        }

        /// <summary> 
        /// Checks if the operation import should be included
        /// </summary>
        /// <param name="operationImport">operation import.</param>
        /// <param name="excludedTypes">A collection of excluded types.</param>
        /// <returns>true if the operation import should be included, otherwise false.<returns>
        public bool IsOperationImportIncluded(IEdmOperationImport operationImport, ICollection<string> excludedTypes)
        {
            IEnumerable<IEdmOperationParameter> parameters = operationImport.Operation.Parameters;

            foreach (var parameter in parameters)
            {
                if (excludedTypes.Contains(parameter.Type.FullName()))
                {
                    return false;
                }
            }

            string returnType = operationImport.Operation.ReturnType?.FullName();

            if (excludedTypes.Contains(returnType))
            {
                return false;
            }

            return true;
        }

        public void ExcludeOperationImports(IEnumerable<string> operationsToExclude)
        {
            foreach (var operationModel in OperationImports)
            {
                operationModel.IsSelected = !operationsToExclude.Contains(operationModel.Name);
            }
        }

        public void EmptyList()
        {
            OperationImports = Enumerable.Empty<OperationImportModel>();
        }

        public void SelectAll()
        {
            foreach (var operation in OperationImports)
            {
                operation.IsSelected = true;
            }
        }

        public void UnselectAll()
        {
            foreach (var operation in OperationImports)
            {
                operation.IsSelected = false;
            }
        }

        private void SaveToUserSettings()
        {
            if (this.UserSettings != null)
            {
                UserSettings.ExcludedOperationImports = this.ExcludedOperationImportsNames?.Any() == true
                    ? this.ExcludedOperationImportsNames.ToList()
                    : new List<string>();
            }
        }

        public void LoadFromUserSettings()
        {
            if (UserSettings != null)
            {
                if (UserSettings.ExcludedOperationImports?.Any() == true)
                {
                    ExcludeOperationImports(UserSettings.ExcludedOperationImports ?? Enumerable.Empty<string>());
                }
            }
        }
    }
}
