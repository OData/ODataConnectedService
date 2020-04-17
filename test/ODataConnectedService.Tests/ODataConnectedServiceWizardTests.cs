using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.ConnectedServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Shell;
using Microsoft.OData.ConnectedService;
using System.ComponentModel;
using Microsoft.OData.ConnectedService.Models;
using FluentAssertions;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Views;

namespace ODataConnectedService.Tests
{
    [TestClass]
    public class ODataConnectedServiceWizardTests
    {

        private ServiceConfigurationV4 GetTestConfig()
        {
            return new ServiceConfigurationV4()
            {
                Endpoint = "https://service/$metadata",
                EdmxVersion = Constants.EdmxVersion4,
                ServiceName = "MyService",
                IncludeCustomHeaders = true,
                CustomHttpHeaders = "Key1:Val1\nKey2:Val2",
                IncludeWebProxy = true,
                WebProxyHost = "http://localhost:8080",
                IncludeWebProxyNetworkCredentials = true,
                WebProxyNetworkCredentialsDomain = "domain",
                WebProxyNetworkCredentialsUsername = "username",
                WebProxyNetworkCredentialsPassword = "password",
                UseDataServiceCollection = true,
                GenerateMultipleFiles = true,
                UseNamespacePrefix = true,
                NamespacePrefix = "Namespace",
                GeneratedFileNamePrefix = "GeneratedCode",
                EnableNamingAlias = true,
                OpenGeneratedFilesInIDE = true,
                MakeTypesInternal = true,
                IgnoreUnexpectedElementsAndAttributes = true,
                IncludeT4File = true
            };
        }

        [TestMethod]
        public void TestConstructor_ShouldUseDefaultSettingsWhenNotUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(false, savedConfig);

