using Microsoft.Extensions.Logging;
using osuRefMaui.Core.Derivatives.Labeling;
using osuRefMaui.Core.IRC.Interfaces;
using osuRefMaui.Core.IRC.LoginInformation;
using System.Collections.Concurrent;
using System.Text;

namespace osuRefMaui.Core.IRC
{
	public class TabHandler
	{
		public const string DefaultTabName = "osu!Bancho";
		private readonly Credentials _credentials;
		private readonly ILogger<TabHandler> _logger;
		private readonly Pathing _pathing;
		private readonly ConcurrentDictionary<string, VerticalStackLayout> _tabChatStacks;

		public TabHandler(ILogger<TabHandler> logger, Credentials credentials, Pathing pathing)
		{
			_logger = logger;
			_credentials = credentials;
			_pathing = pathing;
			_tabChatStacks = new ConcurrentDictionary<string, VerticalStackLayout>(StringComparer.OrdinalIgnoreCase);
		}

		public string ActiveTab { get; set; } = DefaultTabName;
		/// <summary>
		///  Fired when a new tab is created. Bool indicates whether the tab was manually added.
		/// </summary>
		public event Action<string, bool> OnTabCreated;
		/// <summary>
		///  Fired whenever a chat tab is removed or closed.
		/// </summary>
		public event Action<string> OnTabRemoved;

		public bool TryGetChatStack(string channel, out VerticalStackLayout chatStack) =>
			_tabChatStacks.TryGetValue(channel, out chatStack);

		/// <summary>
		///  Routes messages to tabs and does other necessary processing
		///  whenever a message is received
		/// </summary>
		/// <param name="chatMessage"></param>
		public void RouteToTab(IChatMessage chatMessage)
		{
			string channel;
			if (chatMessage.IsFromPublicChannel || chatMessage.Sender == _credentials.Username)
			{
				channel = chatMessage.Channel;
			}
			else
			{
				channel = chatMessage.Sender;
			}

			if (channel == null)
			{
				return;
			}

			// Route to tab
			Label label;
			if (chatMessage is SystemMessage)
			{
				label = new ConsoleSystemLabel(chatMessage);
			}
			else
			{
				label = new ConsoleTextLabel(chatMessage);
			}

			bool routeToDefault = chatMessage.Channel == DefaultTabName;

			if (!routeToDefault)
			{
				if (!TryGetChatStack(channel, out _))
				{
					// Do not add messages from public channels if there isn't a tab for them
					// as they have likely been closed and the incoming messages need to be voided.
					if (chatMessage.IsFromPublicChannel && !chatMessage.Channel.StartsWith("#mp_"))
					{
						return;
					}

					_logger.LogInformation("No tab to route message to. Creating...");

					// Create the tab if it doesn't exist.
					AddTab(channel, false);
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
		/// <param name="channel">The name of the chat channel or user</param>
		/// <param name="manuallyAdded">Whether the channel was added manually by user input</param>
		public void AddTab(string channel, bool manuallyAdded)
		{
			if (channel.Equals(_credentials.Username, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			if (!_tabChatStacks.TryAdd(channel, new VerticalStackLayout()))
			{
				_logger.LogWarning($"An attempt was made to add a tab that already exists: {channel}");
				return;
			}

			OnTabCreated?.Invoke(channel, manuallyAdded);
			_logger.LogInformation($"Tab created: {channel}");
		}

		/// <summary>
		///  Removes a tab from memory and fires events necessary for other classes
		///  to take action whenever a tab is removed
		/// </summary>
		/// <param name="channel"></param>
		public void RemoveTab(string channel)
		{
			if (!_tabChatStacks.TryRemove(channel, out _))
			{
				_logger.LogInformation("Attempted to remove chatStack that does not exist");
			}

			_logger.LogInformation($"Removed tab {channel}, firing event");
			OnTabRemoved?.Invoke(channel);
		}

		/// <summary>
		///  Saves the current chat log
		/// </summary>
		public async Task SaveCurrentLog() => await SaveLog(ActiveTab);

		/// <summary>
		///  Saves all open chat logs
		/// </summary>
		public async Task SaveAllLogs()
		{
			_logger.LogInformation("SaveAllLogs invoked");
			foreach (string key in _tabChatStacks.Keys)
			{
				await SaveLog(key);
			}
		}

		private async Task SaveLog(string channel)
		{
			if (!TryGetChatStack(channel, out var chatStack))
			{
				_logger.LogWarning($"Failed to save log for {channel} -- channel does not exist.");
				return;
			}

			try
			{
				string fileName = Path.Combine(_pathing.SaveLogPath, $"{channel}.txt");
				var textBuilder = new StringBuilder();

				foreach (var child in chatStack.Children)
				{
					if (child is not Label label)
					{
						continue;
					}

					textBuilder.AppendLine(label.FormattedText.ToString());
				}

				await File.WriteAllTextAsync(fileName, textBuilder.ToString());
				_logger.LogInformation($"Saved log for channel {channel} successfully at {fileName}");
			}
			catch (IOException e)
			{
				_logger.LogCritical(e, $"Fatal error occurred while saving log for channel {channel}");
			}
		}
	}
}