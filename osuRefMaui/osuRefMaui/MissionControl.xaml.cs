using osuRefMaui.Core.Derivatives.Buttons;
using osuRefMaui.Core.IRC;

// ReSharper disable RedundantExtendsListEntry
namespace osuRefMaui;

public partial class MissionControl : ContentPage
{
	private static bool _previouslyLoaded;
	private readonly ChatQueue _chatQueue;
	private readonly TabHandler _tabHandler;

	public MissionControl(TabHandler tabHandler, ChatQueue chatQueue)
	{
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
			_tabHandler.OnTabCreated += UI_AddTab;

			// Create default tab
			_tabHandler.AddTab(TabHandler.DefaultTabName);

			_chatQueue.OnDequeue += m =>
			{
				// Route chat labels to tab
				Window.Dispatcher.Dispatch(() => { _tabHandler.RouteToTab(m); });
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

	private void UI_SwapTab(string channel)
	{
		if (!_tabHandler.TryGetChatStack(channel, out var chatStack))
		{
			return;
		}

		ChatScrollView.Content = chatStack;
	}

	private void UI_AddTab(string channel) => Window.Dispatcher.Dispatch(() =>
	{
		var button = new TabButton(channel);
		button.Clicked += (s, _) => UI_SwapTab(((TabButton)s)!.Text);

		ChatTabHorizontalStack.Insert(ChatTabHorizontalStack.Count - 1, button);
	});

	private async void BtnAddChannel_Clicked(object sender, EventArgs e)
	{
		string channel = await DisplayPromptAsync("Add Channel",
			"Type the channel or username to add.", placeholder: "#osu");

		_tabHandler.AddTab(channel);
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