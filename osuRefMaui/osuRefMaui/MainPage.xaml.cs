using osuRefMaui.Core.IRC.LoginInformation;

namespace osuRefMaui;

public partial class MainPage : ContentPage
{
    private readonly Credentials _credentials;

    public MainPage(Credentials credentials)
    {
        _credentials = credentials;
        Loaded += OnLoaded;

		InitializeComponent();
	}

    private void OnLoaded(object sender, EventArgs e)
    {
        if (_credentials.RememberMe)
        {
            // Should be populated
            Username.Text = _credentials.Username;
            Password.Text = _credentials.IrcPassword;
            RememberMe.IsChecked = _credentials.RememberMe;
        }
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {

    }

    private void OnIRCPassButtonClicked(object sender, EventArgs e)
    {

    }
}

