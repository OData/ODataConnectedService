//-----------------------------------------------------------------------------
// <copyright file="ConnectedServiceUserSettings.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;
using Newtonsoft.Json;

namespace Microsoft.OData.CodeGen.Models
{
    [DataContract]
    public class ConnectedServiceUserSettings : UserSettings
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

        private bool openGeneratedFilesInIDE;

        #endregion Private Members

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
        public bool OpenGeneratedFilesInIDE
        {
            get { return openGeneratedFilesInIDE; }
            set
            {
                openGeneratedFilesInIDE = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedServiceUserSettings"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="configName">The name to use for the config file.</param>
        public ConnectedServiceUserSettings(ConnectedServiceLogger logger, string configName = "Settings")
            : base()
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
            UserSettingsPersistenceHelper.Load<ConnectedServiceUserSettings>(
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
        /// Initializes a new instance of the <see cref="ConnectedServiceUserSettings"/> class.
        /// </summary>
        /// <param name="configName">The name to use for the config file.</param>
        [JsonConstructor] // Constructor should be used to create a class during deserialization
        public ConnectedServiceUserSettings(string configName = "Settings")
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
            var userSettings = UserSettingsPersistenceHelper.Load<ConnectedServiceUserSettings>(
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
