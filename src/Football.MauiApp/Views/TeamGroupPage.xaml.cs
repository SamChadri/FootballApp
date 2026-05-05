using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

[QueryProperty(nameof(Season), "Season")]
public partial class TeamGroupPage : ContentPage
{
    public Football.Core.Season? Season
    {
        set
        {
            if (BindingContext is TeamGroupViewModel vm)
                vm.Season = value;
        }
    }

    public TeamGroupPage(TeamGroupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackTapped(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync("..");
}
