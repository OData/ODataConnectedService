// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Tests.Templates;
using Microsoft.OData.ConnectedService.ViewModels;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ODataConnectedService.Tests.TestHelpers;

namespace ODataConnectedService.Tests.ViewModels
{
    [TestClass]
    public class ConfigOdataEndPointViewModelTests
    {
        private static ConfigODataEndpointViewModel configOdataEndPointViewModel;
        private static UserSettings userSettings;
        private static ODataConnectedServiceWizard serviceWizard;

        [TestInitialize]
        public void Init()
        {
            userSettings = new UserSettings();
            serviceWizard = new ODataConnectedServiceWizard(null);
            configOdataEndPointViewModel = new ConfigODataEndpointViewModel(userSettings, serviceWizard);
        }

        [TestMethod]
        public void OnPageLeavingConfigODataEndpointPageTest()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("Simple.xml");
            string expectedTempfileContent = GeneratedCodeHelpers.LoadReferenceContent("TempSimple.xml");
            Task<PageNavigationResult> pageNavigationResultTask;
            PageNavigationResult pageNavigationResult;

            File.WriteAllText("EdmxFile.xml",edmx);

            //Check if an error is thrown if the on leaving the page without providing the endpoint
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            pageNavigationResult = pageNavigationResultTask?.Result;
            Assert.IsNotNull(pageNavigationResult.ErrorMessage);
            Assert.IsTrue(pageNavigationResult.ErrorMessage.Contains(Constants.InputServiceEndpointMsg),"User is not prompted to enter endpoint");
            Assert.IsFalse(pageNavigationResult.IsSuccess);
            Assert.IsTrue(pageNavigationResult.ShowMessageBoxOnFailure);

            //Provide a url without $metadata
            configOdataEndPointViewModel.Endpoint = "http://mysite/ODataService";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is apended if the url does not have it added at the end
            Assert.AreEqual(configOdataEndPointViewModel.Endpoint, "http://mysite/ODataService/$metadata");

            //Check if an exception is thrown for an invalid url and the user is notified
            pageNavigationResult = pageNavigationResultTask?.Result;
            Assert.IsNotNull(pageNavigationResult.ErrorMessage);
            Assert.IsTrue(pageNavigationResult.ErrorMessage.Contains("The remote name could not be resolved"));
            Assert.IsFalse(pageNavigationResult.IsSuccess);
            Assert.IsTrue(pageNavigationResult.ShowMessageBoxOnFailure);


            configOdataEndPointViewModel.Endpoint = Path.Combine(Directory.GetCurrentDirectory(),"EdmxFile.xml");
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if any errors were reported
            pageNavigationResult = pageNavigationResultTask?.Result;
            Assert.IsNull(pageNavigationResult.ErrorMessage);
            Assert.IsTrue(pageNavigationResult.IsSuccess);
            Assert.IsFalse(pageNavigationResult.ShowMessageBoxOnFailure);

            //Check if the content writtent to the temp file is correct
            string actualTempFileContent = File.ReadAllText(configOdataEndPointViewModel.MetadataTempPath);
            Assert.AreEqual(expectedTempfileContent.Trim(), actualTempFileContent.Trim(), "temp metadata file not properly written");

            //Check if Edmx verison of has correctly been detected
            Assert.AreEqual(configOdataEndPointViewModel.EdmxVersion.ToString(),"4.0.0.0","Version not properly detected");
        }
    }
}
