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
        public string GeneratedFileNamePrefix { get; set; }
        public bool IncludeT4File { get; set; }
        public bool MakeTypesInternal { get; set; }
        public bool OpenGeneratedFilesInIDE { get; set; }
        public bool GenerateMultipleFiles { get; set; }      
        public UserSettings UserSettings { get; set; }

        public AdvancedSettingsViewModel(UserSettings userSettings) : base()
        {
            this.Title = "Settings";
            this.Description = "Advanced settings for generating client proxy";
            this.Legend = "Settings";
            this.UserSettings = userSettings;
        }

        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);

            this.View = new AdvancedSettings();
            if (UserSettings != null)
            {
                this.GeneratedFileNamePrefix = UserSettings.GeneratedFileNamePrefix ?? Common.Constants.DefaultReferenceFileName;
                this.OpenGeneratedFilesInIDE = UserSettings.OpenGeneratedFilesInIDE;
                this.UseNamespacePrefix = UserSettings.UseNamespacePrefix;
                this.NamespacePrefix = UserSettings.NamespacePrefix ?? Common.Constants.DefaultReferenceFileName;
                this.UseDataServiceCollection = UserSettings.UseDataServiceCollection;
                this.MakeTypesInternal = UserSettings.MakeTypesInternal;
                this.IncludeT4File = UserSettings.IncludeT4File;
                this.IgnoreUnexpectedElementsAndAttributes = UserSettings.IgnoreUnexpectedElementsAndAttributes;
                this.EnableNamingAlias = UserSettings.EnableNamingAlias;
                this.GenerateMultipleFiles = UserSettings.GenerateMultipleFiles;
            }         
            this.View.DataContext = this;
            if (PageEntering != null)
            {
                this.PageEntering(this, EventArgs.Empty);
            }
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }
    }
}
