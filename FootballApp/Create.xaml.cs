using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Data;
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
    public sealed partial class Create : Page
    {
        private user account;
        private char type = ' ';
        public Create()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private ObservableCollection <Play> Plays = new ObservableCollection<Play>();
        //public ObservableCollection<Play> Plays { get { return new ObservableCollection<Play>(); } }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            PlayList.ItemsSource = Plays;

            NavView.IsPaneOpen = false;

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
                if (content.Equals("Run"))
                {
                    type = 'R';
                }
                else
                {
                    type = 'P';
                }
            }

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach( Play entry in Plays)
            {
                FootballData.AddPlay(entry.playNum, entry.calls, entry.playerNum, entry.tech, entry.purs, entry.mtp,
                    entry.type, entry.stat1, entry.stat2, entry.loaf, entry.comment, entry.position, entry.gameNum);
            }

            Frame.Navigate(typeof(Test), account);

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            if( !string.IsNullOrEmpty(PlayNum.Text) && !string.IsNullOrEmpty(PlayerNum.Text) && !string.IsNullOrEmpty(GameNum.Text) &&
                !string.IsNullOrEmpty(Calls.Text) && !char.IsWhiteSpace(type) && !string.IsNullOrEmpty(Position.Text))
            {
                int id = -1;
               Play entry = new Play(id, int.Parse(PlayNum.Text), Calls.Text, int.Parse(PlayerNum.Text), int.Parse(Tech.Text),
                    int.Parse(Purs.Text), int.Parse(MTP.Text), type, Stat1.Text, Stat2.Text, (bool)Loaf.IsChecked,
                    Comment.Text, Position.Text, int.Parse(GameNum.Text));
                Plays.Add(entry);
                clearForm(PlayNum, Calls, PlayerNum, Tech, Purs, MTP, Stat1, Stat2, Comment, Position, GameNum);
                Loaf.IsChecked = false;
                RunRadio.IsChecked = false;
                PassRadio.IsChecked = false;
               
              
            }
            else
            {
                PlayNum.BorderBrush = new SolidColorBrush(Colors.Red);
                PlayerNum.BorderBrush = new SolidColorBrush(Colors.Red);
                GameNum.BorderBrush = new SolidColorBrush(Colors.Red);
                Calls.BorderBrush = new SolidColorBrush(Colors.Red);
                Position.BorderBrush = new SolidColorBrush(Colors.Red);

            }

        }

        private void clearForm( params TextBox[] list)
        {
            for(int i = 0; i < list.Length; i++)
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
