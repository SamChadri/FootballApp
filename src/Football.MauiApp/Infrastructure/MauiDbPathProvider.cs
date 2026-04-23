using Football.Data.Sqlite;

namespace Football.MauiApp.Infrastructure;

public class MauiDbPathProvider : IDbPathProvider
{
    public string GetFootballDbPath() => 
        Path.Combine(FileSystem.AppDataDirectory, "Football.db");

    public string GetAccountsDbPath() => 
        Path.Combine(FileSystem.AppDataDirectory, "Accounts.db");
}
