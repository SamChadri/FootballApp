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
using FootballApp.Utils;
using FootballDataLibrary;
using UserAccountLibrary;
using System.Diagnostics;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FootballApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        public Login()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Check Microsoft Passport is setup and available on this machine
            if (await MicrosoftPassportHelper.MicrosoftPassportAvailableCheckAsync())
            {
            }
            else
            {
                // Microsoft Passport is not setup so inform the user
                PassportStatus.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 170, 207));
                PassportStatusText.Text = "Microsoft Passport is not setup!\n" +
                    "Please go to Windows Settings and set up a PIN to use it.";
                PassportSignInButton.IsEnabled = false;
            }
        }
        private async void PassportSignInButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UsernameTextBox.Text) && UserAccount.ValidateAccount(UsernameTextBox.Text))
            {
                User currUser = UserAccount.GetUser(UsernameTextBox.Text);
                bool result = await MicrosoftPassportHelper.GetAuthenticationMessageAsync(currUser);
                if (result)
                {
                    Frame.Navigate(typeof(Test), currUser);
                }
                else
                {
                    ErrorMessage.Text = "";

                }

            }
            else
            {
                Debug.WriteLine(UserAccount.ValidateAccount(UsernameTextBox.Text));
                ErrorMessage.Text = "Please enter valid username.";
            }


        }
        private void RegisterButtonTextBlock_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Register));
            ErrorMessage.Text = "";
        }
    }
}
