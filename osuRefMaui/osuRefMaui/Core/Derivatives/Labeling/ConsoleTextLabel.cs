using osuRefMaui.Core.Coloring;
using osuRefMaui.Core.Derivatives.Labeling.Spans;
using osuRefMaui.Core.IRC;
using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.Derivatives.Labeling
{
	public sealed class ConsoleTextLabel : LabelBase
	{
		private readonly IChatMessage _chatMessage;

		public ConsoleTextLabel(IChatMessage chatMessage) : base(chatMessage)
		{
			_chatMessage = chatMessage;
			FormattedText = GetFormattedString();
		}

		/*
		 * Different labels need to be returned based on the type of command issued.
		 * Below are methods that correspond to most (if not all) known command types.
		 *
		 * Although tedious, this is likely the cleanest implementation.
		 */

		protected override FormattedString GetFormattedString() => _chatMessage.Command switch
		{
			IrcCommand.PrivMsg => GetMessageFormattedString(),
			_ => GetCommandFormattedString()
		};

		private FormattedString GetMessageFormattedString()
		{
			var fmt = new FormattedString();

			fmt.Spans.Add(TimestampSpan());
			fmt.Spans.Add(UsernameSpan());
			fmt.Spans.Add(UsernameSeparator());
			fmt.Spans.Add(ContentSpan());

			return fmt;
		}

		private FormattedString GetCommandFormattedString()
		{
			var fmt = new FormattedString();

			fmt.Spans.Add(TimestampSpan());

			// Add command and user
			fmt.Spans.Add(new ConsoleSpan
			{
				Text = $"[{_chatMessage.Command}] ",
				TextColor = ChatPalette.GetColorForCommand(_chatMessage.Command)
			});

			fmt.Spans.Add(new ConsoleSpan
			{
				Text = $">> {_chatMessage.SourceName}"
			});

			return fmt;
		}
	}
}