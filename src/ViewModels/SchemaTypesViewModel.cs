//---------------------------------------------------------------------------------
// <copyright file="SchemaTypesViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class SchemaTypesViewModel : ConnectedServiceWizardPage
    {
        public UserSettings UserSettings { get; set; }

        public IDictionary<string, SchemaTypeModel> SchemaTypeModelMap { get; set; }

        public IEnumerable<SchemaTypeModel> SchemaTypes { get; set; }

        /// <summary>Contains a collection of entities that would depend on another entity</summary>
        public IDictionary<string, ICollection<string>> RelatedTypes { get; set; }

        /// <summary>Gets the schema types that will be excluded while generating code </summary>
        public IEnumerable<string> ExcludedSchemaTypeNames
        {
            get
            {
                return SchemaTypes.Where(o => !o.IsSelected).Select(o => o.Name);
            }
        }

        public event EventHandler<EventArgs> PageEntering;

        internal bool IsEntered;

        public SchemaTypesViewModel(UserSettings userSettings = null) : base()
        {
            Title = "Schema Types";
            Description = "Select schema types to include in the generated code.";
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
            await base.OnPageEnteringAsync(args);
            View = new SchemaTypes() { DataContext = this };
            PageEntering?.Invoke(this, EventArgs.Empty);
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

            //check each excluded schema type and check if they are required. If so, then automatically select them.
            foreach (var schemaType in ExcludedSchemaTypeNames)
            {
                if (RelatedTypes.TryGetValue(schemaType, out ICollection<string> relatedtypes))
                {
                    //Check if any of the related types has been selected
                    if (relatedtypes.Any(o =>
                    {
                        if(SchemaTypeModelMap.TryGetValue(o, out SchemaTypeModel schemaTypeModel))
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
                return await Task.FromResult( new PageNavigationResult
                {
                    ErrorMessage = $"{numberOfTypesToBeIncluded} {Constants.SchemaTypesWillAutomaticallyBeIncluded}",
                    IsSuccess = correctTypeSelection,
                    ShowMessageBoxOnFailure = true
                });
            }
            else
            {
                return await base.OnPageLeavingAsync(args);
            }
        }

        /// <summary>
        /// Creates a list of types that needs to be loaded on the UI.
        /// Initially all the types are loaded
        /// </summary>
        /// <param name="schemaTypes">A list of schema types that need to be laoded.</param>
        /// <param name="boundOperations">The associated bound operations.</param>
        public void LoadSchemaTypes(
            IEnumerable<IEdmSchemaType> schemaTypes, IDictionary<IEdmStructuredType, List<IEdmOperation>> boundOperations)
        {
            var toLoad = new List<SchemaTypeModel>();

            foreach (var type in schemaTypes)
            {
                if (!SchemaTypeModelMap.ContainsKey(type.FullName()) || SchemaTypeModelMap.Count() != schemaTypes.Count())
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
                var schemaTypeModel = new SchemaTypeModel()
                {
                    Name = schemaType.FullTypeName(),
                    ShortName = EdmHelper.GetTypeNameFromFullName(schemaType.FullTypeName())
                };

                // Create propertyChange handler.
                // Anytime a property is selected/unslected, the handler ensures
                // all the related types and operations are selected/unselected
                schemaTypeModel.PropertyChanged += (s, args) =>
                {
                    if (schemaTypeModel.IsSelected && schemaType is IEdmStructuredType structuredType)
                    {
                        //Check for the base type and automatically select it if not selected already.
                        string baseTypeFullName = structuredType.BaseType?.FullTypeName();

                        if (baseTypeFullName != null)
                        {
                            AddRelatedType(baseTypeFullName, structuredType.FullTypeName());

                            if (SchemaTypeModelMap.TryGetValue(baseTypeFullName, out SchemaTypeModel baseTypeSchemaTypeModel)
                                  && !baseTypeSchemaTypeModel.IsSelected)
                            {
                                baseTypeSchemaTypeModel.IsSelected = true;
                            }
                        }

                        // Check the required navigational property types and ensure they are selected as well
                        foreach (var property in structuredType.DeclaredProperties)
                        {
                            string propertyName = property is IEdmNavigationProperty ? property.Type.ToStructuredType().FullTypeName() : property.Type.FullName();

                            if (property.Type.ToStructuredType() != null || property.Type.IsEnum())
                            {
                                propertyName = property.Type.ToStructuredType()?.FullTypeName() ?? property.Type.FullName();
                                AddRelatedType(propertyName, structuredType.FullTypeName());

                                bool hasProperty = SchemaTypeModelMap.TryGetValue(propertyName, out SchemaTypeModel navigationPropertyModel);

                                if (hasProperty && !navigationPropertyModel.IsSelected)
                                {
                                    navigationPropertyModel.IsSelected = true;
                                }
                            }
                        }

                        // Check for bound operations and ensure related types re also selected.
                        // In this case related types means return types and parameter types.
                        if (boundOperations.TryGetValue(structuredType, out List<IEdmOperation> operations))
                        {
                            foreach (var operation in operations)
                            {
                                // Check if return type of associated bound operation has been selected.
                                if (operation.ReturnType != null && (operation.ReturnType.ToStructuredType() != null || operation.ReturnType.IsEnum()))
                                {
                                    string returnTypeFullName = operation.ReturnType.ToStructuredType()?.FullTypeName() ?? operation.ReturnType.FullName();
                                    AddRelatedType(returnTypeFullName, structuredType.FullTypeName());

                                    if (SchemaTypeModelMap.TryGetValue(returnTypeFullName, out SchemaTypeModel referencedschemaTypeModel)
                                        && !referencedschemaTypeModel.IsSelected)
                                    {
                                        referencedschemaTypeModel.IsSelected = true;
                                    }
                                }

                                // Check if parameter types of associated bound operations has been selected.
                                IEnumerable<IEdmOperationParameter> parameters = operation.Parameters;

                                foreach (var parameter in parameters)
                                {
                                    if (parameter.Type.ToStructuredType() != null || parameter.Type.IsEnum())
                                    {
                                        string parameterFullName = parameter.Type.ToStructuredType()?.FullTypeName() ?? parameter.Type.FullName();
                                        AddRelatedType(parameterFullName, structuredType.FullTypeName());

                                        if (SchemaTypeModelMap.TryGetValue(parameterFullName, out SchemaTypeModel model) && !model.IsSelected)
                                        {
                                            model.IsSelected = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                };

                toLoad.Add(schemaTypeModel);
                schemaTypeModel.IsSelected = true;
                SchemaTypeModelMap.Add(schemaType.FullTypeName(), schemaTypeModel);
            }

            SchemaTypes = toLoad.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Add related type and its relation to the RelatedTypes dictionary
        /// </summary>
        /// <param name="type">the schema type fullname.</param>
        /// <param name="relatedType">Related schema type fullname.</param>
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
        /// Sets schemaTypeModel isSelected property to be excluded to false
        /// </summary>
        /// <param name="schemaTypesToExclude">A list of all fullnames of schema types to be exclude.</param>
        public void ExcludeSchemaTypes(IEnumerable<string> schemaTypesToExclude)
        {
            foreach (var schemaTypeModel in SchemaTypes)
            {
                schemaTypeModel.IsSelected = !schemaTypesToExclude.Contains(schemaTypeModel.Name);
            }
        }

        /// <summary>
        /// Empties a list of schema types
        /// </summary>
        public void ClearSchemaTypes()
        {
            SchemaTypes = Enumerable.Empty<SchemaTypeModel>();
        }

        /// <summary>
        /// Selects all schema types
        /// </summary>
        public void SelectAllSchemaTypes()
        {
            foreach (var schemaType in SchemaTypes)
            {
                schemaType.IsSelected = true;
            }
        }

        /// <summary>
        /// Deselect all schema types
        /// </summary>
        public void DeselectAllSchemaTypes()
        {
            foreach (var schemaType in SchemaTypes)
            {
                schemaType.IsSelected = false;
            }
        }

        /// <summary>
        /// Save the selected schema types to user settings
        /// </summary>
        public void SaveToUserSettings()
        {
            if (this.UserSettings != null)
            {
                UserSettings.ExcludedSchemaTypes = this.ExcludedSchemaTypeNames?.Any() == true
                    ? this.ExcludedSchemaTypeNames.ToList()
                    : new List<string>();
            }
        }

        /// <summary>
        /// Loads schema type configurations from user settings
        /// </summary>
        public void LoadFromUserSettings()
        {
            if (UserSettings != null)
            {
                if (UserSettings.ExcludedOperationImports?.Any() == true)
                {
                    ExcludeSchemaTypes(UserSettings.ExcludedSchemaTypes ?? Enumerable.Empty<string>());
                }
            }
        }
    }
}
