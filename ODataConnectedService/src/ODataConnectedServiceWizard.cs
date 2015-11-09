using System;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    internal class ODataConnectedServiceWizard : ConnectedServiceWizard
    {
        private UserSettings userSettings;

        public ConfigODataEndpointViewModel ConfigODataEndpointViewModel { get; set; }

        public AdvancedSettingsViewModel AdvancedSettingsViewModel { get; set; }

        public ConnectedServiceProviderContext Context { get; set; }

        public Project Project { get; set; }

        public Version EdmxVersion
        {
            get { return this.ConfigODataEndpointViewModel.EdmxVersion; }
        }

        public UserSettings UserSettings
        {
            get { return this.userSettings; }
        }

        public ODataConnectedServiceWizard(ConnectedServiceProviderContext context)
        {
            this.Context = context;
            this.Project = ProjectHelper.GetProjectFromHierarchy(this.Context.ProjectHierarchy);
            this.userSettings = UserSettings.Load(context.Logger);

            ConfigODataEndpointViewModel = new ConfigODataEndpointViewModel(this.userSettings);
            AdvancedSettingsViewModel = new AdvancedSettingsViewModel();
            AdvancedSettingsViewModel.NamespacePrefix = ProjectHelper.GetProjectNamespace(this.Project);
            this.Pages.Add(ConfigODataEndpointViewModel);
            this.Pages.Add(AdvancedSettingsViewModel);

            this.IsFinishEnabled = true;
        }

        public override Task<ConnectedServiceInstance> GetFinishedServiceInstanceAsync()
        {
            this.userSettings.Save();
            ODataConnectedServiceInstance instance = new ODataConnectedServiceInstance();
            instance.Name = "OData Connected Service";
            instance.Endpoint = ConfigODataEndpointViewModel.Endpoint;
            instance.EdmxVersion = ConfigODataEndpointViewModel.EdmxVersion;
            instance.MetadataTempFilePath = ConfigODataEndpointViewModel.MetadataTempPath;

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
