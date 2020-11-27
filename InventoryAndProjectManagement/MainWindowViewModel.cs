using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private int? _visiblePageNum = 1;
        private Visibility _partsVis = Visibility.Hidden;
        private Visibility _machVis = Visibility.Visible;
        private string _inventorySearchText = "";
        private string _machinesSearchText = "";
        private List<string> _searchWords;
        private bool _requery = false;
        private bool _isDialogOpen = false;
        private string _machineOrPartNameToDelete = "";
        private object _dialogContent;
        private string _addNameText = "";
        private string _addDescriptionText = "";
        private int? _addQuantityValue;

        public bool IsCreateMachineEnabled
        {
            get => _addNameText != "" && _addDescriptionText != "";
        }

        public bool IsCreatePartEnabled
        {
            get => _addNameText != "" && _addDescriptionText != "" && _addQuantityValue != null;
        }

        public int? AddQuantityValue
        {
            get => _addQuantityValue;
            set
            {
                _addQuantityValue = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsCreatePartEnabled");
            }
        }

        public string AddNameText
        {
            get => _addNameText;
            set
            {
                _addNameText = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsCreateMachineEnabled");
                NotifyPropertyChanged("IsCreatePartEnabled");
            }
        }

        public string AddDescriptionText
        {
            get => _addDescriptionText;
            set
            {
                _addDescriptionText = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsCreateMachineEnabled");
                NotifyPropertyChanged("IsCreatePartEnabled");
            }
        }

        public int MachineOrPartIdToDelete { get; set; } = 0;

        public string MachineOrPartNameToDelete
        {
            get => _machineOrPartNameToDelete;
            set
            {
                _machineOrPartNameToDelete = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                _isDialogOpen = value;
                NotifyPropertyChanged();
            }
        }

        public object DialogContent
        {
            get => _dialogContent;
            set
            {
                _dialogContent = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand DeleteMachineCommand => new RelayCommand<object>(DeleteMachineDialogPopUp);

        private void DeleteMachineDialogPopUp(object aData)
        {
            if (aData is Machine aMachine)
            {
                MachineOrPartIdToDelete = aMachine.Id;
                MachineOrPartNameToDelete = aMachine.Name;
            }
            else if (aData is Part aPart)
            {
                MachineOrPartIdToDelete = aPart.Id;
                MachineOrPartNameToDelete = aPart.Description;
            }

            DialogContent = Application.Current.MainWindow.FindResource("ConfirmContent");
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
                NotifyPropertyChanged();
            }
        }

        public Visibility PartsVisibility
        {
            get => _partsVis;

            set
            {
                _partsVis = value;

                SearchText = SearchText;

                NotifyPropertyChanged();
            }
        }

        public Visibility MachineVisibility
        {
            get => _machVis;

            set
            {
                _machVis = value;
                NotifyPropertyChanged();
            }
        }

        public bool BackEnabled
        {
            get => PageNum > 1;
        }

        public bool NextEnabled { get => PageNum * ItemsPerPage < FilteredItemsCount; }

        public int ItemsPerPage { get; set; } = 50;

        public int FilteredItemsCount { get; private set; }

        public int? PageNum
        {
            get => _visiblePageNum;
            set
            {
                if (PartsVisibility == Visibility.Visible && _searchWords != null)
                {
                    FilteredItemsCount = new ObservableCollection<Part>(Parts.Where(part => _searchWords.Any(searchPart => part.Description.ToLower().Contains(searchPart.ToLower()) || part.Number.ToLower().Contains(searchPart.ToLower())))).Count;
                }
                else if (_searchWords != null)
                {
                    FilteredItemsCount = new ObservableCollection<Machine>(Machines.Where(machine => _searchWords.Any(searchPart => machine.Description.ToLower().Contains(searchPart.ToLower()) || machine.Name.ToLower().Contains(searchPart.ToLower())))).Count;
                }

                if (value > (int)Math.Ceiling((double)FilteredItemsCount / ItemsPerPage)) _visiblePageNum = (int)Math.Ceiling((double)FilteredItemsCount / ItemsPerPage);
                else if (value <= 0) _visiblePageNum = 1;
                else _visiblePageNum = value;

                if (PartsVisibility == Visibility.Visible && (_requery || _visibleParts == null))
                {
                    _visibleParts = new ObservableCollection<Part>();

                    ObservableCollection<Part> filteredList;

                    if (_searchWords != null)
                    {
                        filteredList = new ObservableCollection<Part>(Parts.Where(part => _searchWords.Any(searchPart => part.Description.ToLower().Contains(searchPart.ToLower()) || part.Number.ToLower().Contains(searchPart.ToLower()))));
                    }
                    else
                    {
                        filteredList = Parts;
                    }

                    FilteredItemsCount = filteredList.Count();

                    for (int i = (int)(PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= filteredList.Count() || i < 0) break;

                        _visibleParts.Add(filteredList[i]);
                    }

                    _requery = false;

                    NotifyPropertyChanged("PartsToShow");
                }

                if (MachineVisibility == Visibility.Visible && (_visibleMachines == null || _requery))
                {
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

                    for (int i = (int)(PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= filteredList.Count() || i < 0) break;
                        _visibleMachines.Add(filteredList[i]);
                    }

                    _requery = false;

                    NotifyPropertyChanged("MachinesToShow");
                }

                NotifyPropertyChanged();
                NotifyPropertyChanged("BackEnabled");
                NotifyPropertyChanged("NextEnabled");
            }
        }

        public ObservableCollection<Part> Parts
        {
            get => _parts;

            set
            {
                _parts = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Part> PartsToShow
        {
            get => _visibleParts;
            set
            {
                _visibleParts = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Machine> Machines
        {
            get => _localMachines;
            set
            {
                _localMachines = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<Machine> MachinesToShow
        {
            get => _visibleMachines;
            set
            {
                _visibleMachines = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Parts = new ObservableCollection<Part>();
            Machines = new ObservableCollection<Machine>();
            FilteredItemsCount = ItemsPerPage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Forces a requery
        public void SetPageNum(int aValue, bool aForce = false)
        {
            if (aForce || aValue != PageNum)
            {
                _requery = true;
                PageNum = aValue;
            }
        }

        public void SwitchTabs()
        {
            if (MachineVisibility == Visibility.Visible)
            {
                if (PageNum != null) _pageNumMachines = (int)PageNum;
                PageNum = _pageNumParts;
            }
            else if (PartsVisibility == Visibility.Visible)
            {
                if (PageNum != null) _pageNumParts = (int)PageNum;
                PageNum = _pageNumMachines;
            }
        }

        public void Refresh()
        {
            _requery = true;
            PageNum = PageNum;
        }
    }
}