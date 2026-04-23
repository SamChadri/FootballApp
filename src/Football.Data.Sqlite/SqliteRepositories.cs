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
              ID INTEGER PRIMARY KEY,
              Number INTEGER,
              Date TEXT,
              Opponent TEXT,
              Result TEXT,
              Points INTEGER,
              OpPoints INTEGER
            );
            """;
        await createGames.ExecuteNonQueryAsync(cancellationToken);

        var createPlayers = db.CreateCommand();
        createPlayers.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Players (
              ID INTEGER PRIMARY KEY,
              Number INTEGER,
              Name TEXT,
              Position TEXT,
              Year TEXT,
              TeamId INTEGER
            );
            """;
        await createPlayers.ExecuteNonQueryAsync(cancellationToken);

        var createPlays = db.CreateCommand();
        createPlays.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Plays (
              ID INTEGER PRIMARY KEY,
              PlayNum INTEGER,
              Calls TEXT,
              PlayerId INTEGER,
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
              TeamId INTEGER
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
            INSERT INTO Games(ID,Number, Date, Opponent, Result, Points, OpPoints)
            VALUES ($id,$number, $date, $opponent, $result, $points, $opPoints);
            """;
        cmd.Parameters.AddWithValue("$id", game.Id);
        cmd.Parameters.AddWithValue("$number", game.Number);
        cmd.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("$opponent", game.Opponent);
        cmd.Parameters.AddWithValue("$result", game.Result.ToString());
        cmd.Parameters.AddWithValue("$points", game.Points);
        cmd.Parameters.AddWithValue("$opPoints", game.OpponentPoints);

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
            INSERT INTO Players(ID,Number, Name, Position, Year, TeamId)
            VALUES ($id,$number, $name, $position, $year, $teamId);
            """;
        cmd.Parameters.AddWithValue("$id", player.Id);
        cmd.Parameters.AddWithValue("$number", player.Number);
        cmd.Parameters.AddWithValue("$name", player.Name);
        cmd.Parameters.AddWithValue("$position", player.Position);
        cmd.Parameters.AddWithValue("$year", player.Year);
        cmd.Parameters.AddWithValue("$teamId", player.TeamId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }


    public async Task<int> AddPlayAsync(Play play, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Plays(ID,PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId)
            VALUES ($id,$playNum, $calls, $PlayerId, $tech, $purs, $mtp, $type, $stat1, $stat2, $loaf, $comment, $position, $GameId, $TeamId);
            """;
        cmd.Parameters.AddWithValue("$id", play.Id);
        cmd.Parameters.AddWithValue("$playNum", play.PlayNum);
        cmd.Parameters.AddWithValue("$calls", play.Calls);
        cmd.Parameters.AddWithValue("$PlayerId", play.PlayerId);
        cmd.Parameters.AddWithValue("$tech", play.Tech);
        cmd.Parameters.AddWithValue("$purs", play.Purs);
        cmd.Parameters.AddWithValue("$mtp", play.Mtp);
        cmd.Parameters.AddWithValue("$type", play.Type);
        cmd.Parameters.AddWithValue("$stat1", play.Stat1);
        cmd.Parameters.AddWithValue("$stat2", play.Stat2);
        cmd.Parameters.AddWithValue("$loaf", play.Loaf);
        cmd.Parameters.AddWithValue("$comment", play.Comment);
        cmd.Parameters.AddWithValue("$position", play.Position);
        cmd.Parameters.AddWithValue("$GameId", play.GameId);
        cmd.Parameters.AddWithValue("$TeamId", play.TeamId);

        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Play> GetPlayAsync(int playId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE ID = $playId;";
        cmd.Parameters.AddWithValue("$playId", playId);
        Play? result = null;

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var position = reader.GetString(12);
            var GameId = reader.GetInt32(13);
            var teamId = reader.GetInt32(14);

            result = new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, teamId);
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetPlaysAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays;";
        var result = new List<Play>();

        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var position = reader.GetString(12);
            var GameId = reader.GetInt32(13);
            var teamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, teamId));
        }

        return result;

    }

    public async Task<IReadOnlyList<Game>> GetGamesAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Date, Opponent, Result, Points, OpPoints FROM Games;";

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

            result.Add(new Game(id, number, date, opponent, resultChar, points, opPoints));
        }

        return result;
    }

    public async Task<Game> GetGameAsync(int gameId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Date, Opponent, Result, Points, OpPoints FROM Games WHERE ID = $gameId;";
        cmd.Parameters.AddWithValue("$gameId", gameId);
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

            result = new Game(id, number, date, opponent, resultChar, points, opPoints);
        }

        return result;
    }

    public async Task<IReadOnlyList<Player>> GetPlayersAsync(CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId FROM Players;";

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

            result.Add(new Player(id, number, name, position, year, teamId));
        }

        return result;
    }


    
    public async Task<Player> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId FROM Players WHERE ID = $playerId;";
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

            result = new Player(id, number, name, position, year, teamId);
        }

        return result;
    }
    public async Task<IReadOnlyList<Player>> GetTeamPlayersAsync(int teamId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, Number, Name, Position, Year, TeamId FROM Players WHERE TeamId = $teamId;";
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

            result.Add(new Player(id, number, name, position, year, resultTeamId));
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetTeamPlaysAsync(int teamId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE TeamId = $teamId;";
        cmd.Parameters.AddWithValue("$teamId", teamId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var position = reader.GetString(12);
            var GameId = reader.GetInt32(13);
            var resultTeamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, resultTeamId));
        }

        return result;
    }
    
    public async Task<IReadOnlyList<Play>> GetTeamGamePlaysAsync(int teamId, int gameId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE TeamId = $teamId AND GameId = $gameId;";
        cmd.Parameters.AddWithValue("$teamId", teamId);
        cmd.Parameters.AddWithValue("$gameId", gameId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var position = reader.GetString(12);
            var resultGameId = reader.GetInt32(13);
            var resultTeamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, resultGameId, resultTeamId));
        }

        return result;
    }
    public async Task<IReadOnlyList<Play>> GetPlayerGamePlaysAsync(int playerId, int gameId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE PlayerId = $playerId AND GameId = $gameId;";
        cmd.Parameters.AddWithValue("$playerId", playerId);
        cmd.Parameters.AddWithValue("$gameId", gameId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var position = reader.GetString(12);
            var resultGameId = reader.GetInt32(13);
            var resultTeamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, resultGameId, resultTeamId));
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetPlayerPlaysAsync(int playerId, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE PlayerId = $playerId;";
        cmd.Parameters.AddWithValue("$playerId", playerId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var position = reader.GetString(12);
            var GameId = reader.GetInt32(13);
            var teamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, position, GameId, teamId));
        }

        return result;
    }
    public async Task<IReadOnlyList<Play>> GetPositionGamePlaysAsync(string position, int gameId, CancellationToken cancellationToken = default)
    {

        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE Position = $position AND GameId = $gameId;";
        cmd.Parameters.AddWithValue("$position", position);
        cmd.Parameters.AddWithValue("$gameId", gameId);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var resultPosition = reader.GetString(12);
            var resultGameId = reader.GetInt32(13);
            var resultTeamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, resultPosition, resultGameId, resultTeamId));
        }

        return result;
    }

    public async Task<IReadOnlyList<Play>> GetPositionPlaysAsync(string position, CancellationToken cancellationToken = default)
    {
        var dbPath = _dbPathProvider.GetFootballDbPath();
        await using var db = new SqliteConnection($"Filename={dbPath}");
        await db.OpenAsync(cancellationToken);

        var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT ID, PlayNum, Calls, PlayerId, Tech, Purs, Mtp, Type, Stat1, Stat2, Loaf, Comment, Position, GameId, TeamId FROM Plays WHERE Position = $position;";
        cmd.Parameters.AddWithValue("$position", position);

        var result = new List<Play>();
        await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            var id = reader.GetInt32(0);
            var playNum = reader.GetInt32(1);
            var calls = reader.GetString(2);
            var PlayerId = reader.GetInt32(3);
            var tech = reader.GetInt32(4);
            var purs = reader.GetInt32(5);
            var mtp = reader.GetInt32(6);
            var type = reader.GetChar(7);
            var stat1 = reader.GetString(8);
            var stat2 = reader.GetString(9);
            var loaf = reader.GetBoolean(10);
            var comment = reader.GetString(11);
            var resultPosition = reader.GetString(12);
            var GameId = reader.GetInt32(13);
            var teamId = reader.GetInt32(14);

            result.Add(new Play(id, playNum, calls, PlayerId, tech, purs, mtp, type, stat1, stat2, loaf, comment, resultPosition, GameId, teamId));
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

