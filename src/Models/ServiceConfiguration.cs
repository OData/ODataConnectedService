//-----------------------------------------------------------------------------
// <copyright file="ServiceConfiguration.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.ConnectedService.Models
{
    internal class ServiceConfiguration
    {
        public string ServiceName { get; set; }
        public string Endpoint { get; set; }
        public Version EdmxVersion { get; set; }
        public string GeneratedFileNamePrefix { get; set; }
        public bool UseNamespacePrefix { get; set; }
        public string NamespacePrefix { get; set; }
        public bool UseDataServiceCollection { get; set; }
        public bool MakeTypesInternal { get; set; }
        public bool OpenGeneratedFilesInIDE { get; set; }
        public bool GenerateMultipleFiles { get; set; }
        public string CustomHttpHeaders { get; set; }
        public bool IncludeWebProxy { get; set; }
        public string WebProxyHost { get; set; }
        public bool IncludeWebProxyNetworkCredentials { get; set; }
        public string WebProxyNetworkCredentialsUsername { get; set; }
        public string WebProxyNetworkCredentialsPassword { get; set; }
        public string WebProxyNetworkCredentialsDomain { get; set; }
        public bool IncludeCustomHeaders { get; set; }
        public List<string> ExcludedSchemaTypes { get; set; }

    }

    internal class ServiceConfigurationV4 : ServiceConfiguration
    {
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public bool IncludeT4File { get; set; }
        public List<string> ExcludedOperationImports;
        
    }
}