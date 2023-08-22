//-----------------------------------------------------------------------------
// <copyright file="UserSettings.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.OData.CodeGen.Models
{
    /// <summary>
    /// Represents options in OData Connected Service Config File
    /// </summary>
    [DataContract]
    public class UserSettings : INotifyPropertyChanged
    {
        #region Private Members

        private string serviceName;

        private string endpoint;

        private string generatedFileNamePrefix;

        private bool useNamespacePrefix;

        private string namespacePrefix;

        private bool useDataServiceCollection;

        private bool makeTypesInternal;

        private bool generateMultipleFiles;

        private string customHttpHeaders;

        private bool storeCustomHttpHeaders;

        private bool enableNamingAlias;

        private bool ignoreUnexpectedElementsAndAttributes;

        private bool includeT4File;

        private bool includeWebProxy;

        private string webProxyHost;

        private bool includeWebProxyNetworkCredentials;

        private bool storeWebProxyNetworkCredentials;

        private string webProxyNetworkCredentialsUsername;

        private string webProxyNetworkCredentialsPassword;

        private string webProxyNetworkCredentialsDomain;

        private bool includeCustomHeaders;

        private List<string> excludedOperationImports;

        private List<string> excludedBoundOperations;

        private List<string> excludedSchemaTypes;

        #endregion Private Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Human readable display name of the service instance.
        /// </summary>
        [DataMember]
        public string ServiceName
        {
            get { return serviceName; }
            set
            {
                serviceName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The URI of the metadata document. The value must be set to a valid service document URI or a local file path
        /// </summary>
        [DataMember]
        public string Endpoint
        {
            get { return endpoint; }
            set
            {
                endpoint = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The name of the generated file name, defaults to Reference .vb/.cs
        /// </summary>
        [DataMember]
        public string GeneratedFileNamePrefix
        {
            get { return generatedFileNamePrefix; }
            set
            {
                generatedFileNamePrefix = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// True if <see cref="NamespacePrefix"/> is used<br />
        /// This member appears in config files but is not used
        /// </summary>
        [DataMember]
        public bool UseNamespacePrefix
        {
            get { return useNamespacePrefix; }
            set
            {
                useNamespacePrefix = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The namespace of the client code generated. 
        /// Example: ODataCliCodeGeneratorSample.NorthWindModel or ODataCliCodeGeneratorSample or it could be a name related to the OData endpoint.
        /// </summary>
        [DataMember]
        public string NamespacePrefix
        {
            get { return namespacePrefix; }
            set
            {
                namespacePrefix = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Enables entity and property tracking
        /// </summary>
        [DataMember]
        public bool UseDataServiceCollection
        {
            get { return useDataServiceCollection; }
            set
            {
                useDataServiceCollection = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Apply the "internal" class modifier on generated classes instead of "public" thereby making them invisible outside the assembly.
        /// </summary>
        [DataMember]
        public bool MakeTypesInternal
        {
            get { return makeTypesInternal; }
            set
            {
                makeTypesInternal = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Split the generated classes into separate files instead of generating all the code in a single file.
        /// </summary>
        [DataMember]
        public bool GenerateMultipleFiles
        {
            get { return generateMultipleFiles; }
            set
            {
                generateMultipleFiles = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Headers that will get sent along with the request when fetching the metadata document from the service. Format: Header1:HeaderValue, Header2:HeaderValue.
        /// </summary>
        [IgnoreDataMember] // Do not serialize - may contain authentication tokens
        public string CustomHttpHeaders
        {
            get { return customHttpHeaders; }
            set
            {
                customHttpHeaders = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Disables/Enables upper camel casing
        /// </summary>
        [DataMember]
        public bool EnableNamingAlias
        {
            get { return enableNamingAlias; }
            set
            {
                enableNamingAlias = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// This flag indicates whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any.
        /// </summary>
        [DataMember]
        public bool IgnoreUnexpectedElementsAndAttributes
        {
            get { return ignoreUnexpectedElementsAndAttributes; }
            set
            {
                ignoreUnexpectedElementsAndAttributes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// This flag indicates that T4 file(s) should be generated and included
        /// </summary>
        [DataMember]
        public bool IncludeT4File
        {
            get { return includeT4File; }
            set
            {
                includeT4File = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A flag to indicate whether to include the web proxy or not
        /// </summary>
        [DataMember]
        public bool IncludeWebProxy
        {
            get { return includeWebProxy; }
            set
            {
                includeWebProxy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// This flag indicates that custom http headers should be stored
        /// </summary>
        [DataMember]
        public bool StoreCustomHttpHeaders
        {
            get { return storeCustomHttpHeaders; }
            set
            {
                storeCustomHttpHeaders = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A web proxy host
        /// </summary>
        [DataMember]
        public string WebProxyHost
        {
            get { return webProxyHost; }
            set
            {
                webProxyHost = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A flag to indicate whether to include web proxy network credentials or not.
        /// </summary>
        [DataMember]
        public bool IncludeWebProxyNetworkCredentials
        {
            get { return includeWebProxyNetworkCredentials; }
            set
            {
                includeWebProxyNetworkCredentials = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A flag to indicate whether to store web proxy network credentials or not<br />
        /// This member appears in config files but is not used
        /// </summary>
        [DataMember]
        public bool StoreWebProxyNetworkCredentials
        {
            get { return storeWebProxyNetworkCredentials; }
            set
            {
                storeWebProxyNetworkCredentials = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Web proxy network credentials username
        /// </summary>
        [IgnoreDataMember] // Do not serialize - security consideration
        public string WebProxyNetworkCredentialsUsername
        {
            get { return webProxyNetworkCredentialsUsername; }
            set
            {
                webProxyNetworkCredentialsUsername = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Web proxy network credentials password
        /// </summary>
        [IgnoreDataMember] // Do not serialize - security consideration
        public string WebProxyNetworkCredentialsPassword
        {
            get { return webProxyNetworkCredentialsPassword; }
            set
            {
                webProxyNetworkCredentialsPassword = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Web proxy network credentials domain
        /// </summary>
        [DataMember]
        public string WebProxyNetworkCredentialsDomain
        {
            get { return webProxyNetworkCredentialsDomain; }
            set
            {
                webProxyNetworkCredentialsDomain = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A flag to indicate whether to include custom headers or not<br />
        /// This member appears in config files but is not used
        /// </summary>
        [DataMember]
        public bool IncludeCustomHeaders
        {
            get { return includeCustomHeaders; }
            set
            {
                includeCustomHeaders = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Comma-separated list of the names of operation imports to exclude from the generated code. Example: ExcludedOperationImport1,ExcludedOperationImport2.
        /// </summary>
        [DataMember]
        public List<string> ExcludedOperationImports
        {
            get { return excludedOperationImports; }
            set
            {
                excludedOperationImports = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Comma-separated list of the names of bound operations to exclude from the generated code.Example: BoundOperation1,BoundOperation2.
        /// </summary>
        [DataMember]
        public List<string> ExcludedBoundOperations
        {
            get { return excludedBoundOperations; }
            set
            {
                excludedBoundOperations = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Comma-separated list of the names of entity types to exclude from the generated code.Example: EntityType1,EntityType2,EntityType3.
        /// </summary>
        [DataMember]
        public List<string> ExcludedSchemaTypes
        {
            get { return excludedSchemaTypes; }
            set
            {
                excludedSchemaTypes = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
