namespace osuRefMaui.Core.Coloring;

public static class TabPalette
{
    public static readonly Color TabBackground = Colors.Gray;
    public static readonly Color TabText = Colors.White;
    
    // New direct messages
    public static readonly Color TabDirectUnreadBackground = new Color(204, 116, 182);
    public static readonly Color TabDirectUnreadText = Colors.Black;
    
    // New messages in a standard channel
    public static readonly Color TabGeneralUnreadText = Colors.Black;
    public static readonly Color TabGeneralUnreadBackground = Colors.LightCoral;

    public static readonly string TabTextFontFamily = "Consolas";

    public const int FontSize = 12;

    public static readonly Thickness Padding = new(5, 5);

    public static readonly LayoutOptions HorizontalOptions = LayoutOptions.Start;
    public static readonly LayoutOptions VerticalOptions = LayoutOptions.Start;
}