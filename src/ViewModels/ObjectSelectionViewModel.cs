using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class ObjectSelectionViewModel: ConnectedServiceWizardPage
    {
        public ObjectSelectionViewModel(): base()
        {
            Title = "Object Selection";
            Description = "Select with action and function imports to include/exlcude in the generated code.";
            Legend = "Object Selection";
            OperationImports = new List<OperationImportModel>();
            
        }

        public List<OperationImportModel> OperationImports { get; set; }

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
            View = new ObjectSelection() { DataContext = this };
            PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return await base.OnPageLeavingAsync(args);
        }

        public void LoadOperationImports(IEnumerable<IEdmOperationImport> operationImports)
        {
            OperationImports = operationImports.Select(op => new OperationImportModel() { IsSelected = true, Name = op.Name }).ToList();
        }

        public void ExcludeOperationImports(IEnumerable<string> operationsToExclude)
        {
            foreach (var operationModel in OperationImports)
            {
                operationModel.IsSelected = !operationsToExclude.Contains(operationModel.Name);
            }
        }

    }
}
