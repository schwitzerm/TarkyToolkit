namespace TarkyToolkit.Core.Logging;

public interface ILogger
{
    void LogInfo(string message);
    void LogDebug(string message);
    void LogWarning(string message);
    void LogError(string message);
}
