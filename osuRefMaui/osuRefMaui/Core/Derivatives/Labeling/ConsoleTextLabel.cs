using osuRefMaui.Core.Coloring;
using osuRefMaui.Core.Derivatives.Labeling.Spans;
using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.Derivatives.Labeling
{
	public class ConsoleTextLabel : Label
	{
		private readonly IChatMessage _message;

		public ConsoleTextLabel(IChatMessage message)
		{
			_message = message;

			HorizontalOptions = ChatPalette.ConsoleTextLabelHorizontalOptions;
			VerticalOptions = ChatPalette.ConsoleTextLabelVerticalOptions;
			FontFamily = ChatPalette.ConsoleTextLabelFontFamily;
			Margin = ChatPalette.ConsoleTextLabelMargin;
			FormattedText = GetFormattedString();
		}

		/*
		 * Different labels need to be returned based on the type of command issued.
		 * Below are methods that correspond to most (if not all) known command types.
		 *
		 * Although tedious, this is likely the cleanest implementation.
		 */

		private FormattedString GetFormattedString() => _message.Command switch
		{
			IrcCommand.PrivateMessage => GetMessageFormattedString(),
			_ => GetCommandFormattedString()
		};

		private FormattedString GetMessageFormattedString()
		{
			var fmt = new FormattedString();

			// Add current time
			fmt.Spans.Add(new ConsoleSpan
			{
				Text = _message.TimeStamp.ToString("HH:mm:ss"),
				TextColor = ChatPalette.ConsoleSpanTimeColor
			});

			// Add hyperlinked user
			fmt.Spans.Add(new HyperlinkConsoleTextSpan
			{
				Text = _message.Sender,
				Url = $"https://osu.ppy.sh/u/{_message.Sender}"
			});

			// User separator (e.g. User: message)
			fmt.Spans.Add(new ConsoleSpan
			{
				Text = ": "
			});

			// Add content
			fmt.Spans.Add(new ConsoleSpan
			{
				Text = _message.Content
			});

			return fmt;
		}

		private FormattedString GetCommandFormattedString()
		{
			var fmt = new FormattedString();

			// Add current time
			fmt.Spans.Add(new ConsoleSpan
			{
				Text = _message.TimeStamp.ToString("HH:mm:ss"),
				TextColor = ChatPalette.ConsoleSpanTimeColor
			});

			// Add command and user

			fmt.Spans.Add(new ConsoleSpan
			{
				Text = $"[{_message.Command}] ",
				TextColor = ChatPalette.GetColorForCommand(_message.Command)
			});

			fmt.Spans.Add(new ConsoleSpan
			{
				Text = $">> {_message.Channel}"
			});

			return fmt;
		}
	}
}