//---------------------------------------------------------------------------------
// <copyright file="OperationImportsViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class OperationImportsViewModel: ConnectedServiceWizardPage
    {
        private bool _isSupportedVersion;

        /// <summary>
        /// User settings.
        /// </summary>
        /// <remarks>
        /// <see cref="Models.UserSettings"/>
        /// </remarks>
        public UserSettings UserSettings { get; internal set; }

        private long _operationImportsCount = 0;

        /// <summary>
        /// Gets the operation imports count.
        /// </summary>
        public long OperationImportsCount => this._operationImportsCount;

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

        /// <summary>
        /// A list of operation imports.
        /// </summary>
        public IEnumerable<OperationImportModel> OperationImports { get; set; }

        private string _searchText;

        /// <summary>
        /// Text to filter displayed operation imports.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;

                this.OnPropertyChanged(nameof(SearchText));
                this.OnPropertyChanged(nameof(FilteredOperationImports));
            }
        }

        /// <summary>
        /// Gets the operation imports that start with the search text.
        /// </summary>
        public IEnumerable<OperationImportModel> FilteredOperationImports
        {
            get
            {
                return SearchText == null
                    ? OperationImports
                    : OperationImports.Where(x => x.Name.StartsWith(SearchText, StringComparison.InvariantCultureIgnoreCase));
            }
        }

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
                OnPropertyChanged(nameof(VersionWarningVisibility));
            }
        }

        public Visibility VersionWarningVisibility
        {
            get
            {
                return IsSupportedODataVersion ? Visibility.Hidden : Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the operation imports that will be excluded while generating code.
        /// </summary>
        public IEnumerable<string> ExcludedOperationImportsNames
        {
            get
            {
                return OperationImports.Where(o => !o.IsSelected).Select(o => o.Name);
            }
        }

        public event EventHandler<EventArgs> PageEntering;

        /// <summary>
        /// Executed when entering the page for selecting operation imports.
        /// It fires PageEntering event which ensures all types are loaded on the UI.
        /// It also ensures all related entities are computed and cached.
        /// </summary>
        /// <param name="args">Event arguments being passed to the method.</param>
        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            this.IsEntered = true;
            await base.OnPageEnteringAsync(args).ConfigureAwait(false);
            this.View = new OperationImports { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
            if (this.View is OperationImports view)
            {
                view.SelectedOperationImportsCount.Text = OperationImports.Count(x => x.IsSelected).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Executed when leaving the page for selecting operation imports.
        /// </summary>
        /// <param name="args">Event arguments being passed to the method.</param>
        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            SaveToUserSettings();
            return await base.OnPageLeavingAsync(args).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads operation imports except the ones that require a type that is excluded.
        /// </summary>
        /// <param name="operationImports">A list of all the operation imports.</param>
        /// <param name="excludedSchemaTypes">A collection of schema types that will be excluded from generated code.</param>
        /// <param name="schemaTypeModels">A dictionary of schema type and the associated schematypemodel.</param>
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
                        ReturnType = operation.Operation?.ReturnType?.FullName() ?? "void",
                        ParametersString = EdmHelper.GetParametersString(operation.Operation?.Parameters),
                        IsSelected = IsOperationImportIncluded(operation, excludedSchemaTypes)
                    };

                    operationImportModel.PropertyChanged += (s, args) =>
                    {
                        if (s is OperationImportModel currentOperationImportModel)
                        {
                            IEnumerable<IEdmOperationParameter> parameters = operation.Operation.Parameters;

                            foreach (var parameter in parameters)
                            {
                                if (schemaTypeModels.TryGetValue(parameter.Type.FullName(), out SchemaTypeModel model) && !model.IsSelected)
                                {
                                    model.IsSelected = currentOperationImportModel.IsSelected;
                                }
                            }

                            string returnTypeName = operation.Operation.ReturnType?.FullName();

                            if (returnTypeName != null && schemaTypeModels.TryGetValue(returnTypeName, out SchemaTypeModel schemaTypeModel) && !schemaTypeModel.IsSelected)
                            {
                                schemaTypeModel.IsSelected = currentOperationImportModel.IsSelected;
                            }
                        }

                        if (this.View is OperationImports view)
                        {
                            view.SelectedOperationImportsCount.Text = OperationImports.Count(x => x.IsSelected).ToString(CultureInfo.InvariantCulture);
                        }
                    };
                    toLoad.Add(operationImportModel);

                    alreadyAdded.Add(operation.Name);
                }
            }

            OperationImports = toLoad.OrderBy(o => o.Name).ToList();

            _operationImportsCount = OperationImports.Count();
        }

        /// <summary>
        /// Checks if the operation import should be included.
        /// </summary>
        /// <param name="operationImport">Operation import.</param>
        /// <param name="excludedTypes">A collection of excluded types.</param>
        /// <returns>true if the operation import should be included, otherwise false.</returns>
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

        /// <summary>
        /// Excludes the operation imports from code generation.
        /// </summary>
        /// <remarks>
        /// Sets operationModel isSelected property to be excluded to false.
        /// </remarks>
        /// <param name="operationsToExclude">A list of operation imports to be exclude.</param>
        public void ExcludeOperationImports(IEnumerable<string> operationsToExclude)
        {
            foreach (var operationModel in OperationImports)
            {
                operationModel.IsSelected = !operationsToExclude.Contains(operationModel.Name);
            }
        }

        /// <summary>
        /// Empties a list of operation imports.
        /// </summary>
        public void EmptyList()
        {
            OperationImports = Enumerable.Empty<OperationImportModel>();
            _operationImportsCount = 0;
        }

        /// <summary>
        /// Selects all operation imports.
        /// </summary>
        public void SelectAll()
        {
            foreach (var operation in OperationImports)
            {
                operation.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselect all operation imports.
        /// </summary>
        public void UnselectAll()
        {
            foreach (var operation in OperationImports)
            {
                operation.IsSelected = false;
            }
        }

        /// <summary>
        /// Save the selected operation imports to user settings.
        /// </summary>
        private void SaveToUserSettings()
        {
            if (this.UserSettings != null)
            {
                UserSettings.ExcludedOperationImports = this.ExcludedOperationImportsNames?.Any() == true
                    ? this.ExcludedOperationImportsNames.ToList()
                    : new List<string>();
            }
        }

        /// <summary>
        /// Loads operation import configurations from user settings.
        /// </summary>
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
