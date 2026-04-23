using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class AddGamePage : ContentPage
{
    private readonly AddGameViewModel _viewModel;

    public AddGamePage(AddGameViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await _viewModel.SaveGameAsync();
        await Shell.Current.GoToAsync("..");
    }
}
