using IrcDotNet;
using Microsoft.Extensions.Logging;
using osuRefMaui.Core;
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
        builder.Services.AddLogging(configure =>
        {
        });

        builder.Services.AddSingleton<MainPage>();

        builder.Services.AddSingleton<StandardIrcClient>();

		builder.Services.AddSingleton<ChatQueue>();
        builder.Services.AddSingleton<Credentials>(_ => CredentialsHandler.DeserializeCredentials());


		// Handlers
        builder.Services.AddSingleton<ConnectionHandler>();

		return builder.Build();
	}
}
