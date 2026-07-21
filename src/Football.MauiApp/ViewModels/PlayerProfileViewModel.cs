using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Data.Sqlite;
using Microsoft.Maui.Graphics;

namespace Football.MauiApp.ViewModels;

[QueryProperty(nameof(PlayerItem), "PlayerItem")]
public partial class PlayerProfileViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;

    [ObservableProperty]
    private RosterPlayerItem? playerItem;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string teamIdDisplay = string.Empty;

    [ObservableProperty]
    private double tacklesProgress;

    [ObservableProperty]
    private Color dot1Color = Color.FromArgb("#333333");

    [ObservableProperty]
    private Color dot2Color = Color.FromArgb("#333333");

    [ObservableProperty]
    private Color dot3Color = Color.FromArgb("#333333");

    [ObservableProperty]
    private Color dot4Color = Color.FromArgb("#333333");

    [ObservableProperty]
    private string gamesPlayedPercentDisplay = "0%";

    public PlayerProfileViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    partial void OnPlayerItemChanged(RosterPlayerItem? value)
    {
        if (value == null) return;

        // Split name into first and last
        var parts = value.NameDisplay.Split(' ');
        FirstName = parts.Length > 0 ? parts[0] : string.Empty;
        LastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;

        // Format Team ID (e.g. #SEA-021 where 21 is jersey number or player ID)
        TeamIdDisplay = $"#SEA-{value.Number:D3}";

        // Tackles progress (relative to 65 solo tackles)
        TacklesProgress = Math.Clamp(value.Tackles / 65.0, 0.0, 1.0);

        // Dot Colors based on games played
        Dot1Color = value.GamesPlayed > 0 ? Color.FromArgb("#FF6B00") : Color.FromArgb("#333333");
        Dot2Color = value.GamesPlayed >= 3 ? Color.FromArgb("#FF6B00") : Color.FromArgb("#333333");
        Dot3Color = value.GamesPlayed >= 6 ? Color.FromArgb("#FF6B00") : Color.FromArgb("#333333");
        Dot4Color = value.GamesPlayed >= 9 ? Color.FromArgb("#FF6B00") : Color.FromArgb("#333333");

        // Percentage display (relative to 10 games)
        double pct = (value.GamesPlayed / 10.0) * 100.0;
        GamesPlayedPercentDisplay = $"SEASON {Math.Min((int)pct, 100)}%";
    }

    [RelayCommand]
    private async Task EditPlayerAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.AddPlayerPage));
    }

    [RelayCommand]
    private async Task ExportReportAsync()
    {
        if (App.Current?.MainPage != null)
        {
            await App.Current.MainPage.DisplayAlert("Scouting Report", $"Exported scouting report for {PlayerItem?.NameDisplay}.", "OK");
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
