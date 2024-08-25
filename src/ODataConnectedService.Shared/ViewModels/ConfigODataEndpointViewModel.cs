//----------------------------------------------------------------------------------
// <copyright file="ConfigODataEndpointViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Common;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.Common;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.ConnectedServices;
using static Microsoft.VisualStudio.Shell.ThreadedWaitDialogHelper;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class ConfigODataEndpointViewModel : ConnectedServiceWizardPage
    {
        public Version EdmxVersion { get; set; }

        public string MetadataTempPath { get; set; }

        public UserSettings UserSettings { get; internal set; }

        public ServiceConfiguration ServiceConfiguration { get; set; }

        public event EventHandler<EventArgs> PageEntering;

        public IEdmModel Model { get; set; }

        public ConfigODataEndpointViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Configure endpoint";
            this.Description = "Enter or choose an OData service endpoint to begin";
            this.Legend = "Endpoint";
            this.UserSettings = userSettings;
        }
        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args).ConfigureAwait(false);
            this.View = new ConfigODataEndpoint { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> PageLeaving;

        public override async Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            try
            {
                if (View is ConfigODataEndpoint view)
                {
                    view.NavigationProgressBar.Visibility = System.Windows.Visibility.Visible;
                }
                this.PageLeaving?.Invoke(this, EventArgs.Empty);
                ServiceConfiguration = GetServiceConfiguration();
                ServiceConfiguration.Endpoint = CodeGen.Common.MetadataReader.NormalizeUri(ServiceConfiguration);
                var (path, version) = await CodeGen.Common.MetadataReader.ProcessServiceMetadata(ServiceConfiguration).ConfigureAwait(false);

                this.MetadataTempPath = path;
                this.EdmxVersion = version;
                Model = await EdmHelper.GetEdmModelFromFileAsync(this.MetadataTempPath).ConfigureAwait(false);

                // Makes sense to add MRU endpoint at this point since GetMetadata manipulates UserSettings.Endpoint
                UserSettings.Endpoint = ServiceConfiguration.Endpoint;
                UserSettings.AddMruEndpoint(UserSettings.Endpoint);
                PageLeaving?.Invoke(this, EventArgs.Empty);
                return await base.OnPageLeavingAsync(args).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                return new PageNavigationResult
                {
                    ErrorMessage = e.Message,
                    IsSuccess = false,
                    ShowMessageBoxOnFailure = true
                };
            }
        }


        private ServiceConfiguration GetServiceConfiguration()
        {
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();
            serviceConfiguration.Endpoint = this.UserSettings.Endpoint;
            serviceConfiguration.CustomHttpHeaders = this.UserSettings.CustomHttpHeaders;
            serviceConfiguration.WebProxyHost = this.UserSettings.WebProxyHost;
            serviceConfiguration.IncludeWebProxy = this.UserSettings.IncludeWebProxy;
            serviceConfiguration.IncludeWebProxyNetworkCredentials = this.UserSettings.IncludeWebProxyNetworkCredentials;
            serviceConfiguration.WebProxyNetworkCredentialsUsername = this.UserSettings.WebProxyNetworkCredentialsUsername;
            serviceConfiguration.WebProxyNetworkCredentialsPassword = this.UserSettings.WebProxyNetworkCredentialsPassword;
            serviceConfiguration.WebProxyNetworkCredentialsDomain = this.UserSettings.WebProxyNetworkCredentialsDomain;

            return serviceConfiguration;
        }
    }
}