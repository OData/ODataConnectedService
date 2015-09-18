using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
    {
        private UserSettings userSettings;

        public string Endpoint { get; set; }
        public bool UseDataSvcUtil { get; set; }
        public UserSettings UserSettings
        {
            get { return this.userSettings; }
        }

        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter the endpoint to OData service to begin";
            this.Legend = "Endpoint";
            this.View = new ConfigODataEndpoint();
            this.View.DataContext = this;
            this.userSettings = userSettings;
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            UserSettings.AddToTopOfMruList(((ODataConnectedServiceWizard)this.Wizard).UserSettings.MruEndpoints, this.Endpoint);
            return base.OnPageLeavingAsync(args);
        }
    }
}
