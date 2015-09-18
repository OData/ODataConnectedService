using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.OData.ConnectedService.Views;
using Microsoft.VisualStudio.ConnectedServices;

namespace Microsoft.OData.ConnectedService.ViewModels
{
    internal class AdvancedSettingsViewModel : ConnectedServiceWizardPage
    {
        public bool UseDataServiceCollection { get; set; }
        public bool UseNamespacePrefix { get; set; }
        public string NamespacePrefix { get; set; }
        public string TargetLanguage { get; set; }
        public bool EnableNamingAlias { get; set; }
        public bool IgnoreUnexpectedElementsAndAttributes { get; set; }
        public bool? ConfigDone { get; set; }

        public AdvancedSettingsViewModel() : base()
        {
            this.Title = "Settings";
            this.Description = "Some witty sentence about configuring the provider";
            this.Legend = "Settings";
            this.View = new AdvancedSettings();
            this.View.DataContext = this;
        }

        internal void ConfigAdvancedSettings()
        {
            ConfigAdvancedSettings dialog = new ConfigAdvancedSettings();
            dialog.DataContext = this;
            dialog.Owner = Window.GetWindow(this.View);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ConfigDone = dialog.ShowDialog();
            if (ConfigDone == true)
            {
                this.UseNamespacePrefix = IsChecked(dialog.UseNamespacePrefix);
                this.NamespacePrefix = dialog.NamespacePrefix.Text;
                this.UseDataServiceCollection = IsChecked(dialog.UseDSC);
                this.IgnoreUnexpectedElementsAndAttributes = IsChecked(dialog.IgnoreUnknownAttributeOrElement);
                this.EnableNamingAlias = IsChecked(dialog.EnableCamelCase);
            }
        }

        private bool IsChecked(CheckBox checkBox)
        {
            return checkBox.IsChecked.HasValue && checkBox.IsChecked.Value;
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }
    }
}
