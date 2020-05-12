//-----------------------------------------------------------------------------
// <copyright file="SchemaTypeModel.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors.  All rights reserved. 
//      See License.txt in the project root for license information.
// </copyright>
//----------------------------------------------------------------------------

using System.ComponentModel;

namespace Microsoft.OData.ConnectedService.Models
{
    class SchemaTypeModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public SchemaTypeModel(): this(null, null)
        {
        }

        public SchemaTypeModel(string name, string shortName)
        {
            Name = name;
            ShortName = shortName;
        }

        public string Name { get; set; }

        public string ShortName { get; set; }

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
