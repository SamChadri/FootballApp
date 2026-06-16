using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.Data.Sqlite;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public class GroupSummaryBar
{
    public string Name { get; set; } = string.Empty;
    public int Tackles { get; set; }
    public int Penalties { get; set;}
    public int PlayYards {get; set;}
    public int BarWidth { get; set; }   // pre-computed px width (max 220)
    public string Color { get; set; } = "#FF6B00";
    public string Percentage { get; set; } = string.Empty;
}

[QueryProperty(nameof(Season), "Season")]
public partial class TeamGroupViewModel : ObservableObject
{
    private const int MaxBarWidth = 220;
    private readonly IFootballRepository _repository;

    [ObservableProperty]
    private Season? season;

    [ObservableProperty]
    private string seasonTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<SquadGroup> groups = [];

    [ObservableProperty]
    private ObservableCollection<GroupSummaryBar> groupChart = [];

    partial void OnSeasonChanged(Season? value)
    {
        SeasonTitle = value != null ? $"{value.Year} Season" : "Season";
        LoadDataAsync();
        //BuildGroups();
        //BuildChart();
    }

    public TeamGroupViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }


    private void BuildGroups()
    {
        groups =
        [
            new SquadGroup(0, "Offense", new Offense())
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
            new SquadGroup(0, "Defense", new Defense())
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
            new SquadGroup(0, "Special Teams", new SpecialTeams())
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

    public async Task LoadDataAsync()
    {
        if (Season == null) return;
        var seasonId = Season.Id;

        var plays = await _repository.GetPlaysAsync();
        var teamId = plays.FirstOrDefault(p => p.SeasonId == seasonId)?.TeamId ?? 1;
        var squads = new Squad[] {
            new Offense(),
            new Defense(),
            new SpecialTeams()
        };

        Groups.Clear();
        GroupChart.Clear();

        foreach(var squad in squads)
        {
            var group = await _repository.GetSquadGroupAsync(seasonId, squad, teamId);
            switch(squad)
            {
                case Offense:
                    group.Name = "Offense";
                    group.Icon = "⚡";
                    group.Subtitle = "QB · RB · WR · TE · OL";
                    group.Color = "#FF8A00";
                    break;
                case Defense:
                    group.Name = "Defense";
                    group.Icon = "🛡";
                    group.Subtitle = "DL · LB · DB · S";
                    group.Color = "#FF6B00";
                    break;
                case SpecialTeams:
                    group.Name = "Special Teams";
                    group.Icon = "⭐";
                    group.Subtitle = "K · P · KR";
                    group.Color = "#FFC700";
                    break;
            }
            group.CalculateStats();
            Groups.Add(group);
        }

        int maxTackles = Groups.Count > 0 ? Groups.Max(g => g.Tackles) : 0;
        int total = Groups.Sum(g => g.Tackles);
        if (total == 0) total = 1;

        foreach(var sg in Groups)
        {
            GroupChart.Add(new GroupSummaryBar
            {
                Name = sg.Name,
                Tackles = sg.Tackles,
                Penalties = sg.Penalties,
                PlayYards = sg.PlayYards,
                BarWidth = maxTackles == 0 ? 0 : (int)((double)sg.Tackles / maxTackles * MaxBarWidth),
                Color = sg.Color,
                Percentage = $"{(int)Math.Round((double)sg.Tackles / total * 100)}%"
            });
        }
    }

    [RelayCommand]
    private async Task SelectGroupAsync(SquadGroup group)
    {
        var param = new Dictionary<string, object>
        {
            ["Season"]    = Season!,
            ["TeamGroup"] = group
        };
        await Shell.Current.GoToAsync(nameof(PositionPage), param);
    }
}
