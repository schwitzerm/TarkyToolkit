using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TarkyToolkit.Shared.Logging
{
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
}