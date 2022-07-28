using osuRefMaui.Core.Coloring;
using osuRefMaui.Core.Derivatives.Labeling.Spans;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.Derivatives.Labeling;

public abstract class LabelBase : Label
{
	private readonly IChatMessage _chatMessage;

	protected LabelBase(IChatMessage chatMessage)
	{
		_chatMessage = chatMessage;
		HorizontalOptions = ChatPalette.ConsoleTextLabelHorizontalOptions;
		VerticalOptions = ChatPalette.ConsoleTextLabelVerticalOptions;
		FontFamily = ChatPalette.ConsoleTextLabelFontFamily;
		Margin = ChatPalette.ConsoleTextLabelMargin;
	}

	protected abstract FormattedString GetFormattedString();

	protected virtual Span TimestampSpan() => new ConsoleSpan
	{
		Text = _chatMessage.TimeStamp.ToString("HH:mm:ss") + " ",
		TextColor = ChatPalette.ConsoleSpanTimeColor
	};

	protected virtual Span UsernameSpan() => new HyperlinkConsoleTextSpan
	{
		Text = _chatMessage.Sender,
		Url = $"https://osu.ppy.sh/u/{_chatMessage.Sender}"
	};

	protected virtual Span UsernameSeparator() => new ConsoleSpan
	{
		Text = ": "
	};

	protected virtual Span ContentSpan() => new ConsoleSpan
	{
		Text = _chatMessage.Content
	};
}