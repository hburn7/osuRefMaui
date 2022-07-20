using Microsoft.Extensions.Logging;
using osuRefMaui.Core.IRC.Interfaces;
using System.Collections.Concurrent;

#pragma warning disable CS0108,

namespace osuRefMaui.Core.IRC
{
	public class ChatQueue : ConcurrentQueue<IChatMessage>
	{
		private readonly ILogger<ChatQueue> _logger;
		public ChatQueue(ILogger<ChatQueue> logger) { _logger = logger; }
		public event Action<IChatMessage> OnEnqueue;
		public event Action<IChatMessage> OnDequeue;

		public void Enqueue(IChatMessage message)
		{
			base.Enqueue(message);
			OnEnqueue?.Invoke(message);

			_logger.LogDebug($"Message enqueued: {message}.");
		}

		public bool TryDequeue(out IChatMessage chatMessage)
		{
			if (base.TryDequeue(out chatMessage))
			{
				OnDequeue?.Invoke(chatMessage);
				return true;
			}

			return false;
		}
	}
}