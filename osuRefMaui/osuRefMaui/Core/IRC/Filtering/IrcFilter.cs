namespace osuRefMaui.Core.IRC.Filtering;

public class IrcFilter
{
	public bool FilterJoin { get; set; }
	public bool FilterQuit { get; set; }
	public bool FilterPing { get; set; }
	public bool FilterSlotMove { get; set; }
	public bool FilterTeamChange { get; set; }
}