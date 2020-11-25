using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InventoryAndProjectManagement
{
    public class Machine : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private int? _quantityNeeded;

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
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<Part> _partList;
        public ObservableCollection<Part> PartList { get => _partList; set { _partList = value; NotifyPropertyChanged(); } }

        public Machine(int aId, string aName, string aDescr, List<Part> aPartList = null)
        {
            Id = aId;
            Name = aName;
            Description = aDescr;

            if (aPartList != null) PartList = new ObservableCollection<Part>(aPartList);
            else PartList = new ObservableCollection<Part>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}