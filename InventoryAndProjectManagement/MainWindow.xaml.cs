using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InventoryAndProjectManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static double CardHeight = 0;
        private static double CardWidth = 0;
        private static MachineListItem CardToPutBack;

        public MainWindow()
        {
            InitializeComponent();

            // TODO: We need to probably change this to a company production server when we give it to them
            //SqlConnection conn = new SqlConnection("Server=grovertest.cbwbkynnwz1t.us-east-2.rds.amazonaws.com,1433;Database=master;User Id=admin;Password=groverpassword;");
            //conn.OpenAsync();

            for (int i = 0; i < 20; i++)
            {
                ((WrapPanel)Machines.Content).Children.Add(
                    new MachineListItem
                    {
                        Margin = new Thickness(20),
                        Title = "Machine " + i.ToString(),
                        ImgPath = i % 2 == 0 ? "Images/test.jpg" : null
                    }
                );

                ((WrapPanel)Inventory.Content).Children.Add(
                    new MachineListItem
                    {
                        Margin = new Thickness(20),
                        Title = "Item " + i.ToString(),
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

        private void Machines_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
        }

        private void Inventory_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
        }

        private void WrapPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source is MachineListItem item)
            {
                if (CardHeight == 0)
                {
                    CardHeight = item.ActualHeight;
                    CardWidth = item.ActualWidth;
                }

                MachineListItem itemCopy = new MachineListItem
                {
                    Title = item.Title,
                    ImgPath = item.ImgPath
                };

                Point position;

                if (Machines.IsAncestorOf(item))
                {
                    position = item.TransformToAncestor(Machines).Transform(new Point(0, 0));
                }
                else
                {
                    position = item.TransformToAncestor(Inventory).Transform(new Point(0, 0));
                }

                Canvas.SetTop(itemCopy, position.Y);
                Canvas.SetLeft(itemCopy, position.X);

                itemCopy.MouseLeftButtonDown += Back;

                Data.CanvasItems.Add(itemCopy);

                PopUpCard(ref itemCopy, ref item);

                CardToPutBack = item;
            }
        }

        private void Back(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is MachineListItem card)
            {
                Point position;

                if (Machines.IsAncestorOf(CardToPutBack))
                {
                    position = CardToPutBack.TransformToAncestor(Machines).Transform(new Point(0, 0));
                }
                else
                {
                    position = CardToPutBack.TransformToAncestor(Inventory).Transform(new Point(0, 0));
                }

                DoubleAnimation widthAnim = new DoubleAnimation(card.ActualWidth, CardWidth, TimeSpan.FromMilliseconds(300)),
                                heightAnim = new DoubleAnimation(card.ActualHeight, CardHeight, TimeSpan.FromMilliseconds(300)),
                                topAnim = new DoubleAnimation(Canvas.GetTop(card), position.Y, TimeSpan.FromMilliseconds(300)),
                                leftAnim = new DoubleAnimation(Canvas.GetLeft(card), position.X, TimeSpan.FromMilliseconds(300));

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
                        Value = 1
                    }
                );

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
                        Value = Visibility.Collapsed
                    }
                );

                Storyboard.SetTarget(widthAnim, card);
                Storyboard.SetTargetProperty(widthAnim, new PropertyPath("Width"));
                Storyboard.SetTarget(heightAnim, card);
                Storyboard.SetTargetProperty(heightAnim, new PropertyPath("Height"));
                Storyboard.SetTarget(topAnim, card);
                Storyboard.SetTargetProperty(topAnim, new PropertyPath("(Canvas.Top)"));
                Storyboard.SetTarget(leftAnim, card);
                Storyboard.SetTargetProperty(leftAnim, new PropertyPath("(Canvas.Left)"));
                Storyboard.SetTarget(flipAnimationX, card);
                Storyboard.SetTargetProperty(flipAnimationX, new PropertyPath("RenderTransform.ScaleX"));
                Storyboard.SetTarget(frontVisibilityAnimation, card.FrontContent);
                Storyboard.SetTargetProperty(frontVisibilityAnimation, new PropertyPath("Visibility"));
                Storyboard.SetTarget(backVisibilityAnimation, card.BackContent);
                Storyboard.SetTargetProperty(backVisibilityAnimation, new PropertyPath("Visibility"));

                Storyboard test = new Storyboard();

                test.Children.Add(widthAnim);
                test.Children.Add(heightAnim);
                test.Children.Add(topAnim);
                test.Children.Add(leftAnim);
                test.Children.Add(flipAnimationX);
                test.Children.Add(frontVisibilityAnimation);
                test.Children.Add(backVisibilityAnimation);

                test.Completed += ReturnAnimationCompleted;
                test.Begin();
            }
        }

        private void ReturnAnimationCompleted(object sender, EventArgs e)
        {
            Panel.SetZIndex(CanvasItems, -1);
            Data.CanvasItems.RemoveAt(0);
            CardToPutBack.Visibility = Visibility.Visible;
        }

        private void PopUpCard(ref MachineListItem card, ref MachineListItem original)
        {
            if (CardHeight == 0)
            {
                CardHeight = original.ActualHeight;
                CardWidth = original.ActualWidth;
            }

            original.Visibility = Visibility.Hidden;
            Panel.SetZIndex(CanvasItems, 1);

            DoubleAnimation widthAnim = new DoubleAnimation(CardWidth, Math.Min(1200, .9 * CanvasItems.ActualWidth), TimeSpan.FromMilliseconds(300)),
                            heightAnim = new DoubleAnimation(CardHeight, Math.Min(800, .9 * CanvasItems.ActualHeight), TimeSpan.FromMilliseconds(300)),
                            topAnim = new DoubleAnimation(Canvas.GetTop(card), (CanvasItems.ActualHeight - (double)heightAnim.To) / 2, TimeSpan.FromMilliseconds(300)),
                            leftAnim = new DoubleAnimation(Canvas.GetLeft(card), (CanvasItems.ActualWidth - (double)widthAnim.To) / 2, TimeSpan.FromMilliseconds(300));

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
                    Value = -1
                }
            );

            ObjectAnimationUsingKeyFrames frontVisibilityAnimation = new ObjectAnimationUsingKeyFrames();
            frontVisibilityAnimation.KeyFrames.Add(
                new DiscreteObjectKeyFrame
                {
                    KeyTime = TimeSpan.FromMilliseconds(150),
                    Value = Visibility.Hidden
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

            Storyboard.SetTarget(widthAnim, card);
            Storyboard.SetTargetProperty(widthAnim, new PropertyPath("Width"));
            Storyboard.SetTarget(heightAnim, card);
            Storyboard.SetTargetProperty(heightAnim, new PropertyPath("Height"));
            Storyboard.SetTarget(topAnim, card);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("(Canvas.Top)"));
            Storyboard.SetTarget(leftAnim, card);
            Storyboard.SetTargetProperty(leftAnim, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTarget(flipAnimationX, card);
            Storyboard.SetTargetProperty(flipAnimationX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTarget(frontVisibilityAnimation, card.FrontContent);
            Storyboard.SetTargetProperty(frontVisibilityAnimation, new PropertyPath("Visibility"));
            Storyboard.SetTarget(backVisibilityAnimation, card.BackContent);
            Storyboard.SetTargetProperty(backVisibilityAnimation, new PropertyPath("Visibility"));

            Storyboard test = new Storyboard();

            test.Children.Add(widthAnim);
            test.Children.Add(heightAnim);
            test.Children.Add(topAnim);
            test.Children.Add(leftAnim);
            test.Children.Add(flipAnimationX);
            test.Children.Add(frontVisibilityAnimation);
            test.Children.Add(backVisibilityAnimation);

            test.Begin();
        }
    }
}