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

