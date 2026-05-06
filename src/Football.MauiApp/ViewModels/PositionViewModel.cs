using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public class PositionBar
{
    public string Name { get; set; } = string.Empty;
    public int Tackles { get; set; }
    public int BarHeight { get; set; }  // pre-computed px height (max 100)
}

[QueryProperty(nameof(Season), "Season")]
[QueryProperty(nameof(TeamGroup), "TeamGroup")]
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
    private TeamGroup? teamGroup;

    [ObservableProperty]
    private string pageTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<PositionGroup> positions = [];

    [ObservableProperty]
    private ObservableCollection<PositionBar> positionChart = [];

    partial void OnTeamGroupChanged(TeamGroup? value) => Refresh();
    partial void OnSeasonChanged(Season? value) => Refresh();

    private void Refresh()
    {
        if (TeamGroup == null || Season == null) return;
        PageTitle = $"{Season.Year} · {TeamGroup.Name}";
        Positions = new ObservableCollection<PositionGroup>(TeamGroup.Positions);
        BuildChart();
    }

    private void BuildChart()
    {
        if (TeamGroup == null) return;
        var key = TeamGroup.Name;
        if (!SampleData.TryGetValue(key, out var rows)) return;

        int maxT = rows.Max(r => r.tkl);
        PositionChart = new ObservableCollection<PositionBar>(
            rows.Select(r => new PositionBar
            {
                Name      = r.pos,
                Tackles   = r.tkl,
                BarHeight = (int)((double)r.tkl / maxT * MaxBarHeight)
            })
        );
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
