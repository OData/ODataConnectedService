//-----------------------------------------------------------------------------------
// <copyright file="UserSettingsTest.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using Microsoft.OData.CodeGen.Models;
using Xunit;

namespace ODataConnectedService.Tests.Views
{
    public sealed class UserSettingsTest : IDisposable
    {
        // Will be executed after every test to set test user settings to a known initial state
        public void Dispose()
        {
            var userSettings = new UserSettings(configName: "TestUserSettings");
            userSettings.MruEndpoints.Clear();
            userSettings.Save();
        }

        [Fact]
        public void SaveSettingsWhenSaveMethodIsCalled()
        {
            // Create an instance of UserSettings
            var userSettings = new UserSettings(configName: "TestUserSettings");

            // Set UserSettings
            userSettings.Endpoint = "https://service/$metadata";
            userSettings.GeneratedFileNamePrefix = "MyPrefix";
            userSettings.GenerateMultipleFiles = true;
            userSettings.MakeTypesInternal = true;
            userSettings.OmitVersioningInfo = true;

            // Save settings
            userSettings.Save();

            // Load settings
            var settings = new UserSettings(configName: "TestUserSettings");
            settings.Load();
            Assert.Equal("https://service/$metadata", settings.Endpoint);
            Assert.Equal("MyPrefix", settings.GeneratedFileNamePrefix);
            Assert.True(settings.GenerateMultipleFiles);
            Assert.True(settings.MakeTypesInternal);
            Assert.True(settings.OmitVersioningInfo);
        }

        [Fact]
        public void AddToTopOfMruList_ShouldAddToTopWithoutDuplicatingOrExceedingMax()
        {
            // Create an instance of UserSettings
            var userSettings = new UserSettings(configName: "TestUserSettings");

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
            userSettings.AddMruEndpoint(endpoint1);
            Assert.Single(userSettings.MruEndpoints);

            // Add another endpoint.The latest endpoint to be added is at the top of the MruList.
            userSettings.AddMruEndpoint(endpoint2);
            Assert.Equal(2, userSettings.MruEndpoints.Count);
            Assert.Equal(endpoint2, userSettings.MruEndpoints[0]);
            Assert.Equal(endpoint1, userSettings.MruEndpoints[1]);

            // Add duplicate endpoint, Count should remain the same. But the item is moved to top of the MruList.
            userSettings.AddMruEndpoint(endpoint1);
            Assert.Equal(2, userSettings.MruEndpoints.Count);
            Assert.Equal(endpoint1, userSettings.MruEndpoints[0]);
            Assert.Equal(endpoint2, userSettings.MruEndpoints[1]);

            // Add 9 more endpoints. Total should not exceed 10
            userSettings.AddMruEndpoint(endpoint3);
            userSettings.AddMruEndpoint(endpoint4);
            userSettings.AddMruEndpoint(endpoint5);
            userSettings.AddMruEndpoint(endpoint6);
            userSettings.AddMruEndpoint(endpoint7);
            userSettings.AddMruEndpoint(endpoint8);
            userSettings.AddMruEndpoint(endpoint9);
            userSettings.AddMruEndpoint(endpoint10);
            userSettings.AddMruEndpoint(endpoint11);
            Assert.Equal(10, userSettings.MruEndpoints.Count);
            // endpoint11 is the latest so on top in the MruList
            Assert.Equal(endpoint11, userSettings.MruEndpoints[0]);
            // endpoint2 is the least recent hence removed from the list
            Assert.Contains(userSettings.MruEndpoints, d => !d.Contains(endpoint2));
        }
    }
}
