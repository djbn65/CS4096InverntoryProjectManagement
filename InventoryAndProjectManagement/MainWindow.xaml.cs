using System.Windows;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 100; i++)
            {
                MainItems.Children.Add(
                    new MachineListItem
                    {
                        Width = 275,
                        Margin = new Thickness(20),
                        Title = "Machine " + i.ToString(),
                        ImgPath = i % 2 == 0 ? "Images/test.jpg" : null
                    }
                );
            }
        }
    }
}