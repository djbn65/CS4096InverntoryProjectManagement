using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InventoryAndProjectManagement
{
    internal class Machine : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private ObservableCollection<Part> _partList;
        public ObservableCollection<Part> PartList { get => _partList; set { _partList = value; NotifyPropertyChanged(); } }

        public Machine(int aId, string aName, string aDescr, List<Part> aPartList)
        {
            Id = aId;
            Name = aName;
            Description = aDescr;
            PartList = new ObservableCollection<Part>(aPartList);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}