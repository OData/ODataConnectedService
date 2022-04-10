//-----------------------------------------------------------------------------
// <copyright file="UserSettingsPersistenceHelper.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.Common
{
    public class UserSettingsPersistenceHelper
    {
        /// <summary>
        /// Saves user settings to isolated storage.  The data is stored with the user's roaming profile.
        /// </summary>
        /// <remarks>
        /// Non-critical exceptions are handled by writing an error message in the output window.
        /// </remarks>
        public static void Save(object userSettings, string providerId, string name, Action onSaved, ConnectedServiceLogger logger)
        {
            var fileName = UserSettingsPersistenceHelper.GetStorageFileName(providerId, name);

            UserSettingsPersistenceHelper.ExecuteNoncriticalOperation(
                () =>
                {
                    using (IsolatedStorageFile file = UserSettingsPersistenceHelper.GetIsolatedStorageFile())
                    {
                        IsolatedStorageFileStream stream = null;
                        try
                        {
                            // note: this overwrites existing settings file if it exists
                            stream = file.OpenFile(fileName, FileMode.Create);

                            using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
                            {
                                var dcs = new DataContractSerializer(userSettings.GetType());
                                dcs.WriteObject(writer, userSettings);

                                writer.Flush();
                            }
                        }
                        finally
                        {
                            DisposeStream(stream);
                        }
                    }

                    onSaved?.Invoke();
                },
                logger,
                "Failed loading the {0} user settings",
                fileName);
        }

        /// <summary>
        /// Loads user settings from isolated storage.
        /// </summary>
        /// <remarks>
        /// Non-critical exceptions are handled by writing an error message in the output window and
        /// returning null.
        /// </remarks>
        public static T Load<T>(string providerId, string name, Action<T> onLoaded, ConnectedServiceLogger logger) where T : class
        {
            var fileName = UserSettingsPersistenceHelper.GetStorageFileName(providerId, name);
            T result = null;

            UserSettingsPersistenceHelper.ExecuteNoncriticalOperation(
                () =>
                {
                    using (IsolatedStorageFile file = UserSettingsPersistenceHelper.GetIsolatedStorageFile())
                    {
                        if (file.FileExists(fileName))
                        {
                            IsolatedStorageFileStream stream = null;
                            try
                            {
                                stream = file.OpenFile(fileName, FileMode.Open);
                                var settings = new XmlReaderSettings()
                                {
                                    XmlResolver = null
                                };

                                using (var reader = XmlReader.Create(stream, settings))
                                {
                                    var dcs = new DataContractSerializer(typeof(T));
                                    result = dcs.ReadObject(reader) as T;
                                }
                            }
                            finally
                            {
                                DisposeStream(stream);
                            }

                            if (onLoaded != null && result != null)
                            {
                                onLoaded(result);
                            }
                        }
                    }
                },
                logger,
                "Failed loading the {0} user settings",
                fileName);

            return result;
        }

        private static string GetStorageFileName(string providerId, string name)
        {
            return providerId + "." + name + ".xml";
        }

        private static IsolatedStorageFile GetIsolatedStorageFile()
        {
            return IsolatedStorageFile.GetStore(
                IsolatedStorageScope.Assembly | IsolatedStorageScope.User | IsolatedStorageScope.Roaming, null, null);
        }

        private static void ExecuteNoncriticalOperation(
            Action operation,
            ConnectedServiceLogger logger,
            string failureMessage,
            string failureMessageArg)
        {
            try
            {
                operation();
            }
            catch (Exception ex)
            {
                logger?.WriteMessageAsync(LoggerMessageCategory.Warning, failureMessage, failureMessageArg, ex);
            }
        }

        private static void DisposeStream(IsolatedStorageFileStream stream)
        {
            stream?.Dispose();
        }
    }
}