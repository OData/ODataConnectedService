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
    /// Represents OData Connected Service user settings.
    /// </summary>
    [DataContract]
    public class BaseUserSettings : INotifyPropertyChanged
    {
        #region Private Members

        private string serviceName;

        private string endpoint;

        private string generatedFileNamePrefix;

        private bool useNamespacePrefix;

        private string namespacePrefix;

        private bool useDataServiceCollection;

        private bool makeTypesInternal;

        private bool omitVersioningInfo;

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
        /// Gets or sets the human readable display name of the service instance.
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
        /// Gets or sets the URI of the metadata document. The value must be set to a valid service document URI or a local file path.
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
        /// Gets or sets the name of the generated file name. Defaults to Reference.vb/.cs when not provided.
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
        /// Gets or sets a value that determines whether to use <see cref="NamespacePrefix"/>.
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
        /// Gets or sets the namespace for the generated service proxy classes.
        /// </summary>
        /// <remarks>
        /// Must be a valid C# identifier name, e.g., MyService.Models.
        /// When not provided, a namespace that corresponds to the one that the models of the OData service are defined in is used.
        /// </remarks>
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
        /// Gets or sets a value that determines whether to enable entity and property tracking.
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
        /// Gets or sets a value that determines whether to apply the "internal" class modifier on generated classes instead of "public" thereby making them invisible outside the assembly.
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
        /// Gets or sets a value indicating whether to omit runtime version and code generation timestamp from the generated files.
        /// </summary>
        [DataMember]
        public bool OmitVersioningInfo
        {
            get { return omitVersioningInfo; }
            set
            {
                omitVersioningInfo = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to split the generated classes into separate files instead of generating all the code in a single file.
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
        /// Gets or sets headers that will get sent along with the request when fetching the metadata document from the service. Format: Header1:HeaderValue, Header2:HeaderValue.
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
        /// Gets or sets a value that determines whether to use C# casing style.
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
        /// Gets or sets a value that determines whether to ignore unexpected elements and attributes in the metadata document and generate the client code if any.
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
        /// Gets or sets a value that determines whether T4 file(s) should be generated and included.
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
        /// Gets or sets a value that determines whether to include the web proxy or not.
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
        /// Gets or sets a value that determines whether custom http headers should be stored.
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
        /// Gets or sets the web proxy host.
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
        /// Gets or sets a value that determines whether to include web proxy network credentials or not.
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
        /// Gets or sets a value that determines whether to store web proxy network credentials or not.
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
        /// Gets or sets the web proxy network credentials username.
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
        /// Gets or sets the web proxy network credentials password.
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
        /// Gets or sets the web proxy network credentials domain.
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
        /// Gets or sets a value that determines whether to include custom headers or not.
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
        /// Gets or sets the comma-separated list of the names of operation imports to exclude from the generated code. Example: ExcludedOperationImport1,ExcludedOperationImport2.
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
        /// Gets or sets the comma-separated list of the names of bound operations to exclude from the generated code.Example: BoundOperation1,BoundOperation2.
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
        /// Gets or sets the comma-separated list of the names of entity types to exclude from the generated code.Example: EntityType1,EntityType2,EntityType3.
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
