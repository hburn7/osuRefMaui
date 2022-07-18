using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IrcDotNet;

namespace osuRefMaui.Core.IRC.Interfaces
{
    /// <summary>
    /// Data wrapper for IrcClient.IrcMessage
    /// </summary>
    public interface IChatMessage
    {
        public DateTimeOffset TimeStamp { get; }
        public IrcCommand Command { get; }
        public IrcClient.IrcMessage Source { get; }

        /// <summary>
        /// The channel the message is sent through. This is either an
        /// irc channel or a user.
        /// </summary>
        public string Channel { get; }
        /// <summary>
        /// The content of the message, if any. This is typically
        /// only present with private messages.
        /// </summary>
        public string Content { get; }
        /// <summary>
        /// The name of the user who sent the message
        /// </summary>
        public string Sender { get; }

        /// <summary>
        /// Whether the command is of the given status code
        /// </summary>
        /// <param name="statusCode">Server provided code</param>
        /// <returns></returns>
        public bool IsStatusCode(int statusCode);
    }
}
