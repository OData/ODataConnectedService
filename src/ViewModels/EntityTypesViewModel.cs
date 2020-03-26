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
    internal class EntityTypesViewModel: ConnectedServiceWizardPage
    {
        private bool _isSupportedVersion;

        public EntityTypesViewModel(): base()
        {
            Title = "Entity Selection";
            Description = "Select Entity Types to include in the generated code.";
            Legend = "Entity Types";
            EntityTypes = new List<EntityTypeModel>();
            IsSupportedODataVersion = true;
        }

        public IEnumerable<EntityTypeModel> EntityTypes { get; set; }
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

        public void LoadEntityTypes(IEnumerable<IEdmEntityType> entityTypes)
        {
            var toLoad = new List<EntityTypeModel>();
            var alreadyAdded = new HashSet<string>();
            
            foreach (var entityType in entityTypes)
            {
                if (!alreadyAdded.Contains(entityType.Name))
                {
                    toLoad.Add(new EntityTypeModel()
                    {
                        Name = entityType.Name,
                        IsSelected = true
                    });

                    alreadyAdded.Add(entityType.Name);
                }
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
            EntityTypes = Enumerable.Empty<EntityTypeModel>();
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
