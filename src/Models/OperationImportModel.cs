//-----------------------------------------------------------------------------
// <copyright file="OperationImportModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.ComponentModel;

namespace Microsoft.OData.ConnectedService.Models
{
    class OperationImportModel: INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsSelected"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
