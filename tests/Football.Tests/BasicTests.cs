using Football.Core;
using Football.Data.Sqlite;

namespace Football.Tests;

public sealed class BasicTests
{
    // Shared test season/team id used across tests
    private const int TestSeasonId = 1;
    private const int TestTeamId   = 1;

    // ── helpers ────────────────────────────────────────────────────────────────

    private static Game MakeGame(int id = 1) => new Game(
        Id:             id,
        Number:         1,
        Date:           new DateTime(2019, 10, 1),
        Opponent:       "Ohio State",
        Result:         'W',
        Points:         43,
        OpponentPoints: 20,
        Location:       "H",
        SeasonId:       TestSeasonId
    );

    private static Player MakePlayer(int id = 20) => new Player(
        Id:            id,
        Number:        1,
        Name:          "John Doe",
        Position:      "Quarterback",
        Year:          "2019",
        TeamId:        TestTeamId,
        SeasonId:      TestSeasonId
    );

    private static Play MakePlay(int id = -1) => new Play(
        Id:            id,
        PlayNum:       1,
        Calls:         "NKL-OVER",
        PlayerId:      20,
        NumPenalties:  0,
        PenaltyNames:  "",
        PlayYards:     5,
        Tackles:       2,
        Tech:          1,
        Purs:          1,
        Mtp:           3,
        Type:          'R',
        Stat1:         "INT",
        Stat2:         "",
        Loaf:          true,
        Comment:       "DEPTH",
        Position:      "Quarterback",
        GameId:        1,
        TeamId:        TestTeamId,
        SeasonId:      TestSeasonId
    );

    // ── tests ──────────────────────────────────────────────────────────────────

