//-----------------------------------------------------------------------------
// <copyright file="UserSettings.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Models
{
    [DataContract]
    internal class UserSettings
    {
        private const string Name = "Settings";
        private const int MaxMruEntries = 10;

        private ConnectedServiceLogger logger;

        [DataMember]
        public ObservableCollection<string> MruEndpoints { get; set; }

        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public string Endpoint { get; set; }

        [DataMember]
        public string GeneratedFileNamePrefix { get; set; }

        [DataMember]
        public bool UseNamespacePrefix { get; set; }

        [DataMember]
        public string NamespacePrefix { get; set; }

        [DataMember]
        public bool UseDataServiceCollection { get; set; }

        [DataMember]
        public bool MakeTypesInternal { get; set; }

        [DataMember]
        public bool OpenGeneratedFilesInIDE { get; set; }

        [DataMember]
        public bool GenerateMultipleFiles { get; set; }

        [DataMember]
        public string CustomHttpHeaders { get; set; }

        [DataMember]
        public bool EnableNamingAlias { get; set; }

        [DataMember]
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }

        [DataMember]
        public bool IncludeT4File { get; set; }

        [DataMember]
        public bool IncludeWebProxy { get; set; }

        [DataMember]
        public string WebProxyHost { get; set; }

        [DataMember]
        public bool IncludeWebProxyNetworkCredentials { get; set; }

        [DataMember]
        public string WebProxyNetworkCredentialsUsername { get; set; }

        [DataMember]
        public string WebProxyNetworkCredentialsPassword { get; set; }

        [DataMember]
        public string WebProxyNetworkCredentialsDomain { get; set; }

        [DataMember]
        public bool IncludeCustomHeaders { get; set; }

        [DataMember]
        public List<string> ExcludedOperationImports { get; set; }

        [DataMember]
        public List<string> ExcludedSchemaTypes { get; set; }

        public UserSettings()
        {
            this.MruEndpoints = new ObservableCollection<string>();
        }

        public void Save()
        {
            UserSettingsPersistenceHelper.Save(this, Constants.ProviderId, UserSettings.Name, null, this.logger);
        }

        public static UserSettings Load(ConnectedServiceLogger logger)
        {
            var userSettings = UserSettingsPersistenceHelper.Load<UserSettings>(
                Constants.ProviderId, UserSettings.Name, null, logger) ?? new UserSettings();
            userSettings.logger = logger;

            return userSettings;
        }

        public static void AddToTopOfMruList<T>(ObservableCollection<T> mruList, T item)
        {
            if (mruList == null)
            {
                return;
            }

            var index = mruList.IndexOf(item);
            if (index >= 0)
            {
                // Ensure there aren't any duplicates in the list.
                for (var i = mruList.Count - 1; i > index; i--)
                {
                    if (EqualityComparer<T>.Default.Equals(mruList[i], item))
                    {
                        mruList.RemoveAt(i);
                    }
                }

                if (index > 0)
                {
                    // The item is in the MRU list but it is not at the top.
                    mruList.Move(index, 0);
                }
            }
            else
            {
                // The item is not in the MRU list, make room for it by clearing out the oldest item.
                while (mruList.Count >= UserSettings.MaxMruEntries)
                {
                    mruList.RemoveAt(mruList.Count - 1);
                }

                mruList.Insert(0, item);
            }
        }
    }
}
