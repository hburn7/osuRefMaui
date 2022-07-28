using IrcDotNet;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.IRC;

public class SystemMessage : IChatMessage
{
	private const string SystemName = "[System]";

	public SystemMessage(string channel, string content)
	{
		TimeStamp = DateTimeOffset.Now;
		Command = IrcCommand.Null;
		SourceMessage = new IrcClient.IrcMessage();
		SourceName = SystemName;
		Channel = channel;
		Content = content;
		Sender = channel;
		IsFromPublicChannel = false;
	}

	public DateTimeOffset TimeStamp { get; }
	public IrcCommand Command { get; }
	public IrcClient.IrcMessage SourceMessage { get; }
	public string SourceName { get; }
	public string Channel { get; }
	public string Content { get; }
	public string Sender { get; }
	public bool IsFromPublicChannel { get; }
	public bool IsStatusCode(int statusCode) => false;
	public string ToRawIrcString() => null;
	public override string ToString() => $"{Sender} {{{Channel}}}: {Content}";
}