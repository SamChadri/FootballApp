using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class AddPlayerPage : ContentPage
{
    private readonly AddPlayerViewModel _viewModel;

    public AddPlayerPage(AddPlayerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.PrepareAsync();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _viewModel.SaveAsync();
        await Shell.Current.GoToAsync("..");
    }
}
