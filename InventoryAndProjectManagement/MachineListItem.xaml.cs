using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for MachineListItem.xaml
    /// </summary>
    public partial class MachineListItem : UserControl, INotifyPropertyChanged
    {
        public string Title { get => (string)GetValue(TitleProperty); set { SetValue(TitleProperty, value); NotifyPropertyChanged(); } }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MachineListItem), new PropertyMetadata(" "));

        public string Description { get => (string)GetValue(DescriptionProperty); set { SetValue(DescriptionProperty, value); NotifyPropertyChanged(); } }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MachineListItem), new PropertyMetadata(" "));

        public ObservableCollection<Part> Parts
        {
            get { return (ObservableCollection<Part>)GetValue(PartsProperty); }
            set { SetValue(PartsProperty, value); NotifyPropertyChanged(); }
        }

        public static readonly DependencyProperty PartsProperty = DependencyProperty.Register("Parts", typeof(ObservableCollection<Part>), typeof(MachineListItem), new PropertyMetadata(new ObservableCollection<Part>()));

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

        public bool IsFlipped { get; set; } = false;

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
            Console.WriteLine("HERE");
        }
    }
}