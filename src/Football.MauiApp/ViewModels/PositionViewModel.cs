using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.Data.Sqlite;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public class PositionBar
{
    public string Name { get; set; } = string.Empty;
    public int Tackles { get; set; }
    public int BarHeight { get; set; }  // pre-computed px height (max 100)
    public int SnapCount { get; set;}
    public int GamesPlayed {get; set;}
    public int SnapPercentage {get; set;}

}

[QueryProperty(nameof(Season), "Season")]
[QueryProperty(nameof(SquadGroup), "TeamGroup")]
public partial class PositionViewModel : ObservableObject
{
    private const int MaxBarHeight = 100;

    // Sample tackle distributions per position within each group
    private static readonly Dictionary<string, (string pos, int tkl)[]> SampleData = new()
    {
        ["Offense"]       = [("QB", 12), ("RB", 28), ("WR", 18), ("TE", 15), ("OL", 25)],
        ["Defense"]       = [("DL", 105), ("LB", 87), ("DB", 63), ("S", 42)],
        ["Special Teams"] = [("K", 4), ("P", 6), ("KR", 16)],
    };

    [ObservableProperty]
    private Season? season;

    [ObservableProperty]
    private SquadGroup? teamGroup;

    [ObservableProperty]
    private string pageTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PositionGroup> positions = [];

    [ObservableProperty]
    private ObservableCollection<PositionBar> positionChart = [];

    partial void OnTeamGroupChanged(SquadGroup? value) => Refresh();
    partial void OnSeasonChanged(Season? value) => Refresh();

    private readonly IFootballRepository _repository;

    public PositionViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    private void Refresh()
    {
        if (TeamGroup == null || Season == null) return;
        PageTitle = $"{Season.Year} · {TeamGroup.Name}";
        _ = LoadDataAsync();
    }

    public async Task LoadDataAsync()
    {
        if (TeamGroup == null || Season == null) return;
        var seasonId = Season.Id;
        var teamGroupName = TeamGroup.Name;
        var teamId = 1;

        List<string> posNames = [];
        switch(teamGroupName)
        {
            case "Offense":
                posNames = new Offense().positionNames;
                break;
            case "Defense":
                posNames = new Defense().positionNames;
                break;
            case "Special Teams":
                posNames = new SpecialTeams().positionNames;
                break;
        }

        Positions.Clear();
        PositionChart.Clear();

        foreach(var position in posNames)
        {
            var positionPlayers = (await _repository.GetPositionPlayersAsync(position, teamId)).ToList();
            var positionPlays = (await _repository.GetPositionPlaysAsync(position, teamId, seasonId)).ToList();
            
            var positionGroup = new PositionGroup(position, positionPlayers, positionPlays);
            positionGroup.CalculateStats();
            Positions.Add(positionGroup);
        }

        int maxT = Positions.Count > 0 ? Positions.Max(p => p.Tackles) : 0;

        foreach(var pg in Positions)
        {
            PositionChart.Add(new PositionBar
            {
                Name = pg.Name,
                Tackles = pg.Tackles,
                BarHeight = maxT == 0 ? 0 : (int)((double)pg.Tackles / maxT * MaxBarHeight),
                SnapCount = pg.SnapCount,
                GamesPlayed = pg.GamesPlayed,
                SnapPercentage = (int)pg.SnapPercentage
            });
        }
    }

    [RelayCommand]
    private async Task SelectPositionAsync(PositionGroup position)
    {
        var param = new Dictionary<string, object>
        {
            ["Season"]        = Season!,
            ["PositionGroup"] = position
        };
        await Shell.Current.GoToAsync(nameof(PlayerStatsPage), param);
    }
}
