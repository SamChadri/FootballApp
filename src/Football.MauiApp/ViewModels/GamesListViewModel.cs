using System.Collections.ObjectModel;
using Football.Core;
using Football.Data.Sqlite;

namespace Football.MauiApp.ViewModels;

public class GamesListViewModel
{
    private readonly IFootballRepository _repository;

    public ObservableCollection<Game> Games { get; } = new();

    public GamesListViewModel(IFootballRepository repository)
    {
        _repository = repository;
    }

    public async Task LoadGamesAsync()
    {
        var gamesList = await _repository.GetGamesAsync();
        Games.Clear();
        foreach (var game in gamesList)
        {
            Games.Add(game);
        }
    }
}
