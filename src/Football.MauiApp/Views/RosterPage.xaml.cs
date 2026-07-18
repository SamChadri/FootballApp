using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class RosterPage : ContentPage
{
    private readonly RosterViewModel _viewModel;

    public RosterPage(RosterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
