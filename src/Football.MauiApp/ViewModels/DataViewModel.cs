using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public partial class DataViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Season> seasons = [];

    public DataViewModel()
    {
        var currentYear = DateTime.Now.Year;
        for (int i = 0; i < 4; i++)
            Seasons.Add(new Season { Year = currentYear - i });
    }

    [RelayCommand]
    private async Task SelectSeasonAsync(Season season)
    {
        var param = new Dictionary<string, object> { ["Season"] = season };
        await Shell.Current.GoToAsync(nameof(TeamGroupPage), param);
    }
}
