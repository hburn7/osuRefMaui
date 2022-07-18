using IrcDotNet;
using Microsoft.Extensions.Logging;

namespace osuRefMaui.Core.IRC
{
    public class IncomingMessageHandler
    {
        private readonly StandardIrcClient _client;
        private readonly ILogger<IncomingMessageHandler> _logger;
        private readonly ChatQueue _chatQueue;

        public IncomingMessageHandler(ILogger<IncomingMessageHandler> logger,
            StandardIrcClient client, ChatQueue chatQueue)
        {
            _logger = logger;
            _chatQueue = chatQueue;
            _client = client;

            _client.RawMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            var message = new ChatMessage(e.Message);
            _chatQueue.Enqueue(message);
        }
    }
}
