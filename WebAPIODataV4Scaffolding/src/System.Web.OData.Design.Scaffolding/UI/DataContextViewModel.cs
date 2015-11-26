// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.Web.OData.Design.Scaffolding.VisualStudio;

namespace System.Web.OData.Design.Scaffolding.UI
{
    public class MvcDataContextViewModel : ViewModel, IDialogSettings
    {
        private string _dataContextName;

        public MvcDataContextViewModel(ControllerScaffolderModel model)
            : base(model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Model = model;
            DataContextName = model.DataContextName;
        }

        protected ControllerScaffolderModel Model { get; set; }

        public string DataContextName
        {
            get
            {
                return _dataContextName;
            }
            set
            {
                if (OnPropertyChanged(ref _dataContextName, value))
                {
                    Model.DataContextName = value;
                    SetValidationMessage(Model.ValidateDbContextName(value));
                }
            }
        }

        public virtual void LoadDialogSettings(IProjectSettings settings)
        {
            Contract.Assert(settings != null);

            double dialogWidth;
            if (settings.TryGetDouble(SavedSettingsKeys.DbContextDialogWidthKey, out dialogWidth))
            {
                DialogWidth = dialogWidth;
            }
        }

        public virtual void SaveDialogSettings(IProjectSettings settings)
        {
            settings[SavedSettingsKeys.DbContextDialogWidthKey] = DialogWidth.ToString();
        }
    }
}
