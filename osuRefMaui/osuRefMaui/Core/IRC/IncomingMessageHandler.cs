using IrcDotNet;

namespace osuRefMaui.Core.IRC
{
	public class IncomingMessageHandler
	{
		private readonly ChatQueue _chatQueue;

		// ReSharper disable once SuggestBaseTypeForParameterInConstructor
		public IncomingMessageHandler(StandardIrcClient client, ChatQueue chatQueue)
		{
			_chatQueue = chatQueue;

			client.RawMessageReceived += OnMessageReceived;
		}

		private void OnMessageReceived(object sender, IrcRawMessageEventArgs e)
		{
			var message = new ChatMessage(e.Message);
			_chatQueue.Enqueue(message);
		}
	}
}