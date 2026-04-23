using Football.Core;
using Football.Data.Sqlite;

namespace Football.Tests;

public sealed class BasicTests
{
    [Xunit.Fact]
    public async Task CanInitializeAndInsertGame()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);

        await repo.InitializeAsync();
        var inserted = await repo.AddGameAsync(new Game(
            Id: -1,
            Number: 1,
            Date: new DateTime(2019, 10, 1),
            Opponent: "Ohio State",
            Result: 'W',
            Points: 43,
            OpponentPoints: 20
        ));

        Xunit.Assert.Equal(1, inserted);

        var games = await repo.GetGamesAsync();
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
        var inserted = await repo.AddPlayerAsync(new Player(
            Id: 1,
            Number: 64,
            Name: "Test Player",
            Position: "Defensive Line",
            Year: "FR",
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);

        var players = await repo.GetPlayersAsync();
        Xunit.Assert.Single(players);
        Xunit.Assert.Equal("Test Player", players[0].Name);
    }

    [Xunit.Fact]
    public async Task CanInsertPlay()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPlaysAsync();
        Xunit.Assert.Single(plays);
        Xunit.Assert.Equal("NKL-OVER", plays[0].Calls);
        Xunit.Assert.Equal(64, plays[0].PlayerId);
        Xunit.Assert.Equal(1, plays[0].Tech);
        Xunit.Assert.Equal(1, plays[0].Purs);
        Xunit.Assert.Equal(3, plays[0].Mtp);
        Xunit.Assert.Equal('R', plays[0].Type);
        Xunit.Assert.Equal("INT", plays[0].Stat1);
        Xunit.Assert.Equal("", plays[0].Stat2);
        Xunit.Assert.True(plays[0].Loaf);
    }

    [Xunit.Fact]
    public async Task CanInsertGame()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddGameAsync(new Game(
            Id: -1,
            Number: 1,
            Date: new DateTime(2019, 10, 1),
            Opponent: "Ohio State",
            Result: 'W',
            Points: 43,
            OpponentPoints: 20
        ));
        Xunit.Assert.Equal(1, inserted);

        var games = await repo.GetGamesAsync();
        Xunit.Assert.Single(games);
        Xunit.Assert.Equal(1, games[0].Number);
        Xunit.Assert.Equal(new DateTime(2019, 10, 1), games[0].Date);
        Xunit.Assert.Equal("Ohio State", games[0].Opponent);
        Xunit.Assert.Equal('W', games[0].Result);
        Xunit.Assert.Equal(43, games[0].Points);
        Xunit.Assert.Equal(20, games[0].OpponentPoints);
    }


    [Xunit.Fact]
    public async Task CanGetGame()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddGameAsync(new Game(
            Id: 1,
            Number: 1,
            Date: new DateTime(2019, 10, 1),
            Opponent: "Ohio State",
            Result: 'W',
            Points: 43,
            OpponentPoints: 20
        ));
        Xunit.Assert.Equal(1, inserted);

        var game = await repo.GetGameAsync(1);
        Xunit.Assert.Equal(1, game.Number);
        Xunit.Assert.Equal(new DateTime(2019, 10, 1), game.Date);
        Xunit.Assert.Equal("Ohio State", game.Opponent);
        Xunit.Assert.Equal('W', game.Result);
        Xunit.Assert.Equal(43, game.Points);
        Xunit.Assert.Equal(20, game.OpponentPoints);
    }

    [Xunit.Fact]
    public async Task CanGetPlayer()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayerAsync(new Player(
            Id: 1,
            Number: 64,
            Name: "Test Player",
            Position: "Defensive Line",
            Year: "FR",
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);

        var player = await repo.GetPlayerAsync(1);
        Xunit.Assert.Equal(64, player.Number);
        Xunit.Assert.Equal("Test Player", player.Name);
        Xunit.Assert.Equal("Defensive Line", player.Position);
        Xunit.Assert.Equal("FR", player.Year);
    }

    [Xunit.Fact]
    public async Task CanGetTeamPlayers()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayerAsync(new Player(
            Id: 1,
            Number: 64,
            Name: "Test Player",
            Position: "Defensive Line",
            Year: "FR",
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);

        var players = await repo.GetTeamPlayersAsync(1);
        Xunit.Assert.Single(players);
        Xunit.Assert.Equal("Test Player", players[0].Name);
        Xunit.Assert.Equal("Defensive Line", players[0].Position);
        Xunit.Assert.Equal("FR", players[0].Year);
        Xunit.Assert.Equal(1, players[0].TeamId);
    }

    [Xunit.Fact]
    public async Task CanGetTeamPlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));

        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetTeamPlaysAsync(1);
        Xunit.Assert.Single(plays);
        Xunit.Assert.Equal("NKL-OVER", plays[0].Calls);
        Xunit.Assert.Equal(64, plays[0].PlayerId);
        Xunit.Assert.Equal(1, plays[0].Tech);
        Xunit.Assert.Equal(1, plays[0].Purs);
        Xunit.Assert.Equal(3, plays[0].Mtp);
        Xunit.Assert.Equal('R', plays[0].Type);
        Xunit.Assert.Equal("INT", plays[0].Stat1);
        Xunit.Assert.Equal("", plays[0].Stat2);
        Xunit.Assert.True(plays[0].Loaf);
        Xunit.Assert.Equal("Defensive Line", plays[0].Position);
        Xunit.Assert.Equal(1, plays[0].GameId);
        Xunit.Assert.Equal(1, plays[0].TeamId);
    }
    [Xunit.Fact]
    public async Task CanGetTeamGamePlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();
        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetTeamGamePlaysAsync(1, 1);
        var play = plays[0];
        Xunit.Assert.Equal(1, play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER", play.Calls);
        Xunit.Assert.Equal(64, play.PlayerId);
        Xunit.Assert.Equal(1, play.Tech);
        Xunit.Assert.Equal(1, play.Purs);
        Xunit.Assert.Equal(3, play.Mtp);
        Xunit.Assert.Equal('R', play.Type);
        Xunit.Assert.Equal("INT", play.Stat1);
        Xunit.Assert.Equal("", play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH", play.Comment);
        Xunit.Assert.Equal("Defensive Line", play.Position);
        Xunit.Assert.Equal(1, play.GameId);
        Xunit.Assert.Equal(1, play.TeamId);
    }
    
    [Xunit.Fact]
    public async Task CanGetPlayerGamePlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));

        Xunit.Assert.Equal(1, inserted);

        var plays = await repo.GetPlayerGamePlaysAsync(64,1);
        var play = plays[0];
        Xunit.Assert.Equal(1, play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER", play.Calls);
        Xunit.Assert.Equal(64, play.PlayerId);
        Xunit.Assert.Equal(1, play.Tech);
        Xunit.Assert.Equal(1, play.Purs);
        Xunit.Assert.Equal(3, play.Mtp);
        Xunit.Assert.Equal('R', play.Type);
        Xunit.Assert.Equal("INT", play.Stat1);
        Xunit.Assert.Equal("", play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH", play.Comment);
        Xunit.Assert.Equal("Defensive Line", play.Position);
        Xunit.Assert.Equal(1, play.GameId);
        Xunit.Assert.Equal(1, play.TeamId);

         
    }

    [Xunit.Fact]
    public async Task CanGetPlayerPlays()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));

        Xunit.Assert.Equal(1,inserted);

        var plays = await repo.GetPlayerPlaysAsync(64);
        var play = plays[0];

        Xunit.Assert.Equal(1, play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER", play.Calls);
        Xunit.Assert.Equal(64, play.PlayerId);
        Xunit.Assert.Equal(1, play.Tech);
        Xunit.Assert.Equal(1, play.Purs);
        Xunit.Assert.Equal(3, play.Mtp);
        Xunit.Assert.Equal('R', play.Type);
        Xunit.Assert.Equal("INT", play.Stat1);
        Xunit.Assert.Equal("", play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH", play.Comment);
        Xunit.Assert.Equal("Defensive Line", play.Position);
        Xunit.Assert.Equal(1, play.GameId);
        Xunit.Assert.Equal(1, play.TeamId);


         
        
    }

    [Xunit.Fact]
    public async Task CanGetPositionGamePlaysAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));

        var plays = await repo.GetPositionGamePlaysAsync("Defensive Line",1);
        var play = plays[0];


        Xunit.Assert.Equal(1,inserted);

        Xunit.Assert.Equal(1, play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER", play.Calls);
        Xunit.Assert.Equal(64, play.PlayerId);
        Xunit.Assert.Equal(1, play.Tech);
        Xunit.Assert.Equal(1, play.Purs);
        Xunit.Assert.Equal(3, play.Mtp);
        Xunit.Assert.Equal('R', play.Type);
        Xunit.Assert.Equal("INT", play.Stat1);
        Xunit.Assert.Equal("", play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH", play.Comment);
        Xunit.Assert.Equal("Defensive Line", play.Position);
        Xunit.Assert.Equal(1, play.GameId);
        Xunit.Assert.Equal(1, play.TeamId);

    }

    [Xunit.Fact]
    public async Task CanGetPositionPlaysAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(new Play(
            Id: -1,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));
        Xunit.Assert.Equal(1,inserted);
        var plays = await repo.GetPositionPlaysAsync("Defensive Line");
        var play = plays[0];


        Xunit.Assert.Equal(1, play.PlayNum);
        Xunit.Assert.Equal("NKL-OVER", play.Calls);
        Xunit.Assert.Equal(64, play.PlayerId);
        Xunit.Assert.Equal(1, play.Tech);
        Xunit.Assert.Equal(1, play.Purs);
        Xunit.Assert.Equal(3, play.Mtp);
        Xunit.Assert.Equal('R', play.Type);
        Xunit.Assert.Equal("INT", play.Stat1);
        Xunit.Assert.Equal("", play.Stat2);
        Xunit.Assert.True(play.Loaf);
        Xunit.Assert.Equal("DEPTH", play.Comment);
        Xunit.Assert.Equal("Defensive Line", play.Position);
        Xunit.Assert.Equal(1, play.GameId);
        Xunit.Assert.Equal(1, play.TeamId);

    }

    [Xunit.Fact]
    public async Task CanDeletePlayAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayAsync(new Play(
            Id: 5,
            PlayNum: 1,
            Calls: "NKL-OVER",
            PlayerId: 64,
            Tech: 1,
            Purs: 1,
            Mtp: 3,
            Type: 'R',
            Stat1: "INT",
            Stat2: "",
            Loaf: true,
            Comment: "DEPTH",
            Position: "Defensive Line",
            GameId: 1,
            TeamId: 1
        ));
        Xunit.Assert.Equal(1,inserted);

        var deleted = await repo.DeletePlayAsync(5);
        Xunit.Assert.Equal(1,deleted);

    }

    [Xunit.Fact]
    public async Task CanDeleteGameAsync()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddGameAsync(new Game(
            Id: 5,
            Number: 1,
            Date: new DateTime(2019, 10, 1),
            Opponent: "Ohio State",
            Result: 'W',
            Points: 43,
            OpponentPoints: 20
        ));

        Xunit.Assert.Equal(1, inserted);

        var deleted = await repo.DeleteGameAsync(5);
        Xunit.Assert.Equal(1,deleted);
    }

    [Xunit.Fact]
    public async Task CanDeletePlayer()
    {
        var baseDir = Path.Combine(Path.GetTempPath(), "footballapp-tests", Guid.NewGuid().ToString("N"));
        var dbPathProvider = new TestDbPathProvider(baseDir);
        var repo = new SqliteFootballRepository(dbPathProvider);
        await repo.InitializeAsync();

        var inserted = await repo.AddPlayerAsync(new Player(
            Id: 5,
            Number: 64,
            Name: "Test Player",
            Position: "Defensive Line",
            Year: "FR",
            TeamId: 1
        ));
        Xunit.Assert.Equal(1, inserted);


        var deleted = await repo.DeletePlayerAsync(5);
        Xunit.Assert.Equal(1,deleted);
        
    }
}

file sealed class TestDbPathProvider : IDbPathProvider
{
    private readonly string _baseDir;

    public TestDbPathProvider(string baseDir) => _baseDir = baseDir;

    public string GetFootballDbPath() => Path.Combine(_baseDir, "Football.db");
    public string GetAccountsDbPath() => Path.Combine(_baseDir, "Accounts.db");
}

