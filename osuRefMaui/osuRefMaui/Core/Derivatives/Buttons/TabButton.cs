using osuRefMaui.Core.Coloring;

namespace osuRefMaui.Core.Derivatives.Buttons
{
	public class TabButton : Button
	{
		/// <summary>
		///  Creates a new tab with the specified name
		/// </summary>
		/// <param name="channel">The channel this tab is mapped to</param>
		public TabButton(string channel)
		{
			Text = channel;
			TextColor = TabPalette.TabText;
			FontFamily = TabPalette.TabTextFontFamily;
			BackgroundColor = TabPalette.TabBackground;
			FontSize = TabPalette.FontSize;
			Padding = TabPalette.Padding;
			HorizontalOptions = TabPalette.HorizontalOptions;
			VerticalOptions = TabPalette.VerticalOptions;
		}
	}
}