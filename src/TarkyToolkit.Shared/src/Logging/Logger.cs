using System;
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
}
