using Football.Core;

namespace Football.Data.Sqlite;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IFootballRepository repo)
    {
        await repo.InitializeAsync();
        
        // If there are plays, we assume the DB is already seeded or populated.
        var existingPlays = await repo.GetPlaysAsync();
        if (existingPlays.Count > 0)
            return;

        // Create 3 seasons
        var seasons = new[] { 1, 2, 3 };
        var years = new[] { 2021, 2022, 2023 };

        int playerId = 1;
        int gameId = 1;
        int playId = 1;
        var rand = new Random(42);

        for (int i = 0; i < seasons.Length; i++)
        {
            var seasonId = seasons[i];
            var year = years[i];
            
            // Add Games
            var seasonGameIds = new List<int>();
            for (int g = 1; g <= 3; g++)
            {
                var game = new Game(gameId, g, new DateTime(year, 9, g * 7), $"Opponent {g}", 'W', 24, 10, "H", seasonId);
                await repo.AddGameAsync(game);
                seasonGameIds.Add(gameId);
                gameId++;
            }

            // Add Players
            var offensePositions = new[] { "QB", "RB", "WR", "TE", "OL" };
            var defensePositions = new[] { "DL", "LB", "DB", "S" };
            var specialPositions = new[] { "K", "P", "KR" };

            var currentSeasonPlayers = new List<Player>();

            foreach (var pos in offensePositions.Concat(defensePositions).Concat(specialPositions))
            {
                var p = new Player(playerId, playerId, $"Player {playerId}", pos, "JR", 1, seasonId);
                await repo.AddPlayerAsync(p);
                currentSeasonPlayers.Add(p);
                playerId++;
            }

            // Add Plays
            foreach (var gId in seasonGameIds)
            {
                foreach (var p in currentSeasonPlayers)
                {
                    // Random number of plays per player per game
                    int numPlays = rand.Next(1, 15);
                    for (int pl = 0; pl < numPlays; pl++)
                    {
                        var play = new Play(
                            Id: playId,
                            PlayNum: pl + 1,
                            Calls: "BASE",
                            PlayerId: p.Id,
                            NumPenalties: rand.Next(0, 2),
                            PenaltyNames: "",
                            PlayYards: rand.Next(-2, 15),
                            Tackles: rand.Next(0, 3),
                            Tech: 1, Purs: 1, Mtp: 1,
                            Type: 'R',
                            Stat1: "", Stat2: "",
                            Loaf: false,
                            Comment: "",
                            Position: p.Position,
                            GameId: gId,
                            TeamId: 1,
                            SeasonId: seasonId);
                        await repo.AddPlayAsync(play);
                        playId++;
                    }
                }
            }
        }
    }
}
