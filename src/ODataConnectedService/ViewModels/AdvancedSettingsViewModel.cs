//----------------------------------------------------------------------------------
// <copyright file="AdvancedSettingsViewModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.OData.CodeGen.Models;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class AdvancedSettingsViewModel : ConnectedServiceWizardPage
    {
        public UserSettings UserSettings { get; internal set; }

        internal bool IsEntered;

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
            this.IsEntered = true;
            await base.OnPageEnteringAsync(args).ConfigureAwait(false);
            this.View = new AdvancedSettings { DataContext = this };
            this.PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }
    }
}
