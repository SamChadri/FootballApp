using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using Football.Core;
using Football.Data.Sqlite;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public partial class AddGameViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;

    private int _nextGameId = 1;
    private int _nextGameNumber = 1;

    [ObservableProperty]
    private string opponent = string.Empty;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private string pointsText = "0";

    [ObservableProperty]
    private string opponentPointsText = "0";

    /// <summary>Picker index: 0 Win (W), 1 Loss (L), 2 Tie (T)</summary>
    [ObservableProperty]
    private int resultPickerIndex;

    [ObservableProperty]
    private string seasonIdText = "1";

    private string location = "H";

    public AddGameViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task PrepareAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);
        await UpdateNextGameNumbersAsync(cancellationToken);
    }

    partial void OnSeasonIdTextChanged(string value) => _ = UpdateNextGameNumbersAsync();

    private int ParseSeasonId() =>
        int.TryParse(SeasonIdText, out var id) && id > 0 ? id : 1;

    private async Task UpdateNextGameNumbersAsync(CancellationToken cancellationToken = default)
    {
        var games = await _repository.GetGamesAsync(ParseSeasonId(), cancellationToken);
        if (games.Count == 0)
        {
            _nextGameId = 1;
            _nextGameNumber = 1;
        }
        else
        {
            _nextGameId = games.Max(g => g.Id) + 1;
            _nextGameNumber = games.Max(g => g.Number) + 1;
        }
    }

    public async Task SaveGameAsync(CancellationToken cancellationToken = default)
    {
        var resultChars = new[] { 'W', 'L', 'T' };
        var idx = Math.Clamp(ResultPickerIndex, 0, resultChars.Length - 1);

        if (!int.TryParse(PointsText, out var points))
            points = 0;
        if (!int.TryParse(OpponentPointsText, out var opPts))
            opPts = 0;

        var game = new Game(
            Id: _nextGameId,
            Number: _nextGameNumber,
            Date: Date,
            Opponent: Opponent,
            Result: resultChars[idx],
            Points: points,
            OpponentPoints: opPts,
            Location: location,
            SeasonId: ParseSeasonId());

        try
        {
            var result = await _repository.AddGameAsync(game, cancellationToken);

            await Shell.Current.DisplayAlert("Debug", $"Result = {result}", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Database Error", ex.ToString(), "OK");
        }
        if (result == 1)
        {
            /*
            await Shell.Current.CurrentPage.ShowPopupAsync(
                new SuccessPopup("Game Added", $"Game #{game.Number} was added."));
            ClearForm();
            */

            await Application.Current!.MainPage!.DisplayAlert(
                "Debug",
                "SaveGameAsync reached popup section.",
                "OK");


            ClearForm();
        }
    }

    private void ClearForm()
    {
        opponent = string.Empty;
        pointsText = "0";
        opponentPointsText = "0";
        resultPickerIndex = 0;
        seasonIdText = "1";
        location = "H";
    }
}