            var wizard = new ODataConnectedServiceWizard(context);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            Assert.AreEqual(Constants.DefaultServiceName, endpointPage.ServiceName);
            Assert.IsNull(endpointPage.Endpoint);
            Assert.IsNull(endpointPage.EdmxVersion);
            Assert.IsFalse(endpointPage.IncludeCustomHeaders);
            Assert.IsNull(endpointPage.CustomHttpHeaders);
            Assert.IsFalse(endpointPage.IncludeWebProxy);
            Assert.IsNull(endpointPage.WebProxyHost);
            Assert.IsFalse(endpointPage.IncludeWebProxyNetworkCredentials);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsDomain);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsUsername);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsPassword);

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            Assert.AreEqual(Constants.DefaultReferenceFileName, advancedPage.GeneratedFileNamePrefix);
            Assert.IsFalse(advancedPage.UseNamespacePrefix);
            Assert.AreEqual(Constants.DefaultReferenceFileName, advancedPage.NamespacePrefix);
            Assert.IsFalse(advancedPage.UseDataServiceCollection);
            Assert.IsFalse(advancedPage.EnableNamingAlias);
            Assert.IsFalse(advancedPage.OpenGeneratedFilesInIDE);
            Assert.IsFalse(advancedPage.IncludeT4File);
            Assert.IsFalse(advancedPage.GenerateMultipleFiles);
            Assert.IsFalse(advancedPage.IgnoreUnexpectedElementsAndAttributes);
            Assert.IsFalse(advancedPage.MakeTypesInternal);
        }

        [TestMethod]
        public void TestConstructor_LoadsSavedConfigWhenUpdating()
        {
            var savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);

            var wizard = new ODataConnectedServiceWizard(context);

            Assert.AreEqual(savedConfig, wizard.ServiceConfig);

            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            Assert.AreEqual("https://service/$metadata", endpointPage.Endpoint);
            Assert.AreEqual("MyService", endpointPage.ServiceName);
            Assert.IsTrue(endpointPage.IncludeCustomHeaders);
            Assert.AreEqual("Key1:Val1\nKey2:Val2", endpointPage.CustomHttpHeaders);
            Assert.IsTrue(endpointPage.IncludeWebProxy);
            Assert.AreEqual("http://localhost:8080", endpointPage.WebProxyHost);
            Assert.IsTrue(endpointPage.IncludeWebProxyNetworkCredentials);
            Assert.AreEqual("domain", endpointPage.WebProxyNetworkCredentialsDomain);
            // username and password are not restored from the config
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsUsername);
            Assert.IsNull(endpointPage.WebProxyNetworkCredentialsPassword);

            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            Assert.AreEqual("GeneratedCode", advancedPage.GeneratedFileNamePrefix);
            Assert.IsTrue(advancedPage.UseNamespacePrefix);
            Assert.AreEqual("Namespace", advancedPage.NamespacePrefix);
            Assert.IsTrue(advancedPage.UseDataServiceCollection);
            Assert.IsTrue(advancedPage.EnableNamingAlias);
            Assert.IsTrue(advancedPage.OpenGeneratedFilesInIDE);
            Assert.IsTrue(advancedPage.IncludeT4File);
            Assert.IsTrue(advancedPage.GenerateMultipleFiles);
            Assert.IsTrue(advancedPage.IgnoreUnexpectedElementsAndAttributes);
            Assert.IsTrue(advancedPage.MakeTypesInternal);
        }

        [TestMethod]
        public void TestDisableReaOonlyFieldsWhenUpdating()
        {
            ServiceConfigurationV4 savedConfig = GetTestConfig();
            var context = new TestConnectedServiceProviderContext(true, savedConfig);
            var wizard = new ODataConnectedServiceWizard(context);
            
            // endpoint page
            var endpointPage = wizard.ConfigODataEndpointViewModel;
            endpointPage.OnPageEnteringAsync(new WizardEnteringArgs(null)).Wait();
            var endpointView = endpointPage.View as ConfigODataEndpoint;

            Assert.IsFalse(endpointView.ServiceName.IsEnabled);
            Assert.IsFalse(endpointView.Endpoint.IsEnabled);

            // advanced settings page
            var advancedPage = wizard.AdvancedSettingsViewModel;
            advancedPage.OnPageEnteringAsync(new WizardEnteringArgs(endpointPage)).Wait();
            var advancedView = advancedPage.View as AdvancedSettings;
            Assert.IsFalse(advancedView.IncludeT4File.IsEnabled);
            Assert.IsFalse(advancedView.GenerateMultipleFiles.IsEnabled);
        }
    }

    internal class TestConnectedServiceProviderContext: ConnectedServiceProviderContext
    {
        ServiceConfigurationV4 savedData;

        public TestConnectedServiceProviderContext(bool isUpdating = false, ServiceConfigurationV4 savedData = null) : base()
        {
            this.savedData = savedData;

            if (isUpdating)
            {
                UpdateContext = new ConnectedServiceUpdateContext(new Version(), new TestVsHierarchyItem());
            }
        }

        public override IDictionary<string, object> Args => throw new NotImplementedException();

        public override XmlConfigHelper CreateReadOnlyXmlConfigHelper()
        {
            throw new NotImplementedException();
        }

        public override TData GetExtendedDesignerData<TData>()
        {
            return savedData as TData;
        }

        public override Microsoft.VisualStudio.Shell.IVsHierarchyItem GetServiceFolder(string serviceFolderName)
        {
            return new TestVsHierarchyItem();
        }

        public override void InitializeUpdateContext(Microsoft.VisualStudio.Shell.IVsHierarchyItem serviceFolder)
        {
            throw new NotImplementedException();
        }

        public override void SetExtendedDesignerData<TData>(TData data)
        {
            savedData = data as ServiceConfigurationV4;
        }

        public override IDisposable StartBusyIndicator(string message = null)
        {
            throw new NotImplementedException();
        }
    }

    public class TestVsHierarchyItem : IVsHierarchyItem
    {
        public IVsHierarchyItemIdentity HierarchyIdentity => throw new NotImplementedException();

        public IVsHierarchyItem Parent => throw new NotImplementedException();

        public IEnumerable<IVsHierarchyItem> Children => throw new NotImplementedException();

        public bool AreChildrenRealized => throw new NotImplementedException();

        public string Text => "Item";

        public string CanonicalName => "CanonicalName";

        public bool IsBold { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsCut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsDisposed => false;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
    }

    //public abstract class Test
    //{
    //    public abstract IVsHierarchyItem Stuff();
    //}

    //public class TestSub : Test
    //{
    //    public override IVsHierarchyItem Stuff()
    //    {
    //        return new TestVsHierarchyItem();
    //    }
    //}
}
