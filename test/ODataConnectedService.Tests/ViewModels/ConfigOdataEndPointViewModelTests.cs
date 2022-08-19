//-----------------------------------------------------------------------------------
// <copyright file="ConfigOdataEndPointViewModelTests.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------------------

using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
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
            configOdataEndPointViewModel = new ConfigODataEndpointViewModel(userSettings);
            serviceWizard.Pages.Add(configOdataEndPointViewModel);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

        [TestMethod]
        public void OnPageLeavingConfigODataEndpointPageTest()
        {
            string edmx = GeneratedCodeHelpers.LoadReferenceContent("Simple.xml");
            string expectedTempfileContent = GeneratedCodeHelpers.LoadReferenceContent("TempSimple.xml");
            Task<PageNavigationResult> pageNavigationResultTask;
            PageNavigationResult pageNavigationResult;

            File.WriteAllText("EdmxFile.xml", edmx);

            //Check if an error is thrown if the on leaving the page without providing the endpoint
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            pageNavigationResult = pageNavigationResultTask?.Result;
            Assert.IsNotNull(pageNavigationResult.ErrorMessage);
            Assert.IsTrue(pageNavigationResult.ErrorMessage.Contains(Constants.InputServiceEndpointMsg), "User is not prompted to enter endpoint");
            Assert.IsFalse(pageNavigationResult.IsSuccess);
            Assert.IsTrue(pageNavigationResult.ShowMessageBoxOnFailure);

            //Provide a url without $metadata
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://mysite/ODataService";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is appended as the last segment if it was not the last segment of the url
            Assert.AreEqual("http://mysite/ODataService/$metadata", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Provide a url with $metadata/
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://mysite/ODataService/$metadata/";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is appended as the last segment and '/' is removed
            Assert.AreEqual("http://mysite/ODataService/$metadata", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Provide a url with "$metadata" as the last segment in the url with fragment and query segments
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://user:password@mysite/ODataService/$metadata?$schemaversion=2.0#fragment";
            _ = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if the url is detected as valid and is unmodified
            Assert.AreEqual("http://user:password@mysite/ODataService/$metadata?$schemaversion=2.0#fragment", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Provide a url with query and fragment segments without $metadata
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://user:password@mysite/ODataService?$schemaversion=2.0#fragment";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is appended as the last segment
            Assert.AreEqual("http://user:password@mysite/ODataService/$metadata?$schemaversion=2.0#fragment", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Provide a url with a fragment segment without $metadata
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://user:password@mysite/ODataService#fragment";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is appended as the last segment
            Assert.AreEqual("http://user:password@mysite/ODataService/$metadata#fragment", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Provide a url with $metadata and a fragment segment without a query segment
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://user:password@mysite/ODataService/$metadata#fragment";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is appended as the last segment
            Assert.AreEqual("http://user:password@mysite/ODataService/$metadata#fragment", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Provide a url with $metadata and a query segment without a fragment segment
            configOdataEndPointViewModel.UserSettings.Endpoint = "http://user:password@mysite/ODataService/$metadata?$schemaversion=2.0";
            pageNavigationResultTask = configOdataEndPointViewModel.OnPageLeavingAsync(null);

            //Check if $metadata is appended as the last segment
            Assert.AreEqual("http://user:password@mysite/ODataService/$metadata?$schemaversion=2.0", configOdataEndPointViewModel.ServiceConfiguration.Endpoint);

            //Check if an exception is thrown for an invalid url and the user is notified
            pageNavigationResult = pageNavigationResultTask?.Result;
            Assert.IsNotNull(pageNavigationResult.ErrorMessage);
            Assert.IsTrue(pageNavigationResult.ErrorMessage.Contains("The remote name could not be resolved")
                || pageNavigationResult.ErrorMessage.Contains("The remote server returned an error: (407) Proxy Authentication Required"));
            Assert.IsFalse(pageNavigationResult.IsSuccess);
            Assert.IsTrue(pageNavigationResult.ShowMessageBoxOnFailure);


            configOdataEndPointViewModel.UserSettings.Endpoint = Path.Combine(Directory.GetCurrentDirectory(), "EdmxFile.xml");
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
            Assert.AreEqual(configOdataEndPointViewModel.EdmxVersion.ToString(), "4.0.0.0", "Version not properly detected");
        }
    }
}
