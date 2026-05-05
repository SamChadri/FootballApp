using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class PositionPage : ContentPage
{
    public PositionPage(PositionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackTapped(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}
