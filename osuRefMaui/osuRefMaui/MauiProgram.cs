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

        builder.Services.AddLogging();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<Credentials>(_ => CredentialsHandler.DeserializeCredentials());

		return builder.Build();
	}
}
