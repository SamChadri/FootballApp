using System.Collections.Generic;
using System.Linq;

namespace Football.Core;

public sealed record Season(
    int Id,
    int Year
);

public abstract record Squad
{
    public virtual string Name { get; set; } = string.Empty;
    public virtual List<string> positionNames { get; set; } = new();
}

public sealed record Offense : Squad
{
    public override List<string> positionNames { get; set; } = new() { "WR","TE","T","G","C","FB","HB","QB" };
    public override string Name { get; set; } = "Offense";
}

public sealed record Defense : Squad
{
    public override List<string> positionNames { get; set; } = new() { "DT","DE","NT","MLB","OLB","CB","FS","SS" };
    public override string Name { get; set; } = "Defense";
}

public sealed record SpecialTeams : Squad
{
    public override List<string> positionNames { get; set; } = new() { "K","P","LS","H","KR","PR","G" };
    public override string Name { get; set; } = "Special Teams";
}

// Removed duplicate obsolete squad definitions

public sealed record Game(
    int Id,
    int Number,
    DateTime Date,
    string Opponent,
    char Result,
    int Points,
    int OpponentPoints,
    string Location,
    int SeasonId
);

public sealed record Player(
    int Id,
    int Number,
    string Name,
    string Position,
    string Year,
    int TeamId
)
{
    public int SeasonId { get; set; }
}

public sealed record Play(
    int Id,
    int PlayNum,
    string Calls,
    int PlayerId,
    int NumPenalties,
    string PenaltyNames,
    int PlayYards,
    int Tackles,
    int Tech,
    int Purs,
    int Mtp,
    char Type,
    string Stat1,
    string Stat2,
    bool Loaf,
    string Comment,
    string Position,
    int GameId,
    int TeamId,
    int SeasonId
);

// ── Data Dashboard Models ──────────────────────────────────────────────────

public class SeasonGroup
{
    public SeasonGroup(int seasonId, List<Game> seasonGames) => (SeasonId, SeasonGames) = (seasonId, seasonGames);
    public int SeasonId { get; set; }
    public List<Game> SeasonGames { get; set; } = [];
    public int Wins { get; set; } 
    public int Losses { get; set; }
    public int Ties { get; set; }

    public int PointsFor { get; set; }
    public int PointsAgainst { get; set; }
    public int HomeWins { get; set; }
    public int HomeLosses { get; set; }
    public int HomeTies { get; set; }
    public int AwayWins { get; set; }
    public int AwayLosses { get; set; }
    public int AwayTies { get; set; }
    
    public int CalculateStats()
    {
        Wins = SeasonGames.Count(g => g.Result == 'W');
        Losses = SeasonGames.Count(g => g.Result == 'L');
        Ties = SeasonGames.Count(g => g.Result == 'T');
        PointsFor = SeasonGames.Sum(g => g.Points);
        PointsAgainst = SeasonGames.Sum(g => g.OpponentPoints);
        HomeWins = SeasonGames.Count(g => g.Result == 'W' && g.Location == 'H');
        HomeLosses = SeasonGames.Count(g => g.Result == 'L' && g.Location == 'H');
        HomeTies = SeasonGames.Count(g => g.Result == 'T' && g.Location == 'H');
        AwayWins = SeasonGames.Count(g => g.Result == 'W' && g.Location == 'A');
        AwayLosses = SeasonGames.Count(g => g.Result == 'L' && g.Location == 'A');
        AwayTies = SeasonGames.Count(g => g.Result == 'T' && g.Location == 'A');
        return 0;
    }
}

public class SquadGroup
{
    public SquadGroup(int seasonId, string name, Squad squad)
    {
        SeasonId = seasonId;
        Name = squad.Name;
        Squad = squad;
    }
    public int SeasonId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public Squad Squad { get; set; } = new();
    public List<PositionGroup> Positions { get; set; } = [];
    public sealed record Stats
    {
        public int Tackles {get; set;} = 0;
        public int Penalties {get; set;} = 0;
        public int PlayYards {get; set;} = 0;
    }
    public List<Stats> PositionStats { get; set; } = [];
    public int CalculateStats()
    {
        foreach (var p in Positions)
        {
            var stat = new Stats();
            stat.Tackles = p.PositionPlays.Sum(x => x.Tackles);
            stat.Penalties = p.PositionPlays.Sum(x => x.NumPenalties);
            stat.PlayYards = p.PositionPlays.Sum(x => x.PlayYards);
            PositionStats.Add(stat);
            
        }
        return 0;
    }
}

public class PositionGroup
{
    public string Name { get; set; } = string.Empty;
    public List<Player> PositionPlayers { get; set; } = [];
    public List<Game> PositionGames { get; set; } = [];
    public List<Play> PositionPlays { get; set; } = [];
    public int GamesPlayed { get; set; }
    public int SnapCount { get; set; }
    public double SnapPercentage { get; set; }

    // Constructors
    public PositionGroup() {}
    public PositionGroup(string name, List<Game> games, List<Player> players, List<Play> plays)
    {
        Name = name;
        PositionGames = games;
        PositionPlayers = players;
        PositionPlays = plays;
    }

    public int CalculateStats()
    {
        SnapCount = PositionPlays.Count;
        GamesPlayed = PositionGames.Count;
        SnapPercentage = GamesPlayed == 0 ? 0 : (double)SnapCount * 100 / GamesPlayed;
        return 0;
    }

}

public class StatLine
{
    public string PlayerNumber { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
    public int Tackles { get; set; }
    public int TechTackles { get; set; }
    public int TechTotal { get; set; }
    public int Sacks { get; set; }
    public int Assists { get; set; }
}
