using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using osuRefMaui.Core.IRC.Interfaces;
#pragma warning disable CS0108,

namespace osuRefMaui.Core
{
    public class ChatQueue : ConcurrentQueue<IChatMessage>
    {
        private readonly ILogger<ChatQueue> _logger;
        public event Action<IChatMessage> OnEnqueue;

        public ChatQueue(ILogger<ChatQueue> logger)
        {
            _logger = logger;
        }

        public void Enqueue(IChatMessage message)
        {
            base.Enqueue(message);
            OnEnqueue?.Invoke(message);

            _logger.LogDebug($"Message enqueued: {message}.");
        }
    }
}
