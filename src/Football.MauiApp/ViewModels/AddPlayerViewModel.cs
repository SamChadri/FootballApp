using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using Football.Core;
using Football.Data.Sqlite;
using Football.MauiApp.Views;

namespace Football.MauiApp.ViewModels;

public partial class AddPlayerViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;
    private int _nextPlayerId = 1;
    private int seasonId = 1;

    [ObservableProperty]
    private string numberText = string.Empty;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string position = string.Empty;

    [ObservableProperty]
    private string year = string.Empty;

    [ObservableProperty]
    private string teamIdText = "1";

    public AddPlayerViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task PrepareAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);
        var players = await _repository.GetPlayersAsync(cancellationToken);
        _nextPlayerId = players.Count == 0 ? 1 : players.Max(p => p.Id) + 1;
    }

    private void ClearForm()
    {
        numberText = string.Empty;
        name = string.Empty;
        position = string.Empty;
        year = string.Empty;
        teamIdText = "1";
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(NumberText, out var number))
            number = 0;
        if (!int.TryParse(TeamIdText, out var teamId))
            teamId = 1;

        var player = new Player(
            Id: _nextPlayerId,
            Number: number,
            Name: Name,
            Position: Position,
            Year: Year,
            TeamId: teamId,
            SeasonId: seasonId);

        var result = await _repository.AddPlayerAsync(player, cancellationToken);

        if (result == 1)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(
                new SuccessPopup("Player Added", $"{player.Name} was added."));
            ClearForm();
        }
    }
}
