using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Threading;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MachineListItem currentlyPoppedUpCard = null;
        private static ContentPresenter currentPoppedUpCardCp = null;
        private static bool cardFlipping = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SwitchTabs()
        {
            PageValue.TextChanged -= PageValue_TextChanged;
            Data.SwitchTabs();
            PageValue.TextChanged += PageValue_TextChanged;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Machines != null)
            {
                if (currentlyPoppedUpCard != null)
                {
                    cardFlipping = true;
                    Back(currentlyPoppedUpCard);
                }

                Data.MachineVisibility = Data.MachineVisibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                Data.PartsVisibility = Data.MachineVisibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
                SwitchTabs();
            }
        }

        private void WrapPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source is MachineListItem item)
            {
                if (currentlyPoppedUpCard == null && !cardFlipping)
                {
                    if (Data.MachineVisibility == Visibility.Visible) currentPoppedUpCardCp = MachinesItemsControl.ItemContainerGenerator.ContainerFromItem(item.DataContext) as ContentPresenter;
                    else currentPoppedUpCardCp = InventoryItemsControl.ItemContainerGenerator.ContainerFromItem(item.DataContext) as ContentPresenter;

                    currentlyPoppedUpCard = item;
                    cardFlipping = true;
                    PopUpCard(item);
                }
                else if ((Data.MachineVisibility == Visibility.Visible && currentPoppedUpCardCp == MachinesItemsControl.ItemContainerGenerator.ContainerFromItem(item.DataContext) as ContentPresenter && !cardFlipping) ||
                          currentPoppedUpCardCp == InventoryItemsControl.ItemContainerGenerator.ContainerFromItem(item.DataContext) as ContentPresenter)
                {
                    cardFlipping = true;
                    Back(item);
                }
            }
        }

        private void Back(MachineListItem card)
        {
            DoubleAnimation translateYAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)),
                            tranlsateXAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300));

            card.IsFlipped = false;

            DoubleAnimationUsingKeyFrames flipAnimationX = new DoubleAnimationUsingKeyFrames();
            flipAnimationX.KeyFrames.Add(
                new LinearDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = 0
                }
            );
            flipAnimationX.KeyFrames.Add(
                new LinearDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(300),
                    Value = 1
                }
            );

            DoubleAnimation scaleAnimationY = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));

            ObjectAnimationUsingKeyFrames frontVisibilityAnimation = new ObjectAnimationUsingKeyFrames();
            frontVisibilityAnimation.KeyFrames.Add(
                new DiscreteObjectKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = Visibility.Visible
                }
            );
            ObjectAnimationUsingKeyFrames backVisibilityAnimation = new ObjectAnimationUsingKeyFrames();
            backVisibilityAnimation.KeyFrames.Add(
                new DiscreteObjectKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = Visibility.Hidden
                }
            );

            Storyboard.SetTarget(translateYAnimation, card);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("RenderTransform.Children[1].Y"));
            Storyboard.SetTarget(tranlsateXAnimation, card);
            Storyboard.SetTargetProperty(tranlsateXAnimation, new PropertyPath("RenderTransform.Children[1].X"));
            Storyboard.SetTarget(flipAnimationX, card);
            Storyboard.SetTargetProperty(flipAnimationX, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTarget(scaleAnimationY, card);
            Storyboard.SetTargetProperty(scaleAnimationY, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTarget(frontVisibilityAnimation, card.FrontContent);
            Storyboard.SetTargetProperty(frontVisibilityAnimation, new PropertyPath("Visibility"));
            Storyboard.SetTarget(backVisibilityAnimation, card.BackContent);
            Storyboard.SetTargetProperty(backVisibilityAnimation, new PropertyPath("Visibility"));

            Storyboard test = new Storyboard();

            test.Children.Add(translateYAnimation);
            test.Children.Add(tranlsateXAnimation);
            test.Children.Add(flipAnimationX);
            test.Children.Add(scaleAnimationY);
            test.Children.Add(frontVisibilityAnimation);
            test.Children.Add(backVisibilityAnimation);

            card.GridHeight = card.ActualHeight;
            card.GridWidth = card.ActualWidth;

            test.Completed += FlipBackCompleted;
            test.Begin();
        }

        private void FlipBackCompleted(object sender, EventArgs e)
        {
            if (currentPoppedUpCardCp != null) Panel.SetZIndex(currentPoppedUpCardCp, 0);
            currentPoppedUpCardCp = null;
            currentlyPoppedUpCard = null;
            cardFlipping = false;
        }

        private void PopUpCard(MachineListItem card)
        {
            Panel.SetZIndex(currentPoppedUpCardCp, 1);

            Point cardPosition;

            if (Data.MachineVisibility == Visibility.Visible) cardPosition = card.TransformToAncestor(Machines).Transform(new Point(0, 0));
            else cardPosition = card.TransformToAncestor(Inventory).Transform(new Point(0, 0));

            DoubleAnimation translateYAnimation = new DoubleAnimation(0, Inventory.ActualHeight / 2 - cardPosition.Y - card.ActualHeight / 2, TimeSpan.FromMilliseconds(300)),
                            tranlsateXAnimation = new DoubleAnimation(0, Inventory.ActualWidth / 2 - cardPosition.X - card.ActualWidth / 2, TimeSpan.FromMilliseconds(300));

            card.IsFlipped = true;

            DoubleAnimationUsingKeyFrames flipAnimationX = new DoubleAnimationUsingKeyFrames();
            flipAnimationX.KeyFrames.Add(
                new LinearDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = 0
                }
            );
            flipAnimationX.KeyFrames.Add(
                new LinearDoubleKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(300),
                    Value = -(.95 * Inventory.ActualHeight) / card.ActualHeight
                }
            );

            DoubleAnimation scaleAnimationY = new DoubleAnimation(.95 * Inventory.ActualHeight / card.ActualHeight, TimeSpan.FromMilliseconds(300));

            ObjectAnimationUsingKeyFrames frontVisibilityAnimation = new ObjectAnimationUsingKeyFrames();
            frontVisibilityAnimation.KeyFrames.Add(
                new DiscreteObjectKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = Visibility.Collapsed
                }
            );
            ObjectAnimationUsingKeyFrames backVisibilityAnimation = new ObjectAnimationUsingKeyFrames();
            backVisibilityAnimation.KeyFrames.Add(
                new DiscreteObjectKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = Visibility.Visible
                }
            );

            Storyboard.SetTarget(translateYAnimation, card);
            Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath("RenderTransform.Children[1].Y"));
            Storyboard.SetTarget(tranlsateXAnimation, card);
            Storyboard.SetTargetProperty(tranlsateXAnimation, new PropertyPath("RenderTransform.Children[1].X"));
            Storyboard.SetTarget(flipAnimationX, card);
            Storyboard.SetTargetProperty(flipAnimationX, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTarget(scaleAnimationY, card);
            Storyboard.SetTargetProperty(scaleAnimationY, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTarget(frontVisibilityAnimation, card.FrontContent);
            Storyboard.SetTargetProperty(frontVisibilityAnimation, new PropertyPath("Visibility"));
            Storyboard.SetTarget(backVisibilityAnimation, card.BackContent);
            Storyboard.SetTargetProperty(backVisibilityAnimation, new PropertyPath("Visibility"));

            Storyboard test = new Storyboard();

            test.Children.Add(translateYAnimation);
            test.Children.Add(tranlsateXAnimation);
            test.Children.Add(flipAnimationX);
            test.Children.Add(scaleAnimationY);
            test.Children.Add(frontVisibilityAnimation);
            test.Children.Add(backVisibilityAnimation);

            card.GridHeight = .95 * Inventory.ActualHeight;
            card.GridWidth = .95 * Inventory.ActualHeight;

            test.Completed += FlipUpCompleted;
            test.Begin();
        }

        private void FlipUpCompleted(object sender, EventArgs e)
        {
            cardFlipping = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: We need to probably change this to a company production server when we give it to them
            using (SqlConnection connection = new SqlConnection("Server=grovertest.cbwbkynnwz1t.us-east-2.rds.amazonaws.com,1433;Database=groverdata;User Id=admin;Password=groverpassword;"))
            {
                SqlCommand partsCommand = new SqlCommand("SELECT * FROM [PARTS]", connection);
                SqlCommand machinesCommand = new SqlCommand("SELECT * FROM [MACHINES]", connection);
                partsCommand.Connection.Open();

                using (SqlDataReader reader = partsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var description = reader["part_description"];
                        string partName = (string)reader["part_name"];

                        Data.Parts.Add(new Part((int)reader["part_id"], partName == "BLANK" ? "No Part #" : partName, (description is DBNull) ? "No Description" : (string)description, (int)reader["part_qty"]));
                    }
                }

                using (SqlDataReader reader = machinesCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var description = reader["machine_description"];

                        Data.Machines.Add(new Machine((int)reader["machine_id"], (string)reader["machine_name"], (description is DBNull) ? "No Description" : (string)description, new List<Part>()));
                    }
                }

                foreach (Machine machine in Data.Machines)
                {
                    SqlCommand partIdsCommand = new SqlCommand(string.Format("SELECT sp.part_id, sp.part_amount FROM MACHINES m, MACHINE_STEPS ms, STEPS s, STEP_PARTS sp WHERE m.machine_id = {0} and m.machine_id = ms.machine_id and ms.step_id = s.step_id and s.step_id = sp.step_id", machine.Id), connection);

                    using (SqlDataReader partIdsReader = partIdsCommand.ExecuteReader())
                    {
                        while (partIdsReader.Read())
                        {
                            machine.PartList.Add(Data.Parts.Single(part => part.Id == (int)partIdsReader["part_id"]));
                            machine.PartList.Last().Quantity = (int)(double)partIdsReader["part_amount"];
                        }
                    }
                }
            }

            Data.PageNum = 1;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Data.SetPageNum((int)Data.PageNum - 1, true);
        }

        private void Beginning_Click(object sender, RoutedEventArgs e)
        {
            Data.SetPageNum(1, true);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Data.SetPageNum((int)Data.PageNum + 1, true);
        }

        private void End_Click(object sender, RoutedEventArgs e)
        {
            Data.SetPageNum((int)Math.Ceiling((double)Data.FilteredItemsCount / Data.ItemsPerPage), true);
        }

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Data.PageNum = 1;
            }
        }

        private void DialogHost_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if ((bool)eventArgs.Parameter)
            {
                if (Data.MachineVisibility == Visibility.Visible) Data.Machines.Remove(Data.Machines.Single(machine => machine.Id == Data.MachineOrPartIdToDelete));
                else Data.Parts.Remove(Data.Parts.Single(part => part.Id == Data.MachineOrPartIdToDelete));

                // TODO: Actually delete the item from the database as well
            }
        }

        private void CreateMachine_Click(object sender, RoutedEventArgs e)
        {
            List<Part> partsToAdd = new List<Part>();
            foreach (Part part in Data.Parts.Where(part => part.IsSelected == true))
            {
                partsToAdd.Add(new Part(part.Id, part.Number, part.Description, (int)part.QuantityNeeded));
                part.IsSelected = false;
                part.QuantityNeeded = 0;
            }

            Data.Machines.Add(new Machine(Data.Machines.Last().Id + 1, Data.AddNameText, Data.AddDescriptionText, partsToAdd));
            // TODO: Actually add item to the database

            Data.AddNameText = Data.AddDescriptionText = "";

            Data.IsDialogOpen = false;
            End_Click(null, null);
        }

        private void ActionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (Data.MachineVisibility == Visibility.Visible)
            {
                Data.DialogContent = FindResource("AddMachineContent");
            }
            else
            {
                Data.DialogContent = FindResource("AddPartContent");
            }
            Data.IsDialogOpen = true;
        }

        private void CancelCreation_Click(object sender, RoutedEventArgs e)
        {
            foreach (Part part in Data.Parts.Where(part => part.IsSelected == true))
            {
                part.IsSelected = false;
            }
            Data.AddNameText = Data.AddDescriptionText = "";

            Data.IsDialogOpen = false;
        }

        private void CreatePart_Click(object sender, RoutedEventArgs e)
        {
            Part newPart = new Part(Data.Parts.Last().Id + 1, Data.AddNameText, Data.AddDescriptionText, (int)Data.AddQuantityValue);
            Data.Parts.Add(newPart);

            // Add part to all selected machines in quantity desired
            foreach (Machine machine in Data.Machines.Where(machine => machine.IsSelected == true))
            {
                machine.PartList.Add(new Part(newPart.Id, newPart.Number, newPart.Description, (int)machine.QuantityNeeded));
                machine.QuantityNeeded = 0;
                machine.IsSelected = false;
            }
            // TODO: Actually add item to the database

            Data.AddNameText = Data.AddDescriptionText = "";
            Data.AddQuantityValue = 0;

            Data.IsDialogOpen = false;
            End_Click(null, null);
        }

        private void PreviewTextInputNumericValidation(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+") || e.Text == "";
        }

        private void PageValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Data.PageNum != null) Data.SetPageNum((int)Data.PageNum);
        }
    }
}