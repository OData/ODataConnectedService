using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    class ObjectSelectionViewModel: ConnectedServiceWizardPage
    {
        public ObjectSelectionViewModel(): base()
        {
            Title = "Object Selection";
            Description = "Select with action and function imports to include/exlcude in the generated code.";
            Legend = "Object Selection";
            OperationImports = new List<OperationImportModel>()
            {
                new OperationImportModel() { IsSelected = true, Name = "Func1" },
                new OperationImportModel() { IsSelected = false, Name = "Action2" },
                new OperationImportModel() { IsSelected = true, Name= "Func3" }
            };
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

    }
}
