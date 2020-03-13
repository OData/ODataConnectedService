// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.OData.ConnectedService.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class AdvancedSettingsViewModel : ConnectedServiceWizardPage
    {
        public bool UseDataServiceCollection { get; set; }
        public bool UseNamespacePrefix { get; set; }
        public string NamespacePrefix { get; set; }
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public string GeneratedFileName { get; set; }
        public bool IncludeT4File { get; set; }
        public bool MakeTypesInternal { get; set; }
        public bool OpenGeneratedFilesInIDE { get; set; }
        public bool GenerateMultipleFiles { get; set; }

        public UserSettings UserSettings { get; }

        public AdvancedSettingsViewModel(UserSettings userSettings) : base()
        {
            this.UserSettings = userSettings;
            this.Title = "Settings";
            this.Description = "Advanced settings for generating client proxy";
            this.Legend = "Settings";
            this.ResetDataContext();
        }

        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);

            this.View = new AdvancedSettings();
            this.ResetDataContext();
            this.View.DataContext = this;
            PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }

        private void ResetDataContext()
        {
            this.UseNamespacePrefix = false;
            this.NamespacePrefix = null;
            this.UseDataServiceCollection = true;
            this.IgnoreUnexpectedElementsAndAttributes = false;
            this.EnableNamingAlias = false;
            this.GeneratedFileName = Common.Constants.DefaultReferenceFileName;
            this.IncludeT4File = false;
            this.MakeTypesInternal = false;
            this.OpenGeneratedFilesInIDE = false;
            this.GenerateMultipleFiles = false;

        }

    }
}