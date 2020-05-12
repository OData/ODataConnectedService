//----------------------------------------------------------------------------------
// <copyright file="ConfigODataEndpointViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

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

        public string CustomHttpHeaders { get; set; }

        public UserSettings UserSettings { get; set; }

        public bool IncludeWebProxy { get; set; }

        public string WebProxyHost { get; set; }

        public bool IncludeWebProxyNetworkCredentials { get; set; }

        public string WebProxyNetworkCredentialsUsername { get; set; }

        public string WebProxyNetworkCredentialsPassword { get; set; }

        public string WebProxyNetworkCredentialsDomain { get; set; }

        public bool IncludeCustomHeaders { get; set; }

        public event EventHandler<EventArgs> PageEntering;

        public ODataConnectedServiceWizard ServiceWizard { get; set; }

        public ConfigODataEndpointViewModel(UserSettings userSettings, ODataConnectedServiceWizard serviceWizard) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter or choose an OData service endpoint to begin";
            this.Legend = "Endpoint";
            this.ServiceWizard = serviceWizard;
            this.UserSettings = userSettings;
            this.ServiceName = Constants.DefaultServiceName;
        }
        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);
            this.View = new ConfigODataEndpoint { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> PageLeaving;
        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            SaveToUserSettings();
            var wizard = this.Wizard as ODataConnectedServiceWizard;
            UserSettings.AddToTopOfMruList(wizard?.UserSettings?.MruEndpoints, this.Endpoint);
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

        internal string GetMetadata(out Version edmxVersion)
        {
            if (string.IsNullOrEmpty(this.Endpoint))
            {
                throw new ArgumentNullException("OData Service Endpoint", Constants.InputServiceEndpointMsg);
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
            var metadataUri = new Uri(this.Endpoint);
            if (!metadataUri.IsFile)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(this.Endpoint);
                if (this.CustomHttpHeaders != null)
                {
                    var headerElements = this.CustomHttpHeaders.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var headerElement in headerElements)
                    {
                        // Trim header for empty spaces
                        var header = headerElement.Trim();
                        webRequest.Headers.Add(header);
                    }
                }

                if (IncludeWebProxy)
                {
                    var proxy = new WebProxy(WebProxyHost);
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
                var xmlUrlResolver = new XmlUrlResolver
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials
                };

                metadataStream = (Stream)xmlUrlResolver.GetEntity(metadataUri, null, typeof(Stream));
            }

            var workFile = Path.GetTempFileName();

            try
            {
                using (XmlReader reader = XmlReader.Create(metadataStream))
                {
                    using (var writer = XmlWriter.Create(workFile))
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

        private void SaveToUserSettings()
        {
            if (this.UserSettings != null)
            {
                UserSettings.ServiceName = this.ServiceName;
                UserSettings.Endpoint = this.Endpoint;
                UserSettings.WebProxyHost = this.WebProxyHost;
                UserSettings.CustomHttpHeaders = this.CustomHttpHeaders;
                UserSettings.WebProxyNetworkCredentialsDomain = this.WebProxyNetworkCredentialsDomain;
                UserSettings.WebProxyNetworkCredentialsPassword = this.WebProxyNetworkCredentialsPassword;
                UserSettings.WebProxyNetworkCredentialsUsername = this.WebProxyNetworkCredentialsUsername;
                UserSettings.IncludeCustomHeaders = this.IncludeCustomHeaders;
                UserSettings.IncludeWebProxy = this.IncludeWebProxy;
                UserSettings.IncludeWebProxyNetworkCredentials = this.IncludeWebProxyNetworkCredentials;
            }
        }

        public void LoadFromUserSettings()
        {
            if (this.UserSettings != null)
            {
                this.ServiceName = UserSettings.ServiceName ?? Constants.DefaultServiceName;
                this.Endpoint = UserSettings.Endpoint;
                this.WebProxyHost = UserSettings.WebProxyHost;
                this.IncludeWebProxy = UserSettings.IncludeWebProxy;
                this.IncludeCustomHeaders = UserSettings.IncludeCustomHeaders;
                this.CustomHttpHeaders = UserSettings.CustomHttpHeaders;
                this.IncludeWebProxyNetworkCredentials = UserSettings.IncludeWebProxyNetworkCredentials;
                this.WebProxyNetworkCredentialsDomain = UserSettings.WebProxyNetworkCredentialsDomain;
                this.WebProxyNetworkCredentialsPassword = UserSettings.WebProxyNetworkCredentialsPassword;
                this.WebProxyNetworkCredentialsUsername = UserSettings.WebProxyNetworkCredentialsUsername;
                this.View = new ConfigODataEndpoint { DataContext = this };
            }
        }
    }
}