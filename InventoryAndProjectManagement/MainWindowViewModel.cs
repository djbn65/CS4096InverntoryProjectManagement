using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InventoryAndProjectManagement
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Part> _parts;
        private ObservableCollection<Machine> _localMachines;
        private ObservableCollection<Machine> _visibleMachines;
        private ObservableCollection<Part> _visibleParts;
        private int _pageNumParts = 1;
        private int _pageNumMachines = 1;
        private int _visiblePageNum;
        private Visibility _partsVis = Visibility.Hidden;
        private Visibility _machVis = Visibility.Visible;
        private string _inventorySearchText = "";
        private string _machinesSearchText = "";
        private List<string> _searchWords;
        private bool _requery = false;
        private bool _isDialogOpen = false;

        public int MachineIdToDelete { get; set; } = 0;

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                _isDialogOpen = value;
                OnPropertyChanged("IsDialogOpen");
            }
        }

        public ICommand DeleteMachineCommand => new RelayCommand<int>(DeleteMachineDialogPopUp);

        private void DeleteMachineDialogPopUp(int aId)
        {
            MachineIdToDelete = aId;
            IsDialogOpen = true;
        }

        public string SearchText
        {
            get => PartsVisibility == Visibility.Visible ? _inventorySearchText : _machinesSearchText;
            set
            {
                _requery = (value != SearchText);

                if (PartsVisibility == Visibility.Visible) _inventorySearchText = value;
                else _machinesSearchText = value;

                _searchWords = new List<string>(SearchText.Split(' '));
                OnPropertyChanged("SearchText");
            }
        }

        public Visibility PartsVisibility
        {
            get => _partsVis;

            set
            {
                _partsVis = value;

                SearchText = SearchText;

                if (_partsVis == Visibility.Visible) { _pageNumMachines = PageNum; PageNum = _pageNumParts; }
                else { _pageNumParts = PageNum; PageNum = _pageNumMachines; }

                OnPropertyChanged("PartsVisibility");
            }
        }

        public Visibility MachineVisibility
        {
            get => _machVis;

            set
            {
                _machVis = value;
                OnPropertyChanged("MachineVisibility");
            }
        }

        public bool BackEnabled
        {
            get => PageNum > 1;
        }

        public bool NextEnabled { get => PageNum * ItemsPerPage < FilteredItemsCount; }

        public int ItemsPerPage { get; set; } = 50;

        public int FilteredItemsCount { get; private set; }

        public int PageNum
        {
            get => _visiblePageNum;
            set
            {
                if (PartsVisibility == Visibility.Visible && _searchWords != null)
                {
                    FilteredItemsCount = new ObservableCollection<Part>(Parts.Where(part => _searchWords.Any(searchPart => part.Descr.ToLower().Contains(searchPart.ToLower()) || part.Number.ToLower().Contains(searchPart.ToLower())))).Count;
                }
                else if (_searchWords != null)
                {
                    FilteredItemsCount = new ObservableCollection<Machine>(Machines.Where(machine => _searchWords.Any(searchPart => machine.Description.ToLower().Contains(searchPart.ToLower()) || machine.Name.ToLower().Contains(searchPart.ToLower())))).Count;
                }

                if (value > (int)Math.Ceiling((double)FilteredItemsCount / ItemsPerPage)) _visiblePageNum = (int)Math.Ceiling((double)FilteredItemsCount / ItemsPerPage);
                else _visiblePageNum = value;

                if (PartsVisibility == Visibility.Visible && (_visiblePageNum != _pageNumParts || _visibleParts == null || _requery))
                {
                    _pageNumParts = 0;
                    _visibleParts = new ObservableCollection<Part>();

                    ObservableCollection<Part> filteredList;

                    if (_searchWords != null)
                    {
                        filteredList = new ObservableCollection<Part>(Parts.Where(part => _searchWords.Any(searchPart => part.Descr.ToLower().Contains(searchPart.ToLower()) || part.Number.ToLower().Contains(searchPart.ToLower()))));
                    }
                    else
                    {
                        filteredList = Parts;
                    }

                    FilteredItemsCount = filteredList.Count();

                    for (int i = (PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= filteredList.Count() || i < 0) break;

                        _visibleParts.Add(filteredList[i]);
                    }

                    _requery = false;

                    OnPropertyChanged("PartsToShow");
                }

                if (MachineVisibility == Visibility.Visible && (_visiblePageNum != _pageNumMachines || _visibleMachines == null || _requery))
                {
                    _pageNumMachines = 0;
                    _visibleMachines = new ObservableCollection<Machine>();

                    ObservableCollection<Machine> filteredList;

                    if (_searchWords != null)
                    {
                        filteredList = new ObservableCollection<Machine>(Machines.Where(machine => _searchWords.Any(searchPart => machine.Description.ToLower().Contains(searchPart.ToLower()) || machine.Name.ToLower().Contains(searchPart.ToLower()))));
                    }
                    else
                    {
                        filteredList = Machines;
                    }

                    FilteredItemsCount = filteredList.Count();

                    for (int i = (PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= filteredList.Count() || i < 0) break;
                        _visibleMachines.Add(filteredList[i]);
                    }

                    _requery = false;

                    OnPropertyChanged("MachinesToShow");
                }

                OnPropertyChanged("PageNum");
                OnPropertyChanged("BackEnabled");
                OnPropertyChanged("NextEnabled");
            }
        }

        public ObservableCollection<Part> Parts
        {
            get => _parts;

            set
            {
                _parts = value;
                OnPropertyChanged("Parts");
            }
        }

        public ObservableCollection<Part> PartsToShow
        {
            get => _visibleParts;
            set
            {
                _visibleParts = value;
                OnPropertyChanged("PartsToShow");
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

        public ObservableCollection<Machine> MachinesToShow
        {
            get => _visibleMachines;
            set
            {
                _visibleMachines = value;
                OnPropertyChanged("MachinesToShow");
            }
        }

        public MainWindowViewModel()
        {
            Parts = new ObservableCollection<Part>();
            Machines = new ObservableCollection<Machine>();
            Parts.CollectionChanged += PartOrMachineListChanged;
            Machines.CollectionChanged += Machines_CollectionChanged;
            FilteredItemsCount = ItemsPerPage;
        }

        private void Machines_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _requery = true;
            PageNum = PageNum;
        }

        private void PartOrMachineListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("NextEnabled");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}