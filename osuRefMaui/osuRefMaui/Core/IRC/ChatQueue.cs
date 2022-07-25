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
		/// <summary>
		/// Fired whenever a message is enqueued
		/// </summary>
		public event Action<IChatMessage> OnEnqueue;
		/// <summary>
		/// Fired whenever a message is dequeued
		/// </summary>
		public event Action<IChatMessage> OnDequeue;
		/// <summary>
		/// Fired when the queue remains empty for 500ms after a dequeue.
		/// </summary>
		public event Action OnPersistentEmpty;

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
				
				if (IsEmpty)
				{
					// If the queue is empty and remains empty after 500ms, fire the OnPersistentEmpty event
					Task.Run(async () =>
					{
						await Task.Delay(500);
						if (IsEmpty)
						{
							OnPersistentEmpty?.Invoke();
						}
					});
				}
				
				return true;
			}

			return false;
		}
	}
}