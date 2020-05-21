//---------------------------------------------------------------------------------
// <copyright file="OperationImportsViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
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
    internal class BoundOperationsViewModel : ConnectedServiceWizardPage
    {
        private bool _isSupportedVersion;

        public UserSettings UserSettings { get; set; }

        internal bool IsEntered;

        public BoundOperationsViewModel(UserSettings userSettings = null) : base()
        {
            Title = "Bound Functions/Actions";
            Description = "Select bound functions and actions to include in the generated code.";
            Legend = "Bound Functions/Actions Selection";
            BoundOperations = new List<BoundOperationModel>();
            IsSupportedODataVersion = true;
            this.UserSettings = userSettings;
        }

        public IEnumerable<BoundOperationModel> BoundOperations { get; set; }

        /// <summary>
        /// Whether the connected service supports bound operation selection feature for the current OData version, default is true
        /// </summary>
        public bool IsSupportedODataVersion
        {
            get => _isSupportedVersion;
            set
            {
                _isSupportedVersion = value;
                OnPropertyChanged(nameof(VersionWarningVisibility));
            }
        }

        public Visibility VersionWarningVisibility => IsSupportedODataVersion ? Visibility.Hidden : Visibility.Visible;

        public IEnumerable<string> ExcludedBoundOperationsNames
        {
            get
            {
                return BoundOperations.Where(o => !o.IsSelected).Select(o => o.Name);
            }
        }

        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            this.IsEntered = true;
            await base.OnPageEnteringAsync(args);
            this.View = new BoundOperations { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
            if (this.View is BoundOperations view)
            {
                view.SelectedBoundOperationsCount.Text = BoundOperations.Count(x => x.IsSelected).ToString();
            }
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            SaveToUserSettings();
            return await base.OnPageLeavingAsync(args);
        }

        /// <summary>
        /// Loads bound operations except the ones that require a type that is excluded
        /// </summary>
        /// <param name="boundOperations">a list of all the bound operations.</param>
        /// <param name="excludedSchemaTypes">A collection of schema types that will be excluded from generated code.</param>
        /// <param name="schemaTypeModels">a dictionary of schema type and the associated schematypemodel.</param>
        public void LoadBoundOperations(IDictionary<IEdmType, List<IEdmOperation>> boundOperations, ICollection<string> excludedSchemaTypes, IDictionary<string, SchemaTypeModel> schemaTypeModels)
        {
            var toLoad = new List<BoundOperationModel>();
            var alreadyAdded = new HashSet<string>();

            foreach (KeyValuePair<IEdmType,List<IEdmOperation>> boundOperation in boundOperations)
            {
                IEdmType edmType = boundOperation.Key;
                foreach (IEdmOperation operation in boundOperation.Value)
                {
                    string name = $"{operation.Name}({edmType.FullTypeName()})";
                    if (!alreadyAdded.Contains(name))
                    {
                        var boundOperationModel = new BoundOperationModel
                        {
                            Name = name,
                            IsSelected = IsBoundOperationIncluded(operation, excludedSchemaTypes)
                        };

                        boundOperationModel.PropertyChanged += (s, args) =>
                        {
                            if (s is BoundOperationModel currentBoundOperationModel)
                            {
                                IEnumerable<IEdmOperationParameter> parameters = operation.Parameters;

                                foreach (var parameter in parameters)
                                {
                                    var parameterTypeName = parameter.Type.IsCollection()
                                        ? parameter.Type.AsCollection()?.ElementType()?.FullName()
                                        : parameter.Type.FullName();

                                    if (parameterTypeName != null && schemaTypeModels.TryGetValue(parameterTypeName, out SchemaTypeModel model) && !model.IsSelected)
                                    {
                                        model.IsSelected = currentBoundOperationModel.IsSelected;
                                    }
                                }

                                string returnTypeName = operation.ReturnType?.IsCollection() == true
                                    ? operation.ReturnType?.AsCollection()?.ElementType()?.FullName()
                                    : operation.ReturnType?.FullName();

                                if (returnTypeName != null && schemaTypeModels.TryGetValue(returnTypeName, out SchemaTypeModel schemaTypeModel) && !schemaTypeModel.IsSelected)
                                {
                                    schemaTypeModel.IsSelected = currentBoundOperationModel.IsSelected;
                                }

                                if (this.View is BoundOperations view)
                                {
                                    view.SelectedBoundOperationsCount.Text = BoundOperations.Count(x => x.IsSelected).ToString();
                                }
                            }
                        };
                        toLoad.Add(boundOperationModel);

                        alreadyAdded.Add(name);
                    }
                }
            }

            BoundOperations = toLoad.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Checks if the bound operation should be included
        /// </summary>
        /// <param name="operation">bound operation.</param>
        /// <param name="excludedTypes">A collection of excluded types.</param>
        /// <returns>true if the bound operation should be included, otherwise false.</returns>
        public bool IsBoundOperationIncluded(IEdmOperation operation, ICollection<string> excludedTypes)
        {
            IEnumerable<IEdmOperationParameter> parameters = operation.Parameters;

            foreach (var parameter in parameters)
            {
                var parameterType = parameter.Type?.IsCollection() == true
                    ? parameter.Type?.AsCollection()?.ElementType()?.FullName()
                    : parameter.Type?.FullName();

                if (excludedTypes.Contains(parameterType))
                {
                    return false;
                }
            }

            var returnType = operation.ReturnType?.IsCollection() == true
                ? operation.ReturnType?.AsCollection()?.ElementType()?.FullName()
                : operation.ReturnType?.FullName();

            if (excludedTypes.Contains(returnType))
            {
                return false;
            }

            return true;
        }

        public void ExcludeBoundOperations(IEnumerable<string> operationsToExclude)
        {
            foreach (var operationModel in BoundOperations)
            {
                operationModel.IsSelected = !operationsToExclude.Contains(operationModel.Name);
            }
        }

        public void EmptyList()
        {
            BoundOperations = Enumerable.Empty<BoundOperationModel>();
        }

        public void SelectAll()
        {
            foreach (var operation in BoundOperations)
            {
                operation.IsSelected = true;
            }
        }

        public void DeselectAll()
        {
            foreach (var operation in BoundOperations)
            {
                operation.IsSelected = false;
            }
        }

        private void SaveToUserSettings()
        {
            if (this.UserSettings != null)
            {
                UserSettings.ExcludedBoundOperations = this.ExcludedBoundOperationsNames?.Any() == true
                    ? this.ExcludedBoundOperationsNames.ToList()
                    : new List<string>();
            }
        }

        public void LoadFromUserSettings()
        {
            if (UserSettings != null)
            {
                if (UserSettings.ExcludedBoundOperations?.Any() == true)
                {
                    ExcludeBoundOperations(UserSettings.ExcludedBoundOperations ?? Enumerable.Empty<string>());
                }
            }
        }
    }
}
