using Football.MauiApp.ViewModels;

namespace Football.MauiApp.Views;

public partial class PlayerProfilePage : ContentPage
{
	public PlayerProfilePage(PlayerProfileViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
