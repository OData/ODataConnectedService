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

        public OperationImportsViewModel(UserSettings userSettings = null) : base()
        {
            Title = "Object Selection";
            Description = "Select function and action imports to include in the generated code.";
            Legend = "Function/Action Imports";
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
            await base.OnPageEnteringAsync(args);
            this.View = new OperationImports { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            SaveToUserSettings();
            return await base.OnPageLeavingAsync(args);
        }

        public void LoadOperationImports(IEnumerable<IEdmOperationImport> operationImports)
        {
            var toLoad = new List<OperationImportModel>();
            var alreadyAdded = new HashSet<string>();

            foreach (var operation in operationImports)
            {
                if (!alreadyAdded.Contains(operation.Name))
                {
                    toLoad.Add(new OperationImportModel()
                    {
                        Name = operation.Name,
                        IsSelected = true
                    });

                    alreadyAdded.Add(operation.Name);
                }
            }

            OperationImports = toLoad.OrderBy(o => o.Name).ToList();
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

        public void SaveToUserSettings()
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
