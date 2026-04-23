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

		return builder.Build();
	}
}
