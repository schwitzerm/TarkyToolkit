using System;
using System.Threading.Tasks;
using BepInEx.Logging;
using UnityEngine;

namespace TarkyToolkit.Shared.Logging
{
    /// <summary>
    /// Represents a logger interface for handling log messages in a Unity environment
    /// </summary>
    public abstract class Logger : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// Gets the name of the source associated with the logger
        /// </summary>
        public abstract string SourceName { get; }

        /// <summary>
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        /// <param name="host">A MonoBehaviour that will host the logging coroutine</param>
        /// <param name="logger">A ManualLogSource to use for local logging</param>
        public abstract void SetupProcessing(MonoBehaviour host, ManualLogSource logger);

        /// <summary>
        /// Stops the log processing coroutine if it's running
        /// </summary>
        public abstract void StopProcessing();

        /// <summary>
        /// Logs an info message
        /// </summary>
        /// <param name="message">The message to log</param>
        public abstract void LogInfo(string message);

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        public abstract void LogDebug(string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        public abstract void LogWarning(string message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        public abstract void LogError(string message);

        public abstract void Dispose();
    }

    /// <summary>
    /// Represents an asynchronous logger interface for handling log messages in a Unity environment
    /// </summary>
    public abstract class AsyncLogger : MonoBehaviour, IDisposable
    {
        /// <summary>
        /// Gets the name of the source associated with the logger
        /// </summary>
        public abstract string SourceName { get; }

        /// <summary>
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        public abstract void SetupProcessing(MonoBehaviour host, string url, ThreadSafeLogger localLogger);

        /// <summary>
        /// Stops the log processing coroutine if it's running
        /// </summary>
        public abstract void StopProcessing();

        /// <summary>
        /// Logs a debug message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public abstract Task LogDebug(string message);

        /// <summary>
        /// Logs an info message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public abstract Task LogInfo(string message);

        /// <summary>
        /// Logs a warning message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public abstract Task LogWarning(string message);

        /// <summary>
        /// Logs an error message asynchronously
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <returns>A Task representing the asynchronous operation</returns>
        public abstract Task LogError(string message);

        public abstract void Dispose();
    }

    /// <summary>
    /// Represents a thread-safe logger interface that extends ILogger.
    /// This interface guarantees that all logging operations are safe for concurrent access from multiple threads.
    /// </summary>
    public abstract class ThreadSafeLogger : Logger
    {
    }
}
