// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Web.OData.Design.Scaffolding.VisualStudio;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.AspNet.Scaffolding;

namespace System.Web.OData.Design.Scaffolding.UI
{
    /// <remarks>
    /// There's a workaround in place here for some issues with ComboBox selection and Text Search.
    /// See MvcViewScaffolderViewModel for details.
    /// </remarks>
    public class ControllerScaffolderViewModel : ViewModel, IDialogSettings
    {
        private IDialogHost _dialogHost;
        private string _controllerName;
        private ModelType _modelType;
        private string _modelTypeName;
        private string _dataContextTypeName;
        private ModelType _dataContextType;
        private bool _isViewGenerationSelected;
        private bool _isLayoutPageSelected;
        private bool _isReferenceScriptLibrariesSelected;
        private string _layoutPageFile;
        private bool _isAsyncSelected;

        public ControllerScaffolderViewModel(ControllerScaffolderModel model)
            : base(model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Model = model;

            ControllerName = model.ControllerName;
            IsAsyncSelected = model.IsAsyncSelected;
            SetValidationMessage(Model.ValidateControllerName(ControllerName), "ControllerName");

            IsViewGenerationSupported = Model.IsViewGenerationSupported;
            if (IsViewGenerationSupported)
            {
                IsViewGenerationSelected = Model.IsViewGenerationSelected;
                IsLayoutPageSelected = Model.IsLayoutPageSelected;
                IsReferenceScriptLibrariesSelected = Model.IsReferenceScriptLibrariesSelected;
                LayoutPageFile = model.LayoutPageFile;
            }

            DataContextTypesInternal = new ObservableCollection<ModelType>();
            ModelTypesInternal = new ObservableCollection<ModelType>();

            // The CustomSort here will ensure that the <Add new> item stays at the top. Custom Sort
            // is mutually exclusive with the use of SortDescriptions.
            DataContextTypes = new ListCollectionView(DataContextTypesInternal);
            DataContextTypes.CustomSort = new DataContextModelTypeComparer();

            ModelTypes = CollectionViewSource.GetDefaultView(ModelTypesInternal);
            ModelTypes.SortDescriptions.Add(new SortDescription("ShortTypeName", ListSortDirection.Ascending));

            IsModelClassSupported = Model.IsModelClassSupported;
            IsDataContextSupported = Model.IsDataContextSupported;

            if (Model.IsModelClassSupported)
            {
                foreach (ModelType modelType in Model.ModelTypes)
                {
                    ModelTypesInternal.Add(modelType);
                }

                SetValidationMessage(Model.ValidateModelType(null), "ModelType");
            }

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

            AddNewDataContextCommand = new RelayCommand(AddNewDataContext);
            SelectLayoutCommand = new RelayCommand(SelectLayout);

            AsyncInformationIcon = GetInformationIcon();
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

        public string ControllerName
        {
            get
            {
                return _controllerName;
            }
            set
            {
                if (OnPropertyChanged(ref _controllerName, value))
                {
                    _controllerName = value == null ? null : value.Trim();
                    SetValidationMessage(Model.ValidateControllerName(_controllerName));
                    Model.ControllerName = _controllerName;
                }
            }
        }

        public bool IsAsyncSelected
        {
            get
            {
                return _isAsyncSelected;
            }
            set
            {
                if (OnPropertyChanged<bool>(ref _isAsyncSelected, value))
                {
                    _isAsyncSelected = value;
                    Model.IsAsyncSelected = _isAsyncSelected;
                }
            }
        }

        public bool IsAsyncSupported
        {
            get
            {
                return Model.IsAsyncSupported;
            }
        }

        public ImageSource AsyncInformationIcon { get; private set; }

        public bool IsModelClassSupported { get; private set; }

        public bool IsDataContextSupported { get; private set; }

        public ICommand AddNewDataContextCommand { get; private set; }

        public ListCollectionView DataContextTypes { get; private set; }

        internal ObservableCollection<ModelType> DataContextTypesInternal { get; private set; }

        private ModelType AddedDataContextItem { get; set; }

        public ICollectionView ModelTypes { get; private set; }

        internal ObservableCollection<ModelType> ModelTypesInternal { get; private set; }

        public ModelType ModelType
        {
            get
            {
                return _modelType;
            }
            set
            {
                if (OnPropertyChanged(ref _modelType, value))
                {
                    if (Model.IsModelClassSupported)
                    {
                        SetValidationMessage(Model.ValidateModelType(value));

                        if (!IsControllerNameUserSet(Model.ModelType, ControllerName))
                        {
                            ControllerName = Model.GenerateControllerName(value == null ? null : value.ShortTypeName);
                        }
                        Model.ModelType = value;
                    }
                }
            }
        }

        public string ModelTypeName
        {
            get
            {
                return _modelTypeName;
            }
            set
            {
                if (OnPropertyChanged(ref _modelTypeName, value))
                {
                    if (Model.IsModelClassSupported && ModelType != null)
                    {
                        if (ModelType.DisplayName.StartsWith(value, StringComparison.Ordinal))
                        {
                            _modelTypeName = ModelType.DisplayName;
                        }
                    }
                }
            }
        }

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
                    if (Model.IsModelClassSupported)
                    {
                        SetValidationMessage(Model.ValidateDataContextType(value));
                    }
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
                    if (IsModelClassSupported && DataContextType != null)
                    {
                        if (DataContextType.DisplayName.StartsWith(value, StringComparison.Ordinal))
                        {
                            _dataContextTypeName = DataContextType.DisplayName;
                        }
                    }
                }
            }
        }

        public bool IsViewGenerationSupported { get; private set; }

        public bool IsViewGenerationSelected
        {
            get
            {
                return _isViewGenerationSelected;
            }
            set
            {
                if (OnPropertyChanged<bool>(ref _isViewGenerationSelected, value))
                {
                    Model.IsViewGenerationSelected = value;
                }
            }
        }

        public bool IsLayoutPageSelected
        {
            get
            {
                return _isLayoutPageSelected;
            }
            set
            {
                if (OnPropertyChanged<bool>(ref _isLayoutPageSelected, value))
                {
                    Model.IsLayoutPageSelected = value;
                }
            }
        }

        public ICommand SelectLayoutCommand { get; private set; }

        public bool IsReferenceScriptLibrariesSelected
        {
            get
            {
                return _isReferenceScriptLibrariesSelected;
            }
            set
            {
                if (OnPropertyChanged(ref _isReferenceScriptLibrariesSelected, value))
                {
                    Model.IsReferenceScriptLibrariesSelected = value;
                }
            }
        }

        public string LayoutPageFile
        {
            get
            {
                return _layoutPageFile;
            }
            set
            {
                if (OnPropertyChanged<string>(ref _layoutPageFile, value))
                {
                    Model.LayoutPageFile = value;
                }
            }
        }

        protected ControllerScaffolderModel Model { get; set; }

        /// <summary>
        /// Adds a DataContext item with the given name.
        /// </summary>
        /// <param name="typeName">The full type name of the DataContext to add.</param>
        /// <returns>The newly created item.</returns>'
        /// <remarks>
        /// We'll only generate a single DataContext item as a result of scaffolding. If the user previously
        /// selected 'new' and then did it again, we'll remove the item that was added the first time.
        /// </remarks>
        public ModelType AddNewDataContext(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

            if (AddedDataContextItem != null)
            {
                // If the selected datacontext is removed from the collection, DataContextType will be set as the 
                // value before the deleted value from the collection. If the selected datacontext name and previous datacontext
                // name are the same, the combo box will not be updated with the newly selected datacontext name but the previous
                // value from the collection. Hence, setting the datacontext to null before removing the previously added datacontext 
                // from the collection.
                DataContextType = null;
                DataContextTypeName = null;
                DataContextTypesInternal.Remove(AddedDataContextItem);
            }

            AddedDataContextItem = new ModelType(typeName);
            DataContextTypesInternal.Add(AddedDataContextItem);
            return AddedDataContextItem;
        }

        public string GenerateDefaultDataContextTypeName()
        {
            return Model.GenerateDefaultDataContextTypeName();
        }

        private void DialogHost_Closing(object sender, CancelEventArgs e)
        {
            string errorMessage = Model.GetErrorIfInvalidIdentifier(ControllerName);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                DisplayErrorMessage(DialogHost, errorMessage);
                e.Cancel = true;
                return;
            }

            if (Model.ControllerExists(ControllerName))
            {
                MessageBoxResult result = DialogHost.RequestConfirmation(
                    String.Format(CultureInfo.CurrentCulture, Resources.OverwriteMessage, ControllerName),
                    Resources.AddControllerWindowTitle);

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

        private void AddNewDataContext(object param)
        {
            CreateDataContextDialog dialog = new CreateDataContextDialog();

            Model.DataContextName = GenerateDefaultDataContextTypeName();
            MvcDataContextViewModel viewModel = new MvcDataContextViewModel(Model);
            dialog.DataContext = viewModel;

            if (dialog.ShowModal() == true)
            {
                DataContextType = AddNewDataContext(viewModel.DataContextName);
                DataContextTypeName = DataContextType.DisplayName;
            }
            else
            {
                DataContextType = null;
                DataContextTypeName = null;
            }
        }

        private void SelectLayout(object unused)
        {
            string filter;
            ProjectLanguage language = Model.ActiveProject.GetCodeLanguage();
            if (language == ProjectLanguage.CSharp)
            {
                filter = Resources.MasterPageCsHtmlFilter;
            }
            else if (language == ProjectLanguage.VisualBasic)
            {
                filter = Resources.MasterPageVbHtmlFilter;
            }
            else
            {
                Contract.Assert(false, "We shouldn't get here, this project's language is not supported.");
                return;
            }

            string file;
            if (DialogHost.TrySelectFile(
                Model.ActiveProject,
                Resources.LayoutPageSelectorHeading,
                filter,
                SavedSettingsKeys.LayoutPageFileKey,
                out file))
            {
                LayoutPageFile = file;
            }
        }

        private static BitmapSource GetInformationIcon()
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                    SystemIcons.Information.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        private bool IsControllerNameUserSet(ModelType previousModelSelected, string controllerName)
        {
            if (String.IsNullOrWhiteSpace(controllerName) || String.IsNullOrWhiteSpace(Model.ControllerRootName))
            {
                return false;
            }

            if (previousModelSelected != null)
            {
                string generatedControllerName = Model.GenerateControllerName(previousModelSelected.ShortTypeName);
                if (String.Equals(generatedControllerName, controllerName, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void LoadDialogSettings(IProjectSettings settings)
        {
            Contract.Assert(settings != null);

            double dialogWidth;
            if (settings.TryGetDouble(SavedSettingsKeys.ControllerDialogWidthKey, out dialogWidth))
            {
                DialogWidth = dialogWidth;
            }
        }

        public virtual void SaveDialogSettings(IProjectSettings settings)
        {
            settings[SavedSettingsKeys.ControllerDialogWidthKey] = DialogWidth.ToString();
        }
    }
}
