//-----------------------------------------------------------------------------
// <copyright file="BoundOperationModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.ComponentModel;

namespace Microsoft.OData.ConnectedService.Models
{
    class BoundOperationModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; set; }

        public string ShortName { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}