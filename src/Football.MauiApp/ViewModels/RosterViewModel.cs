using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;


namespace Football.MauiApp.ViewModels;

/// <summary>
/// Display model for a single player row on the Roster page.
/// </summary>
public class RosterPlayerItem
{
    public int Id { get; set; }
    public string JerseyDisplay { get; set; } = string.Empty;   // e.g. "07"
    public string NameDisplay { get; set; } = string.Empty;     // e.g. "DEVIN WITHERSPOON"
    public string SubtitleDisplay { get; set; } = string.Empty;  // e.g. "WR | SENIOR"
    public string Position { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public int Number { get; set; }
    public string HeadshotFileName { get; set; } = string.Empty;

    // Dummy stats
    public int Tackles { get; set; }
    public int Snaps { get; set; }
    public int GamesPlayed { get; set; }
}



public partial class RosterViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;
    private List<RosterPlayerItem> _allPlayers = [];

    [ObservableProperty]
    private ObservableCollection<RosterPlayerItem> players = [];

    [ObservableProperty]
    private string selectedFilter = "All";

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isSearchVisible;

    private static readonly List<string> OffensePositions =
        new Offense().positionNames;

    private static readonly List<string> DefensePositions =
        new Defense().positionNames;

    private static readonly List<string> SpecialTeamsPositions =
        new SpecialTeams().positionNames;

    public RosterViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Loads the roster from the database. Each player's stats are loaded or calculated from their plays.
    /// </summary>
    public async Task LoadDataAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);
        var dbPlayers = await _repository.GetPlayersAsync(cancellationToken);

        // If the DB has old dummy players or no players, seed it first.
        if (dbPlayers.Count == 0 || dbPlayers.Any(p => p.Name.StartsWith("Player ")))
        {
            await DatabaseSeeder.SeedAsync(_repository);
            dbPlayers = await _repository.GetPlayersAsync(cancellationToken);
        }

        // We will default to showing stats for the current season (Season 3, 2023)
        int activeSeasonId = 3;
        var currentSeasonPlayers = dbPlayers.Where(p => p.SeasonId == activeSeasonId).ToList();

        var items = new List<RosterPlayerItem>();

        foreach (var p in currentSeasonPlayers)
        {
            var plays = await _repository.GetPlayerPlaysAsync(p.Id, cancellationToken);
            var seasonPlays = plays.Where(pl => pl.SeasonId == activeSeasonId).ToList();

            // Calculate stats from plays
            int tackles = seasonPlays.Sum(pl => pl.Tackles);
            int snaps = seasonPlays.Count; 
            int gamesPlayed = seasonPlays.Select(pl => pl.GameId).Distinct().Count();

            // Fallback default values if no plays recorded
            if (snaps == 0)
            {
                var rng = new Random(p.Id);
                tackles = rng.Next(5, 50);
                snaps = rng.Next(50, 300);
                gamesPlayed = rng.Next(1, 10);
            }

            items.Add(new RosterPlayerItem
            {
                Id = p.Id,
                Number = p.Number,
                JerseyDisplay = p.Number.ToString("D2"),
                NameDisplay = p.Name.ToUpperInvariant(),
                Position = p.Position,
                Year = MapYear(p.Year),
                SubtitleDisplay = $"{p.Position.ToUpperInvariant()} | {MapYear(p.Year).ToUpperInvariant()}",
                HeadshotFileName = BuildHeadshotFileName(p),
                Tackles = tackles,
                Snaps = snaps,
                GamesPlayed = gamesPlayed
            });
        }

        _allPlayers = items.OrderBy(p => p.Number).ThenBy(p => p.NameDisplay).ToList();
        ApplyFilter();
    }

    private static string MapYear(string yearCode) => yearCode.ToUpperInvariant() switch
    {
        "FR" => "Freshman",
        "SO" => "Sophomore",
        "JR" => "Junior",
        "SR" => "Senior",
        _ => yearCode
    };

    private static string BuildHeadshotFileName(Player player)
    {
        var safeName = player.Name
            .Replace("'", "")
            .Replace(" ", "_")
            .ToLowerInvariant();

        return $"jersey_number_{player.Number}_{safeName}.webp";
    }

    [RelayCommand]
    private void Filter(string unit)
    {
        SelectedFilter = unit;
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var filtered = _allPlayers.AsEnumerable();

        // Unit filter
        if (SelectedFilter != "All")
        {
            List<string> allowedPositions = SelectedFilter switch
            {
                "Offense" => OffensePositions,
                "Defense" => DefensePositions,
                "Special Teams" => SpecialTeamsPositions,
                _ => []
            };

            filtered = filtered.Where(p =>
                allowedPositions.Contains(p.Position, StringComparer.OrdinalIgnoreCase));
        }

        // Text search
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var q = SearchText.Trim();
            filtered = filtered.Where(p =>
                p.NameDisplay.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.JerseyDisplay.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                p.Position.Contains(q, StringComparison.OrdinalIgnoreCase));
        }

        Players = new ObservableCollection<RosterPlayerItem>(filtered);
    }

    [RelayCommand]
    private void ToggleSearch()
    {
        IsSearchVisible = !IsSearchVisible;
        if (!IsSearchVisible)
        {
            SearchText = string.Empty;
        }
    }

    [RelayCommand]
    private async Task SelectPlayerAsync(RosterPlayerItem item)
    {
        if (item == null) return;

        var season = new Season(3, 2023); // default to current active season 3 (2023)
        var teamId = 1;
        var posGroup = await _repository.GetPositionGroupAsync(item.Position, teamId, season.Id);

        var parameters = new Dictionary<string, object>
        {
            { "Season", season },
            { "PositionGroup", posGroup }
        };

        await Shell.Current.GoToAsync(nameof(PlayerStatsPage), parameters);
    }

    [RelayCommand]
    private async Task AddPlayerAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddPlayerPage));
    }
}
