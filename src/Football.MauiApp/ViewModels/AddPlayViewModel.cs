using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using Football.Core;
using Football.Data.Sqlite;
using Football.MauiApp.Views;

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
    private string seasonIdText = "1";

    [ObservableProperty]
    private int selectedGameIndex;

    [ObservableProperty]
    private int selectedPlayerIndex;

    [ObservableProperty]
    private string playNumText = "1";

    [ObservableProperty]
    private string numPenaltiesText = "0";

    [ObservableProperty]
    private string penaltyNames = string.Empty;

    [ObservableProperty]
    private string playYardsText = "0";

    [ObservableProperty]
    private string tacklesText = "0";

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

        PlayerLabels.Clear();
        _playerIds.Clear();

        var players = await _repository.GetPlayersAsync(cancellationToken);
        foreach (var p in players.OrderBy(x => x.Number))
        {
            PlayerLabels.Add($"#{p.Number} {p.Name}");
            _playerIds.Add(p.Id);
        }

        var plays = await _repository.GetPlaysAsync(cancellationToken);
        _nextPlayId = plays.Count == 0 ? 1 : plays.Max(p => p.Id) + 1;

        await ReloadGamesAsync(cancellationToken);

        SelectedPlayerIndex = PlayerLabels.Count > 0 ? 0 : 0;
    }

    partial void OnSeasonIdTextChanged(string value) => _ = ReloadGamesAsync();

    private int ParseSeasonId() =>
        int.TryParse(SeasonIdText, out var id) && id > 0 ? id : 1;

    private async Task ReloadGamesAsync(CancellationToken cancellationToken = default)
    {
        GameLabels.Clear();
        _gameIds.Clear();

        var games = await _repository.GetGamesAsync(ParseSeasonId(), cancellationToken);
        foreach (var g in games.OrderBy(x => x.Date))
        {
            GameLabels.Add($"{g.Date:MMM d, yyyy} · vs {g.Opponent}");
            _gameIds.Add(g.Id);
        }

        SelectedGameIndex = GameLabels.Count > 0 ? 0 : 0;
    }

    private void ClearForm()
    {
        seasonIdText = string.Empty;
        selectedGameIndex = 0;
        selectedPlayerIndex = 0;
        playNumText = string.Empty;
        numPenaltiesText = string.Empty;
        penaltyNames = string.Empty;
        playYardsText = string.Empty;
        tacklesText = string.Empty;
        calls = string.Empty;
        techText = string.Empty;
        pursText = string.Empty;
        mtpText = string.Empty;
        typeText = string.Empty;
        stat1 = string.Empty;
        stat2 = string.Empty;
        loaf = false;
        comment = string.Empty;
        position = string.Empty;
        teamIdText = string.Empty;
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
        if (!int.TryParse(NumPenaltiesText, out var numPenalties))
            numPenalties = 0;
        if (!int.TryParse(PlayYardsText, out var playYards))
            playYards = 0;
        if (!int.TryParse(TacklesText, out var tackles))
            tackles = 0;

        var typeTrim = (TypeText ?? string.Empty).Trim();
        var typeChar = typeTrim.Length > 0 ? typeTrim[0] : ' ';

        var play = new Play(
            Id: _nextPlayId,
            PlayNum: playNum,
            Calls: Calls,
            PlayerId: _playerIds[pi],
            NumPenalties: numPenalties,
            PenaltyNames: PenaltyNames,
            PlayYards: playYards,
            Tackles: tackles,
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
            TeamId: teamId,
            SeasonId: ParseSeasonId());

        var result = await _repository.AddPlayAsync(play, cancellationToken);
        if (result == 1)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(
                new SuccessPopup("Play Added", $"Play #{play.PlayNum} was added."));
            ClearForm();
        }
    }
}
