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
			Source = message;

			TimeStamp = DateTimeOffset.Now;
			Command = IdentifyCommand();
			Channel = IdentifyChannel();
			Content = IdentifyContent();
			Sender = IdentifySender();
			StatusCode = IdentifyStatusCode();
		}

		public int? StatusCode { get; }
		public IrcClient.IrcMessage Source { get; }
		public DateTimeOffset TimeStamp { get; }
		public IrcCommand Command { get; }
		public string Channel { get; }
		public string Content { get; }
		public string Sender { get; }
		public bool IsFromPublicChannel => Sender.StartsWith("#");
		public bool IsStatusCode(int statusCode) => Command == IrcCommand.Other && StatusCode == statusCode;
		public override string ToString() => $"ChatMessage({TimeStamp}, {Command}, {Sender}: {Channel}, {Content})";

		private IrcCommand IdentifyCommand() => Source.Command.ToLower() switch
		{
			"logout" => IrcCommand.Quit,
			"quit" => IrcCommand.Quit,
			"join" => IrcCommand.Join,
			"privmsg" => IrcCommand.PrivateMessage,
			"msg" => IrcCommand.PrivateMessage,
			"query" => IrcCommand.Query,
			"chat" => IrcCommand.Query,
			"part" => IrcCommand.Part,
			"leave" => IrcCommand.Part,
			"ping" => IrcCommand.Ping,
			"me" => IrcCommand.Me,
			"mode" => IrcCommand.Mode,
			"replaced" => IrcCommand.Replaced,
			"" => IrcCommand.Empty,
			_ when string.IsNullOrWhiteSpace(Source.Command) => IrcCommand.Null,
			_ when !Source.Command.StartsWith("/") => IrcCommand.PrivateMessage,
			_ => IrcCommand.Other
		};

		private string IdentifyChannel() => Source.Parameters[0];

		private string IdentifyContent()
		{
			if (Command == IrcCommand.PrivateMessage && (!Source.Source?.Name?.Equals("cho.ppy.sh") ?? false))
			{
				return Source.Parameters[1];
			}

			return string.Join(" ", Source.Parameters.ToArray()[1..]).Trim();
		}

		private string IdentifySender() => Source.Source?.Name;

		private int? IdentifyStatusCode()
		{
			if (int.TryParse(Source.Command, out int code))
			{
				return code;
			}

			return null;
		}
	}
}