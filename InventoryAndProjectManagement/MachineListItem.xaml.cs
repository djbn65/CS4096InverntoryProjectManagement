using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for MachineListItem.xaml
    /// </summary>
    public partial class MachineListItem : UserControl, INotifyPropertyChanged
    {
        private string _title = null;
        public string Title { get => _title; set { _title = value; NotifyPropertyChanged(); } }

        private string _path = null;
        public string ImgPath { get => _path; set { _path = value; NotifyPropertyChanged(); } }

        private double _currentScaleX = 1;
        public double CurrentScaleX { get => _currentScaleX; set { _currentScaleX = value; NotifyPropertyChanged(); } }

        private double _currentScaleY = 1;
        public double CurrentScaleY { get => _currentScaleY; set { _currentScaleY = value; NotifyPropertyChanged(); } }

        private double _currentTranslateX;
        public double CurrentTranslateX { get => _currentTranslateX; set { _currentTranslateX = value; NotifyPropertyChanged(); } }

        private double _currentTranslateY;
        public double CurrentTranslateY { get => _currentTranslateY; set { _currentTranslateY = value; NotifyPropertyChanged(); } }

        private bool _isFlipping = false;

        public MachineListItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double WindowWidth = Application.Current.MainWindow.ActualWidth;
            double WindowHeight = Application.Current.MainWindow.ActualHeight;
            Point parentRelativePoint = ((FrameworkElement)((FrameworkElement)Parent).Parent).TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

            if (WindowWidth > WindowHeight)
            {
                CurrentScaleX = CurrentScaleY = Card.IsFlipped ? 1 : (.9 * (WindowHeight - parentRelativePoint.Y - Margin.Top) / ActualHeight);
            }
            else
            {
                CurrentScaleX = CurrentScaleY = Card.IsFlipped ? 1 : (.9 * WindowWidth / ActualWidth);
            }

            if (Panel.GetZIndex(this) == 0)
            {
                Panel.SetZIndex(this, 1);
            }

            Point relativePoint = TransformToAncestor(Application.Current.MainWindow).Transform(new Point(0, 0));

            if (CurrentTranslateX == 0)
            {
                CurrentTranslateX = WindowWidth / 2 - relativePoint.X - ActualWidth / 2 - Margin.Right;
            }
            else
            {
                CurrentTranslateX = 0;
            }

            if (CurrentTranslateY == 0)
            {
                CurrentTranslateY = (WindowHeight + parentRelativePoint.Y) / 2 - (relativePoint.Y + ActualHeight / 2) - Margin.Top;
            }
            else
            {
                CurrentTranslateY = 0;
            }

            _isFlipping = true;

            Card.IsFlipped = !Card.IsFlipped;
        }

        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_isFlipping && !Card.IsFlipped)
            {
                CurrentScaleX = 1.02;
                CurrentScaleY = 1.02;
            }
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isFlipping && !Card.IsFlipped)
            {
                CurrentScaleX = 1;
                CurrentScaleY = 1;
            }
        }

        private void CardFlip_Completed(object sender, EventArgs e)
        {
            _isFlipping = false;

            if (!Card.IsFlipped)
            {
                Panel.SetZIndex(this, 0);
            }
        }

        private void More_Click(object sender, RoutedEventArgs e)
        {
            Card.RaiseEvent(
                new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left)
                {
                    RoutedEvent = MouseLeftButtonDownEvent
                }
            );
        }
    }
}