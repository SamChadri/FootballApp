using Football.Core;
using Football.Data.Sqlite;

namespace Football.MauiApp.ViewModels;

public class AddGameViewModel
{
    private readonly IFootballRepository _repository;

    public string Opponent { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public int Points { get; set; }
    public int OpponentPoints { get; set; }
    public char Result { get; set; } = 'W';

    public AddGameViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task SaveGameAsync()
    {
        var game = new Game(
            Id: -1, // Database will generate or handle
            Number: 1, // Placeholder
            Date: Date,
            Opponent: Opponent,
            Result: Result,
            Points: Points,
            OpponentPoints: OpponentPoints
        );

        await _repository.AddGameAsync(game);
    }
}
