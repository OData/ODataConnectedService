//-----------------------------------------------------------------------------------
// <copyright file="UserSettingsTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using Microsoft.OData.ConnectedService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ODataConnectedService.Tests.Views
{
    [TestClass]
    public class UserSettingsTest
    {
        [TestMethod]
        public void SaveSettingsWhenSaveMethodIsCalled()
        {
            // Create an instance of UserSettings
            var userSettings = new UserSettings();

            // Set UserSettings
            userSettings.Endpoint = "https://service/$metadata";
            userSettings.GeneratedFileNamePrefix = "MyPrefix";
            userSettings.GenerateMultipleFiles = true;
            userSettings.MakeTypesInternal = true;

            // Save settings
            userSettings.Save();

            //Load settings
            UserSettings settings = UserSettings.Load(null);
            Assert.AreEqual("https://service/$metadata", settings.Endpoint);
            Assert.AreEqual("MyPrefix", settings.GeneratedFileNamePrefix);
            Assert.AreEqual(true, settings.GenerateMultipleFiles);
            Assert.AreEqual(true, settings.MakeTypesInternal);

            // Reset User Settings
            ResetUserSettings();
        }

        [TestMethod]
        public void AddToTopOfMruList_ShouldAddToTopWithoutDuplicatingOrExceedingMax()
        {
            // Create an instance of UserSettings
            var userSettings = new UserSettings();

            var endpoint1 = "https://service1/$metadata";
            var endpoint2 = "https://service2/$metadata";
            var endpoint3 = "https://service3/$metadata";
            var endpoint4 = "https://service4/$metadata";
            var endpoint5 = "https://service5/$metadata";
            var endpoint6 = "https://service6/$metadata";
            var endpoint7 = "https://service7/$metadata";
            var endpoint8 = "https://service8/$metadata";
            var endpoint9 = "https://service9/$metadata";
            var endpoint10 = "https://service10/$metadata";
            var endpoint11 = "https://service11/$metadata";

            // Add an endpoint.
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint1);
            Assert.AreEqual(1, userSettings.MruEndpoints.Count);            

            // Add another endpoint.The latest endpoint to be added is at the top of the MruList.
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint2);
            Assert.AreEqual(2, userSettings.MruEndpoints.Count);
            Assert.AreEqual(endpoint2, userSettings.MruEndpoints[0]);
            Assert.AreEqual(endpoint1, userSettings.MruEndpoints[1]);

            // Add duplicate endpoint, Count should remain the same. But the item is moved to top of the MruList.
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint1);
            Assert.AreEqual(2, userSettings.MruEndpoints.Count);
            Assert.AreEqual(endpoint1, userSettings.MruEndpoints[0]);
            Assert.AreEqual(endpoint2, userSettings.MruEndpoints[1]);

            //Add 9 more endpoints. Total should not exceed 10
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint3);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint4);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint5);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint6);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint7);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint8);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint9);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint10);
            UserSettings.AddToTopOfMruList(userSettings?.MruEndpoints, endpoint11);
            Assert.AreEqual(10, userSettings.MruEndpoints.Count);
            //endpoint11 is the latest so on top in the MruList
            Assert.AreEqual(endpoint11, userSettings.MruEndpoints[0]);
            //endpoint2 is the least recent hence removed from the list
            Assert.IsFalse(userSettings.MruEndpoints.Contains(endpoint2));

            // Reset User Settings
            ResetUserSettings();
        }

        private void ResetUserSettings()
        {
            var settings = new UserSettings();
            settings.Save();
        }
    }
}
