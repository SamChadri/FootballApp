using Football.Core;

namespace Football.Data.Sqlite;

public interface IDbPathProvider
{
    string GetFootballDbPath();
    string GetAccountsDbPath();
}

public interface IFootballRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);

    Task<int> AddGameAsync(Game game, CancellationToken cancellationToken = default);
    Task<int> AddPlayerAsync(Player player, CancellationToken cancellationToken = default);
    Task<int> AddPlayAsync(Play play, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Game>> GetGamesAsync(CancellationToken cancellationToken = default);
    Task<Game> GetGameAsync(int gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetPlayersAsync(CancellationToken cancellationToken = default);
    Task<Play> GetPlayAsync(int playId, CancellationToken cancellationToken = default);
    Task<Player> GetPlayerAsync(int playerId, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<Play>> GetPlaysAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Player>> GetTeamPlayersAsync(int teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Play>> GetTeamPlaysAsync(int teamId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Play>> GetTeamGamePlaysAsync(int teamId, int gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Play>> GetPlayerGamePlaysAsync(int playerId, int gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Play>> GetPlayerPlaysAsync(int playerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Play>> GetPositionGamePlaysAsync(string position, int gameId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Play>> GetPositionPlaysAsync(string position, CancellationToken cancellationToken = default);
    Task<int> DeleteGameAsync(int gameId, CancellationToken cancellationToken = default);
    Task<int> DeletePlayerAsync(int playerId, CancellationToken cancellationToken = default);
    Task<int> DeletePlayAsync(int playId, CancellationToken cancellationToken = default);
    
}

public interface IUserAccountRepository
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

