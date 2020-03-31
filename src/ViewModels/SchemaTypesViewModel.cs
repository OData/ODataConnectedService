using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class SchemaTypesViewModel: ConnectedServiceWizardPage
    {
        private bool _isSupportedVersion;

        public SchemaTypesViewModel(): base()
        {
            Title = "Entity Selection";
            Description = "Select Entity Types to include in the generated code.";
            Legend = "Entity Types";
            EntityTypes = new List<SchemaTypeModel>();
            IsSupportedODataVersion = true;
            SchemaTypeModel = new Dictionary<string, SchemaTypeModel>();
        }

        public IDictionary<string, SchemaTypeModel> SchemaTypeModel { get; set; }

        public IEnumerable<SchemaTypeModel> EntityTypes { get; set; }
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

        public IEnumerable<string> ExcludedEntityTypeNames
        {
            get
            {
                return EntityTypes.Where(o => !o.IsSelected).Select(o => o.Name);
            }
        }

        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);
            View = new EntityTypes() { DataContext = this };
            PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return await base.OnPageLeavingAsync(args);
        }

        public void LoadEntityTypes(
            IEnumerable<IEdmSchemaType> schemaTypes, IDictionary<IEdmStructuredType, List<IEdmOperation>> boundOperations)
        {
            var toLoad = new List<SchemaTypeModel>();

            foreach (var schemaType in schemaTypes)
            {
                var schemaTypeModel = new SchemaTypeModel()
                {
                    Name = schemaType.FullTypeName(),
                    ShortName = EdmHelper.GetTypeNameFromFullName(schemaType.FullTypeName()),
                    IsSelected = true
                };

                schemaTypeModel.PropertyChanged += (s, args) => {
                    if (schemaTypeModel.IsSelected && schemaType is IEdmStructuredType structuredType)
                    {
                        foreach (var property in structuredType.DeclaredProperties)
                        {
                            string propertyName = property is IEdmNavigationProperty ? property.Type.ToStructuredType().FullTypeName() : property.Type.FullName();
                            bool hasProperty = SchemaTypeModel.TryGetValue(propertyName, out SchemaTypeModel navigationPropertyModel);

                            if (hasProperty && !navigationPropertyModel.IsSelected)
                            {
                                navigationPropertyModel.IsSelected = true;
                            }
                        }

                         if(boundOperations.TryGetValue(structuredType,out List<IEdmOperation> operations))
                        {
                            foreach (var operation in operations)
                            {
                                string returnTypeName = operation.ReturnType?.FullName();

                                if (returnTypeName != null && SchemaTypeModel.TryGetValue(returnTypeName, out SchemaTypeModel referencedschemaTypeModel)
                                    && !referencedschemaTypeModel.IsSelected)
                                {
                                    referencedschemaTypeModel.IsSelected = true;
                                }

                                IEnumerable<IEdmOperationParameter> parameters = operation.Parameters;

                                foreach (var parameter in parameters)
                                {
                                    if (SchemaTypeModel.TryGetValue(parameter.Type.FullName(), out SchemaTypeModel model) && !model.IsSelected)
                                    {
                                        model.IsSelected = true;
                                    }
                                }
                            }
                        }
                    }
                    };

                toLoad.Add(schemaTypeModel);

                SchemaTypeModel.Add(schemaType.FullTypeName(), schemaTypeModel);               
            }

            EntityTypes = toLoad.OrderBy(o => o.Name).ToList();
        }

        public void ExcludeEntityTypes(IEnumerable<string> entitiesToExclude)
        {
            foreach (var entityModel in EntityTypes)
            {
                entityModel.IsSelected = !entitiesToExclude.Contains(entityModel.Name);
            }
        }

        public void EmptyList()
        {
            EntityTypes = Enumerable.Empty<SchemaTypeModel>();
        }

        public void SelectAll()
        {
            foreach (var operation in EntityTypes)
            {
                operation.IsSelected = true;
            }
        }

        public void UnselectAll()
        {
            foreach (var entityType in EntityTypes)
            {
                entityType.IsSelected = false;
            }
        }

    }
}
