namespace Football.MauiApp.Views;

public partial class DataPage : ContentPage
{
    public DataPage()
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//HomePage");
}
