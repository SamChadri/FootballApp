using CommunityToolkit.Maui.Views;

namespace Football.MauiApp.Views;

public partial class SuccessPopup : Popup
{
    public bool AddAnother { get; private set; }

    public SuccessPopup(string title, string message)
    {
        InitializeComponent();

        TitleLabel.Text = title;
        MessageLabel.Text = message;
    }

    private void DoneClicked(object sender, EventArgs e)
    {
        AddAnother = false;
        Close(false);
    }

    private void AddAnotherClicked(object sender, EventArgs e)
    {
        AddAnother = true;
        Close(true);
    }
}