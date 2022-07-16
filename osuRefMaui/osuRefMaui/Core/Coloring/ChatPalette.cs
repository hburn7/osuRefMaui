using System;
using osuRefMaui.Core.IRC;

namespace osuRefMaui.Core.Coloring
{
	public static class ChatPalette
	{
        public static Color LabelTextHighlight = Colors.Orange;

        public static Color HyperlinkConsoleSpanTextColor = Colors.LightBlue;

        // ConsoleSpan properties
        public static Color ConsoleSpanTextColor = Colors.White;
        public static Color ConsoleSpanTimeColor = Colors.Orange;
        public static string ConsoleSpanFontFamily = "Consolas";


        // ConsoleTextLabel properties
        public static LayoutOptions ConsoleTextLabelVerticalOptions = LayoutOptions.Start;
        public static LayoutOptions ConsoleTextLabelHorizontalOptions = LayoutOptions.Start;
        public static string ConsoleTextLabelFontFamily = "Consolas";
        public static Thickness ConsoleTextLabelMargin = new(7, 0);

        public static Color GetColorForCommand(IrcCommand command)
        {
            return command switch
            {
                IrcCommand.Join => Colors.Blue,
                IrcCommand.Quit => Colors.Red,
                IrcCommand.Ping => Colors.Green,
                IrcCommand.Part => Colors.Violet,
                IrcCommand.Replaced => Colors.Red,
                IrcCommand.Mode => Colors.Salmon,
                _ => Colors.Grey
            };
        }
    }
}

