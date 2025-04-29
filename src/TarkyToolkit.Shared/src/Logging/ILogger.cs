using System.Threading.Tasks;

namespace TarkyToolkit.Shared.Logging
{
    public interface ILogger
    {
        string SourceName { get; }

        void LogInfo(string message);
        void LogDebug(string message);
        void LogWarning(string message);
        void LogError(string message);
    }

    public interface IAsyncLogger
    {
        string SourceName { get; }

        Task LogDebug(string message);
        Task LogInfo(string message);
        Task LogWarning(string message);
        Task LogError(string message);
    }

    public interface IThreadSafeLogger : ILogger
    {
    }
}
