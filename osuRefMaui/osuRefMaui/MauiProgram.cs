﻿using IrcDotNet;
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
		builder.UseMauiApp<App>();
		
		// Configure services
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddDebug();
        });

		// Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MissionControl>();

        builder.Services.AddSingleton<StandardIrcClient>();

		builder.Services.AddSingleton<ChatQueue>();
        builder.Services.AddSingleton<Credentials>(_ => CredentialsHandler.DeserializeCredentials());

		// Handlers
        builder.Services.AddSingleton<ConnectionHandler>();
        builder.Services.AddSingleton<IncomingMessageHandler>();
        builder.Services.AddSingleton<TabHandler>();

		return builder.Build();
	}
}
