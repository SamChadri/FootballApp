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
using UserAccountLibrary;

using user = UserAccountLibrary.User;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class Profile : Page
    {
        public user account;
        public Profile()
        {
            this.InitializeComponent();
        }

        public StatLine ViewStatLine { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavView.IsPaneOpen = false;

            account = (user)e.Parameter;
            string fullName = account.firstName + " " + account.lastName;
            Player athlete = FootballData.GetPlayer(fullName);

            PlayerName.Text = fullName + "  #" + athlete.number.ToString();

            Position.Text = athlete.position;
            Year.Text = athlete.year;

            Email.Text = account.email;
            Username.Text = account.username;

            Roster.IsEnabled = false;


            //This method is used for testing purposes only.

            SampleViewModel viewModel = new SampleViewModel(athlete.position);

            string label =  "#" + athlete.number.ToString();



            ObservableCollection<Stat> playerStats = viewModel.data.generateStatline(label, true).StatList;

            Stats.ItemsSource = playerStats;

            this.ViewStatLine = viewModel.data.generateStatline(label, true);

            PlayNum.Text = "Total Plays:    " + this.ViewStatLine.totalPlays;






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
