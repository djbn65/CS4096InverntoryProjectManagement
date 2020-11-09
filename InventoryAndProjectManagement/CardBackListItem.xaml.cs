using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for CardBackListItem.xaml
    /// </summary>
    public partial class CardBackListItem : UserControl, INotifyPropertyChanged
    {
        public string PartName
        {
            get => (string)GetValue(PartNameProperty);
            set
            {
                SetValue(PartNameProperty, value);
                NotifyPropertyChanged();
            }
        }

        public static readonly DependencyProperty PartNameProperty =
            DependencyProperty.Register("PartName", typeof(string), typeof(CardBackListItem), new PropertyMetadata(" "));

        public int Qty
        {
            get => (int)GetValue(QtyProperty);
            set
            {
                SetValue(QtyProperty, value);
                NotifyPropertyChanged();
                NotifyPropertyChanged("QtyNeeded");
            }
        }

        public static readonly DependencyProperty QtyProperty =
            DependencyProperty.Register("Qty", typeof(int), typeof(CardBackListItem), new PropertyMetadata(0));

        public CardBackListItem()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}