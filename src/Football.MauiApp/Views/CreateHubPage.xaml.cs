namespace Football.MauiApp.Views;

public partial class CreateHubPage : ContentPage
{
    public CreateHubPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnGameTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddGamePage));
    }

    private async void OnPlayerTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddPlayerPage));
    }

    private async void OnPlayTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddPlayPage));
    }
}
