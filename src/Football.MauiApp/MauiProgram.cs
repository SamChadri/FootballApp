using Microsoft.Extensions.Logging;
using Football.Data.Sqlite;
using Football.MauiApp.Infrastructure;

using Football.MauiApp.Views;
using Football.MauiApp.ViewModels;

namespace Football.MauiApp;

public static class MauiProgram
{
	public static Microsoft.Maui.Hosting.MauiApp CreateMauiApp()
	{
		var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Data Layer
        builder.Services.AddSingleton<IDbPathProvider, MauiDbPathProvider>();
        builder.Services.AddSingleton<IFootballRepository, SqliteFootballRepository>();

        // ViewModels & Pages
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<GamesListPage>();
        builder.Services.AddTransient<GamesListViewModel>();
        builder.Services.AddTransient<AddGamePage>();
        builder.Services.AddTransient<AddGameViewModel>();
        builder.Services.AddTransient<CreateHubPage>();
        builder.Services.AddTransient<AddPlayerPage>();
        builder.Services.AddTransient<AddPlayerViewModel>();
        builder.Services.AddTransient<AddPlayPage>();
        builder.Services.AddTransient<AddPlayViewModel>();

        // Data drill-down pages
        builder.Services.AddTransient<DataPage>();
        builder.Services.AddTransient<DataViewModel>();
        builder.Services.AddTransient<TeamGroupPage>();
        builder.Services.AddTransient<TeamGroupViewModel>();
        builder.Services.AddTransient<PositionPage>();
        builder.Services.AddTransient<PositionViewModel>();
        builder.Services.AddTransient<PlayerStatsPage>();
        builder.Services.AddTransient<PlayerStatsViewModel>();
        builder.Services.AddTransient<RosterPage>();
        builder.Services.AddTransient<RosterViewModel>();

        builder.Services.AddTransient<PlayerProfilePage>();
builder.Services.AddTransient<PlayerProfileViewModel>();

		return builder.Build();
	}
}
