using System.ComponentModel;

namespace Microsoft.OData.ConnectedService.Models
{
    class BoundOperationModel : INotifyPropertyChanged
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
