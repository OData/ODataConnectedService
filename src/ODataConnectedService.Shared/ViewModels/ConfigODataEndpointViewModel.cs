//----------------------------------------------------------------------------------
// <copyright file="ConfigODataEndpointViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
    {
        public Version EdmxVersion { get; set; }

        public string MetadataTempPath { get; set; }

        public UserSettings UserSettings { get; internal set; }

        public event EventHandler<EventArgs> PageEntering;

        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter or choose an OData service endpoint to begin";
            this.Legend = "Endpoint";
            this.UserSettings = userSettings;
        }
        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args).ConfigureAwait(false);
            this.View = new ConfigODataEndpoint { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> PageLeaving;

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            try
            {
                this.MetadataTempPath = GetMetadata(out var version);
                // Makes sense to add MRU endpoint at this point since GetMetadata manipulates UserSettings.Endpoint
                UserSettings.AddMruEndpoint(UserSettings.Endpoint);
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
            if (string.IsNullOrEmpty(UserSettings.Endpoint))
            {
                throw new ArgumentNullException("OData Service Endpoint", string.Format(CultureInfo.InvariantCulture, Constants.InputServiceEndpointMsg));
            }

            if (UserSettings.Endpoint.StartsWith("https:", StringComparison.Ordinal)
                || UserSettings.Endpoint.StartsWith("http", StringComparison.Ordinal))
            {
                if (!UserSettings.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                {
                    UserSettings.Endpoint = UserSettings.Endpoint.TrimEnd('/') + "/$metadata";
                }
            }

            Stream metadataStream;
            var metadataUri = new Uri(UserSettings.Endpoint);
            if (!metadataUri.IsFile)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(metadataUri);
                if (!string.IsNullOrEmpty(UserSettings.CustomHttpHeaders))
                {
                    var headerElements = UserSettings.CustomHttpHeaders.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var headerElement in headerElements)
                    {
                        // Trim header for empty spaces
                        var header = headerElement.Trim();
                        webRequest.Headers.Add(header);
                    }
                }

                if (UserSettings.IncludeWebProxy)
                {
                    if (!string.IsNullOrEmpty(UserSettings.WebProxyHost))
                    {
                        var proxy = new WebProxy(UserSettings.WebProxyHost);
                        if (UserSettings.IncludeWebProxyNetworkCredentials)
                        {
                            proxy.Credentials = new NetworkCredential(
                                UserSettings.WebProxyNetworkCredentialsUsername,
                                UserSettings.WebProxyNetworkCredentialsPassword,
                                UserSettings.WebProxyNetworkCredentialsDomain);
                        }

                        webRequest.Proxy = proxy;
                    }
                    
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
                            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The metadata is an empty file"));
                        }

                        Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out edmxVersion);
                        writer.WriteNode(reader, false);
                    }
                }
                return workFile;
            }
            catch (WebException e)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot access {0}", UserSettings.Endpoint), e);
            }
            finally
            {
                this.DisposeStream(metadataStream);
            }
        }

        private void DisposeStream(Stream stream)
        {
            stream?.Dispose();
        }
    }
}