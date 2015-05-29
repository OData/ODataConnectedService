using System;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceProviderExport("ODataConnectedService")]
    internal class ODataConnectedServiceProvider : ConnectedServiceProvider
    {
        public ODataConnectedServiceProvider()
        {
            this.Name = "OData Connected Service";
            this.Category = "OData";
            this.Description = "OData Connected Service for V1-V4";
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Resources/Icon.png"));
            this.CreatedBy = "OData";
            this.Version = new Version(0, 1, 0);
            this.MoreInfoUri = new Uri("https://github.com/odata/odata.net");
        }
        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            var wizard = new ODataConnectedServiceWizard(context);
            return Task.FromResult<ConnectedServiceConfigurator>(wizard);
        }
    }
}