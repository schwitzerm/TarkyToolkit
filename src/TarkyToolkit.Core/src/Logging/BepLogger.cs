using BepInEx.Logging;

namespace TarkyToolkit.Core.Logging;

public class BepLogger(ManualLogSource logger) : Logger
{
    public override void LogInfo(string message)
    {
        logger.LogInfo(message);
    }

    public override void LogDebug(string message)
    {
        logger.LogDebug(message);
    }

    public override void LogWarning(string message)
    {
        logger.LogWarning(message);
    }

    public override void LogError(string message)
    {
        logger.LogError(message);
    }
}
