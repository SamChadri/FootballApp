namespace Football.MauiApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(Views.AddGamePage), typeof(Views.AddGamePage));
	}
}
