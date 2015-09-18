using Microsoft.OData.ConnectedService.Common;
using Microsoft.VisualStudio.ConnectedServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.ConnectedService.Models
{
    [DataContract]
    internal class UserSettings
    {
        private const string Name = "Settings";
        private const int MaxMruEntries = 10;

        private ConnectedServiceLogger logger;

        [DataMember]
        public ObservableCollection<string> MruEndpoints { get; private set; }

        private UserSettings()
        {
            this.MruEndpoints = new ObservableCollection<string>();
        }

        public void Save()
        {
            UserSettingsPersistenceHelper.Save(this, Constants.ProviderId, UserSettings.Name, null, this.logger);
        }

        public static UserSettings Load(ConnectedServiceLogger logger)
        {
            UserSettings userSettings = UserSettingsPersistenceHelper.Load<UserSettings>(
                Constants.ProviderId, UserSettings.Name, null, logger) ?? new UserSettings();
            userSettings.logger = logger;

            return userSettings;
        }

        public static void AddToTopOfMruList<T>(ObservableCollection<T> mruList, T item)
        {
            int index = mruList.IndexOf(item);
            if (index >= 0)
            {
                // Ensure there aren't any duplicates in the list.
                for (int i = mruList.Count - 1; i > index; i--)
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
