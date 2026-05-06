using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Football.Core;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public class SeasonSummaryBar
{
    public int Year { get; set; }
    public int Tackles { get; set; }
    public int BarWidth { get; set; }  // pre-computed px width (max 220)
    public string TackleLabel => $"{Tackles} TKL";
}

public partial class DataViewModel : ObservableObject
{
    private const int MaxBarWidth = 220;

    [ObservableProperty]
    private ObservableCollection<Season> seasons = [];

    [ObservableProperty]
    private ObservableCollection<SeasonSummaryBar> seasonChart = [];

    public DataViewModel()
    {
        var currentYear = DateTime.Now.Year;

        // Sample per-season tackle totals — oldest → newest
        var rawTackles = new[] { 287, 312, 298, 341 };
        var maxTackles = rawTackles.Max();

        for (int i = 0; i < 4; i++)
        {
            var year = currentYear - (3 - i);
            Seasons.Add(new Season { Year = year });
            SeasonChart.Add(new SeasonSummaryBar
            {
                Year = year,
                Tackles = rawTackles[i],
                BarWidth = (int)((double)rawTackles[i] / maxTackles * MaxBarWidth)
            });
        }
    }

    [RelayCommand]
    private async Task SelectSeasonAsync(Season season)
    {
        var param = new Dictionary<string, object> { ["Season"] = season };
        await Shell.Current.GoToAsync(nameof(TeamGroupPage), param);
    }
}
