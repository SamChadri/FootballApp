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
using System.Diagnostics;
using Windows.System;
using FootballApp.Utils;
using FootballDataLibrary;
using System.Collections;


using user = UserAccountLibrary.User;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Season : Page
    {
        private user account;
        public Season()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavView.IsPaneOpen = false;

            account = (user)e.Parameter;
            string fullName = account.firstName + " " + account.lastName;
            Player athlete = FootballData.GetPlayer(fullName);

            //Sample Data is randomly generated for testing purposes.
            //Use the Stats Class Ideally for retreival of real data.

            SampleData stats = new SampleData();
            StatLine OlineSeason = stats.SeasonOline();
            StatLine DlineSeason = stats.SeasonDline();
            StatLine SafteySeason = stats.SeasonSafety();
            StatLine ReceiverSeason = stats.SeasonReceivers();

            List<StatLine> data = new List<StatLine>();

            List<Game> games =  FootballData.GetGames();

  
            data.Add(OlineSeason);
            data.Add(DlineSeason);
            data.Add(SafteySeason);
            data.Add(ReceiverSeason);

            radChart.Series[0].ItemsSource = games;
            radChart.Series[1].ItemsSource = games;
            pieChart.Series[0].ItemsSource = data;

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



        private void NavigationViewControl_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            UpdateMargin(sender, false);
        }

        private void NavigationViewControl_PaneOpening(NavigationView sender, object args)
        {
            UpdateMargin(sender, true);
        }

        private void UpdateMargin(NavigationView sender, bool open)
        {
            /**
            const int smallLeftIndent = 0, largeLeftIndent = 325;

            Thickness currMargin = HomeTiles.Margin;
            if (sender.DisplayMode == NavigationViewDisplayMode.Compact && open)
            {
                HomeTiles.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                Debug.WriteLine("Pane was Opened. HomeTiles Margin: " + HomeTiles.Margin);

            }
            else
            {
                HomeTiles.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                Debug.WriteLine("Pane was Closed. HomeTiles Margin: " + HomeTiles.Margin);

            }
            **/
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



        private void HomeTiles_ItemClick(object sender, ItemClickEventArgs e)
        {
            String tile = (String)e.ClickedItem;
            Hashtable param = new Hashtable();

            param.Add("Position", tile);
            param.Add("Account", account);

            switch (tile)
            {
                case "Offensive Line":
                    Frame.Navigate(typeof(D_Line), param, new DrillInNavigationTransitionInfo());
                    break;
                case "Defensive Line":
                    Frame.Navigate(typeof(D_Line), param, new DrillInNavigationTransitionInfo());
                    break;
                case "Receivers":
                    Frame.Navigate(typeof(D_Line), param, new DrillInNavigationTransitionInfo());
                    break;
                case "Safeties":
                    Frame.Navigate(typeof(D_Line), param, new DrillInNavigationTransitionInfo());
                    break;
            }
        }
    }
}
