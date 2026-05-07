using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class AddPlayPage : ContentPage
{
    private readonly AddPlayViewModel _viewModel;

    public AddPlayPage(AddPlayViewModel viewModel)
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
        try
        {
            await _viewModel.SaveAsync();
            await Shell.Current.GoToAsync("..");
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlert("Cannot save play", ex.Message, "OK");
        }
    }
}
