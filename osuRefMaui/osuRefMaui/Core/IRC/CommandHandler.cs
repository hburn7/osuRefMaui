using osuRefMaui.Core.IRC.Interfaces;

namespace osuRefMaui.Core.IRC;

public class CommandHandler : ICommandHandler
{
	// How many arguments belong to this command? -1 = unlimited
	private readonly int _expectedArgs;
	private readonly string _rawCommand;
	private readonly string _rawInput;

	/// <summary>
	///  Resolves command from a fully-formed command string
	/// </summary>
	/// <param name="rawInput">The singular command in string form with no arguments.</param>
	/// <exception cref="InvalidOperationException"></exception>
	public CommandHandler(string rawInput)
	{
		_rawInput = rawInput;

		if (!_rawInput.StartsWith("/"))
		{
			throw new InvalidOperationException("No command to process.");
		}

		_rawInput = rawInput;
		_rawCommand = _rawInput.Split("/")[1].Split(" ")[0];

		Command = ResolveCommand();
		CustomCommand = ResolveCustomCommand();
		IsCustomCommand = CustomCommand != null;

		if (Command == null && CustomCommand == null)
		{
			throw new InvalidOperationException("Invalid command");
		}

		if (Command != null && CustomCommand != null)
		{
			throw new InvalidOperationException("Custom command matched both custom and non custom parameters.");
		}
		
		_expectedArgs = IsCustomCommand ? ExpectedArgs(CustomCommand!.Value) : ExpectedArgs(Command!.Value);

		ValidArgumentCount = ValidateArgCount();
	}

	public IrcCommand? Command { get; }
	public CustomCommand? CustomCommand { get; }
	public bool IsCustomCommand { get; }
	public bool ValidArgumentCount { get; }
	public string[] Args
	{
		get
		{
			// ReSharper disable once ConvertIfStatementToReturnStatement
			try
			{
				return _rawInput.Split("/")[1].Split(" ")[1..];
			}
			catch (Exception)
			{
				return Array.Empty<string>();
			}
		}
	}

	private IrcCommand? ResolveCommand() => _rawCommand.ToLower() switch
	{
		"quit" or "disconnect" or "logout" => IrcCommand.Quit,
		"part" or "leave" or "close" => IrcCommand.Part, // Also responsible for closing tabs
		"join" or "add" => IrcCommand.Join,
		"chat" or "query" or "msg" or "message" or "privmsg" or "w" or "whisper" or "privatemessage" or "pm" or "r" => IrcCommand.PrivMsg,
		_ => null
	};

	private CustomCommand? ResolveCustomCommand() => _rawCommand.ToLower() switch
	{
		"clear" or "clean" => IRC.CustomCommand.Clear,
		"save" or "savelog" => IRC.CustomCommand.Savelog,
		_ => null
	};

	private int ExpectedArgs(IrcCommand command) => command switch
	{
		IrcCommand.Quit => 0,
		IrcCommand.Join => -1,
		IrcCommand.Part => -1,
		IrcCommand.PrivMsg => -1,
		_ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
	};

	private int ExpectedArgs(CustomCommand command) => command switch
	{
		IRC.CustomCommand.Clear => 0,
		IRC.CustomCommand.Savelog => 0,
		_ => throw new ArgumentOutOfRangeException(nameof(command), command, null)
	};

	private bool ValidateArgCount() => Args.Length == _expectedArgs || _expectedArgs == -1;
}