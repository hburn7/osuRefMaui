using Microsoft.Extensions.Logging;
using osuRefMaui.Core.Coloring;
using osuRefMaui.Core.Derivatives.Buttons;
using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.Filtering;

// ReSharper disable RedundantExtendsListEntry
namespace osuRefMaui;

public partial class MissionControl : ContentPage
{
	private static bool _previouslyLoaded;
	private readonly ChatQueue _chatQueue;
	private readonly IrcFilter _filter;
	private readonly ILogger<MissionControl> _logger;
	private readonly OutgoingMessageHandler _outgoingMessageHandler;
	private readonly TabHandler _tabHandler;

	public MissionControl(ILogger<MissionControl> logger, TabHandler tabHandler, ChatQueue chatQueue,
		OutgoingMessageHandler outgoingMessageHandler, IrcFilter filter)
	{
		_logger = logger;
		_tabHandler = tabHandler;
		_chatQueue = chatQueue;
		_outgoingMessageHandler = outgoingMessageHandler;
		_filter = filter;

		_previouslyLoaded = false;

		InitializeComponent();

		Loaded += MissionControl_Loaded;
	}

	private void MissionControl_Loaded(object sender, EventArgs e)
	{
		if (!_previouslyLoaded)
		{
			// Setup chat filter
			InitFilter();

			// Event handler which sets child's clicked event handler which then resets the color of the tab
			ChatTabHorizontalStack.ChildAdded += ChatTabHorizontalStackOnChildAdded;

			_tabHandler.OnTabCreated += UI_AddTab;

			// Create default tab
			_tabHandler.AddTab(TabHandler.DefaultTabName, false);

			_chatQueue.OnDequeue += m =>
			{
				// Route chat labels to tab
				Window.Dispatcher.Dispatch(async () =>
				{
					_tabHandler.RouteToTab(m);
					UI_RecolorTab(m.Sender);
				});
			};

			/*
			 * Scroll to bottom whenever the queue has remain emptied for a short time.
			 * This is for situations where a batch of messages 
			 * are rapidly dequeued whilst the auto scroller is attempting
			 * to scroll to bottom, thus causing the auto scroller to not
			 * go to the bottom.
			 *
			 * This way, the auto scroller works in all cases.
			 */
			_chatQueue.OnPersistentEmpty += () =>
			{
				Window.Dispatcher.Dispatch(async () =>
				{
					await UI_ScrollToBottom();
				});
			};

			// Swap to default tab
			UI_SwapTab(TabHandler.DefaultTabName);

			_tabHandler.AddTab("#osu", false);
			_tabHandler.AddTab("BanchoBot", false);
			
			_outgoingMessageHandler.Send("Joining #osu...", "#osu", false);
			_outgoingMessageHandler.Send("Contacting BanchoBot...", "BanchoBot", false);
			_outgoingMessageHandler.Send("!roll", "BanchoBot");

			// Chat dequeue loop
			Task.Run(async () =>
			{
				while (true)
				{
					/*
			         * Every 0.25s the chat dequeue loop is run which checks for new messages.
			         * If there is a message in the queue, it is immediately dequeued.
			         *
			         * Dequeuing messages fires an event that other classes can listen to
			         * and act upon. This is how the UI gets updated.
			         */

					while (_chatQueue.TryDequeue(out _)) {}

					await Task.Delay(250);
				}
			});
		}

		_previouslyLoaded = true;
	}

	private void InitFilter()
	{
		_filter.FilterJoin = cbFilterBanchoJoin.IsChecked;
		_filter.FilterQuit = cbFilterBanchoQuit.IsChecked;
		_filter.FilterPing = cbFilterBanchoPing.IsChecked;
		_filter.FilterSlotMove = cbFilterBanchoSlotMove.IsChecked;
	}

	/// <summary>
	///  Subscribes the added button's Clicked event handler to reset the coloring and clear
	///  the new message alert.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ChatTabHorizontalStackOnChildAdded(object sender, ElementEventArgs e)
	{
		try
		{
			/*
			 * Anything that needs to happen when a tab is clicked happens here.
			 */

			// Reset button color to default when clicked. This clears the notification coloring.
			var button = (Button)e.Element;
			button.Clicked += (s, _) =>
			{
				Window.Dispatcher.Dispatch(async () =>
				{
					// Disable chatbox when viewing the server tab
					ChatBox.IsEnabled = _tabHandler.ActiveTab != TabHandler.DefaultTabName;

					// Reset tab color to default
					UI_RecolorTab(((Button)s)!.Text, true);

					// Scroll to bottom of scrollview
					if (ChatScrollView.Children.Any())
					{
						await UI_ScrollToBottom();
					}
				});
			};
		}
		catch (Exception exception)
		{
			_logger.LogCritical(exception, $"Something was added to the tab stack that was not a button: Element = {e.Element}");
		}
	}

