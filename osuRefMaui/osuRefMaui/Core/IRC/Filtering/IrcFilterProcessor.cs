using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.IRC.Filtering;

public class IrcFilterProcessor
{
	private readonly IChatMessage _chatMessage;
	private readonly IrcFilter _filter;
	private readonly int[] _spamCodes = { 353 };

	public IrcFilterProcessor(IrcFilter filter, IChatMessage chatMessage)
	{
		_filter = filter;
		_chatMessage = chatMessage;
	}

	public bool IsFiltered() => IsSpam() ||
	                            (_filter.FilterJoin && FilterJoin()) ||
	                            (_filter.FilterQuit && FilterQuit()) ||
	                            (_filter.FilterPing && FilterPing()) ||
	                            (_filter.FilterSlotMove && FilterSlotMove()) ||
	                            (_filter.FilterTeamChange && FilterTeamChange());

	private bool FilterJoin() => _chatMessage.Command == IrcCommand.Join;
	private bool FilterQuit() => _chatMessage.Command == IrcCommand.Quit;
	private bool FilterPing() => _chatMessage.Command == IrcCommand.Ping;

	private bool FilterSlotMove()
	{
		if (_chatMessage.Command == IrcCommand.PrivMsg && _chatMessage.Sender!.Equals("BanchoBot"))
		{
			return _chatMessage.Content.Contains("moved to slot", StringComparison.OrdinalIgnoreCase);
		}

		return false;
	}

	private bool FilterTeamChange()
	{
		if (_chatMessage.Command == IrcCommand.PrivMsg && _chatMessage.Sender!.Equals("BanchoBot"))
		{
			return _chatMessage.Content.Contains("changed to Blue", StringComparison.OrdinalIgnoreCase) ||
			       _chatMessage.Content.Contains("changed to Red", StringComparison.OrdinalIgnoreCase);
		}

		return false;
	}

	// Spam / status code filters
	private bool IsSpam()
	{
		foreach (int code in _spamCodes)
		{
			if (_chatMessage.IsStatusCode(code))
			{
				return true;
			}
		}

		return false;
	}
}