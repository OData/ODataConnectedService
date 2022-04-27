//-----------------------------------------------------------------------------
// <copyright file="MetadataReader.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------


using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.OData.CodeGen.Models;

namespace Microsoft.OData.CodeGen.Common
{
    /// <summary>
    /// Contains methods for reading the OData metadata
    /// </summary>
    public static class MetadataReader
    {
        /// <summary>
        /// Reads the metadata version from the metadata url prvided.
        /// </summary>
        /// <param name="serviceConfiguration">The <see cref="ServiceConfiguration"/> of the metadata provided.</param>
        /// <returns>The <see cref="Version"/> of the metadata</returns>
        public static string GetMetadataVersion(ServiceConfiguration serviceConfiguration, out Version edmxVersion)
        {
            if (string.IsNullOrEmpty(serviceConfiguration.Endpoint))
            {
                throw new ArgumentNullException("OData Service Endpoint", string.Format(CultureInfo.InvariantCulture, Constants.InputServiceEndpointMsg));
            }

            if (serviceConfiguration.Endpoint.StartsWith("https:", StringComparison.Ordinal)
                || serviceConfiguration.Endpoint.StartsWith("http", StringComparison.Ordinal))
            {
                if (!serviceConfiguration.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                {
                    serviceConfiguration.Endpoint = serviceConfiguration.Endpoint.TrimEnd('/') + "/$metadata";
                }
            }

            Stream metadataStream;
            var metadataUri = new Uri(serviceConfiguration.Endpoint);
            if (!metadataUri.IsFile)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(metadataUri);
                if (serviceConfiguration.CustomHttpHeaders != null)
                {
                    var headerElements = serviceConfiguration.CustomHttpHeaders.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var headerElement in headerElements)
                    {
                        // Trim header for empty spaces
                        var header = headerElement.Trim();
                        webRequest.Headers.Add(header);
                    }
                }

                if (serviceConfiguration.IncludeWebProxy)
                {
                    var proxy = new WebProxy(serviceConfiguration.WebProxyHost);
                    if (serviceConfiguration.IncludeWebProxyNetworkCredentials)
                    {
                        proxy.Credentials = new NetworkCredential(
                            serviceConfiguration.WebProxyNetworkCredentialsUsername,
                            serviceConfiguration.WebProxyNetworkCredentialsPassword,
                            serviceConfiguration.WebProxyNetworkCredentialsDomain);
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
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot access {0}", serviceConfiguration.Endpoint), e);
            }
            finally
            {
                metadataStream?.Dispose();
            }
        }
    }
}
