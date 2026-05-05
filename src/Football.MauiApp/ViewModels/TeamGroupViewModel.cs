using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

[QueryProperty(nameof(Season), "Season")]
public partial class TeamGroupViewModel : ObservableObject
{
    [ObservableProperty]
    private Season? season;

    [ObservableProperty]
    private string seasonTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TeamGroup> groups = [];

    partial void OnSeasonChanged(Season? value)
    {
        SeasonTitle = value != null ? $"{value.Year} Season" : "Season";
        BuildGroups();
    }

    private void BuildGroups()
    {
        Groups =
        [
            new TeamGroup
            {
                Name = "Offense",
                Icon = "⚡",
                Subtitle = "QB · RB · WR · TE · OL",
                Positions =
                [
                    new PositionGroup { Name = "QB" },
                    new PositionGroup { Name = "RB" },
                    new PositionGroup { Name = "WR" },
                    new PositionGroup { Name = "TE" },
                    new PositionGroup { Name = "OL" }
                ]
            },
            new TeamGroup
            {
                Name = "Defense",
                Icon = "🛡",
                Subtitle = "DL · LB · DB · S",
                Positions =
                [
                    new PositionGroup { Name = "DL" },
                    new PositionGroup { Name = "LB" },
                    new PositionGroup { Name = "DB" },
                    new PositionGroup { Name = "S" }
                ]
            },
            new TeamGroup
            {
                Name = "Special Teams",
                Icon = "⭐",
                Subtitle = "K · P · KR",
                Positions =
                [
                    new PositionGroup { Name = "K" },
                    new PositionGroup { Name = "P" },
                    new PositionGroup { Name = "KR" }
                ]
            }
        ];
    }

    [RelayCommand]
    private async Task SelectGroupAsync(TeamGroup group)
    {
        var param = new Dictionary<string, object>
        {
            ["Season"] = Season!,
            ["TeamGroup"] = group
        };
        await Shell.Current.GoToAsync(nameof(PositionPage), param);
    }
}
