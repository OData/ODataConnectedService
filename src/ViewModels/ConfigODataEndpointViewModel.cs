// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
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

        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter or choose an OData service endpoint to begin";
            this.Legend = "Endpoint";
            this.View = new ConfigODataEndpoint();
            this.ServiceName = Constants.DefaultServiceName;
            this.View.DataContext = this;
            this.UserSettings = userSettings;
        }

        public event EventHandler<EventArgs> PageLeaving;

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            var wizard = this.Wizard as ODataConnectedServiceWizard;
            UserSettings.AddToTopOfMruList(wizard.UserSettings.MruEndpoints, this.Endpoint);
            Version version;
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

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.Endpoint);
            if(this.CustomHttpHeaders !=null)
            {
                string[] headerElements = this.CustomHttpHeaders.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var headerElement in headerElements)
                {
                    // Trim header for empty spaces
                    var header = headerElement.Trim();
                    webRequest.Headers.Add(header);
                }
            }

            WebResponse webResponse = webRequest.GetResponse();
            Stream metadataStream = webResponse.GetResponseStream();

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
        }
    }
}