	private async Task UI_ScrollToBottom()
	{
		var endChild = (VisualElement)ChatScrollView.Children[^1];
		double delta = ChatScrollView.GetScrollPositionForElement(endChild, ScrollToPosition.End).Y - ChatScrollView.ScrollY;

		if (delta > 500)
		{
			return;
		}

		await ChatScrollView.ScrollToAsync(0, ChatScrollView.GetScrollPositionForElement(endChild, ScrollToPosition.End).Y, true);
	}

	private void UI_SwapTab(string channel)
	{
		if (!_tabHandler.TryGetChatStack(channel, out var chatStack))
		{
			return;
		}

		ChatScrollView.Content = chatStack;
		_tabHandler.ActiveTab = channel;
	}

	private void UI_AddTab(string channel, bool manuallyAdded) => Window.Dispatcher.Dispatch(() =>
	{
		var button = new TabButton(channel);
		button.Clicked += (s, _) => UI_SwapTab(((TabButton)s)!.Text);

		ChatTabHorizontalStack.Insert(ChatTabHorizontalStack.Count - 1, button);

		if (!manuallyAdded)
		{
			UI_RecolorTab(channel);
		}
	});

	private void UI_RecolorTab(string channel, bool resetToDefault = false)
	{
		if (channel == _tabHandler.ActiveTab)
		{
			resetToDefault = true;
		}

		bool isDirectMessage = !channel.StartsWith("#") && !channel.Equals(TabHandler.DefaultTabName);

		try
		{
			var button = (Button)ChatTabHorizontalStack.First(x => ((Button)x).Text.Equals(channel, StringComparison.OrdinalIgnoreCase));

			if (resetToDefault)
			{
				button.TextColor = TabPalette.TabText;
				button.BackgroundColor = TabPalette.TabBackground;
				return;
			}

			if (isDirectMessage)
			{
				button.TextColor = TabPalette.TabDirectUnreadText;
				button.BackgroundColor = TabPalette.TabDirectUnreadBackground;
			}
			else
			{
				button.TextColor = TabPalette.TabGeneralUnreadText;
				button.BackgroundColor = TabPalette.TabGeneralUnreadBackground;
			}
		}
		catch (InvalidOperationException) {}
	}

	private async void BtnAddChannel_Clicked(object sender, EventArgs e)
	{
		string channel = await DisplayPromptAsync("Add Channel",
			"Type the channel or username to add.", placeholder: "#osu");

		_tabHandler.AddTab(channel, true);
	}

	private void ChatBox_Completed(object sender, EventArgs e)
	{
		string rawText = ((Entry)sender).Text;

		if (string.IsNullOrWhiteSpace(rawText))
		{
			((Entry)sender).Text = "";
			return;
		}
		
		// Process /commands
		if (rawText.StartsWith("/"))
		{
			var cmdHandler = new CommandHandler(rawText);
			if (cmdHandler.IsCustomCommand)
			{
				// Process custom commands
				if (cmdHandler.CustomCommand == CustomCommand.Clear)
				{
					if(_tabHandler.TryGetChatStack(_tabHandler.ActiveTab, out var chatStack))
					{
						chatStack.Children.Clear();
					}
				}
			}
			else
			{
				// Process chat commands
				var message = _outgoingMessageHandler.CreateChatMessage(cmdHandler);
				_outgoingMessageHandler.Send(message);

				//todo: Listen to channel join / leave / etc. events and handle them in the UI.
			}
		}
		else
		{
			// Send "private message" to the current channel
			_outgoingMessageHandler.Send(rawText);
		}

		((Entry)sender).Text = "";
	}

	private void cmdMpTimer120_Clicked(object sender, EventArgs e) {}
	private void cmdMpTimer90_Clicked(object sender, EventArgs e) {}
	private void cmdMpStart10_Clicked(object sender, EventArgs e) {}
	private void cmdMpStart5_Clicked(object sender, EventArgs e) {}
	private void cmdMpAbort_Clicked(object sender, EventArgs e) {}

	private void filterCheckBoxBanchoJoin_CheckChanged(object sender, EventArgs e)
	{
		bool isChecked = ((CheckBox)sender).IsChecked;
		_filter.FilterJoin = isChecked;
	}

	private void filterCheckBoxBanchoQuit_CheckChanged(object sender, EventArgs e)
	{
		bool isChecked = ((CheckBox)sender).IsChecked;
		_filter.FilterQuit = isChecked;
	}

	private void filterCheckBoxBanchoPing_CheckChanged(object sender, EventArgs e)
	{
		bool isChecked = ((CheckBox)sender).IsChecked;
		_filter.FilterPing = isChecked;
	}

	private void filterCheckBoxBanchoSlotMove_CheckChanged(object sender, EventArgs e)
	{
		bool isChecked = ((CheckBox)sender).IsChecked;
		_filter.FilterSlotMove = isChecked;
	}
}