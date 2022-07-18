using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.LoginInformation;

namespace osuRefMaui;

public partial class MainPage : ContentPage
{
    private readonly Credentials _credentials;
    private readonly ConnectionHandler _connectionHandler;

    // IncomingMessageHandler constructed here but may not be used.
    public MainPage(ILogger<MainPage> logger, Credentials credentials, ConnectionHandler connectionHandler,
        IncomingMessageHandler incomingMessageHandler)
    {
        _credentials = credentials;
        _connectionHandler = connectionHandler;

        // Events and subscriptions
        Loaded += OnLoaded;
        Connectivity.ConnectivityChanged += async (_, e) =>
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
            {
                await DisplayAlert("Connectivity changed", "Internet access restored.", "Okay.");
            }
            else
            {
                await DisplayAlert("Connectivity changed", "Internet connection lost. " +
                                                           "Attempting to reconnect...", "Okay");
            }
        };

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

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Store credentials and login
        if (string.IsNullOrWhiteSpace(Username.Text))
        {
            await DisplayAlert("Invalid Login", "Invalid username entry", "Okay");
            return;
        }
        if (string.IsNullOrWhiteSpace(Password.Text))
        {
            await DisplayAlert("Invalid Login", "Invalid password entry", "Okay");
        }

        _credentials.Username = Username.Text;
        _credentials.IrcPassword = Password.Text;
        _credentials.RememberMe = RememberMe.IsChecked;

        if (RememberMe.IsChecked)
        {
            CredentialsHandler.SerializeCredentials(_credentials);
        }

        if (_connectionHandler.Connect())
        {
            // Push navigation
        }
    }

    private async void OnIRCPassButtonClicked(object sender, EventArgs e)
    {
        const string url = "https://osu.ppy.sh/p/irc";

        try
        {
            await Browser.Default.OpenAsync(url);
        }
        catch (Exception exception)
        {
            await DisplayAlert("Browser Error", "An error occured when opening the browser." +
                                                $"Do you have one installed? (Error: {exception.Message})", "Close");
        }
    }
}