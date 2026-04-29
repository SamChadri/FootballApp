using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public partial class HomeItem : ObservableObject
{
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string bgImage = "home_bg.png";

    [ObservableProperty]
    private string route = string.Empty;

    [ObservableProperty]
    private bool isSelected;
}

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<HomeItem> menuItems;

    [ObservableProperty]
    private int currentPosition;

    [ObservableProperty]
    private HomeItem currentItem;

    public HomeViewModel()
    {
        MenuItems = new ObservableCollection<HomeItem>
        {
            new HomeItem { Title = "Teams", BgImage = "home_bg.png", Route = nameof(GamesListPage) },
            new HomeItem { Title = "Settings", BgImage = "home_bg.png", Route = nameof(GamesListPage) },
            new HomeItem { Title = "Games", BgImage = "home_bg.png", Route = nameof(GamesListPage) },
            new HomeItem { Title = "Players", BgImage = "home_bg.png", Route = nameof(GamesListPage) },
            new HomeItem { Title = "Create", BgImage = "home_bg.png", Route = nameof(AddGamePage) }
        };

        CurrentPosition = 2; // Start on "Games"
        CurrentItem = MenuItems[CurrentPosition];
        MenuItems[CurrentPosition].IsSelected = true;
    }

    [RelayCommand]
    private async Task EnterSectionAsync(HomeItem item)
    {
        if (item != null && !string.IsNullOrEmpty(item.Route))
        {
            await Shell.Current.GoToAsync(item.Route);
        }
    }

    [RelayCommand]
    private void SelectItem(HomeItem item)
    {
        if (item == null) return;
        var idx = MenuItems.IndexOf(item);
        if (idx < 0) return;
        foreach (var m in MenuItems) m.IsSelected = false;
        item.IsSelected = true;
        CurrentPosition = idx;
        CurrentItem = item;
    }

    partial void OnCurrentPositionChanged(int value)
    {
        if (value >= 0 && value < MenuItems.Count)
        {
            foreach (var m in MenuItems) m.IsSelected = false;
            MenuItems[value].IsSelected = true;
            CurrentItem = MenuItems[value];
        }
    }
}
