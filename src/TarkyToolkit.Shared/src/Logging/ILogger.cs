using System.Threading.Tasks;

namespace TarkyToolkit.Shared.Logging
{
    /// <summary>
    /// Represents a logger interface for handling log messages in a Unity environment
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the name of the source associated with the logger
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        /// <param name="host">A MonoBehaviour that will host the logging coroutine</param>
        void SetupProcessing(UnityEngine.MonoBehaviour host);

        /// <summary>
        /// Stops the log processing coroutine if it's running
        /// </summary>
        void StopProcessing();

        /// <summary>
        /// Logs an info message
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogInfo(string message);

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogDebug(string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        void LogError(string message);
    }

    /// <summary>
    /// Represents an asynchronous logger interface for handling log messages in a Unity environment
    /// </summary>
    public interface IAsyncLogger
    {
        /// <summary>
        /// Gets the name of the source associated with the logger
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        /// <param name="host">A MonoBehaviour that will host the logging coroutine</param>
        void SetupProcessing(UnityEngine.MonoBehaviour host);

        /// <summary>
        /// Stops the log processing coroutine if it's running
        /// </summary>
        void StopProcessing();

        /// <summary>
        /// Logs a debug message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        Task LogDebug(string message);

        /// <summary>
        /// Logs an info message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        Task LogInfo(string message);

        /// <summary>
        /// Logs a warning message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        Task LogWarning(string message);

        /// <summary>
        /// Logs an error message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        Task LogError(string message);
    }

    /// <summary>
    /// Represents a thread-safe logger interface that extends ILogger.
    /// This interface guarantees that all logging operations are safe for concurrent access from multiple threads.
    /// </summary>
    public interface IThreadSafeLogger : ILogger
    {
    }
}
