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
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Foundation.Metadata;
using UserAccountLibrary;
using Windows.UI.Xaml.Media.Animation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballApp
{

    public class HomeTile
    {

        public string title { get; set; }
        public string icon { get; set; }

        public HomeTile(String title, String icon)
        {
            this.title = title;
            this.icon = System.Net.WebUtility.HtmlDecode(icon);
        }
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Test : Page
    {

        private ObservableCollection<HomeTile> _tiles = new ObservableCollection<HomeTile>();
        private User currUser;

        public ObservableCollection<HomeTile> Tiles
        {
            get { return this._tiles; }
        }

        public Test()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            currUser = (User)e.Parameter;

            Tiles.Add(new HomeTile("Season", "&#xE8A1;"));
            Tiles.Add(new HomeTile("Create", "&#xE710;"));
            Tiles.Add(new HomeTile("Profile", "&#xE779;"));
            Tiles.Add(new HomeTile("Settings", "&#xE713;"));
        }

        private void NavigationViewControl_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args)
        {
            UpdateMargin(sender, false);
        }

        private void NavigationViewControl_PaneOpening(NavigationView sender, object args)
        {
            UpdateMargin(sender, true);
        }

        private void UpdateMargin(NavigationView sender , bool open)
        {
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
        }

        private void HomeTiles_ItemClick(object sender, ItemClickEventArgs e)
        {
           HomeTile tile =  (HomeTile)e.ClickedItem;

            switch (tile.title)
            {
                case "Season":
                    Frame.Navigate(typeof(Season), currUser, new DrillInNavigationTransitionInfo());
                    break;
                case "Create":
                    Frame.Navigate(typeof(Choose), currUser, new EntranceNavigationTransitionInfo());
                    break;
                case "Profile":
                    Frame.Navigate(typeof(Profile), currUser, new DrillInNavigationTransitionInfo());
                    break;
                case "Settings":
                    break;
            }
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
                        break;
                    case "Logout":
                        Frame.Navigate(typeof(Login));
                        break;

                }
            }
        }
    }

}
