using IrcDotNet;
using osuRefMaui.Core.IRC.Filtering;

namespace osuRefMaui.Core.IRC
{
	public class IncomingMessageHandler
	{
		private readonly ChatQueue _chatQueue;
		private readonly IrcFilter _filter;

		// ReSharper disable once SuggestBaseTypeForParameterInConstructor
		public IncomingMessageHandler(StandardIrcClient client, ChatQueue chatQueue, IrcFilter filter)
		{
			_chatQueue = chatQueue;
			_filter = filter;

			client.RawMessageReceived += OnMessageReceived;
		}

		private void OnMessageReceived(object sender, IrcRawMessageEventArgs e)
		{
			var message = new ChatMessage(e.Message);

			var filterProcessor = new IrcFilterProcessor(_filter, message);
			if (!filterProcessor.IsFiltered())
			{
				_chatQueue.Enqueue(message);
			}
		}
	}
}