using Microsoft.Extensions.Logging;
using osuRefMaui.Core.Coloring;
using osuRefMaui.Core.Derivatives.Buttons;
using osuRefMaui.Core.IRC;

// ReSharper disable RedundantExtendsListEntry
namespace osuRefMaui;

public partial class MissionControl : ContentPage
{
	private static bool _previouslyLoaded;
	private readonly ChatQueue _chatQueue;
	private readonly ILogger<MissionControl> _logger;
	private readonly TabHandler _tabHandler;

	public MissionControl(ILogger<MissionControl> logger, TabHandler tabHandler, ChatQueue chatQueue)
	{
		_logger = logger;
		_tabHandler = tabHandler;
		_chatQueue = chatQueue;

		_previouslyLoaded = false;

		InitializeComponent();

		Loaded += MissionControl_Loaded;
	}

	private void MissionControl_Loaded(object sender, EventArgs e)
	{
		if (!_previouslyLoaded)
		{
			// Event handler which sets child's clicked event handler which then resets the color of the tab
			ChatTabHorizontalStack.ChildAdded += ChatTabHorizontalStackOnChildAdded;

			_tabHandler.OnTabCreated += UI_AddTab;

			// Create default tab
			_tabHandler.AddTab(TabHandler.DefaultTabName, false);

			_chatQueue.OnDequeue += m =>
			{
				// Route chat labels to tab
				Window.Dispatcher.Dispatch(() =>
				{
					_tabHandler.RouteToTab(m);
					UI_RecolorTab(m.Sender);
				});
			};

			// Swap to default tab
			UI_SwapTab(TabHandler.DefaultTabName);

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
			// Reset button color to default when clicked. This clears the notification coloring.
			var button = (Button)e.Element;
			button.Clicked += (s, _) =>
			{
				Window.Dispatcher.Dispatch(() => { UI_RecolorTab(((Button)s)!.Text, true); });
			};
		}
		catch (Exception exception)
		{
			_logger.LogCritical(exception, $"Something was added to the tab stack that was not a button: Element = {e.Element}");
		}
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

	private void ChatBox_Completed(object sender, EventArgs e) {}
	private void cmdMpTimer120_Clicked(object sender, EventArgs e) {}
	private void cmdMpTimer90_Clicked(object sender, EventArgs e) {}
	private void cmdMpStart10_Clicked(object sender, EventArgs e) {}
	private void cmdMpStart5_Clicked(object sender, EventArgs e) {}
	private void cmdMpAbort_Clicked(object sender, EventArgs e) {}
	private void filterCheckBoxBanchoJoin_CheckChanged(object sender, EventArgs e) {}
	private void filterCheckBoxBanchoQuit_CheckChanged(object sender, EventArgs e) {}
	private void filterCheckBoxBanchoPing_CheckChanged(object sender, EventArgs e) {}
	private void filterCheckBoxBanchoSlotMove_CheckChanged(object sender, EventArgs e) {}
}