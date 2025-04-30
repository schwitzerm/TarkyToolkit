namespace TarkyToolkit.Shared.Logging
{
    /// <summary>
    /// Represents a thread-safe logger interface that extends ILogger.
    /// This interface guarantees that all logging operations are safe for concurrent access from multiple threads.
    /// </summary>
    public abstract class ThreadSafeLogger : Logger
    {
    }
}