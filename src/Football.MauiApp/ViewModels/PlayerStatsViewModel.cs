using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.Data.Sqlite;


namespace Football.MauiApp.ViewModels;

[QueryProperty(nameof(Season), "Season")]
[QueryProperty(nameof(PositionGroup), "PositionGroup")]
public partial class PlayerStatsViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;
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
        _ = LoadDataAsync();
    }

    public PlayerStatsViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadDataAsync()
    {
        if (PositionGroup == null || Season == null) return;
        var seasonId = Season.Id;
        var position = PositionGroup.Name;
        var teamId = 1;

        var positionPlayers = await _repository.GetPositionPlayersAsync(position, teamId);
        var playerPlays = new Dictionary<int, List<Play>>();
        
        PlayerStats.Clear();

        foreach(var player in positionPlayers)
        {
            var plays = await _repository.GetPlayerPlaysAsync(player.Id);
            playerPlays[player.Id] = plays.Where(p => p.SeasonId == seasonId).ToList();

            PlayerStats.Add(
                new StatLine
                {
                    PlayerNumber = player.Number.ToString(),
                    PlayerName = player.Name,
                    Tackles = playerPlays[player.Id].Sum(p => p.Tackles),
                    PlayYards = playerPlays[player.Id].Sum(p => p.PlayYards),
                    NumPenalties = playerPlays[player.Id].Sum(p => p.NumPenalties)
                }
            );
        }

        DonutSegments = new ObservableCollection<ChartSegment>(
            PlayerStats.Select(s => new ChartSegment
            {
                Label = s.PlayerName.Split(' ').Last().Trim(),
                Value = s.Tackles
            })
        );

        GameStats.Clear();
        var games = await _repository.GetGamesAsync(seasonId);
        var allSeasonPlays = playerPlays.Values.SelectMany(x => x).ToList();
        var gameTackles = allSeasonPlays.GroupBy(p => p.GameId)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Tackles));

        foreach (var game in games.OrderBy(g => g.Date))
        {
            GameStats.Add(new GameStat
            {
                GameLabel = $"Wk {game.Number}",
                TotalTackles = gameTackles.GetValueOrDefault(game.Id, 0)
            });
        }
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
