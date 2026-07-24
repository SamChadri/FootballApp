using Football.Core;
using Microsoft.Data.Sqlite;

namespace Football.Data.Sqlite;

public sealed class SqliteFootballRepository : IFootballRepository
{
    private readonly IDbPathProvider _dbPathProvider;

    public SqliteFootballRepository(IDbPathProvider dbPathProvider)
    {
        _dbPathProvider = dbPathProvider;
    }

    public async Task DropAllTablesAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = """
            DROP TABLE IF EXISTS Plays;
            DROP TABLE IF EXISTS Players;
            DROP TABLE IF EXISTS Games;
            DROP TABLE IF EXISTS Seasons;
            """;
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath) ?? ".");

        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        // Minimal schema for phase 1 (expand as we port features)
        var createGames = db.CreateCommand();
        createGames.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Games (
              ID INTEGER PRIMARY KEY AUTOINCREMENT,
              Number INTEGER,
              Date TEXT,
              Opponent TEXT,
              Result TEXT,
              Points INTEGER,
              OpPoints INTEGER,
              Location TEXT,
              SeasonId INTEGER
            );
            """;
        await createGames.ExecuteNonQueryAsync(cancellationToken);

        var createSeasons = db.CreateCommand();
        createSeasons.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Seasons (
              ID INTEGER PRIMARY KEY AUTOINCREMENT,
              Year INTEGER
            );
            """;
        await createSeasons.ExecuteNonQueryAsync(cancellationToken);

        var createPlayers = db.CreateCommand();
        createPlayers.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Players (
              ID INTEGER PRIMARY KEY AUTOINCREMENT,
              Number INTEGER,
              Name TEXT,
              Position TEXT,
              Year TEXT,
              TeamId INTEGER,
              SeasonId INTEGER
            );
            """;
        await createPlayers.ExecuteNonQueryAsync(cancellationToken);

        var createPlays = db.CreateCommand();
        createPlays.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Plays (
              ID INTEGER PRIMARY KEY AUTOINCREMENT,
              PlayNum INTEGER,
              Calls TEXT,
              PlayerId INTEGER,
              NumPenalties INTEGER,
              PenaltyNames TEXT,
              PlayYards INTEGER,
              Tackles INTEGER,
              Tech INTEGER,
              Purs INTEGER,
              Mtp INTEGER,
              Type TEXT,
              Stat1 TEXT,
              Stat2 TEXT,
              Loaf BOOLEAN,
              Comment TEXT,
              Position TEXT,
              GameId INTEGER,
              TeamId INTEGER,
              SeasonId INTEGER
            );
            """;
        await createPlays.ExecuteNonQueryAsync(cancellationToken);
        
    }

    public async Task<int> AddGameAsync(Game game, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO Games(Number, Date, Opponent, Result, Points, OpPoints, Location, SeasonId)
            VALUES ($number, $date, $opponent, $result, $points, $opPoints, $location, $seasonId);
            """;
        cmd.Parameters.AddWithValue("$number", game.Number);
        cmd.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$opponent", game.Opponent);
        cmd.Parameters.AddWithValue("$result", game.Result.ToString());
        cmd.Parameters.AddWithValue("$points", game.Points);
        cmd.Parameters.AddWithValue("$opPoints", game.OpponentPoints);
        cmd.Parameters.AddWithValue("$location", game.Location);
        cmd.Parameters.AddWithValue("$seasonId", game.SeasonId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> AddPlayerAsync(Player player, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO Players(Number, Name, Position, Year, TeamId, SeasonId)
            VALUES ($number, $name, $position, $year, $teamId, $seasonId);
            """;
        cmd.Parameters.AddWithValue("$number", player.Number);
        cmd.Parameters.AddWithValue("$name", player.Name);
        cmd.Parameters.AddWithValue("$position", player.Position);
        cmd.Parameters.AddWithValue("$year", player.Year);
        cmd.Parameters.AddWithValue("$teamId", player.TeamId);
        cmd.Parameters.AddWithValue("$seasonId", player.SeasonId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }


    public async Task<int> AddPlayAsync(Play play, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Plays(PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId)
            VALUES ($playNum, $calls, $PlayerId, $numPenalties, $penaltyNames, $playYards, $tackles, $tech, $purs, $mtp, $type, $stat1, $stat2, $loaf, $comment, $position, $GameId, $TeamId, $seasonId);
            """;
        cmd.Parameters.AddWithValue("$playNum", play.PlayNum);
        cmd.Parameters.AddWithValue("$calls", play.Calls);
        cmd.Parameters.AddWithValue("$PlayerId", play.PlayerId);
        cmd.Parameters.AddWithValue("$numPenalties", play.NumPenalties);
        cmd.Parameters.AddWithValue("$penaltyNames", play.PenaltyNames);
        cmd.Parameters.AddWithValue("$playYards", play.PlayYards);
        cmd.Parameters.AddWithValue("$tackles", play.Tackles);
        cmd.Parameters.AddWithValue("$tech", play.Tech);
        cmd.Parameters.AddWithValue("$purs", play.Purs);
        cmd.Parameters.AddWithValue("$mtp", play.Mtp);
        cmd.Parameters.AddWithValue("$type", play.Type.ToString());
        cmd.Parameters.AddWithValue("$stat1", play.Stat1);
        cmd.Parameters.AddWithValue("$stat2", play.Stat2);
        cmd.Parameters.AddWithValue("$loaf", play.Loaf ? 1 : 0);
        cmd.Parameters.AddWithValue("$comment", play.Comment);
        cmd.Parameters.AddWithValue("$position", play.Position);
        cmd.Parameters.AddWithValue("$GameId", play.GameId);
        cmd.Parameters.AddWithValue("$TeamId", play.TeamId);
        cmd.Parameters.AddWithValue("$seasonId", play.SeasonId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Play> GetPlayAsync(int playId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE ID = $playId;";
        cmd.Parameters.AddWithValue("$playId", playId);
        Play? result = null;

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var position = reader.GetString(16);
            var GameId = reader.GetInt32(17);
            var teamId = reader.GetInt32(18);
            var seasonId = reader.GetInt32(19);
            
            result = new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, teamId, seasonId);
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetPlaysAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays;";
        var result = new List<Play>();

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var position = reader.GetString(16);
            var GameId = reader.GetInt32(17);
            var teamId = reader.GetInt32(18);
            var seasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, teamId, seasonId));
        }

        return result;

    }

    public async Task<IReadOnlyList<Game>> GetGamesAsync(int seasonId,CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Date, Opponent, Result, Points, OpPoints, Location, SeasonId FROM Games WHERE SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Game>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var date = DateTime.Parse(reader.GetString(2));
            var opponent = reader.GetString(3);
            var resultChar = reader.GetString(4) is { Length: > 0 } s ? s[0] : ' ';
            var points = reader.GetInt32(5);
            var opPoints = reader.GetInt32(6);
            var location = reader.GetString(7);
            var resultSeasonId = reader.GetInt32(8);

            result.Add(new Game(id, number, date, opponent, resultChar, points, opPoints, location, resultSeasonId));
        }

        return result;
    }

    public async Task<Game> GetGameAsync(int gameId, int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Date, Opponent, Result, Points, OpPoints, Location, SeasonId FROM Games WHERE ID = $gameId AND SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$gameId", gameId);
        cmd.Parameters.AddWithValue("$seasonId", seasonId);
        Game? result = null;
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var date = DateTime.Parse(reader.GetString(2));
            var opponent = reader.GetString(3);
            var resultChar = reader.GetString(4) is { Length: > 0 } s ? s[0] : ' ';
            var points = reader.GetInt32(5);
            var opPoints = reader.GetInt32(6);
            var location = reader.GetString(7);
            var resultSeasonId = reader.GetInt32(8);

            result = new Game(id, number, date, opponent, resultChar, points, opPoints, location, resultSeasonId);
        }

        return result;
    }

    public async Task<IReadOnlyList<Player>> GetPlayersAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId, SeasonId FROM Players;";

        var result = new List<Player>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var name = reader.GetString(2);
            var position = reader.GetString(3);
            var year = reader.GetString(4);
            var teamId = reader.GetInt32(5);
            var seasonId = reader.GetInt32(6);

            result.Add(new Player(id, number, name, position, year, teamId, seasonId));
        }

        return result;
    }


    
    public async Task<Player> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId, SeasonId FROM Players WHERE ID = $playerId;";
        cmd.Parameters.AddWithValue("$playerId", playerId);
        Player? result = null;
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var name = reader.GetString(2);
            var position = reader.GetString(3);
            var year = reader.GetString(4);
            var teamId = reader.GetInt32(5);
            var seasonId = reader.GetInt32(6);

            result = new Player(id, number, name, position, year, teamId, seasonId);
        }

        return result;
    }
    public async Task<IReadOnlyList<Player>> GetTeamPlayersAsync(int teamId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId, SeasonId FROM Players WHERE TeamId = $teamId;";
        cmd.Parameters.AddWithValue("$teamId", teamId);

        var result = new List<Player>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var name = reader.GetString(2);
            var position = reader.GetString(3);
            var year = reader.GetString(4);
            var resultTeamId = reader.GetInt32(5);
            var seasonId = reader.GetInt32(6);

            result.Add(new Player(id, number, name, position, year, resultTeamId, seasonId));
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetTeamPlaysAsync(int teamId, int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE TeamId = $teamId AND SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$teamId", teamId);
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var position = reader.GetString(16);
            var GameId = reader.GetInt32(17);
            var resultTeamId = reader.GetInt32(18);
            var resultSeasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, resultTeamId, resultSeasonId));
        }

        return result;
    }
    
    public async Task<IReadOnlyList<Play>> GetTeamGamePlaysAsync(int teamId, int gameId, int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE TeamId = $teamId AND GameId = $gameId AND SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$teamId", teamId);
        cmd.Parameters.AddWithValue("$gameId", gameId);
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var position = reader.GetString(16);
            var resultGameId = reader.GetInt32(17);
            var resultTeamId = reader.GetInt32(18);
            var resultSeasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, resultGameId, resultTeamId, resultSeasonId));
        }

        return result;
    }
    public async Task<IReadOnlyList<Play>> GetPlayerGamePlaysAsync(int playerId, int gameId, int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE PlayerId = $playerId AND GameId = $gameId AND SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$playerId", playerId);
        cmd.Parameters.AddWithValue("$gameId", gameId);
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var position = reader.GetString(16);
            var resultGameId = reader.GetInt32(17);
            var resultTeamId = reader.GetInt32(18);
            var resultSeasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, resultGameId, resultTeamId, resultSeasonId));
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetPlayerPlaysAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE PlayerId = $playerId;";
        cmd.Parameters.AddWithValue("$playerId", playerId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var position = reader.GetString(16);
            var GameId = reader.GetInt32(17);
            var teamId = reader.GetInt32(18);
            var seasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, teamId, seasonId));
        }

        return result;
    }
    public async Task<IReadOnlyList<Play>> GetPositionGamePlaysAsync(string position, int gameId, int seasonId, CancellationToken cancellationToken = default)
    {

        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE Position = $position AND GameId = $gameId AND SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$position", position);
        cmd.Parameters.AddWithValue("$gameId", gameId);
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var playerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var resultPosition = reader.GetString(16);
            var resultGameId = reader.GetInt32(17);
            var resultTeamId = reader.GetInt32(18);
            var resultSeasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, playerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, resultPosition, resultGameId, resultTeamId, resultSeasonId));
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetPositionPlaysAsync(string position, int teamId, int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, NumPenalties, PenaltyNames, PlayYards, Tackles, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId, SeasonId FROM Plays WHERE Position = $position AND SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$position", position);
        cmd.Parameters.AddWithValue("$seasonId", seasonId);
        cmd.Parameters.AddWithValue("$teamId", teamId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var numPenalties = reader.GetInt32(4);
            var penaltyNames = reader.GetString(5);
            var playYards = reader.GetInt32(6);
            var tackles = reader.GetInt32(7);
            var tech = reader.GetInt32(8);
            var purs = reader.GetInt32(9);
            var mtp = reader.GetInt32(10);
            var type = reader.GetString(11) is { Length: > 0 } s ? s[0] : ' ';
            var stat1 = reader.GetString(12);
            var stat2 = reader.GetString(13);
            var loaf = (reader.GetInt32(14) != 0);
            var comment = reader.GetString(15);
            var resultPosition = reader.GetString(16);
            var GameId = reader.GetInt32(17);
            var resultTeamId = reader.GetInt32(18);
            var resultSeasonId = reader.GetInt32(19);

            result.Add(new Play(id, playNum, calls, PlayerId, numPenalties, penaltyNames, playYards, tackles, tech, purs, mtp, type, stat1, stat2, loaf, comment, resultPosition, GameId, resultTeamId, resultSeasonId));
        }

        return result;
    }


    public async Task<IReadOnlyList<Player>> GetPositionPlayersAsync(string position, int teamId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId, SeasonId FROM Players WHERE Position = $position AND TeamId = $teamId;";
        cmd.Parameters.AddWithValue("$position", position);
        cmd.Parameters.AddWithValue("$teamId", teamId);

        var result = new List<Player>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var name = reader.GetString(2);
            var playerPosition = reader.GetString(3);
            var year = reader.GetString(4);
            var resultTeamId = reader.GetInt32(5);
            var seasonId = reader.GetInt32(6);
            result.Add(new Player(id, number, name, playerPosition, year, resultTeamId, seasonId));
        }

        return result;
    }

    public async Task<SeasonGroup> GetSeasonGroupAsync(int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Game>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var date = DateTime.Parse(reader.GetString(2));
            var opponent = reader.GetString(3);
            var resultChar = reader.GetString(4) is { Length: > 0 } s ? s[0] : ' ';
            var points = reader.GetInt32(5);
            var opponentPoints = reader.GetInt32(6);
            var location = reader.GetString(7);
            var resultSeasonId = reader.GetInt32(8);

            result.Add(new Game(id, number, date, opponent, resultChar, points, opponentPoints, location, resultSeasonId));
        }
        return new SeasonGroup(seasonId, result);
    }

    public async Task<SquadGroup> GetSquadGroupAsync(int seasonId,Squad squad,int teamId, CancellationToken cancellationToken = default)
    {
        var squadGroup = new SquadGroup(seasonId, squad.Name, squad);
        foreach (var position in squad.positionNames)
        {
            var positionGroup = await GetPositionGroupAsync(position, teamId, seasonId, cancellationToken);
            squadGroup.Positions.Add(positionGroup);
        }
        return squadGroup;
        
    }


    public async Task<PositionGroup> GetPositionGroupAsync(string position, int teamId, int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);
        

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM Games WHERE SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var positionGames = new List<Game>();

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var date = DateTime.Parse(reader.GetString(2));
            var opponent = reader.GetString(3);
            var resultChar = reader.GetString(4) is { Length: > 0 } s ? s[0] : ' ';
            var points = reader.GetInt32(5);
            var opponentPoints = reader.GetInt32(6);
            var location = reader.GetString(7);
            var resultSeasonId = reader.GetInt32(8);

            positionGames.Add(new Game(id, number, date, opponent, resultChar, points, opponentPoints, location, resultSeasonId));
        }
        var positionPlayers = await GetPositionPlayersAsync(position, teamId, cancellationToken);
        var positionPlays = await GetPositionPlaysAsync(position, teamId, seasonId, cancellationToken);

        return new PositionGroup(position, positionPlayers.ToList(), positionPlays.ToList());
    }

    public async Task<IReadOnlyList<Game>> GetSeasonGamesAsync(int seasonId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Date, Opponent, Result, Points, OpponentPoints, Location, SeasonId FROM Games WHERE SeasonId = $seasonId;";
        cmd.Parameters.AddWithValue("$seasonId", seasonId);

        var result = new List<Game>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var number = reader.GetInt32(1);
            var date = DateTime.Parse(reader.GetString(2));
            var opponent = reader.GetString(3);
            var resultChar = reader.GetString(4) is { Length: > 0 } s ? s[0] : ' ';
            var points = reader.GetInt32(5);
            var opponentPoints = reader.GetInt32(6);
            var location = reader.GetString(7);
            var resultSeasonId = reader.GetInt32(8);

            result.Add(new Game(id, number, date, opponent, resultChar, points, opponentPoints, location, resultSeasonId));
        }

        return result;
    }

    public async Task<int> DeleteGameAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "DELETE FROM Games WHERE ID = $gameId;";
        cmd.Parameters.AddWithValue("$gameId", gameId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> DeletePlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "DELETE FROM Players WHERE ID = $playerId;";
        cmd.Parameters.AddWithValue("$playerId", playerId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
    public async Task<int> DeletePlayAsync(int playId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "DELETE FROM Plays WHERE ID = $playId;";
        cmd.Parameters.AddWithValue("$playId", playId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}

