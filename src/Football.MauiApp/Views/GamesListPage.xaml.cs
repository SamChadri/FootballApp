using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class GamesListPage : ContentPage
{
    private readonly GamesListViewModel _viewModel;

    public GamesListPage(GamesListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadGamesAsync();
    }

    private async void OnAddGameClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddGamePage));
    }
}
