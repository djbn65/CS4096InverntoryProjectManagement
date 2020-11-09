using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace InventoryAndProjectManagement
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<MachineListItem> _canvasItems;
        private ObservableCollection<Part> _parts;
        private ObservableCollection<Machine> _localMachines;
        private ObservableCollection<Machine> _visibleMachines;
        private ObservableCollection<Part> _visibleParts;
        private int _pageNumParts = 1;
        private int _pageNumMachines = 1;
        private int _visiblePageNum;
        private Visibility _partsVis = Visibility.Hidden;
        private Visibility _machVis = Visibility.Visible;
        private string _searchText;
        private List<string> _searchWords;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
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

        public bool NextEnabled { get => _partsVis == Visibility.Visible ? PageNum * ItemsPerPage < Parts.Count() : PageNum * ItemsPerPage < Machines.Count(); }

        public int ItemsPerPage { get; set; } = 50;

        public int PageNum
        {
            get => _visiblePageNum;
            set
            {
                if (PartsVisibility == Visibility.Visible && value > (int)Math.Ceiling((double)Parts.Count / ItemsPerPage)) _visiblePageNum = (int)Math.Ceiling((double)Parts.Count / ItemsPerPage);
                else if (MachineVisibility == Visibility.Visible && value > (int)Math.Ceiling((double)Machines.Count / ItemsPerPage)) _visiblePageNum = (int)Math.Ceiling((double)Machines.Count / ItemsPerPage);
                else _visiblePageNum = value;

                if (PartsVisibility == Visibility.Visible && (_visiblePageNum != _pageNumParts || _visibleParts == null))
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

                    for (int i = (PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= filteredList.Count()) break;

                        _visibleParts.Add(filteredList[i]);
                    }

                    OnPropertyChanged("PartsToShow");
                }
                else if (PartsVisibility == Visibility.Visible)
                {
                    _visibleParts = null;
                }

                if (MachineVisibility == Visibility.Visible && (_visiblePageNum != _pageNumMachines || _visibleMachines == null))
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

                    for (int i = (PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= filteredList.Count()) break;
                        _visibleMachines.Add(filteredList[i]);
                    }

                    OnPropertyChanged("MachinesToShow");
                }
                else if (MachineVisibility == Visibility.Visible)
                {
                    _visibleMachines = null;
                }

                OnPropertyChanged("PageNum");
                OnPropertyChanged("BackEnabled");
                OnPropertyChanged("NextEnabled");
            }
        }

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
            CanvasItems = new ObservableCollection<MachineListItem>();
            Parts = new ObservableCollection<Part>();
            Machines = new ObservableCollection<Machine>();
            Parts.CollectionChanged += PartOrMachineListChanged;
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