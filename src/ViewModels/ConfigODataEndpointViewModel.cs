// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
    {
        public string Endpoint { get; set; }
        public string ServiceName { get; set; }
        public Version EdmxVersion { get; set; }
        public string MetadataTempPath { get; set; }
        public UserSettings UserSettings { get; }
        public string CustomHttpHeaders { get; set; }


        public bool IncludeWebProxy { get; set; }

        public string WebProxyHost { get; set; }

        public bool IncludeWebProxyNetworkCredentials { get; set; }

        public string WebProxyNetworkCredentialsUsername { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }

        public string WebProxyNetworkCredentialsDomain { get; set; }

        public bool IncludeCustomHeaders { get; set; }

        public event EventHandler<EventArgs> PageEntering;


        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter or choose an OData service endpoint to begin";
            this.Legend = "Endpoint";
            this.View = new ConfigODataEndpoint();
            this.View.DataContext = this;
            this.ResetDataContext();
            this.UserSettings = userSettings;
        }

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);

            if (PageEntering != null)
            {
                this.PageEntering(this, EventArgs.Empty);
            }
        }

        private void ResetDataContext()
        {
            this.ServiceName = Constants.DefaultServiceName;
        }

        public event EventHandler<EventArgs> PageLeaving;

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            var wizard = this.Wizard as ODataConnectedServiceWizard;
            UserSettings.AddToTopOfMruList(wizard.UserSettings.MruEndpoints, this.Endpoint);

            try
            {
                this.MetadataTempPath = GetMetadata(out var version);
                this.EdmxVersion = version;
                PageLeaving?.Invoke(this, EventArgs.Empty);
                return base.OnPageLeavingAsync(args);
            }
            catch (Exception e)
            {
                return Task.FromResult(
                    new PageNavigationResult
                    {
                        ErrorMessage = e.Message,
                        IsSuccess = false,
                        ShowMessageBoxOnFailure = true
                    });
            }
        }

        private string GetMetadata(out Version edmxVersion)
        {
            if (string.IsNullOrEmpty(this.Endpoint))
            {
                throw new ArgumentNullException("OData Service Endpoint", "Please input the service endpoint");
            }

            if (this.Endpoint.StartsWith("https:", StringComparison.Ordinal)
                || this.Endpoint.StartsWith("http", StringComparison.Ordinal))
            {
                if (!this.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                {
                    this.Endpoint = this.Endpoint.TrimEnd('/') + "/$metadata";
                }
            }
            Stream metadataStream;
            Uri metadataUri = new Uri(this.Endpoint);
            if (!metadataUri.IsFile)
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.Endpoint);
                if (this.CustomHttpHeaders != null)
                {
                    string[] headerElements = this.CustomHttpHeaders.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var headerElement in headerElements)
                    {
                        // Trim header for empty spaces
                        var header = headerElement.Trim();
                        webRequest.Headers.Add(header);
                    }
                }

                if (IncludeWebProxy)
                {
                    WebProxy proxy = new WebProxy(WebProxyHost);
                    if (IncludeWebProxyNetworkCredentials)
                    {
                        proxy.Credentials = new NetworkCredential(WebProxyNetworkCredentialsUsername, WebProxyNetworkCredentialsPassword, WebProxyNetworkCredentialsDomain);
                    }

                    webRequest.Proxy = proxy;
                }

                WebResponse webResponse = webRequest.GetResponse();
                metadataStream = webResponse.GetResponseStream();
                
            }
            else
            {
                // Set up XML secure resolver
                XmlUrlResolver xmlUrlResolver = new XmlUrlResolver()
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials

                };

                metadataStream = (Stream)xmlUrlResolver.GetEntity(metadataUri, null, typeof(Stream));
            }

            string workFile = Path.GetTempFileName();

            try
            {   
                using (XmlReader reader = XmlReader.Create(metadataStream))
                {
                    using (XmlWriter writer = XmlWriter.Create(workFile))
                    {
                        while (reader.NodeType != XmlNodeType.Element)
                        {
                            reader.Read();
                        }

                        if (reader.EOF)
                        {
                            throw new InvalidOperationException("The metadata is an empty file");
                        }

                        Common.Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
                        writer.WriteNode(reader, false);
                    }
                }
                return workFile;
            }
            catch (WebException e)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot access {0}", this.Endpoint), e);
            }
            finally
            {
                metadataStream?.Dispose();
            }
        }
    }
}