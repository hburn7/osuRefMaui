using IrcDotNet;
using Microsoft.Extensions.Logging;
using osuRefMaui.Core;
using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.Filtering;
using osuRefMaui.Core.IRC.LoginInformation;
#if WINDOWS
using WinUIEx;
using Microsoft.Maui.LifecycleEvents;
#endif

namespace osuRefMaui;

public static class MauiProgram
{
	private const int DefaultWidth = 850;
	private const int DefaultHeight = 650;

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder.UseMauiApp<App>();

#if WINDOWS
		// Set size and center on screen
		builder.ConfigureLifecycleEvents(events =>
		{
			events.AddWindows(wndLifeCycleBuilder =>
			{
				wndLifeCycleBuilder.OnWindowCreated(window =>
				{
					//Set size and center on screen using WinUIEx extension method
					window.CenterOnScreen(DefaultWidth, DefaultHeight);
				});
			});
		});
		// MACCATALYST https://github.com/dotnet/maui/discussions/2370#discussioncomment-3232561
#endif

		// Configure services
		builder.Services.AddLogging(loggingBuilder => { loggingBuilder.AddDebug(); });

		// Pages
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MissionControl>();

		builder.Services.AddSingleton<StandardIrcClient>();

		builder.Services.AddSingleton<ChatQueue>();
		builder.Services.AddSingleton(_ => CredentialsHandler.DeserializeCredentials());

		builder.Services.AddSingleton<IrcFilter>();

		// Handlers
		builder.Services.AddSingleton<ConnectionHandler>();
		builder.Services.AddSingleton<IncomingMessageHandler>();
		builder.Services.AddSingleton<OutgoingMessageHandler>();
		builder.Services.AddSingleton<TabHandler>();

		// Other
		builder.Services.AddSingleton<Pathing>();

		return builder.Build();
	}
}