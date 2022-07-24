#nullable enable
using IrcDotNet;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.IRC
{
	/// <summary>
	///  Data wrapper for IrcClient.IrcMessage
	/// </summary>
	public class ChatMessage : IChatMessage
	{
		public ChatMessage(IrcClient.IrcMessage message)
		{
			SourceMessage = message;

			TimeStamp = DateTimeOffset.Now;
			Command = IdentifyCommand();
			Channel = IdentifyChannel();
			Content = IdentifyContent();
			Sender = IdentifySender();
			StatusCode = IdentifyStatusCode();
		}

		public int? StatusCode { get; }
		public IrcClient.IrcMessage SourceMessage { get; }
		public string? SourceName => SourceMessage.Source?.Name;
		public DateTimeOffset TimeStamp { get; }
		public IrcCommand Command { get; }
		public string Channel { get; }
		public string Content { get; }
		public string? Sender { get; }
		public bool IsFromPublicChannel => Sender?.StartsWith("#") ?? false;
		public bool IsStatusCode(int statusCode) => Command == IrcCommand.Other && StatusCode == statusCode;
		public string ToRawIrcString() => $"{Command.ToString().ToUpper()} {string.Join(" ", SourceMessage.Parameters)}".Trim();
		public override string ToString() => $"ChatMessage({TimeStamp}, {Command}, {Sender}: {Channel}, {Content})";

		private IrcCommand IdentifyCommand() => SourceMessage.Command.ToLower() switch
		{
			"logout" => IrcCommand.Quit,
			"quit" => IrcCommand.Quit,
			"join" => IrcCommand.Join,
			"privmsg" => IrcCommand.PrivMsg,
			"msg" => IrcCommand.PrivMsg,
			"query" => IrcCommand.Query,
			"chat" => IrcCommand.Query,
			"part" => IrcCommand.Part,
			"leave" => IrcCommand.Part,
			"ping" => IrcCommand.Ping,
			"me" => IrcCommand.Me,
			"mode" => IrcCommand.Mode,
			"replaced" => IrcCommand.Replaced,
			"" => IrcCommand.Empty,
			_ when string.IsNullOrWhiteSpace(SourceMessage.Command) => IrcCommand.Null,
			_ when !SourceMessage.Command.StartsWith("/") => IrcCommand.PrivMsg,
			_ => IrcCommand.Other
		};

		private string IdentifyChannel() => SourceMessage.Parameters[0];

		private string IdentifyContent()
		{
			if (Command == IrcCommand.PrivMsg && (!SourceMessage.Source?.Name?.Equals("cho.ppy.sh") ?? false))
			{
				return SourceMessage.Parameters[1];
			}

			return string.Join(" ", SourceMessage.Parameters.ToArray()[1..]).Trim();
		}

		private string? IdentifySender()
		{
			if (SourceMessage.Source?.Name?.Equals("cho.ppy.sh") ?? false)
			{
				return TabHandler.DefaultTabName;
			}

			return Command == IrcCommand.PrivMsg ? SourceMessage.Source?.Name : TabHandler.DefaultTabName;
		}

		private int? IdentifyStatusCode()
		{
			if (int.TryParse(SourceMessage.Command, out int code))
			{
				return code;
			}

			return null;
		}
	}
}