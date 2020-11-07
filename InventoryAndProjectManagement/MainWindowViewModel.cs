using System.Collections.ObjectModel;
using System.ComponentModel;

namespace InventoryAndProjectManagement
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MachineListItem> _canvasItems;

        public ObservableCollection<MachineListItem> CanvasItems
        {
            get => _canvasItems;
            set
            {
                _canvasItems = value;
                OnPropertyChanged("CanvasItems");
            }
        }

        public MainWindowViewModel()
        {
            CanvasItems = new ObservableCollection<MachineListItem>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}