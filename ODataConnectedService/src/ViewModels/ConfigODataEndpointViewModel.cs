using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Views;
using System.Diagnostics;
using System.Windows;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
    {
        public string Endpoint { get; set; }
        public bool UseDataSvcUtil { get; set; }

        public ConfigODataEndpointViewModel() : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter the endpoint to OData service to begin";
            this.Legend = "Endpoint";
            this.View = new ConfigODataEndpoint();
            this.View.DataContext = this;
        }

        public override Task<WizardNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }
    }
}
