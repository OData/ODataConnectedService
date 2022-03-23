//-----------------------------------------------------------------------------
// <copyright file="MetadataReader.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------


using System;
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
        public static Version GetMetadataVersion(ServiceConfiguration serviceConfiguration)
        {
            if (string.IsNullOrEmpty(serviceConfiguration.Endpoint))
                throw new ArgumentNullException("OData Service Endpoint", "Input the metadata document resource");

            if (File.Exists(serviceConfiguration.Endpoint))
                serviceConfiguration.Endpoint = new FileInfo(serviceConfiguration.Endpoint).FullName;

            if (serviceConfiguration.Endpoint.StartsWith("https:", StringComparison.Ordinal)
                || serviceConfiguration.Endpoint.StartsWith("http", StringComparison.Ordinal))
            {
                if (!serviceConfiguration.Endpoint.EndsWith("$metadata", StringComparison.Ordinal))
                {
                    serviceConfiguration.Endpoint = serviceConfiguration.Endpoint.TrimEnd('/') + "/$metadata";
                }
            }

            

            Stream metadataStream;
            Uri metadataUri = new Uri(serviceConfiguration.Endpoint);

            if (!metadataUri.IsFile)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(metadataUri);

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

            try
            {
                using (XmlReader reader = XmlReader.Create(metadataStream))
                {
                    while (reader.NodeType != XmlNodeType.Element)
                    {
                        reader.Read();
                    }

                    if (reader.EOF)
                    {
                        throw new InvalidOperationException("The metadata is an empty file");
                    }

                    Constants.SupportedEdmxNamespaces.TryGetValue(reader.NamespaceURI, out var edmxVersion);
                    return edmxVersion;
                }
            }
            catch (WebException e)
            {
                throw new InvalidOperationException(string.Format("The metadata cannot be accessed"), e);
            }
            finally
            { 
                metadataStream?.Dispose();
            }
        }
    }
}
