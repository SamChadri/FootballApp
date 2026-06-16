namespace Football.MauiApp.Views;

public partial class DataPage : ContentPage
{
    private readonly ViewModels.DataViewModel _viewModel;

    public DataPage(ViewModels.DataViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }

    private async void OnBackTapped(object sender, EventArgs e) =>
        await Shell.Current.GoToAsync("//HomePage");
}