    [Xunit.Fact]
    public async Task CanInitializeAndInsertGame()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);

        await repo.InitializeAsync();
        var inserted = await repo.AddGameAsync(MakeGame());

        Xunit.Assert.Equal(1, inserted);

        var games = await repo.GetGamesAsync(TestSeasonId);
        Xunit.Assert.Single(games);
        Xunit.Assert.Equal("Ohio State", games[0].Opponent);
    }

    [Xunit.Fact]
    public async Task CanInsertPlayer()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayerAsync(MakePlayer());
        Xunit.Assert.Equal(1, inserted);

        var players = await repo.GetPlayersAsync();
        Xunit.Assert.Single(players);
        Xunit.Assert.Equal("John Doe", players[0].Name);
    }

    [Xunit.Fact]
    public async Task CanInsertPlay()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPlaysAsync();
        Xunit.Assert.Single(plays);
        Xunit.Assert.Equal("NKL-OVER", plays[0].Calls);
        Xunit.Assert.Equal(20,    plays[0].PlayerId);
        Xunit.Assert.Equal(1,     plays[0].Tech);
        Xunit.Assert.Equal(1,     plays[0].Purs);
        Xunit.Assert.Equal(3,     plays[0].Mtp);
        Xunit.Assert.Equal('R',   plays[0].Type);
        Xunit.Assert.Equal("INT", plays[0].Stat1);
        Xunit.Assert.Equal("",    plays[0].Stat2);
        Xunit.Assert.True(plays[0].Loaf);
    }

    [Xunit.Fact]
    public async Task CanInsertGame()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddGameAsync(MakeGame());
        Xunit.Assert.Equal(1, inserted);

        var games = await repo.GetGamesAsync(TestSeasonId);
        Xunit.Assert.Single(games);
        Xunit.Assert.Equal(1,                        games[0].Number);
        Xunit.Assert.Equal(new DateTime(2019, 10, 1), games[0].Date);
        Xunit.Assert.Equal("Ohio State",              games[0].Opponent);
        Xunit.Assert.Equal('W',                       games[0].Result);
        Xunit.Assert.Equal(43,                        games[0].Points);
        Xunit.Assert.Equal(20,                        games[0].OpponentPoints);
    }

    [Xunit.Fact]
    public async Task CanGetGame()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddGameAsync(MakeGame(id: 1));
        Xunit.Assert.Equal(1, inserted);

        var game = await repo.GetGameAsync(1, TestSeasonId);
        Xunit.Assert.Equal(1,                        game.Number);
        Xunit.Assert.Equal(new DateTime(2019, 10, 1), game.Date);
        Xunit.Assert.Equal("Ohio State",              game.Opponent);
        Xunit.Assert.Equal('W',                       game.Result);
        Xunit.Assert.Equal(43,                        game.Points);
        Xunit.Assert.Equal(20,                        game.OpponentPoints);
    }

    [Xunit.Fact]
    public async Task CanGetPlayer()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayerAsync(MakePlayer(id: 1));
        Xunit.Assert.Equal(1, inserted);

        var player = await repo.GetPlayerAsync(1);
        Xunit.Assert.Equal(1,              player.Number);
        Xunit.Assert.Equal("John Doe",   player.Name);
        Xunit.Assert.Equal("Quarterback",player.Position);
        Xunit.Assert.Equal("2019",            player.Year);
    }

    [Xunit.Fact]
    public async Task CanGetTeamPlayers()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayerAsync(MakePlayer());
        Xunit.Assert.Equal(1, inserted);

        var players = await repo.GetTeamPlayersAsync(TestTeamId);
        Xunit.Assert.Single(players);
        Xunit.Assert.Equal("John Doe",    players[0].Name);
        Xunit.Assert.Equal("Quarterback", players[0].Position);
        Xunit.Assert.Equal("2019",             players[0].Year);
        Xunit.Assert.Equal(TestTeamId,       players[0].TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetTeamPlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetTeamPlaysAsync(TestTeamId, TestSeasonId);
        Xunit.Assert.Single(plays);
        Xunit.Assert.Equal("NKL-OVER",       plays[0].Calls);
        Xunit.Assert.Equal(20,               plays[0].PlayerId);
        Xunit.Assert.Equal(1,                plays[0].Tech);
        Xunit.Assert.Equal(1,                plays[0].Purs);
        Xunit.Assert.Equal(3,                plays[0].Mtp);
        Xunit.Assert.Equal('R',              plays[0].Type);
        Xunit.Assert.Equal("INT",            plays[0].Stat1);
        Xunit.Assert.Equal("",               plays[0].Stat2);
        Xunit.Assert.True(plays[0].Loaf);
        Xunit.Assert.Equal("Quarterback", plays[0].Position);
        Xunit.Assert.Equal(1,                plays[0].GameId);
        Xunit.Assert.Equal(TestTeamId,       plays[0].TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetTeamGamePlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetTeamGamePlaysAsync(TestTeamId, gameId: 1, TestSeasonId);
        var play  = plays[0];
        Xunit.Assert.Equal(1,                play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER",       play.Calls);
        Xunit.Assert.Equal(20,               play.PlayerId);
        Xunit.Assert.Equal(1,                play.Tech);
        Xunit.Assert.Equal(1,                play.Purs);
        Xunit.Assert.Equal(3,                play.Mtp);
        Xunit.Assert.Equal('R',              play.Type);
        Xunit.Assert.Equal("INT",            play.Stat1);
        Xunit.Assert.Equal("",               play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH",          play.Comment);
        Xunit.Assert.Equal("Quarterback", play.Position);
        Xunit.Assert.Equal(1,                play.GameId);
        Xunit.Assert.Equal(TestTeamId,       play.TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetPlayerGamePlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPlayerGamePlaysAsync(playerId: 20, gameId: 1, TestSeasonId);
        var play  = plays[0];
        Xunit.Assert.Equal(1,                play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER",       play.Calls);
        Xunit.Assert.Equal(20,               play.PlayerId);
        Xunit.Assert.Equal(1,                play.Tech);
        Xunit.Assert.Equal(1,                play.Purs);
        Xunit.Assert.Equal(3,                play.Mtp);
        Xunit.Assert.Equal('R',              play.Type);
        Xunit.Assert.Equal("INT",            play.Stat1);
        Xunit.Assert.Equal("",               play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH",          play.Comment);
        Xunit.Assert.Equal("Quarterback", play.Position);
        Xunit.Assert.Equal(1,                play.GameId);
        Xunit.Assert.Equal(TestTeamId,       play.TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetPlayerPlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPlayerPlaysAsync(20);
        var play  = plays[0];
        Xunit.Assert.Equal(1,                play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER",       play.Calls);
        Xunit.Assert.Equal(20,               play.PlayerId);
        Xunit.Assert.Equal(1,                play.Tech);
        Xunit.Assert.Equal(1,                play.Purs);
        Xunit.Assert.Equal(3,                play.Mtp);
        Xunit.Assert.Equal('R',              play.Type);
        Xunit.Assert.Equal("INT",            play.Stat1);
        Xunit.Assert.Equal("",               play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH",          play.Comment);
        Xunit.Assert.Equal("Quarterback", play.Position);
        Xunit.Assert.Equal(1,                play.GameId);
        Xunit.Assert.Equal(TestTeamId,       play.TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetPositionGamePlaysAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPositionGamePlaysAsync("Quarterback", gameId: 1, TestSeasonId);
        var play  = plays[0];

        Xunit.Assert.Equal(1,                play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER",       play.Calls);
        Xunit.Assert.Equal(20,               play.PlayerId);
        Xunit.Assert.Equal(1,                play.Tech);
        Xunit.Assert.Equal(1,                play.Purs);
        Xunit.Assert.Equal(3,                play.Mtp);
        Xunit.Assert.Equal('R',              play.Type);
        Xunit.Assert.Equal("INT",            play.Stat1);
        Xunit.Assert.Equal("",               play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH",          play.Comment);
        Xunit.Assert.Equal("Quarterback", play.Position);
        Xunit.Assert.Equal(1,                play.GameId);
        Xunit.Assert.Equal(TestTeamId,       play.TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetPositionPlaysAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPositionPlaysAsync("Quarterback", TestTeamId, TestSeasonId);
        var play  = plays[0];

        Xunit.Assert.Equal(1,                play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER",       play.Calls);
        Xunit.Assert.Equal(20,               play.PlayerId);
        Xunit.Assert.Equal(1,                play.Tech);
        Xunit.Assert.Equal(1,                play.Purs);
        Xunit.Assert.Equal(3,                play.Mtp);
        Xunit.Assert.Equal('R',              play.Type);
        Xunit.Assert.Equal("INT",            play.Stat1);
        Xunit.Assert.Equal("",               play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH",          play.Comment);
        Xunit.Assert.Equal("Quarterback", play.Position);
        Xunit.Assert.Equal(1,                play.GameId);
        Xunit.Assert.Equal(TestTeamId,       play.TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetSeasonGroupAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddGameAsync(MakeGame());
        Xunit.Assert.Equal(1, inserted);

        var group = await repo.GetSeasonGroupAsync(TestSeasonId);
        Xunit.Assert.Equal(TestSeasonId, group.SeasonId);
        Xunit.Assert.Single(group.SeasonGames);


    }

    [Xunit.Fact]
    public async Task CanGetSquadGroupsAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();


        Offense nsquad = new Offense();

        var inserted = await repo.AddGameAsync(MakeGame());
        Xunit.Assert.Equal(1, inserted);

        var inserted2 = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted2);

        var inserted3 = await repo.AddPlayerAsync(MakePlayer());
        Xunit.Assert.Equal(1, inserted3);


        var group = await repo.GetSquadGroupAsync(TestSeasonId,nsquad,TestTeamId);
        Xunit.Assert.Equal(nsquad.positionNames.Count, group.Positions.Count);
        Xunit.Assert.Equal(TestSeasonId, group.SeasonId);
        Xunit.Assert.Equal(nsquad.Name, group.Name);
        
    }

    [Xunit.Fact]
    public async Task CanGetPositionGroupAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddGameAsync(MakeGame());
        Xunit.Assert.Equal(1, inserted);

        var inserted2 = await repo.AddPlayAsync(MakePlay());
        Xunit.Assert.Equal(1, inserted2);

        var inserted3 = await repo.AddPlayerAsync(MakePlayer());
        Xunit.Assert.Equal(1, inserted3);

        var group = await repo.GetPositionGroupAsync("Quarterback",TestTeamId,TestSeasonId);;
        Xunit.Assert.Equal("Quarterback", group.Name);
        Xunit.Assert.Single(group.PositionPlayers);
        Xunit.Assert.Single(group.PositionPlays);
        
        




    }

    [Xunit.Fact]
    public async Task CanDeletePlayAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(MakePlay(id: 5));
        Xunit.Assert.Equal(1, inserted);

        var deleted = await repo.DeletePlayAsync(5);
        Xunit.Assert.Equal(1, deleted);
    }

    [Xunit.Fact]
    public async Task CanDeleteGameAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddGameAsync(MakeGame(id: 5));
        Xunit.Assert.Equal(1, inserted);

        var deleted = await repo.DeleteGameAsync(5);
        Xunit.Assert.Equal(1, deleted);
    }

    [Xunit.Fact]
    public async Task CanDeletePlayer()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayerAsync(MakePlayer(id:5));
        Xunit.Assert.Equal(1, inserted);

        var deleted = await repo.DeletePlayerAsync(5);
        Xunit.Assert.Equal(1, deleted);
    }

    [Xunit.Fact]
    public async Task CanSeedDatabaseWithRealRoster()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        
        await DatabaseSeeder.SeedAsync(repo);
        
        var players = await repo.GetPlayersAsync();
        Xunit.Assert.NotEmpty(players);
        Xunit.Assert.DoesNotContain(players, p => p.Name.StartsWith("Player "));
        
        // Check a known player
        var macResetich = players.FirstOrDefault(p => p.Name == "Mac Resetich");
        Xunit.Assert.NotNull(macResetich);
        Xunit.Assert.Equal(0, macResetich.Number);
    }
}

file sealed class TestDbPathProvider : IDbPathProvider
{
    private readonly string _baseDir;

    public TestDbPathProvider(string baseDir) => _baseDir = baseDir;

    public string GetFootballDbPath() => Path.Combine(_baseDir, "Football.db");
    public string GetAccountsDbPath() => Path.Combine(_baseDir, "Accounts.db");
}
