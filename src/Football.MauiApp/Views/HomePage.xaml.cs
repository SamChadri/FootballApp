using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class HomePage : ContentPage
{
    private string _currentBg = "home_bg.jpg";

    public HomePage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        // Initial setup
        if (BindingContext is HomeViewModel vm)
        {
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(HomeViewModel.CurrentPosition))
                {
                    UpdateBackground(vm.CurrentItem.BgImage);
                }
            };
        }
    }

    private async void UpdateBackground(string newSource)
    {
        if (_currentBg == newSource) return;

        // Simple cross-fade between A and B
        BgImageB.Source = newSource;
        await Task.WhenAll(
            BgImageA.FadeTo(0, 500, Easing.Linear),
            BgImageB.FadeTo(1, 500, Easing.Linear)
        );

        // Swap references
        var temp = BgImageA;
        BgImageA = BgImageB;
        BgImageB = temp;
        
        _currentBg = newSource;
    }
}
