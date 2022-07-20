using Microsoft.Extensions.Logging;
using osuRefMaui.Core.Derivatives.Labeling;
using osuRefMaui.Core.IRC.Interfaces;
using osuRefMaui.Core.IRC.LoginInformation;
using System.Collections.Concurrent;

namespace osuRefMaui.Core.IRC
{
	public class TabHandler
	{
		public const string DefaultTabName = "osu!Bancho";
		private readonly Credentials _credentials;
		private readonly ILogger<TabHandler> _logger;
		private readonly ConcurrentDictionary<string, VerticalStackLayout> _tabChatStacks;

		public TabHandler(ILogger<TabHandler> logger, Credentials credentials)
		{
			_logger = logger;
			_credentials = credentials;
			_tabChatStacks = new ConcurrentDictionary<string, VerticalStackLayout>(StringComparer.OrdinalIgnoreCase);
		}

		/// The main purpose of these Actions is to
		/// give a notification to the UI thread
		/// so it knows to create the visuals.
		/// <summary>
		///  Fired when the program deletes a tab
		/// </summary>
		public event Action<string> OnTabChatCleared;
		/// <summary>
		///  Fired when a new tab is created
		/// </summary>
		public event Action<string> OnTabCreated;

		public bool TryGetChatStack(string channel, out VerticalStackLayout chatStack) =>
			_tabChatStacks.TryGetValue(channel, out chatStack);

		/// <summary>
		///  Routes messages to tabs and does other necessary processing
		///  whenever a message is received
		/// </summary>
		/// <param name="chatMessage"></param>
		public void RouteToTab(IChatMessage chatMessage)
		{
			string channel = chatMessage.Channel;

			// Route to tab
			var label = new ConsoleTextLabel(chatMessage);

			bool routeToDefault = chatMessage.Sender == "cho.ppy.sh";

			if (!routeToDefault && (chatMessage.Command == IrcCommand.PrivateMessage || channel.StartsWith("#")))
			{
				if (!TryGetChatStack(channel, out _))
				{
					_logger.LogInformation("No tab to route message to. Creating...");

					// Create the tab if it doesn't exist.
					AddTab(channel);
					TryGetChatStack(channel, out _);
				}
			}
			else
			{
				// Server message
				channel = DefaultTabName;
			}

			AddLabelToTab(label, channel);
		}

		private void AddLabelToTab(Label label, string channel)
		{
			if (!TryGetChatStack(channel, out var chatStack))
			{
				_logger.LogWarning($"Failed to route label {label} to tab {channel}");
				return;
			}

			chatStack.Children.Add(label);
		}

		/// <summary>
		///  Adds a new tab to the internal collection
		/// </summary>
		/// <param name="channel"></param>
		public void AddTab(string channel)
		{
			if (channel.Equals(_credentials.Username, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if (!_tabChatStacks.TryAdd(channel, new VerticalStackLayout()))
			{
				_logger.LogWarning($"An attempt was made to add a tab that already exists: {channel}");
			}

			OnTabCreated?.Invoke(channel);
			_logger.LogInformation($"Tab created: {channel}");
		}

		/// <summary>
		///  Clears a chat stack from memory.
		/// </summary>
		private void ClearStack(string channel)
		{
			if (!_tabChatStacks.TryRemove(channel, out _))
			{
				throw new InvalidOperationException("No tab to remove with the given name.");
			}

			OnTabChatCleared?.Invoke(channel);
		}
	}
}