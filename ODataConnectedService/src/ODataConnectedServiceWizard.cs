using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.ViewModels;

namespace Microsoft.OData.ConnectedService
{
    internal class ODataConnectedServiceWizard : ConnectedServiceWizard
    {
        public ConfigODataEndpointViewModel ConfigODataEndpointViewModel { get; set; }
        public AdvancedSettingsViewModel AdvancedSettingsViewModel { get; set; }
        public ConnectedServiceProviderContext Context { get; set; }
        public Project Project { get; set; }

        public ODataConnectedServiceWizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this.Project = ProjectHelper.GetProjectFromHierarchy(this.Context.ProjectHierarchy);

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel();


            AdvancedSettingsViewModel = new AdvancedSettingsViewModel();
            AdvancedSettingsViewModel.NamespacePrefix = ProjectHelper.GetProjectNamespace(this.Project);
            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);

            this.IsFinishEnabled = true;
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            ODataConnectedServiceInstance instance = new ODataConnectedServiceInstance();
            instance.Name = "OData Connected Service";
            instance.Endpoint = ConfigODataEndpointViewModel.Endpoint;
            instance.GenByDataSvcUtil = ConfigODataEndpointViewModel.UseDataSvcUtil;
            if (AdvancedSettingsViewModel.UseNamespacePrefix)
            {
                instance.NamespacePrefix = AdvancedSettingsViewModel.NamespacePrefix;
            }

            instance.IgnoreUnexpectedElementsAndAttributes = AdvancedSettingsViewModel.IgnoreUnexpectedElementsAndAttributes;
            instance.UseDataServiceCollection = AdvancedSettingsViewModel.UseDataServiceCollection;
            instance.EnableNamingAlias = AdvancedSettingsViewModel.EnableNamingAlias;

            return Task.FromResult<ConnectedServiceInstance>(instance);
        }
    }
}
