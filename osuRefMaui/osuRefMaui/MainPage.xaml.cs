using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.LoginInformation;

// ReSharper disable RedundantExtendsListEntry
namespace osuRefMaui;

public partial class MainPage : ContentPage
{
	private readonly ConnectionHandler _connectionHandler;
	private readonly Credentials _credentials;
	private readonly MissionControl _missionControl;

	// ReSharper disable once UnusedParameter.Local
	// IncomingMessageHandler constructed here, but may not be used.
	public MainPage(MissionControl missionControl,
		Credentials credentials, ConnectionHandler connectionHandler, IncomingMessageHandler incomingMessageHandler)
	{
		_missionControl = missionControl;
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

	private async void OnLoaded(object sender, EventArgs e)
	{
		if (_credentials.RememberMe)
		{
			// Should be populated
			Username.Text = _credentials.Username;
			Password.Text = _credentials.IrcPassword;
			RememberMe.IsChecked = _credentials.RememberMe;
			
			// Automatically login
			OnLoginClicked(null, null);
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

		Login.IsEnabled = false;
		if (await _connectionHandler.Connect())
		{
			// Push to next page once connected
			await Window.Navigation.PushModalAsync(_missionControl);
		}
		else
		{
			await DisplayAlert("Invalid Login", "Failed to connect to osu!Bancho. " +
			                                    "Please check your internet connection and credentials.", "Okay");
		}

		Login.IsEnabled = true;
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