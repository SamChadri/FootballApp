using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.Data.Sqlite;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public class SeasonSummaryBar
{
    public int Year { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Ties { get; set; }
    public int PointsFor { get; set; }
    public int PointsAgainst { get; set; }
    public int BarWidth { get; set; }  // pre-computed px width (max 220)
    public string WinLabel => $"{Wins} WINS";
}

public partial class DataViewModel : ObservableObject
{
    private const int MaxBarWidth = 220;
    private readonly IFootballRepository _repository;

    [ObservableProperty]
    private ObservableCollection<Season> seasons = [];

    [ObservableProperty]
    private ObservableCollection<SeasonSummaryBar> seasonChart = [];

    public DataViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadDataAsync()
    {
        await DatabaseSeeder.SeedAsync(_repository);

        var plays = await _repository.GetPlaysAsync();
        
        // Find active seasons from plays
        var activeSeasonIds = plays.Select(p => p.SeasonId).Distinct().OrderBy(id => id).ToList();

        Seasons.Clear();
        SeasonChart.Clear();

        if (activeSeasonIds.Count == 0) return;

        var seasonGroups = new List<SeasonGroup>();
        foreach (var id in activeSeasonIds)
        {
            var sg = await _repository.GetSeasonGroupAsync(id);
            sg.CalculateStats();
            seasonGroups.Add(sg);
        }

        int maxWins = seasonGroups.Count > 0 ? seasonGroups.Max(sg => sg.Wins) : 0;

        // Dynamically build the UI for the loaded seasons
        foreach (var sg in seasonGroups)
        {
            var year = 2020 + sg.SeasonId;
            var seasonObj = new Season(sg.SeasonId, year);
            
            Seasons.Add(seasonObj);
            SeasonChart.Add(new SeasonSummaryBar
            {
                Year = year,
                Wins = sg.Wins,
                BarWidth = maxWins == 0 ? 0 : (int)((double)sg.Wins / maxWins * MaxBarWidth)
            });
        }
    }

    [RelayCommand]
    private async Task SelectSeasonAsync(Season season)
    {
        if (season == null) return;
        var param = new Dictionary<string, object> { ["Season"] = season };
        await Shell.Current.GoToAsync(nameof(TeamGroupPage), param);
    }
}
