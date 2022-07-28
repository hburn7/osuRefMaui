using osuRefMaui.Core.Coloring;
using osuRefMaui.Core.Derivatives.Labeling.Spans;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.Derivatives.Labeling;

/// <summary>
///  Represents a message that appears as if it is sent by the application (system)
/// </summary>
public sealed class ConsoleSystemLabel : LabelBase
{
	public ConsoleSystemLabel(IChatMessage chatMessage) : base(chatMessage) { FormattedText = GetFormattedString(); }

	protected override FormattedString GetFormattedString()
	{
		var fmt = new FormattedString();

		fmt.Spans.Add(TimestampSpan());
		fmt.Spans.Add(UsernameSpan());
		fmt.Spans.Add(UsernameSeparator());
		fmt.Spans.Add(ContentSpan());

		return fmt;
	}

	protected override Span UsernameSpan() => new ConsoleSpan
	{
		Text = "[System]",
		TextColor = ChatPalette.SystemTextColor
	};

	protected override Span UsernameSeparator() => new ConsoleSpan
	{
		Text = " >> ",
		TextColor = ChatPalette.SystemTextColor
	};

	protected override Span ContentSpan()
	{
		var span = base.ContentSpan();
		span.TextColor = ChatPalette.SystemTextColor;
		return span;
	}
}