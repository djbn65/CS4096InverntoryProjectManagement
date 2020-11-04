﻿using System.Windows;
using System.Windows.Controls;

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
                ((WrapPanel)Machines.Content).Children.Add(
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Machines != null)
            {
                Machines.Visibility = (Machines.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);
                Inventory.Visibility = (Machines.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);
            }
        }
    }
}