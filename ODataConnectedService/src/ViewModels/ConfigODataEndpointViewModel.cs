using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
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
        public Version EdmxVersion { get; set; }
        public string MetadataTempPath { get; set; }
        public UserSettings UserSettings
        {
            get { return this.userSettings; }
        }

        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter or choose an OData service endpoint to begin";
            this.Legend = "Endpoint";
            this.View = new ConfigODataEndpoint();
            this.View.DataContext = this;
            this.userSettings = userSettings;
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            UserSettings.AddToTopOfMruList(((ODataConnectedServiceWizard)this.Wizard).UserSettings.MruEndpoints, this.Endpoint);
            Version version;
            try
            {
                this.MetadataTempPath = GetMetadata(this.Endpoint, out version);
                this.EdmxVersion = version;
                return base.OnPageLeavingAsync(args);
            }
            catch (Exception e)
            {
                return Task.FromResult<PageNavigationResult>(
                    new PageNavigationResult()
                    {
                        ErrorMessage = e.Message,
                        IsSuccess = false,
                        ShowMessageBoxOnFailure = true
                    });
            }
        }

        private string GetMetadata(string address, out Version edmxVersion)
        {
            if (String.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("OData Service Endpoint", "Please input the service endpoint");
            }

            if (address.StartsWith("https:") || address.StartsWith("http"))
            {
                if (!address.EndsWith("$metadata"))
                {
                    address = address.TrimEnd('/') + "/$metadata";
                }
            }

            XmlReaderSettings readerSettings = new XmlReaderSettings()
            {
                XmlResolver = new XmlUrlResolver()
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials
                }
            };

            string workFile = Path.GetTempFileName();

            try
            {
                using (XmlReader reader = XmlReader.Create(address, readerSettings))
                {
                    using (XmlWriter writer = XmlWriter.Create(workFile))
                    {
                        while (reader.NodeType != XmlNodeType.Element)
                        {
                            reader.Read();
                        }

                        if (reader.EOF)
                        {
                            throw new Exception("The metadata is an empty file");
                        }

                        Common.Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
                        writer.WriteNode(reader, false);
                    }
                }
                return workFile;
            }
            catch (WebException e)
            {
                throw new Exception(string.Format("Cannot access {0}", address), e);
            }
        }
    }
}
