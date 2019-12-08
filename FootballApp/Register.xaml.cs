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
using FootballDataLibrary;
using UserAccountLibrary;
using FootballApp.Utils;

using user =  UserAccountLibrary.User;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Register : Page
    {
        public String access;
        public Register()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

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

        }
        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            //In the real world you would normally validate the entered credentials and information before 
            //allowing a user to register a new account. 
            //For this sample though we will skip that step and just register an account if username is not null.
            RadioButton rb = sender as RadioButton;
            if (!string.IsNullOrEmpty(Username.Text) && !string.IsNullOrEmpty(access)
                && !string.IsNullOrEmpty(Email.Text) && !string.IsNullOrEmpty(FirstName.Text) )
            {
                //Register a new account
                if(access == "Coach")
                {
                    //request verification code
                }
                if (UserAccount.ValidateAccount(Username.Text))
                {
                    ErrorMessage.Text = "Username is taken, please choose a different one.";
                    return;
                }
                UserAccount.InsertUser(Username.Text, access, Email.Text, FirstName.Text, LastName.Text);
                user currUser = UserAccount.GetUser(Username.Text);
                //Register new account with Microsoft Passport
                bool success  = await MicrosoftPassportHelper.CreatePassportKeyAsync(currUser.username, currUser.userId);
                //Navigate to the Welcome Screen. 
                if (success)
                {
                    Frame.Navigate(typeof(Login));
                }
                //Frame.Navigate(typeof(Welcome), _account);
            }
            else
            {
                ErrorMessage.Text = "Please fill out the entire form";
            }

        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();

        }
        private void BGRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if(rb != null)
            {
                access = rb.Content.ToString();
            }
        

        }
    }
}
