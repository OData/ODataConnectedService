//-----------------------------------------------------------------------------
// <copyright file="UserSettings.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;
using Newtonsoft.Json;

namespace Microsoft.OData.CodeGen.Models
{
    [DataContract]
    public class UserSettings : INotifyPropertyChanged
    {
        #region Const Members

        private const int MaxMruEntries = 10;

        #endregion Const Members

        #region Readonly Members

        private readonly ConnectedServiceLogger logger;

        private readonly string configName;

        #endregion Readonly Members

        #region Private Members

        private ObservableCollection<string> mruEndpoints;

        private string serviceName;

        private string endpoint;

        private string generatedFileNamePrefix;

        private bool useNamespacePrefix;

        private string namespacePrefix;

        private bool useDataServiceCollection;

        private bool makeTypesInternal;

        private bool openGeneratedFilesInIDE;

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

        [DataMember]
        public ObservableCollection<string> MruEndpoints
        {
            get { return mruEndpoints; }
            set
            {
                mruEndpoints = value;
                OnPropertyChanged();
            }
        }

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

        [DataMember]
        public bool OpenGeneratedFilesInIDE
        {
            get { return openGeneratedFilesInIDE; }
            set
            {
                openGeneratedFilesInIDE = value;
                OnPropertyChanged();
            }
        }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettings"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="configName">The name to use for the config file.</param>
        public UserSettings(ConnectedServiceLogger logger, string configName = "Settings")
        {
            this.configName = configName;
            this.logger = logger;
            // Desired defaults
            GeneratedFileNamePrefix = Constants.DefaultReferenceFileName;
            ServiceName = Constants.DefaultServiceName;
            UseDataServiceCollection = true; // To support entity and property tracking
            EnableNamingAlias = true; // To force upper camel case in the event that entities are named in lower camel case
            IgnoreUnexpectedElementsAndAttributes = true; // Ignore unexpected elements and attributes in the metadata document
            ExcludedBoundOperations = new List<string>();
            ExcludedOperationImports = new List<string>();
            ExcludedSchemaTypes = new List<string>();
            MruEndpoints = new ObservableCollection<string>();
            UserSettingsPersistenceHelper.Load<UserSettings>(
                providerId: Constants.ProviderId,
                name: configName,
                onLoaded: (userSettings) =>
                {
                    // onLoaded action is currently triggered only if settings are loaded from config file
                    // Do this defensively all the same...
                    if (userSettings != null)
                    {
                        foreach (var mruEndpoint in userSettings.MruEndpoints)
                        {
                            MruEndpoints.Add(mruEndpoint);
                        }
                    }
                },
                logger: logger);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSettings"/> class.
        /// </summary>
        /// <param name="configName">The name to use for the config file.</param>
        [JsonConstructor] // Constructor should be used to create a class during deserialization
        public UserSettings(string configName = "Settings")
            : this(null, configName)
        {
        }

        /// <summary>
        /// Saves user settings to config file.
        /// </summary>
        public void Save()
        {
            UserSettingsPersistenceHelper.Save(this, Constants.ProviderId, configName, null, logger);
        }

        /// <summary>
        /// Loads user settings from config file.
        /// </summary>
        public void Load()
        {
            var userSettings = UserSettingsPersistenceHelper.Load<UserSettings>(
                Constants.ProviderId, configName, null, logger);

            if (userSettings != null)
            {
                this.CopyPropertiesFrom(userSettings);
            }
        }

        /// <summary>
        /// Adds endpoint to the most recently used endpoints collection.
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        public void AddMruEndpoint(string mruEndpoint)
        {
            if (string.IsNullOrWhiteSpace(mruEndpoint))
            {
                return;
            }

            var index = MruEndpoints.IndexOf(mruEndpoint);

            if (index >= 0)
            {
                // Remove possible duplicates
                for (var i = MruEndpoints.Count - 1; index > 0 && i > index; i--)
                {
                    if (MruEndpoints[i].Equals(mruEndpoint, StringComparison.Ordinal))
                    {
                        MruEndpoints.RemoveAt(i);
                    }
                }

                // Endpoint not at index 0
                if (index > 0)
                {
                    MruEndpoints.Move(index, 0);
                }
            }
            else
            {
                while (MruEndpoints.Count >= MaxMruEntries)
                {
                    MruEndpoints.RemoveAt(MruEndpoints.Count - 1);
                }

                MruEndpoints.Insert(0, mruEndpoint);
            }
        }
    }
}
