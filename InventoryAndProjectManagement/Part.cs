using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InventoryAndProjectManagement
{
    public class Part : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        private int? _quantityNeeded = 0;

        public int? QuantityNeeded
        {
            get => _quantityNeeded;
            set
            {
                _quantityNeeded = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isSelected = false;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;

                if (IsSelected == false) QuantityNeeded = 0;

                NotifyPropertyChanged();
            }
        }

        public Part(int aId, string aNumber, string aDescr, int aQty)
        {
            Id = aId;
            Number = aNumber;
            Description = aDescr;
            Quantity = aQty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}