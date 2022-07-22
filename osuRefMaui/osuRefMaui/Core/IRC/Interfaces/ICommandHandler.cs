namespace osuRefMaui.Core.IRC.Interfaces;

public interface ICommandHandler
{
	/// <summary>
	/// The IrcCommand resolved from the input. Null if the command is custom.
	/// </summary>
	public IrcCommand? Command { get; }
	/// <summary>
	/// The CustomCommand resolved from the input. Null if not custom.
	/// </summary>
	public CustomCommand? CustomCommand { get; }
	/// <summary>
	/// Whether the command is one of a few special custom commands.
	/// </summary>
	public bool IsCustomCommand { get; }
	/// <summary>
	/// Whether the amount of arguments provided matches the expected amount for the command.
	/// </summary>
	public bool ValidArgumentCount { get; }
	/// <summary>
	/// The arguments provided to the command
	/// </summary>
	public string[] Args { get; }
}