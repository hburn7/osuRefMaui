using IrcDotNet;
using Microsoft.Maui.Controls;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.IRC
{
    /// <summary>
    /// Data wrapper for IrcClient.IrcMessage
    /// </summary>
    public class ChatMessage : IChatMessage
    {
        public IrcClient.IrcMessage Source { get; }

        public ChatMessage(IrcClient.IrcMessage message)
        {
            Source = message;

            TimeStamp = DateTimeOffset.Now;
            Command = IdentifyCommand();
            Channel = IdentifyChannel();
            Content = IdentifyContent();
            Sender = IdentifySender();
            Recipient = IdentifyRecipient();
        }

        public DateTimeOffset TimeStamp { get; }
        public IrcCommand Command { get; }
        public string Channel { get; }
        public string Content { get; }
        public string Sender { get; }
        public string Recipient { get; }

        public override string ToString()
        {
            return $"ChatMessage({TimeStamp}, {Command}, {Sender}: {Recipient} ({Channel}), {Content})";
        }

        public bool IsStatusCode(int statusCode)
        {
            throw new NotImplementedException();
        }

        private IrcCommand IdentifyCommand()
        {
            return Source.Command.ToLower() switch
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
        }
        private string IdentifyChannel()
        {
            // todo: Test
            return Recipient;
        }
        private string IdentifyContent()
        {
            if (Command == IrcCommand.PrivateMessage && (!Source.Source?.Name?.Equals("cho.ppy.sh") ?? false))
            {
                return Source.Parameters[1];
            }

            return string.Join(" ", Source.Parameters.ToArray()[1..]).Trim();
        }
        private string IdentifySender()
        {
            return Source.Source?.Name;
        }
        private string IdentifyRecipient()
        {
            // todo: test
            return Source.Parameters[0];
        }
    }
}

