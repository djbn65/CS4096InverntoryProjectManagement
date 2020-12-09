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
        private ObservableCollection<Machine> _machines;
        private ObservableCollection<Project> _projects;
        private ObservableCollection<Machine> _visibleMachines;
        private ObservableCollection<Part> _visibleParts;
        private ObservableCollection<Project> _visibleProjects;
        private int _pageNumParts = 1;
        private int _pageNumMachines = 1;
        private int _pageNumProjects = 1;
        private int? _visiblePageNum = 1;
        private Visibility _partsVis = Visibility.Hidden;
        private Visibility _machVis = Visibility.Hidden;
        private Visibility _projVis = Visibility.Visible;
        private string _inventorySearchText = "";
        private string _machinesSearchText = "";
        private string _projectSearchText = "";
        private List<string> _searchWords;
        private bool _requery = false;
        private bool _isDialogOpen = false;
        private string _machineOrPartNameToDelete = "";
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

        public bool IsCreateProjectEnabled
        {
            get => IsCreateMachineEnabled && Machines.Any(machine => machine.IsSelected);
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
                NotifyPropertyChanged("IsCreateProjectEnabled");
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
                NotifyPropertyChanged("IsCreateProjectEnabled");
            }
        }

        private string _notEnoughPartsText;

        public string NotEnoughPartsText
        {
            get => _notEnoughPartsText;
            set
            {
                _notEnoughPartsText = value;
                NotifyPropertyChanged();
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

        private ICommand _deleteMachineCommand;

        public ICommand DeleteMachineCommand
        {
            get => _deleteMachineCommand;
            set
            {
                _deleteMachineCommand = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _infoClickCommand;

        public ICommand InfoClickCommand
        {
            get => _infoClickCommand;
            set
            {
                _infoClickCommand = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _backSideDeletecommand;

        public ICommand BackSideItemDeleteCommand
        {
            get => _backSideDeletecommand;
            set
            {
                _backSideDeletecommand = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _finishProjectCommand;

        public ICommand FinishProjectCommand
        {
            get => _finishProjectCommand;
            set
            {
                _finishProjectCommand = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _allocatePartCommand;

        public ICommand AllocatePartCommand
        {
            get => _allocatePartCommand;
            set
            {
                _allocatePartCommand = value;
                NotifyPropertyChanged();
            }
        }

        private ICommand _closeCardCommand;

        public ICommand CloseCardCommand
        {
            get => _closeCardCommand;
            set
            {
                _closeCardCommand = value;
                NotifyPropertyChanged();
            }
        }

        public string SearchText
        {
            get => PartsVisibility == Visibility.Visible ? _inventorySearchText : (MachineVisibility == Visibility.Visible ? _machinesSearchText : _projectSearchText);
            set
            {
                _requery = (value != SearchText);

                if (PartsVisibility == Visibility.Visible) _inventorySearchText = value;
                else if (MachineVisibility == Visibility.Visible) _machinesSearchText = value;
                else _projectSearchText = value;

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

        public Visibility ProjectsVisibility
        {
            get => _projVis;

            set
            {
                _projVis = value;
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
                else if (MachineVisibility == Visibility.Visible && _searchWords != null)
                {
                    FilteredItemsCount = new ObservableCollection<Machine>(Machines.Where(machine => _searchWords.Any(searchPart => machine.Description.ToLower().Contains(searchPart.ToLower()) || machine.Name.ToLower().Contains(searchPart.ToLower())))).Count;
                }
                else if (_searchWords != null)
                {
                    FilteredItemsCount = new ObservableCollection<Project>(Projects.Where(project => _searchWords.Any(searchPart => project.Description.ToLower().Contains(searchPart.ToLower()) || project.Name.ToLower().Contains(searchPart.ToLower())))).Count;
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

                    if (filteredList == null) FilteredItemsCount = 0;
                    else FilteredItemsCount = filteredList.Count();

                    for (int i = (int)(PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= FilteredItemsCount || i < 0) break;
                        _visibleParts.Add(filteredList[i]);
                    }

                    _requery = false;

                    NotifyPropertyChanged("PartsToShow");
                }
                else if (MachineVisibility == Visibility.Visible && (_visibleMachines == null || _requery))
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

                    if (filteredList == null) FilteredItemsCount = 0;
                    else FilteredItemsCount = filteredList.Count();

                    for (int i = (int)(PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= FilteredItemsCount || i < 0) break;
                        _visibleMachines.Add(filteredList[i]);
                    }

                    _requery = false;

                    NotifyPropertyChanged("MachinesToShow");
                }
                else if (ProjectsVisibility == Visibility.Visible && (_visibleProjects == null || _requery))
                {
                    _visibleProjects = new ObservableCollection<Project>();

                    ObservableCollection<Project> filteredList;

                    if (_searchWords != null)
                    {
                        filteredList = new ObservableCollection<Project>(Projects.Where(project => _searchWords.Any(searchPart => project.Description.ToLower().Contains(searchPart.ToLower()) || project.Name.ToLower().Contains(searchPart.ToLower()))));
                    }
                    else
                    {
                        filteredList = Projects;
                    }

                    if (filteredList == null) FilteredItemsCount = 0;
                    else FilteredItemsCount = filteredList.Count();

                    for (int i = (int)(PageNum - 1) * ItemsPerPage; i < PageNum * ItemsPerPage; ++i)
                    {
                        if (i >= FilteredItemsCount || i < 0) break;
                        _visibleProjects.Add(filteredList[i]);
                    }

                    _requery = false;

                    NotifyPropertyChanged("ProjectsToShow");
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
            get => _machines;
            set
            {
                _machines = value;
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

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                NotifyPropertyChanged();
                Refresh();
            }
        }

        public ObservableCollection<Project> ProjectsToShow
        {
            get => _visibleProjects;
            set
            {
                _visibleProjects = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Parts = new ObservableCollection<Part>();
            Machines = new ObservableCollection<Machine>();
            Projects = new ObservableCollection<Project>();
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

        public void SwitchTabs(string aContent)
        {
            if (aContent == "Projects")
            {
                if (PageNum != null)
                {
                    if (MachineVisibility == Visibility.Visible)
                    {
                        _pageNumMachines = (int)PageNum;
                        _machinesSearchText = SearchText;
                    }
                    else
                    {
                        _pageNumParts = (int)PageNum;
                        _inventorySearchText = SearchText;
                    }
                }

                ProjectsVisibility = Visibility.Visible;
                MachineVisibility = PartsVisibility = Visibility.Hidden;
                SearchText = _projectSearchText;

                PageNum = _pageNumProjects;
            }
            else if (aContent == "Machines")
            {
                if (PageNum != null)
                {
                    if (PartsVisibility == Visibility.Visible)
                    {
                        _pageNumParts = (int)PageNum;
                        _inventorySearchText = SearchText;
                    }
                    else
                    {
                        _pageNumProjects = (int)PageNum;
                        _projectSearchText = SearchText;
                    }
                }

                MachineVisibility = Visibility.Visible;
                ProjectsVisibility = PartsVisibility = Visibility.Hidden;
                SearchText = _machinesSearchText;

                PageNum = _pageNumMachines;
            }
            else
            {
                if (PageNum != null)
                {
                    if (MachineVisibility == Visibility.Visible)
                    {
                        _pageNumMachines = (int)PageNum;
                        _machinesSearchText = SearchText;
                    }
                    else
                    {
                        _pageNumProjects = (int)PageNum;
                        _projectSearchText = SearchText;
                    }
                }

                PartsVisibility = Visibility.Visible;
                ProjectsVisibility = MachineVisibility = Visibility.Hidden;
                SearchText = _inventorySearchText;

                PageNum = _pageNumParts;
            }
        }

        public void Refresh()
        {
            _requery = true;
            PageNum = PageNum;
        }

        public void UpdateCreateProjectEnabled()
        {
            NotifyPropertyChanged("IsCreateProjectEnabled");
        }
    }
}