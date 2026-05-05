namespace Football.MauiApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.AddGamePage),     typeof(Views.AddGamePage));
        Routing.RegisterRoute(nameof(Views.DataPage),        typeof(Views.DataPage));
        Routing.RegisterRoute(nameof(Views.TeamGroupPage),   typeof(Views.TeamGroupPage));
        Routing.RegisterRoute(nameof(Views.PositionPage),    typeof(Views.PositionPage));
        Routing.RegisterRoute(nameof(Views.PlayerStatsPage), typeof(Views.PlayerStatsPage));
    }
}
