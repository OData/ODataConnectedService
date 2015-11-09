using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService
{
    [ConnectedServiceProviderExport(Constants.ProviderId)]
    internal class ODataConnectedServiceProvider : ConnectedServiceProvider
    {
        public ODataConnectedServiceProvider()
        {
            this.Name = "OData Connected Service";
            this.Category = "OData";
            this.Description = "OData Connected Service for V1-V4";
            this.Icon = new BitmapImage(new Uri("pack://application:,,/" + this.GetType().Assembly.ToString() + ";component/Resources/Icon.png"));
            this.CreatedBy = "OData";
            this.Version = new Version(0, 2, 0);
            this.MoreInfoUri = new Uri("https://github.com/odata/lab");
        }

        public override Task<ConnectedServiceConfigurator> CreateConfiguratorAsync(ConnectedServiceProviderContext context)
        {
            var wizard = new ODataConnectedServiceWizard(context);
            return Task.FromResult<ConnectedServiceConfigurator>(wizard);
        }

        public override IEnumerable<Tuple<string, Uri>> GetSupportedTechnologyLinks()
        {
            yield return new Tuple<string, Uri>("OData Website", new Uri("http://www.odata.org/"));
            yield return new Tuple<string, Uri>("OData Docs and Samples", new Uri("http://odata.github.io/odata.net/"));
        }

    }
}