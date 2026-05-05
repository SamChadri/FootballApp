namespace Football.Core;

public sealed record Game(
    int Id,
    int Number,
    DateTime Date,
    string Opponent,
    char Result,
    int Points,
    int OpponentPoints
);

public sealed record Player(
    int Id,
    int Number,
    string Name,
    string Position,
    string Year,
    int TeamId
);

public sealed record Play(
    int Id,
    int PlayNum,
    string Calls,
    int PlayerId,
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
    int TeamId
);

// ── Data Dashboard Models ──────────────────────────────────────────────────

public class Season
{
    public int Year { get; set; }
    public List<Game> Games { get; set; } = [];
}

public class TeamGroup
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public List<PositionGroup> Positions { get; set; } = [];
}

public class PositionGroup
{
    public string Name { get; set; } = string.Empty;
    public List<Player> Players { get; set; } = [];
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
