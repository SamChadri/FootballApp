using CommunityToolkit.Mvvm.ComponentModel;
using Football.Core;
using Football.Data.Sqlite;

namespace Football.MauiApp.ViewModels;

public partial class AddPlayerViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;
    private int _nextPlayerId = 1;

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
            TeamId: teamId);

        await _repository.AddPlayerAsync(player, cancellationToken);
    }
}
