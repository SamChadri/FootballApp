using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;

namespace Football.MauiApp.ViewModels;

[QueryProperty(nameof(Season), "Season")]
[QueryProperty(nameof(PositionGroup), "PositionGroup")]
public partial class PlayerStatsViewModel : ObservableObject
{
    [ObservableProperty]
    private Season? season;

    [ObservableProperty]
    private PositionGroup? positionGroup;

    [ObservableProperty]
    private string pageTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<StatLine> playerStats = [];

    // Donut chart: label + value pairs for rendering
    [ObservableProperty]
    private ObservableCollection<ChartSegment> donutSegments = [];

    // Bar chart: game-by-game totals
    [ObservableProperty]
    private ObservableCollection<GameStat> gameStats = [];

    partial void OnPositionGroupChanged(PositionGroup? value) => Refresh();
    partial void OnSeasonChanged(Season? value) => Refresh();

    private void Refresh()
    {
        if (PositionGroup == null || Season == null) return;
        PageTitle = $"{PositionGroup.Name} · {Season.Year}";
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        // Sample data — replace with real DB queries when data layer is ready
        PlayerStats =
        [
            new StatLine { PlayerNumber = "21", PlayerName = "J. Harris",   Tackles = 42, TechTackles = 18, Sacks = 3, Assists = 12 },
            new StatLine { PlayerNumber = "4",  PlayerName = "M. Thompson", Tackles = 35, TechTackles = 14, Sacks = 1, Assists = 9  },
            new StatLine { PlayerNumber = "7",  PlayerName = "D. Williams", Tackles = 28, TechTackles = 11, Sacks = 2, Assists = 7  },
            new StatLine { PlayerNumber = "24", PlayerName = "R. Johnson",  Tackles = 22, TechTackles = 8,  Sacks = 0, Assists = 5  },
        ];

        DonutSegments = new ObservableCollection<ChartSegment>(
            PlayerStats.Select(s => new ChartSegment
            {
                Label = s.PlayerName.Split('.').Last().Trim(),
                Value = s.Tackles
            })
        );

        GameStats =
        [
            new GameStat { GameLabel = "Wk 1",  TotalTackles = 18 },
            new GameStat { GameLabel = "Wk 2",  TotalTackles = 24 },
            new GameStat { GameLabel = "Wk 3",  TotalTackles = 15 },
            new GameStat { GameLabel = "Wk 4",  TotalTackles = 30 },
            new GameStat { GameLabel = "Wk 5",  TotalTackles = 20 },
            new GameStat { GameLabel = "Wk 6",  TotalTackles = 27 },
            new GameStat { GameLabel = "Wk 7",  TotalTackles = 22 },
            new GameStat { GameLabel = "Wk 8",  TotalTackles = 19 },
        ];
    }

    [RelayCommand]
    private static async Task GoBackAsync() =>
        await Shell.Current.GoToAsync("..");
}

// Simple chart data transfer objects (no lib dependency needed at VM layer)
public class ChartSegment
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class GameStat
{
    public string GameLabel { get; set; } = string.Empty;
    public int TotalTackles { get; set; }
}
