﻿using System;
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
                connection.Open();

                SqlCommand partsCommand = new SqlCommand("SELECT * FROM [PARTS]", connection);
                SqlCommand machinesCommand = new SqlCommand("SELECT * FROM [MACHINES]", connection);

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
                    SqlCommand partIdsCommand = new SqlCommand(string.Format("SELECT sp.part_id, sp.part_amount FROM MACHINES m, STEPS s, STEP_PARTS sp WHERE m.machine_id = {0} and m.machine_id = s.step_id and s.step_id = sp.step_id", machine.Id), connection);

                    using (SqlDataReader partIdsReader = partIdsCommand.ExecuteReader())
                    {
                        while (partIdsReader.Read())
                        {
                            Part partToAdd = Data.Parts.Single(part => part.Id == (int)partIdsReader["part_id"]);

                            machine.PartList.Add(new Part(partToAdd.Id, partToAdd.Number, partToAdd.Description, (int)(double)partIdsReader["part_amount"]));
                        }
                    }
                }
            }

            Data.PageNum = 1;
            Data.Machines.CollectionChanged += PartsOrMachinesCollectionChanged;
            Data.Parts.CollectionChanged += PartsOrMachinesCollectionChanged;
        }

        private void PartsOrMachinesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Data.Refresh();
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
                if (Data.MachineVisibility == Visibility.Visible)
                {
                    using (SqlConnection connection = new SqlConnection("Server=grovertest.cbwbkynnwz1t.us-east-2.rds.amazonaws.com,1433;Database=groverdata;User Id=admin;Password=groverpassword;"))
                    {
                        try
                        {
                            connection.Open();

                            SqlCommand deleteMachineCommand = new SqlCommand(string.Format("DELETE FROM MACHINES WHERE machine_id = {0};", Data.MachineOrPartIdToDelete), connection);

                            if (deleteMachineCommand.ExecuteNonQuery() == 1)
                            {
                                Data.Machines.Remove(Data.Machines.Single(machine => machine.Id == Data.MachineOrPartIdToDelete));
                            }
                        }
                        catch
                        {
                            // Handle exception
                        }
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection("Server=grovertest.cbwbkynnwz1t.us-east-2.rds.amazonaws.com,1433;Database=groverdata;User Id=admin;Password=groverpassword;"))
                    {
                        try
                        {
                            connection.Open();

                            SqlCommand deletePartCommand = new SqlCommand(string.Format("DELETE FROM PARTS WHERE part_id = {0}", Data.MachineOrPartIdToDelete), connection);

                            if (deletePartCommand.ExecuteNonQuery() == 1)
                            {
                                Data.Parts.Remove(Data.Parts.Single(part => part.Id == Data.MachineOrPartIdToDelete));

                                // Delete the part from all machines that have it
                                foreach (Machine machine in Data.Machines.Where(machine => machine.PartList.Any(part => part.Id == Data.MachineOrPartIdToDelete)))
                                {
                                    machine.PartList.Remove(machine.PartList.Single(part => part.Id == Data.MachineOrPartIdToDelete));
                                }
                            }
                        }
                        catch
                        {
                            // Handle exception
                        }
                    }
                }
            }
        }

        private void CreateMachine_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection("Server=grovertest.cbwbkynnwz1t.us-east-2.rds.amazonaws.com,1433;Database=groverdata;User Id=admin;Password=groverpassword;"))
            {
                try
                {
                    connection.Open();

                    SqlCommand addMachineCommand = new SqlCommand(string.Format("INSERT INTO MACHINES (machine_name, machine_description) VALUES ('{0}', '{1}'); SELECT SCOPE_IDENTITY();", Data.AddNameText, Data.AddDescriptionText), connection);

                    if (int.TryParse(addMachineCommand.ExecuteScalar().ToString(), out int newId))
                    {
                        SqlCommand addStepCommand = new SqlCommand(string.Format("INSERT INTO STEPS (step_id, step_number, step_description) VALUES ({0}, 1, 'First Step')", newId), connection);

                        // Add the step for this machine
                        if (addStepCommand.ExecuteNonQuery() == 1)
                        {
                            // Add parts to new machine
                            List<Part> failedParts = new List<Part>();
                            List<Part> partsToAdd = new List<Part>();
                            foreach (Part part in Data.Parts.Where(part => part.IsSelected))
                            {
                                SqlCommand addPartToMachineCommand = new SqlCommand(string.Format("INSERT INTO STEP_PARTS (step_id, step_number, part_id, part_amount) VALUES ({0}, 1, {1}, {2});", newId, part.Id, (int)part.QuantityNeeded), connection);

                                try
                                {
                                    if (addPartToMachineCommand.ExecuteNonQuery() == 1)
                                    {
                                        partsToAdd.Add(new Part(part.Id, part.Number, part.Description, (int)part.QuantityNeeded));
                                    }
                                    else
                                    {
                                        failedParts.Add(new Part(part.Id, part.Number, part.Description, (int)part.QuantityNeeded));
                                    }
                                }
                                catch
                                {
                                    failedParts.Add(new Part(part.Id, part.Number, part.Description, (int)part.QuantityNeeded));
                                }

                                part.IsSelected = false;
                                part.QuantityNeeded = null;
                            }

                            Data.Machines.Add(new Machine(newId, Data.AddNameText, Data.AddDescriptionText, partsToAdd));

                            if (failedParts.Count > 0)
                            {
                                // TODO: Notify of failed parts

                                foreach (Part part in failedParts)
                                {
                                    Console.WriteLine(string.Format("Failed part number: {0};\nFailed part description: {1}", part.Number, part.Description));
                                }
                            }

                            Data.AddNameText = Data.AddDescriptionText = "";

                            Data.IsDialogOpen = false;
                        }
                    }
                    else
                    {
                        // TODO: Handle Insert Failure
                        // This means no rows were affected
                    }
                }
                catch (SqlException exception)
                {
                    switch (exception.Number)
                    {
                        // Unique Key Constraint Violation
                        case 2627:
                            // TODO: Machine name already exists. Alert user
                            break;
                    }
                }
                catch
                {
                    // Handle other exception
                }
            }
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
            foreach (Part part in Data.Parts.Where(part => part.IsSelected))
            {
                part.IsSelected = false;
            }

            foreach (Machine machine in Data.Machines.Where(machine => machine.IsSelected))
            {
                machine.IsSelected = false;
            }

            Data.AddNameText = Data.AddDescriptionText = "";
            Data.AddQuantityValue = null;

            Data.IsDialogOpen = false;
        }

        private void CreatePart_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection("Server=grovertest.cbwbkynnwz1t.us-east-2.rds.amazonaws.com,1433;Database=groverdata;User Id=admin;Password=groverpassword;"))
            {
                try
                {
                    connection.Open();

                    SqlCommand addPartCommand =
                        new SqlCommand(
                            string.Format(
                                "INSERT INTO PARTS (part_name, part_description, part_unit, part_qty)" +
                                " VALUES ('{0}', '{1}', 1, {2}); SELECT SCOPE_IDENTITY();",
                                Data.AddNameText, Data.AddDescriptionText, Data.AddQuantityValue
                            ),
                            connection
                    );

                    if (int.TryParse(addPartCommand.ExecuteScalar().ToString(), out int newId))
                    {
                        Data.Parts.Add(new Part(newId, Data.AddNameText, Data.AddDescriptionText, (int)Data.AddQuantityValue));

                        // Add parts to selected machines
                        List<Machine> failedMachines = new List<Machine>();

                        foreach (Machine machine in Data.Machines.Where(machine => machine.IsSelected))
                        {
                            SqlCommand addPartToMachineCommand =
                                new SqlCommand(
                                    string.Format(
                                        "INSERT INTO STEP_PARTS (step_id, step_number, part_id, part_amount)" +
                                        " VALUES ({0}, 1, {1}, {2});",
                                        machine.Id, newId, machine.QuantityNeeded),
                                    connection
                            );

                            try
                            {
                                if (addPartToMachineCommand.ExecuteNonQuery() == 1)
                                {
                                    machine.PartList.Add(new Part(newId, Data.AddNameText, Data.AddDescriptionText, (int)machine.QuantityNeeded));
                                }
                                else
                                {
                                    failedMachines.Add(new Machine(machine.Id, machine.Name, machine.Description));
                                }
                            }
                            catch
                            {
                                failedMachines.Add(new Machine(machine.Id, machine.Name, machine.Description));
                            }

                            machine.IsSelected = false;
                            machine.QuantityNeeded = null;
                        }

                        if (failedMachines.Count > 0)
                        {
                            // TODO: Notify of failed machines

                            foreach (Machine machine in failedMachines)
                            {
                                Console.WriteLine(string.Format("Failed machine name: {0};\nFailed machine description: {1}", machine.Name, machine.Description));
                            }
                        }
                    }
                }
                catch (SqlException exception)
                {
                    // Handle specific sql exception numbers
                    switch (exception.Number)
                    {
                        default:
                            break;
                    }
                }
                catch
                {
                    // Handle other exception
                }
            }

            Data.AddNameText = Data.AddDescriptionText = "";
            Data.AddQuantityValue = null;

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

        private void ResetValueIfEmpty(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == "") textBox.Text = "0";
        }
    }
}