#nullable enable

using IrcDotNet;

namespace osuRefMaui.Core.IRC.Interfaces
{
	/// <summary>
	///  Data wrapper for IrcClient.IrcMessage
	/// </summary>
	public interface IChatMessage
	{
		public DateTimeOffset TimeStamp { get; }
		public IrcCommand Command { get; }
		/// <summary>
		/// The source IRC message that the data is derived from.
		/// </summary>
		public IrcClient.IrcMessage SourceMessage { get; }
		/// <summary>
		///  The source name. This is the name of the user
		///  who took an action (i.e. the name of someone who logged out).
		/// </summary>
		public string? SourceName { get; }
		/// <summary>
		///  The channel the message is sent through. This is either an
		///  irc channel or a user.
		/// </summary>
		public string Channel { get; }
		/// <summary>
		///  The content of the message, if any. This is typically
		///  only present with private messages.
		/// </summary>
		public string Content { get; }
		/// <summary>
		///  The name of the user who sent the message
		/// </summary>
		public string? Sender { get; }
		/// <summary>
		///  Returns whether the chat message is from
		///  a public channel, such as #osu. If false,
		///  the message is an incoming direct message.
		/// </summary>
		public bool IsFromPublicChannel { get; }

		/// <summary>
		///  Whether the command is of the given status code
		/// </summary>
		/// <param name="statusCode">Server provided code</param>
		/// <returns></returns>
		public bool IsStatusCode(int statusCode);
	}
}