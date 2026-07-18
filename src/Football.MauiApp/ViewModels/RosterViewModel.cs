using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.Data.Sqlite;
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
    public string HeadshotFileName { get; set; } = string.Empty; // e.g. "jersey_number_7_matthew_bailey.webp"
    public Player Player { get; set; } = null!;
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

    public async Task LoadDataAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);
        var dbPlayers = await _repository.GetPlayersAsync(cancellationToken);

        _allPlayers = dbPlayers.Select(p => new RosterPlayerItem
        {
            Id = p.Id,
            Number = p.Number,
            JerseyDisplay = p.Number.ToString("D2"),
            NameDisplay = p.Name.ToUpperInvariant(),
            Position = p.Position,
            Year = p.Year,
            SubtitleDisplay = $"{p.Position.ToUpperInvariant()} | {p.Year.ToUpperInvariant()}",
            HeadshotFileName = BuildHeadshotFileName(p),
            Player = p
        })
        .OrderBy(p => p.Number)
        .ToList();

        ApplyFilter();
    }

    /// <summary>
    /// Builds the resource filename for a player headshot.
    /// Matches the naming pattern produced by image_scraper.py:
    ///   jersey_number_{number}_{firstname}_{lastname}.webp
    /// </summary>
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
        if (item?.Player == null) return;

        // Navigate to PlayerStatsPage if the player has play data,
        // otherwise this is a no-op for now.
        await Shell.Current.GoToAsync(nameof(PlayerStatsPage));
    }

    [RelayCommand]
    private async Task AddPlayerAsync()
    {
        await Shell.Current.GoToAsync(nameof(AddPlayerPage));
    }
}
