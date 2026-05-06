using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public class GroupSummaryBar
{
    public string Name { get; set; } = string.Empty;
    public int Tackles { get; set; }
    public int BarWidth { get; set; }   // pre-computed px width (max 220)
    public string Color { get; set; } = "#FF6B00";
    public string Percentage { get; set; } = string.Empty;
}

[QueryProperty(nameof(Season), "Season")]
public partial class TeamGroupViewModel : ObservableObject
{
    private const int MaxBarWidth = 220;

    [ObservableProperty]
    private Season? season;

    [ObservableProperty]
    private string seasonTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TeamGroup> groups = [];

    [ObservableProperty]
    private ObservableCollection<GroupSummaryBar> groupChart = [];

    partial void OnSeasonChanged(Season? value)
    {
        SeasonTitle = value != null ? $"{value.Year} Season" : "Season";
        BuildGroups();
        BuildChart();
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

    private void BuildChart()
    {
        // Sample total-tackle distribution across the three unit groups
        var raw = new (string name, int tackles, string color)[]
        {
            ("Offense",       98,  "#FF8A00"),
            ("Defense",       217, "#FF6B00"),
            ("Special Teams", 26,  "#FFC700"),
        };
        int total = raw.Sum(r => r.tackles);
        int maxT  = raw.Max(r => r.tackles);

        GroupChart = new ObservableCollection<GroupSummaryBar>(
            raw.Select(r => new GroupSummaryBar
            {
                Name       = r.name,
                Tackles    = r.tackles,
                BarWidth   = (int)((double)r.tackles / maxT * MaxBarWidth),
                Color      = r.color,
                Percentage = $"{(int)Math.Round((double)r.tackles / total * 100)}%"
            })
        );
    }

    [RelayCommand]
    private async Task SelectGroupAsync(TeamGroup group)
    {
        var param = new Dictionary<string, object>
        {
            ["Season"]    = Season!,
            ["TeamGroup"] = group
        };
        await Shell.Current.GoToAsync(nameof(PositionPage), param);
    }
}
