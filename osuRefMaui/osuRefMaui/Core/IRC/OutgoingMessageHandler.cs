#nullable enable
using IrcDotNet;
using osuRefMaui.Core.IRC.Interfaces;
using osuRefMaui.Core.IRC.LoginInformation;
using System.Text;

namespace osuRefMaui.Core.IRC;

public class OutgoingMessageHandler
{
	private readonly IrcClient _client;
	private readonly Credentials _credentials;
	private readonly TabHandler _tabHandler;

	public OutgoingMessageHandler(StandardIrcClient client, TabHandler tabHandler, Credentials credentials)
	{
		_client = client;
		_tabHandler = tabHandler;
		_credentials = credentials;
	}

	/// <summary>
	///  Sends the IChatMessage to the IRC server.
	/// </summary>
	/// <param name="chatMessage"></param>
	public void DispatchToIrc(IChatMessage chatMessage)
	{
		var sb = new StringBuilder();
		sb.Append(chatMessage.Command.ToString().ToUpper())
		  .Append(" ")
		  .Append(chatMessage.Channel);

		_client.SendRawMessage(sb.ToString());
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
				IrcCommand.PrivateMessage => new ChatMessage(new IrcClient.IrcMessage(_client, $"{_credentials.Username}!cho@ppy.sh",
					"PRIVMSG",
					new List<string>(15) { commandHandler.Args[0], string.Join(" ", commandHandler.Args[1..]) })),
				_ => throw new NullReferenceException("Command may not be null here.")
			};
		}

		throw new InvalidOperationException();
	}
}