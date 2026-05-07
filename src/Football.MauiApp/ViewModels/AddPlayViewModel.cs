using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Football.Core;
using Football.Data.Sqlite;

namespace Football.MauiApp.ViewModels;

public partial class AddPlayViewModel : ObservableObject
{
    private readonly IFootballRepository _repository;
    private int _nextPlayId = 1;
    private readonly List<int> _gameIds = [];
    private readonly List<int> _playerIds = [];

    public ObservableCollection<string> GameLabels { get; } = new();
    public ObservableCollection<string> PlayerLabels { get; } = new();

    [ObservableProperty]
    private int selectedGameIndex;

    [ObservableProperty]
    private int selectedPlayerIndex;

    [ObservableProperty]
    private string playNumText = "1";

    [ObservableProperty]
    private string calls = string.Empty;

    [ObservableProperty]
    private string techText = "0";

    [ObservableProperty]
    private string pursText = "0";

    [ObservableProperty]
    private string mtpText = "0";

    /// <summary>Single-letter play type (e.g. R, P)</summary>
    [ObservableProperty]
    private string typeText = "R";

    [ObservableProperty]
    private string stat1 = string.Empty;

    [ObservableProperty]
    private string stat2 = string.Empty;

    [ObservableProperty]
    private bool loaf;

    [ObservableProperty]
    private string comment = string.Empty;

    [ObservableProperty]
    private string position = string.Empty;

    [ObservableProperty]
    private string teamIdText = "1";

    public AddPlayViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task PrepareAsync(CancellationToken cancellationToken = default)
    {
        await _repository.InitializeAsync(cancellationToken);

        GameLabels.Clear();
        PlayerLabels.Clear();
        _gameIds.Clear();
        _playerIds.Clear();

        var games = await _repository.GetGamesAsync(cancellationToken);
        foreach (var g in games.OrderBy(x => x.Date))
        {
            GameLabels.Add($"{g.Date:MMM d, yyyy} · vs {g.Opponent}");
            _gameIds.Add(g.Id);
        }

        var players = await _repository.GetPlayersAsync(cancellationToken);
        foreach (var p in players.OrderBy(x => x.Number))
        {
            PlayerLabels.Add($"#{p.Number} {p.Name}");
            _playerIds.Add(p.Id);
        }

        var plays = await _repository.GetPlaysAsync(cancellationToken);
        _nextPlayId = plays.Count == 0 ? 1 : plays.Max(p => p.Id) + 1;

        SelectedGameIndex = GameLabels.Count > 0 ? 0 : 0;
        SelectedPlayerIndex = PlayerLabels.Count > 0 ? 0 : 0;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (GameLabels.Count == 0 || PlayerLabels.Count == 0)
            throw new InvalidOperationException("Add at least one game and one player before logging a play.");

        var gi = Math.Clamp(SelectedGameIndex, 0, _gameIds.Count - 1);
        var pi = Math.Clamp(SelectedPlayerIndex, 0, _playerIds.Count - 1);

        if (!int.TryParse(PlayNumText, out var playNum))
            playNum = 1;
        if (!int.TryParse(TechText, out var tech))
            tech = 0;
        if (!int.TryParse(PursText, out var purs))
            purs = 0;
        if (!int.TryParse(MtpText, out var mtp))
            mtp = 0;
        if (!int.TryParse(TeamIdText, out var teamId))
            teamId = 1;

        var typeTrim = (TypeText ?? string.Empty).Trim();
        var typeChar = typeTrim.Length > 0 ? typeTrim[0] : ' ';

        var play = new Play(
            Id: _nextPlayId,
            PlayNum: playNum,
            Calls: Calls,
            PlayerId: _playerIds[pi],
            Tech: tech,
            Purs: purs,
            Mtp: mtp,
            Type: typeChar,
            Stat1: Stat1,
            Stat2: Stat2,
            Loaf: Loaf,
            Comment: Comment,
            Position: Position,
            GameId: _gameIds[gi],
            TeamId: teamId);

        await _repository.AddPlayAsync(play, cancellationToken);
    }
}
