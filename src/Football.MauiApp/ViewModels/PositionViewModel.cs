using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

[QueryProperty(nameof(Season), "Season")]
[QueryProperty(nameof(TeamGroup), "TeamGroup")]
public partial class PositionViewModel : ObservableObject
{
    [ObservableProperty]
    private Season? season;

    [ObservableProperty]
    private TeamGroup? teamGroup;

    [ObservableProperty]
    private string pageTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PositionGroup> positions = [];

    partial void OnTeamGroupChanged(TeamGroup? value) => Refresh();
    partial void OnSeasonChanged(Season? value) => Refresh();

    private void Refresh()
    {
        if (TeamGroup == null || Season == null) return;
        PageTitle = $"{Season.Year} · {TeamGroup.Name}";
        Positions = new ObservableCollection<PositionGroup>(TeamGroup.Positions);
    }

    [RelayCommand]
    private async Task SelectPositionAsync(PositionGroup position)
    {
        var param = new Dictionary<string, object>
        {
            ["Season"] = Season!,
            ["PositionGroup"] = position
        };
        await Shell.Current.GoToAsync(nameof(PlayerStatsPage), param);
    }
}
