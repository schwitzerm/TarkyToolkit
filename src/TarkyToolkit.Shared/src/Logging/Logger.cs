namespace TarkyToolkit.Shared.Logging
{
    public abstract class Logger
    {
        public abstract void LogInfo(string message);
        public abstract void LogDebug(string message);
        public abstract void LogWarning(string message);
        public abstract void LogError(string message);
    }
}
