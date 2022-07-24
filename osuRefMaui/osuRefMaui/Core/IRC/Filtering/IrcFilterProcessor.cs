using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.IRC.Filtering;

public class IrcFilterProcessor
{
	private readonly IChatMessage _chatMessage;
	private readonly IrcFilter _filter;

	public IrcFilterProcessor(IrcFilter filter, IChatMessage chatMessage)
	{
		_filter = filter;
		_chatMessage = chatMessage;
	}

	public bool IsFiltered() => (_filter.FilterJoin && FilterJoin()) ||
	                            (_filter.FilterQuit && FilterQuit()) ||
	                            (_filter.FilterPing && FilterPing()) ||
	                            (_filter.FilterSlotMove && FilterSlotMove());

	private bool FilterJoin() => _chatMessage.Command == IrcCommand.Join;
	private bool FilterQuit() => _chatMessage.Command == IrcCommand.Quit;
	private bool FilterPing() => _chatMessage.Command == IrcCommand.Ping;

	private bool FilterSlotMove()
	{
		if (_chatMessage.Command == IrcCommand.PrivMsg)
		{
			if (_chatMessage.Sender!.Equals("BanchoBot"))
			{
				// todo: convert to regex -- this is just a quick implementation
				return _chatMessage.Content.Contains("moved to slot", StringComparison.OrdinalIgnoreCase);
			}
		}

		return false;
	}
}