using BepInEx.Logging;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit;

public class BepLogger(ManualLogSource logger) : ILogger
{
    public void LogInfo(string message)
    {
        logger.LogInfo(message);
    }

    public void LogDebug(string message)
    {
        logger.LogDebug(message);
    }

    public void LogWarning(string message)
    {
        logger.LogWarning(message);
    }

    public void LogError(string message)
    {
        logger.LogError(message);
    }
}
