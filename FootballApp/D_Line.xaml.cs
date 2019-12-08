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
    public sealed partial class D_Line : Page
    {
        private user account;
        private string position;

        public D_Line()
        {
            this.InitializeComponent();
        }
        public SampleViewModel ViewModel { get; set; }

        public List<Game> Games { get { return FootballData.GetGames(); } }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Hashtable param = (Hashtable)e.Parameter;
            position = (string)param["Position"];
            this.ViewModel = new SampleViewModel(position);

            NavView.IsPaneOpen = false;

            account = (user)param["Account"];
            string fullName = account.firstName + " " + account.lastName;
            Player athlete = FootballData.GetPlayer(fullName);

            Header.Text = position + " Stats";

            StatLine line1 = ViewModel.data.generateStatline("#21", true);
            StatLine line2 = ViewModel.data.generateStatline("#4", true);
            StatLine line3 = ViewModel.data.generateStatline("#7", true);
            StatLine line4 = ViewModel.data.generateStatline("#24", true);

            List<StatLine> source = new List<StatLine>();
            source.Add(line1);
            source.Add(line2);
            source.Add(line3);
            source.Add(line4);

            SampleData test = new SampleData();
            StatLine testD = test.SeasonDline();
            double num = (double)(testD.tech / testD.techTotal) * 100;
            Debug.WriteLine(num);

            DoughChart.Series[0].ItemsSource = source;

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
        private void HomeTiles_ItemClick(object sender, ItemClickEventArgs e)
        {
            Game tile = (Game)e.ClickedItem;
            Hashtable param = new Hashtable();
            param.Add("Game", tile);
            param.Add("Position", position);
            param.Add("Account", account);

            Frame.Navigate(typeof(Games), param, new DrillInNavigationTransitionInfo());

        }
    }
}
