// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Restier.Scaffolding.UI
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using Microsoft.Restier.Scaffolding.VisualStudio;

    /// <remarks>
    /// There's a workaround in place here for some issues with ComboBox selection and Text Search.
    /// See MvcViewScaffolderViewModel for details.
    /// </remarks>
    public class ConfigScaffolderViewModel : ViewModel, IDialogSettings
    {
        private IDialogHost _dialogHost;
        private string _configName;
        private ModelType _configType;
        private string _dataContextTypeName;
        private ModelType _dataContextType;

        public ConfigScaffolderViewModel(ConfigScaffolderModel model)
            : base(model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Model = model;

            // ConfigName = model.ConfigName;
            ConfigTypesInternal = new ObservableCollection<ModelType>();
            ConfigTypes = new ListCollectionView(ConfigTypesInternal);
            ConfigTypes.CustomSort = new DataContextModelTypeComparer();
            foreach (ModelType modelType in Model.ConfigTypes)
            {
                ConfigTypesInternal.Add(modelType);
            }

            if (model.ConfigType != null)
            {
                ConfigType = Model.ConfigType;
                ConfigName = ConfigType.DisplayName;
            }

            // SetValidationMessage(Model.ValidateDataContextType(DataContextType), "DataContextType");

            DataContextTypesInternal = new ObservableCollection<ModelType>();
            DataContextTypes = new ListCollectionView(DataContextTypesInternal);
            DataContextTypes.CustomSort = new DataContextModelTypeComparer();

            IsDataContextSupported = Model.IsDataContextSupported;
            if (Model.IsDataContextSupported)
            {
                foreach (ModelType modelType in Model.DataContextTypes)
                {
                    DataContextTypesInternal.Add(modelType);
                }

                if (model.DataContextType != null)
                {
                    // We have a saved datacontext selection
                    DataContextType = Model.DataContextType;
                    DataContextTypeName = DataContextType.DisplayName;
                }

                SetValidationMessage(Model.ValidateDataContextType(DataContextType), "DataContextType");
            }
        }

        public IDialogHost DialogHost
        {
            get
            {
                return _dialogHost;
            }
            set
            {
                IDialogHost oldValue = _dialogHost;
                if (OnPropertyChanged<IDialogHost>(ref _dialogHost, value))
                {
                    if (oldValue != null)
                    {
                        oldValue.Closing -= DialogHost_Closing;
                    }

                    if (value != null)
                    {
                        value.Closing += DialogHost_Closing;
                    }
                }
            }
        }

        public ModelType ConfigType
        {
            get
            {
                return _configType;
            }
            set
            {
                if (OnPropertyChanged(ref _configType, value))
                {
                    Model.ConfigType = value;
                    // SetValidationMessage(Model.ValidateDataContextType(value));
                }
            }
        }

        public string ConfigName
        {
            get
            {
                return _configName;
            }
            set
            {
                if (OnPropertyChanged(ref _configName, value))
                {
                    if (ConfigType != null)
                    {
                        if (ConfigType.DisplayName.StartsWith(value, StringComparison.Ordinal))
                        {
                            _configName = ConfigType.DisplayName;
                        }
                    }
                }
            }
        }

        public ListCollectionView ConfigTypes { get; private set; }

        internal ObservableCollection<ModelType> ConfigTypesInternal { get; private set; }

        public bool IsDataContextSupported { get; private set; }

        public ListCollectionView DataContextTypes { get; private set; }

        internal ObservableCollection<ModelType> DataContextTypesInternal { get; private set; }

        public ModelType DataContextType
        {
            get
            {
                return _dataContextType;
            }
            set
            {
                if (OnPropertyChanged(ref _dataContextType, value))
                {
                    Model.DataContextType = value;
                    SetValidationMessage(Model.ValidateDataContextType(value));
                }
            }
        }

        public string DataContextTypeName
        {
            get
            {
                return _dataContextTypeName;
            }
            set
            {
                if (OnPropertyChanged(ref _dataContextTypeName, value))
                {
                    if (DataContextType != null)
                    {
                        if (DataContextType.DisplayName.StartsWith(value, StringComparison.Ordinal))
                        {
                            _dataContextTypeName = DataContextType.DisplayName;
                        }
                    }
                }
            }
        }

        protected ConfigScaffolderModel Model { get; set; }

        private void DialogHost_Closing(object sender, CancelEventArgs e)
        {
            //string errorMessage = Model.GetErrorIfInvalidIdentifier(ConfigName);
            //if (!String.IsNullOrEmpty(errorMessage))
            //{
            //    DisplayErrorMessage(DialogHost, errorMessage);
            //    e.Cancel = true;
            //    return;
            //}

            if (Model.ConfigExists(ConfigType.ShortTypeName))
            {
                MessageBoxResult result = DialogHost.RequestConfirmation(
                    String.Format(CultureInfo.CurrentCulture, Resources.OverwriteMessage, ConfigType.ShortTypeName),
                    Resources.AddConfigWindowTitle);

                if (result == MessageBoxResult.Yes)
                {
                    Model.IsOverwritingFiles = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public virtual void LoadDialogSettings(IProjectSettings settings)
        {
            Contract.Assert(settings != null);

            double dialogWidth;
            if (settings.TryGetDouble(SavedSettingsKeys.ConfigDialogWidthKey, out dialogWidth))
            {
                DialogWidth = dialogWidth;
            }
        }

        public virtual void SaveDialogSettings(IProjectSettings settings)
        {
            settings[SavedSettingsKeys.ConfigDialogWidthKey] = DialogWidth.ToString();
        }
    }
}
