//---------------------------------------------------------------------------------
// <copyright file="SchemaTypesViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class SchemaTypesViewModel : ConnectedServiceWizardPage
    {
        /// <summary>
        /// User settings.
        /// </summary>
        /// <remarks>
        /// <see cref="Models.UserSettings"/>
        /// </remarks>
        public UserSettings UserSettings { get; set; }

        /// <summary>
        /// A dictionary that contains schema type full names with it's related models.
        /// </summary>
        public IDictionary<string, SchemaTypeModel> SchemaTypeModelMap { get; set; }

        /// <summary>
        /// A list of schema types.
        /// </summary>
        public IEnumerable<SchemaTypeModel> SchemaTypes { get; set; }

        private string _searchText;

        /// <summary>
        /// Text to filter displayed schema types or it's bound operations.
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;

                this.OnPropertyChanged(nameof(SearchText));
                this.OnPropertyChanged(nameof(FilteredSchemaTypes));
            }
        }

        /// <summary>
        /// Gets the schema types that start with the search text or with it's bound operations start with the search text.
        /// </summary>
        public IEnumerable<SchemaTypeModel> FilteredSchemaTypes
        {
            get
            {
                return SearchText == null
                    ? SchemaTypes
                    : SchemaTypes.Where(x =>
                        x.ShortName.StartsWith(SearchText, StringComparison.InvariantCultureIgnoreCase) ||
                        x.BoundOperations.Any(o =>
                            o.ShortName.StartsWith(SearchText, StringComparison.InvariantCultureIgnoreCase)));
            }
        }

        private long _schemaTypesCount = 0;

        /// <summary>
        /// Gets the schema types count.
        /// </summary>
        public long SchemaTypesCount => this._schemaTypesCount;

        private long _boundOperationsCount = 0;

        /// <summary>
        /// Gets the bound operations count.
        /// </summary>
        public long BoundOperationsCount => this._boundOperationsCount;

        /// <summary>
        /// Contains a collection of entities that would depend on another entity.
        /// </summary>
        public IDictionary<string, ICollection<string>> RelatedTypes { get; set; }

        /// <summary>
        /// Gets the schema types that will be excluded while generating code.
        /// </summary>
        public IEnumerable<string> ExcludedSchemaTypeNames
        {
            get
            {
                return SchemaTypes.Where(o => !o.IsSelected).Select(o => o.Name);
            }
        }

        public event EventHandler<EventArgs> PageEntering;

        public event EventHandler<EventArgs> PageLeaving;

        internal bool IsEntered;

        public SchemaTypesViewModel(UserSettings userSettings = null) : base()
        {
            Title = "Schema Types";
            Description = "Select schema types with its bound operations to include in the generated code.";
            Legend = "Schema Types Selection";
            SchemaTypes = new List<SchemaTypeModel>();
            SchemaTypeModelMap = new Dictionary<string, SchemaTypeModel>();
            RelatedTypes = new Dictionary<string, ICollection<string>>();
            this.UserSettings = userSettings;
        }

        /// <summary>
        /// Executed when entering the page for selecting schema types.
        /// It fires PageEntering event which ensures all types are loaded on the UI.
        /// It also ensures all related entities are computed and cached.
        /// </summary>
        /// <param name="args">Event arguments being passed to the method.</param>
        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            this.IsEntered = true;
            await base.OnPageEnteringAsync(args).ConfigureAwait(false);
            View = new SchemaTypes() {DataContext = this};
            PageEntering?.Invoke(this, EventArgs.Empty);
            if (this.View is SchemaTypes view)
            {
                view.SelectedSchemaTypesCount.Text = SchemaTypes.Count(x => x.IsSelected).ToString(CultureInfo.InvariantCulture);
                view.SelectedBoundOperationsCount.Text = SchemaTypes
                    .Where(x => x.IsSelected && x.BoundOperations?.Any() == true).SelectMany(x => x.BoundOperations)
                    .Count(x => x.IsSelected).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Executed when leaving the page for selecting schema types.
        /// It checks if there is a type that is required but has not been selected.
        /// If found, these types are automatically selected and the user notified.
        /// </summary>
        /// <param name="args">Event arguments being passed to the method.</param>
        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            SaveToUserSettings();

            var numberOfTypesToBeIncluded = 0;

            var correctTypeSelection = true;

            PageLeaving?.Invoke(this, EventArgs.Empty);

            // Check each excluded schema type and check if they are required. If so, then automatically select them.
            foreach (var schemaType in ExcludedSchemaTypeNames)
            {
                if (RelatedTypes.TryGetValue(schemaType, out ICollection<string> relatedTypes))
                {
                    // Check if any of the related types has been selected.
                    if (relatedTypes.Any(o =>
                    {
                        if (SchemaTypeModelMap.TryGetValue(o, out SchemaTypeModel schemaTypeModel))
                        {
                            return schemaTypeModel.IsSelected;
                        }

                        return false;
                    }))
                    {
                        if (SchemaTypeModelMap.TryGetValue(schemaType, out SchemaTypeModel schemaTypeModel))
                        {
                            schemaTypeModel.IsSelected = true;
                            correctTypeSelection = false;
                            numberOfTypesToBeIncluded++;
                        }
                    }
                }
            }

            if (!correctTypeSelection)
            {
                return await Task.FromResult(new PageNavigationResult
                {
                    ErrorMessage = $"{numberOfTypesToBeIncluded} {Constants.SchemaTypesWillAutomaticallyBeIncluded}",
                    IsSuccess = correctTypeSelection,
                    ShowMessageBoxOnFailure = true
                }).ConfigureAwait(false);
            }
            else
            {
                return await base.OnPageLeavingAsync(args).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Save the selected schema types to user settings.
        /// </summary>
        public void SaveToUserSettings()
        {
            if (this.UserSettings != null)
            {
                UserSettings.ExcludedSchemaTypes = this.ExcludedSchemaTypeNames?.Any() == true
                    ? this.ExcludedSchemaTypeNames.ToList()
                    : new List<string>();

                UserSettings.ExcludedBoundOperations = this.ExcludedBoundOperationsNames?.Any() == true
                    ? this.ExcludedBoundOperationsNames.ToList()
                    : new List<string>();
            }
        }

        /// <summary>
        /// Loads schema type configurations from user settings.
        /// </summary>
        public void LoadFromUserSettings()
        {
            if (UserSettings != null)
            {
                if (UserSettings.ExcludedSchemaTypes?.Any() == true)
                {
                    ExcludeSchemaTypes(UserSettings.ExcludedSchemaTypes ?? Enumerable.Empty<string>(), UserSettings.ExcludedBoundOperations ?? Enumerable.Empty<string>());
                }

                if (UserSettings.ExcludedBoundOperations?.Any() == true)
                {
                    foreach (var schemaType in SchemaTypes)
                    {
                        ExcludeBoundOperations(schemaType,
                            UserSettings.ExcludedBoundOperations ?? Enumerable.Empty<string>());
                    }
                }
            }
        }

        #region SchemaTypes

        /// <summary>
        /// Creates a list of types that needs to be loaded on the UI.
        /// Initially all the types are loaded.
        /// </summary>
        /// <param name="schemaTypes">A list of schema types that need to be loaded.</param>
        /// <param name="boundOperations">The associated bound operations.</param>
        public void LoadSchemaTypes(
            IEnumerable<IEdmSchemaType> schemaTypes, IDictionary<IEdmType, List<IEdmOperation>> boundOperations)
        {
            var toLoad = new List<SchemaTypeModel>();

            foreach (var type in schemaTypes)
            {
                if (!SchemaTypeModelMap.ContainsKey(type.FullName()) ||
                    SchemaTypeModelMap.Count != schemaTypes.Count())
                {
                    SchemaTypes = toLoad;
                    SchemaTypeModelMap.Clear();
                    break;
                }
            }

            if (SchemaTypes.Any())
            {
                return;
            }

            foreach (var schemaType in schemaTypes)
            {
                var schemaTypeModel = new SchemaTypeModel
                {
                    Name = schemaType.FullTypeName(),
                    ShortName = EdmHelper.GetTypeNameFromFullName(schemaType.FullTypeName())
                };

                // Create propertyChange handler.
                // Anytime a property is selected/unselected, the handler ensures
                // all the related types and operations are selected/unselected
                schemaTypeModel.PropertyChanged += (s, args) =>
                {
                    if (schemaTypeModel.IsSelected && schemaType is IEdmStructuredType structuredType)
                    {
                        // Check for the base type and automatically select it if not selected already.
                        string baseTypeFullName = structuredType.BaseType?.FullTypeName();

                        if (baseTypeFullName != null)
                        {
                            AddRelatedType(baseTypeFullName, structuredType.FullTypeName());

                            if (SchemaTypeModelMap.TryGetValue(baseTypeFullName,
                                    out SchemaTypeModel baseTypeSchemaTypeModel)
                                && !baseTypeSchemaTypeModel.IsSelected)
                            {
                                baseTypeSchemaTypeModel.IsSelected = true;
                            }
                        }

                        // Check the required property types and ensure they are selected as well.
                        foreach (var property in structuredType.DeclaredProperties)
                        {
                            IEdmTypeReference propertyType = property.Type.IsCollection()
                                ? property.Type.AsCollection().ElementType()
                                : property.Type;

                            if (propertyType.ToStructuredType() != null || propertyType.IsEnum())
                            {
                                string propertyTypeName = propertyType.ToStructuredType()?.FullTypeName() ?? propertyType.FullName();
                                AddRelatedType(propertyTypeName, structuredType.FullTypeName());

                                bool hasProperty = SchemaTypeModelMap.TryGetValue(propertyTypeName, out SchemaTypeModel propertySchemaTypeModel);

                                if (hasProperty && !propertySchemaTypeModel.IsSelected)
                                {
                                    propertySchemaTypeModel.IsSelected = true;
                                }
                            }
                        }

                        // Check for bound operations and ensure related types are also selected.
                        // In this case related types means return types and parameter types.
                        if (boundOperations.TryGetValue(structuredType, out List<IEdmOperation> operations))
                        {
                            foreach (var operation in operations)
                            {
                                // Check if return type of associated bound operation has been selected.
                                if (operation.ReturnType != null &&
                                    (operation.ReturnType.ToStructuredType() != null || operation.ReturnType.IsEnum()))
                                {
                                    string returnTypeFullName =
                                        operation.ReturnType.ToStructuredType()?.FullTypeName() ??
                                        operation.ReturnType.FullName();
                                    AddRelatedType(returnTypeFullName, structuredType.FullTypeName());

                                    if (SchemaTypeModelMap.TryGetValue(returnTypeFullName,
                                            out SchemaTypeModel referencedSchemaTypeModel)
                                        && !referencedSchemaTypeModel.IsSelected)
                                    {
                                        referencedSchemaTypeModel.IsSelected = true;
                                    }
                                }

                                // Check if parameter types of associated bound operations has been selected.
                                IEnumerable<IEdmOperationParameter> parameters = operation.Parameters;

                                foreach (var parameter in parameters)
                                {
                                    if (parameter.Type.ToStructuredType() != null || parameter.Type.IsEnum())
                                    {
                                        string parameterFullName =
                                            parameter.Type.ToStructuredType()?.FullTypeName() ??
                                            parameter.Type.FullName();
                                        AddRelatedType(parameterFullName, structuredType.FullTypeName());

                                        if (SchemaTypeModelMap.TryGetValue(parameterFullName,
                                            out SchemaTypeModel model) && !model.IsSelected)
                                        {
                                            model.IsSelected = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!schemaTypeModel.IsSelected)
                    {
                        // Automatically deselect related types for deselected type.
                        if (RelatedTypes.TryGetValue(schemaTypeModel.Name, out ICollection<string> relatedTypes))
                        {
                            foreach (var relatedType in relatedTypes)
                            {
                                if (SchemaTypeModelMap.TryGetValue(relatedType,
                                        out SchemaTypeModel relatedSchemaTypeModel)
                                    && relatedSchemaTypeModel.IsSelected)
                                {
                                    relatedSchemaTypeModel.IsSelected = false;
                                }
                            }
                        }

                        // Deselect all related bound operations.
                        foreach (var boundOperation in schemaTypeModel.BoundOperations)
                        {
                            boundOperation.IsSelected = false;
                        }
                    }

                    if (this.View is SchemaTypes view)
                    {
                        view.SelectedSchemaTypesCount.Text = SchemaTypes.Count(x => x.IsSelected).ToString(CultureInfo.InvariantCulture);
                    }
                };

                // load bound operations that require the schema type.
                var boundOperationsToLoad = boundOperations
                    .Where(x => x.Key == schemaType || x.Key.AsElementType() == schemaType)
                    .ToDictionary(x => x.Key, x => x.Value);
                LoadBoundOperations(schemaTypeModel, boundOperationsToLoad,
                    ExcludedSchemaTypeNames.ToList(), SchemaTypeModelMap);

                toLoad.Add(schemaTypeModel);
                schemaTypeModel.IsSelected = true;
                SchemaTypeModelMap.Add(schemaType.FullTypeName(), schemaTypeModel);
            }

            SchemaTypes = toLoad.OrderBy(o => o.Name).ToList();

            _schemaTypesCount = SchemaTypes.Count();
            _boundOperationsCount = SchemaTypes.Where(x => x?.BoundOperations?.Any() == true)
                .SelectMany(x => x.BoundOperations).Count();
        }

        /// <summary>
        /// Add related type and its relation to the RelatedTypes dictionary.
        /// </summary>
        /// <param name="type">The schema type full name.</param>
        /// <param name="relatedType">Related schema type full name.</param>
        public void AddRelatedType(string type, string relatedType)
        {
            if (RelatedTypes.TryGetValue(type, out ICollection<string> relatedTypes))
            {
                relatedTypes.Add(relatedType);
            }
            else
            {
                relatedTypes = new HashSet<string>() { relatedType };
                RelatedTypes.Add(type, relatedTypes);
            }
        }

        /// <summary>
        /// Sets schemaTypeModel isSelected property to be excluded to false.
        /// </summary>
        /// <param name="schemaTypesToExclude">A list of all full names of schema types to be exclude.</param>
        /// <param name="boundOperationsToExclude">A list of all full names of bound operations to be exclude.</param>
        public void ExcludeSchemaTypes(IEnumerable<string> schemaTypesToExclude, IEnumerable<string> boundOperationsToExclude)
        {
            foreach (var schemaTypeModel in SchemaTypes)
            {
                foreach (var boundOperationModel in schemaTypeModel.BoundOperations)
                {
                    boundOperationModel.IsSelected = !boundOperationsToExclude.Contains(boundOperationModel.Name);
                }

                schemaTypeModel.IsSelected = !schemaTypesToExclude.Contains(schemaTypeModel.Name);
            }
        }

        /// <summary>
        /// Empties a list of schema types.
        /// </summary>
        public void ClearSchemaTypes()
        {
            SchemaTypes = Enumerable.Empty<SchemaTypeModel>();
            _schemaTypesCount = 0;
        }

        /// <summary>
        /// Selects all schema types.
        /// </summary>
        public void SelectAllSchemaTypes()
        {
            foreach (var schemaType in SchemaTypes)
            {
                schemaType.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselect all schema types.
        /// </summary>
        public void DeselectAllSchemaTypes()
        {
            foreach (var schemaType in SchemaTypes)
            {
                schemaType.IsSelected = false;
            }
        }

        #endregion

        #region BoundOperations

        /// <summary>
        /// Gets the bound operations that will be excluded while generating code.
        /// </summary>
        public IEnumerable<string> ExcludedBoundOperationsNames
        {
            get
            {
                return SchemaTypes.SelectMany(x => x.BoundOperations)
                    .Where(o => !o.IsSelected).Select(o => o.Name).OrderBy(x => x);
            }
        }

        /// <summary>
        /// Loads bound operations except the ones that require a type that is excluded.
        /// </summary>
        /// <param name="schemaType">A schema type model.</param>
        /// <param name="boundOperations">A list of all the bound operations.</param>
        /// <param name="excludedSchemaTypes">A collection of schema types that will be excluded from generated code.</param>
        /// <param name="schemaTypeModels">A dictionary of schema type and the associated schematypemodel.</param>
        private void LoadBoundOperations(SchemaTypeModel schemaType,
            IDictionary<IEdmType, List<IEdmOperation>> boundOperations, ICollection<string> excludedSchemaTypes,
            IDictionary<string, SchemaTypeModel> schemaTypeModels)
        {
            var toLoad = new ObservableCollection<BoundOperationModel>();
            var alreadyAdded = new HashSet<string>();
            foreach (KeyValuePair<IEdmType, List<IEdmOperation>> boundOperation in boundOperations)
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
                            ShortName = operation.Name,
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

                                    if (parameterTypeName != null &&
                                        schemaTypeModels.TryGetValue(parameterTypeName, out SchemaTypeModel model) &&
                                        !model.IsSelected && model.IsSelected != currentBoundOperationModel.IsSelected)
                                    {
                                        model.IsSelected = currentBoundOperationModel.IsSelected;
                                    }
                                }

                                string returnTypeName = operation.ReturnType?.IsCollection() == true
                                    ? operation.ReturnType?.AsCollection()?.ElementType()?.FullName()
                                    : operation.ReturnType?.FullName();

                                if (returnTypeName != null &&
                                    schemaTypeModels.TryGetValue(returnTypeName, out SchemaTypeModel schemaTypeModel) &&
                                    !schemaTypeModel.IsSelected &&
                                    schemaTypeModel.IsSelected != currentBoundOperationModel.IsSelected)
                                {
                                    schemaTypeModel.IsSelected = currentBoundOperationModel.IsSelected;
                                }

                                if (this.View is SchemaTypes view)
                                {
                                    view.SelectedBoundOperationsCount.Text = SchemaTypes
                                        .Where(x => x.IsSelected && x.BoundOperations?.Any() == true)
                                        .SelectMany(x => x.BoundOperations).Count(x => x.IsSelected).ToString(CultureInfo.InvariantCulture);
                                }
                            }
                        };
                        toLoad.Add(boundOperationModel);

                        alreadyAdded.Add(name);
                    }
                }
            }

            schemaType.BoundOperations = toLoad.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Checks if the bound operation should be included.
        /// </summary>
        /// <param name="operation">Bound operation.</param>
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

        /// <summary>
        /// Excludes the bound operations from code generation.
        /// </summary>
        /// <remarks>
        /// Sets operationModel isSelected property to be excluded to false.
        /// </remarks>
        /// <param name="schemaType">The schema type.</param>
        /// <param name="operationsToExclude">A list of operation imports to be exclude.</param>
        public void ExcludeBoundOperations(SchemaTypeModel schemaType, IEnumerable<string> operationsToExclude)
        {
            foreach (var operationModel in schemaType.BoundOperations)
            {
                operationModel.IsSelected = !operationsToExclude.Contains(operationModel.Name);
            }
        }

        /// <summary>
        /// Clears schema type bound operations.
        /// </summary>
        /// <param name="schemaType">The schema type.</param>
        public void ClearBoundOperationList(SchemaTypeModel schemaType)
        {
            schemaType.BoundOperations = new List<BoundOperationModel>();
        }

        /// <summary>
        /// Selects all bound operations.
        /// </summary>
        public void SelectAllBoundOperations()
        {
            foreach (var boundOperation in SchemaTypes.SelectMany(x => x.BoundOperations))
            {
                boundOperation.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselect all bound operations.
        /// </summary>
        public void DeselectAllBoundOperations()
        {
            foreach (var boundOperation in SchemaTypes.SelectMany(x => x.BoundOperations))
            {
                boundOperation.IsSelected = false;
            }
        }

        /// <summary>
        /// Selects schema type's bound operations.
        /// </summary>
        /// <param name="schemaType">The schema type.</param>
        public void SelectAllBoundOperationsForSchemaType(SchemaTypeModel schemaType)
        {
            foreach (var boundOperation in schemaType.BoundOperations)
            {
                boundOperation.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselect schema type's bound operations.
        /// </summary>
        /// <param name="schemaType">The schema type.</param>
        public void DeselectAllBoundOperationsForSchemaType(SchemaTypeModel schemaType)
        {
            foreach (var boundOperation in schemaType.BoundOperations)
            {
                boundOperation.IsSelected = false;
            }
        }

        #endregion
    }
}
