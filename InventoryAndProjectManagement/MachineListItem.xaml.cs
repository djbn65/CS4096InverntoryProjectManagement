using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using GalaSoft.MvvmLight.Command;
using System.Linq;
using System.Data.SqlClient;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for MachineListItem.xaml
    /// </summary>
    public partial class MachineListItem : UserControl, INotifyPropertyChanged
    {
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set
            {
                SetValue(TitleProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MachineListItem), new PropertyMetadata(" "));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set
            {
                SetValue(DescriptionProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MachineListItem), new PropertyMetadata(" "));

        public object BackSideItems
        {
            get { return GetValue(BackSideItemsProperty); }
            set { SetValue(BackSideItemsProperty, value); NotifyPropertyChanged(); }
        }

        public static readonly DependencyProperty BackSideItemsProperty = DependencyProperty.Register("BackSideItems", typeof(object), typeof(MachineListItem), new PropertyMetadata(new object()));

        public int Quantity
        {
            get => (int)GetValue(QuantityProperty);
            set
            {
                SetValue(QuantityProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(int), typeof(MachineListItem), new PropertyMetadata(-1));

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set
            {
                SetValue(DeleteCommandProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty DeleteCommandProperty = DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(MachineListItem));

        public ICommand InfoCommand
        {
            get => (ICommand)GetValue(InfoCommandProperty);
            set
            {
                SetValue(InfoCommandProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty InfoCommandProperty = DependencyProperty.Register("InfoCommand", typeof(ICommand), typeof(MachineListItem));

        public MachineListItem InfoCommandData => this;

        public ICommand DeleteBacksideItemCommand
        {
            get => (ICommand)GetValue(DeleteBacksideItemCommandProperty);
            set
            {
                SetValue(DeleteBacksideItemCommandProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty DeleteBacksideItemCommandProperty = DependencyProperty.Register("DeleteBacksideItemCommand", typeof(ICommand), typeof(MachineListItem));

        public ICommand CompleteCommand
        {
            get => (ICommand)GetValue(CompleteCommandProperty);
            set
            {
                SetValue(CompleteCommandProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty CompleteCommandProperty = DependencyProperty.Register("CompleteCommand", typeof(ICommand), typeof(MachineListItem));

        public ICommand CloseCardCommand
        {
            get => (ICommand)GetValue(CloseCardCommandProperty);
            set
            {
                SetValue(CloseCardCommandProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty CloseCardCommandProperty = DependencyProperty.Register("CloseCardCommand", typeof(ICommand), typeof(MachineListItem));

        private void AllocateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button allocateButton)
            {
                if (BackSideItems is ObservableCollection<Part> partList)
                {
                    if (allocateButton.CommandParameter is int partId)
                    {
                        using (SqlConnection connection = new SqlConnection(Settings.GetConnection()))
                        {
                            connection.Open();

                            SqlCommand allocationStatusCommand =
                                new SqlCommand(string.Format(
                                    "SELECT COUNT(*) as count " +
                                    "FROM ALLOCATION " +
                                    "WHERE part_id={0} and project_id={1}",
                                    partId, Id),
                                    connection
                                );

                            bool doUpdate = false;

                            using (SqlDataReader reader = allocationStatusCommand.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bool dataBaseAllocationValue = (int)reader["count"] == 1;

                                    if (partList.Single(part => part.Id == partId).IsAllocated == dataBaseAllocationValue)
                                    {
                                        doUpdate = true;
                                        partList.Single(part => part.Id == partId).IsAllocated = !partList.Single(part => part.Id == partId).IsAllocated;
                                    }
                                    else
                                    {
                                        partList.Single(part => part.Id == partId).IsAllocated = dataBaseAllocationValue;
                                    }
                                }
                                else
                                {
                                    // Error because no count was returned
                                }
                            }

                            if (doUpdate)
                            {
                                SqlCommand allocationChangeCommand;
                                if (partList.Single(part => part.Id == partId).IsAllocated)
                                {
                                    // Do Insert
                                    allocationChangeCommand =
                                        new SqlCommand(string.Format(
                                            "INSERT INTO ALLOCATION (part_id, project_id) VALUES({0}, {1})",
                                            partId, Id),
                                            connection
                                        );
                                }
                                else
                                {
                                    // Do Delete
                                    allocationChangeCommand =
                                        new SqlCommand(string.Format(
                                            "DELETE FROM ALLOCATION WHERE part_id = {0} and project_id = {1}",
                                            partId, Id),
                                            connection
                                        );
                                }

                                if (allocationChangeCommand.ExecuteNonQuery() != 1)
                                {
                                    // Need to revert since it already changed above
                                    partList.Single(part => part.Id == partId).IsAllocated = !partList.Single(part => part.Id == partId).IsAllocated;

                                    // Handle Error
                                }
                            }
                        }

                        allocateButton.Command.Execute(System.Tuple.Create(Id, partId, partList.Single(part => part.Id == partId).Quantity, partList.Single(part => part.Id == partId).IsAllocated));
                    }
                }
            }
        }

        public ICommand AllocateCommand
        {
            get => (ICommand)GetValue(AllocateCommandProperty);
            set
            {
                SetValue(AllocateCommandProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty AllocateCommandProperty = DependencyProperty.Register("AllocateCommand", typeof(ICommand), typeof(MachineListItem));

        public int Id
        {
            get => (int)GetValue(IdProperty);
            set
            {
                SetValue(IdProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(MachineListItem));

        private object _deleteData;

        public object DeleteData
        {
            get => _deleteData;
            private set
            {
                _deleteData = value;
                NotifyPropertyChanged();
            }
        }

        private double _gridHeight;

        public double GridHeight
        {
            get => _gridHeight;
            set
            {
                _gridHeight = value;
                NotifyPropertyChanged();
            }
        }

        private double _gridWidth;

        public double GridWidth
        {
            get => _gridWidth;
            set
            {
                _gridWidth = value;
                NotifyPropertyChanged();
            }
        }

        private string _path = null;
        public string ImgPath { get => _path; set { _path = value; NotifyPropertyChanged(); } }

        private bool _isCheckEnabled = false;

        public bool IsCheckButtonEnabled
        {
            get => _isCheckEnabled;
            set
            {
                _isCheckEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFlipped { get; set; } = false;

        public bool IsProject { get; set; } = false;

        public MachineListItem()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MachineItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsFlipped)
            {
                (FindResource("HoverEnter") as Storyboard).Begin();
            }
        }

        private void MachineItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsFlipped)
            {
                (FindResource("HoverExit") as Storyboard).Begin();
            }
        }

        private void MachineItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (BackSideItems is ObservableCollection<Part>) DeleteData = new Machine(Id, Description, Title);
            else DeleteData = new Part(Id, Title, Description, 0);
        }

        private void Finished_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = !IsEnabled;
            (sender as Button).Command.Execute((sender as Button).CommandParameter);
        }
    }
}