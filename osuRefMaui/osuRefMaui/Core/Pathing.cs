using Microsoft.Extensions.Logging;

namespace osuRefMaui.Core;

public class Pathing
{
	private const string AppDirName = "oRef";
	private readonly ILogger<Pathing> _logger;

	public Pathing(ILogger<Pathing> logger)
	{
		_logger = logger;
		BasePath = Path.Combine(Path.GetTempPath(), AppDirName);
		SaveLogPath = Path.Combine(BasePath, "chatlogs");

		InitDirs();
	}

	public string BasePath { get; }
	public string SaveLogPath { get; }

	private void InitDirs()
	{
		if (!Directory.Exists(BasePath))
		{
			_logger.LogInformation($"No base directory found. Creating at {BasePath}");
			Directory.CreateDirectory(BasePath);

			_logger.LogInformation($"No chatlogs directory found. Creating at {BasePath}");
			Directory.CreateDirectory(SaveLogPath);
		}
	}
}