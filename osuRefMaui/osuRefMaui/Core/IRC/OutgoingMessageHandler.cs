#nullable enable
using IrcDotNet;
using Microsoft.Extensions.Logging;
using osuRefMaui.Core.IRC.Interfaces;
using osuRefMaui.Core.IRC.LoginInformation;

namespace osuRefMaui.Core.IRC;

public class OutgoingMessageHandler
{
	private readonly ChatQueue _chatQueue;
	private readonly IrcClient _client;
	private readonly Credentials _credentials;
	private readonly ILogger<OutgoingMessageHandler> _logger;
	private readonly TabHandler _tabHandler;

	public OutgoingMessageHandler(ILogger<OutgoingMessageHandler> logger,
		StandardIrcClient client, TabHandler tabHandler, Credentials credentials,
		ChatQueue chatQueue)
	{
		_logger = logger;
		_client = client;
		_tabHandler = tabHandler;
		_credentials = credentials;
		_chatQueue = chatQueue;
	}

	public IChatMessage CreateChatMessage(CommandHandler commandHandler)
	{
		string channel = _tabHandler.ActiveTab;

		if (commandHandler.ValidArgumentCount)
		{
			return commandHandler.Command switch
			{
				IrcCommand.Quit => new ChatMessage(new IrcClient.IrcMessage(_client, $"{_credentials.Username}!cho@ppy.sh", "QUIT",
					new List<string>(15) { "quit" })),
				IrcCommand.Join => new ChatMessage(new IrcClient.IrcMessage(_client, $"{_credentials.Username}!cho@ppy.sh", "JOIN",
					new List<string>(15) { commandHandler.Args[0] })),
				IrcCommand.Part => new ChatMessage(new IrcClient.IrcMessage(_client, $"{_credentials.Username}!cho@ppy.sh", "PART",
					new List<string>(15) { channel })),
				IrcCommand.PrivMsg => new ChatMessage(new IrcClient.IrcMessage(_client, $"{_credentials.Username}!cho@ppy.sh",
					"PRIVMSG",
					new List<string>(15) { commandHandler.Args[0], string.Join(" ", commandHandler.Args[1..]) })),
				_ => throw new NullReferenceException("Command may not be null here.")
			};
		}

		throw new InvalidOperationException();
	}

	/// <summary>
	///  Deploys the message to the specified chat channel encoded
	///  as a private message to that chat channel.
	/// </summary>
	/// <param name="text">The raw message to send. Should not be a slash command.</param>
	/// <param name="channel">The chat channel to deliver to. Defaults to active tab.</param>
	/// <param name="dispatch">Whether to actually dispatch the content to IRC. If false, there is just a display.</param>
	public void Send(string text, string channel = null, bool dispatch = true)
	{
		channel ??= _tabHandler.ActiveTab;
		if (text.StartsWith("/"))
		{
			throw new InvalidOperationException("Commands should not be processed here.");
		}

		var message = new ChatMessage(new IrcClient.IrcMessage(_client, $"{_credentials.Username}!cho@ppy.sh",
			"PRIVMSG", new List<string>(15) { channel, text }));

		Send(message, dispatch);
	}

	/// <summary>
	///  Sends the chat message to IRC (if specified) and enqueues it for display
	/// </summary>
	/// <param name="chatMessage"></param>
	/// <param name="dispatch">Whether to actually dispatch the content to IRC. If false, there is just a display.</param>
	public void Send(IChatMessage chatMessage, bool dispatch = true)
	{
		_logger.LogInformation($"Sending & enqueueing message '{chatMessage.ToRawIrcString()}'");
		_chatQueue.Enqueue(chatMessage);

		if (dispatch)
		{
			_client.SendRawMessage(chatMessage.ToRawIrcString());
		}
	}
}