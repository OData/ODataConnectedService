using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public OperationImportsViewModel(): base()
        {
            Title = "Object Selection";
            Description = "Select function and action imports to include in the generated code.";
            Legend = "Function/Action Imports";
            OperationImports = new List<OperationImportModel>();
            IsSupportedODataVersion = true;
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
            await base.OnPageEnteringAsync(args);
            View = new OperationImports() { DataContext = this };
            PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return await base.OnPageLeavingAsync(args);
        }

        public void LoadOperationImports(IEnumerable<IEdmOperationImport> operationImports, ICollection<string> excludedSchemaTypes, IDictionary<string,SchemaTypeModel> schemaTypeModels)
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
                            if (schemaTypeModels.TryGetValue(parameter.Type.FullName(), out SchemaTypeModel model))
                            {
                                model.IsSelected = true;
                            }
                        }

                        string returnType = operation.Operation.ReturnType?.FullName();

                        if(returnType != null && schemaTypeModels.TryGetValue(returnType, out SchemaTypeModel schemaTypeModel))
                        {
                            schemaTypeModel.IsSelected = true;
                        }
                    };
                    toLoad.Add(operationImportModel);

                    alreadyAdded.Add(operation.Name);
                }
            }

            OperationImports = toLoad.OrderBy(o => o.Name).ToList();
        }

        public  bool IsOperationImportIncluded(IEdmOperationImport operationImport, ICollection<string> excludedTypes)
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

    }
}
