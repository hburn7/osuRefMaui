using osuRefMaui.Core.Coloring;

namespace osuRefMaui.Core.Derivatives.Labeling.Spans
{
    public class HyperlinkConsoleTextSpan : ConsoleSpan
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkConsoleTextSpan));

        public string Url
        {
            get => (string)GetValue(UrlProperty);
            set => SetValue(UrlProperty, value);
        }

        public HyperlinkConsoleTextSpan()
        {
            var cmd = new Command(async () =>
            {
                await Browser.Default.OpenAsync(Url);
            });

            TextColor = ChatPalette.HyperlinkConsoleSpanTextColor;
            GestureRecognizers.Add(new ClickGestureRecognizer
            {
                Command = cmd
            });
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = cmd
            });
        }
    }
}
