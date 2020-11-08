using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InventoryAndProjectManagement
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MachineListItem> _canvasItems;
        private ObservableCollection<Part> _localParts;
        private ObservableCollection<Machine> _localMachines;

        public ObservableCollection<MachineListItem> CanvasItems
        {
            get => _canvasItems;
            set
            {
                _canvasItems = value;
                OnPropertyChanged("CanvasItems");
            }
        }

        public ObservableCollection<Part> Parts
        {
            get => _localParts;
            set
            {
                _localParts = value;
                OnPropertyChanged("Parts");
            }
        }

        public ObservableCollection<Machine> Machines
        {
            get => _localMachines;
            set
            {
                _localMachines = value;
                OnPropertyChanged("Machines");
            }
        }

        public MainWindowViewModel()
        {
            CanvasItems = new ObservableCollection<MachineListItem>();
            Parts = new ObservableCollection<Part>();
            Machines = new ObservableCollection<Machine>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}