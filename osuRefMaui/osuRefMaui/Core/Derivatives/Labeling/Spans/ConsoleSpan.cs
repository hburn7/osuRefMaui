using osuRefMaui.Core.Coloring;

namespace osuRefMaui.Core.Derivatives.Labeling.Spans
{
	public class ConsoleSpan : Span
	{
		public ConsoleSpan()
		{
			FontFamily = ChatPalette.ConsoleSpanFontFamily;
			TextColor = ChatPalette.ConsoleSpanTextColor;
		}
	}
}