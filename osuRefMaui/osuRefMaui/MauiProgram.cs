using IrcDotNet;
using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.LoginInformation;

namespace osuRefMaui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Configure services
        builder.Services.AddLogging();

		builder.Services.AddSingleton<StandardIrcClient>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<Credentials>(_ => CredentialsHandler.DeserializeCredentials());

		// Handlers
        builder.Services.AddSingleton<ConnectionHandler>();

		return builder.Build();
	}
}
