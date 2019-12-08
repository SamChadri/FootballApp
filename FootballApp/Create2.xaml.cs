using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FootballDataLibrary;
using System.Diagnostics;
using Windows.System;
using FootballApp.Utils;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using Windows.UI;

using user = UserAccountLibrary.User;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Create2 : Page
    {
        private user account;
        private char result = ' ';
        public Create2()
        {
            this.InitializeComponent();
        }

        private ObservableCollection<Game> Games = new ObservableCollection<Game>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            NavView.IsPaneOpen = false;
            GameList.ItemsSource = Games;

            account = (user)e.Parameter;


        }
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add keyboard accelerators for backwards navigation.
            var goBack = new KeyboardAccelerator { Key = VirtualKey.GoBack };
            goBack.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(goBack);

            // ALT routes here
            var altLeft = new KeyboardAccelerator
            {
                Key = VirtualKey.Left,
                Modifiers = VirtualKeyModifiers.Menu
            };
            altLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(altLeft);
        }
        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private void NavView_BackRequested(NavigationView sender,
                                   NavigationViewBackRequestedEventArgs args)
        {
            On_BackRequested();
        }
        private bool On_BackRequested()
        {
            if (!Frame.CanGoBack)
            {
                Debug.WriteLine("Cannot Go back");
                return false;
            }

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == NavigationViewDisplayMode.Minimal))
            {
                return false;
            }
            Frame.GoBack();
            return true;
        }

        private void BGRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                string content = rb.Content.ToString();
                if (content.Equals("Win"))
                {
                    result = 'W';
                }
                else
                {
                    result = 'L';
                }
            }

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Game entry in Games)
            {
                FootballData.AddGame(entry.number, entry.date.ToString("yyyy-MM-dd"), entry.opponent, entry.result, entry.points, entry.opPoints);
            }

            Frame.Navigate(typeof(Test), account);

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(Opponent.Text) && !string.IsNullOrEmpty(Points.Text) &&
                !string.IsNullOrEmpty(OpPoints.Text) && !char.IsWhiteSpace(result))
            {
                int id = -1;
                Game entry = new Game(id, int.Parse(GameNum.Text), GameDate.Date.DateTime,
                    Opponent.Text, result, int.Parse(Points.Text), int.Parse(OpPoints.Text));
                Games.Add(entry);
                clearForm(GameNum, Opponent, Points, OpPoints);
                WinRadio.IsChecked = false;
                LossRadio.IsChecked = false;


            }
            else
            {
                GameNum.BorderBrush = new SolidColorBrush(Colors.Red);
                Opponent.BorderBrush = new SolidColorBrush(Colors.Red);
                Points.BorderBrush = new SolidColorBrush(Colors.Red);
                OpPoints.BorderBrush = new SolidColorBrush(Colors.Red);
            }

        }

        private void clearForm(params TextBox[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                list[i].Text = "";
                list[i].BorderBrush = new SolidColorBrush(Colors.White);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();


        }

        private void NavView_ItemInvoked(NavigationView sender,
            NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                // Frame.Navigate(typeof(Settings));
            }
            else if (args.InvokedItemContainer != null)
            {
                string navItemTag = args.InvokedItemContainer.Tag.ToString();

                switch (navItemTag)
                {
                    case "Home":
                        Frame.Navigate(typeof(Test), account);
                        break;
                    case "Create":
                        Frame.Navigate(typeof(Choose), account);
                        break;
                    case "Profile":
                        Frame.Navigate(typeof(Profile), account);
                        break;
                    case "Season":
                        Frame.Navigate(typeof(Season), account);
                        break;
                }
            }
        }

    }
}
