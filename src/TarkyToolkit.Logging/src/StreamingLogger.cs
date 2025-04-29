using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using TarkyToolkit.Shared.Logging;
using TarkyToolkit.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace TarkyToolkit.Logging
{
    /// <summary>
    /// A thread-safe logger that streams log messages to a remote endpoint.
    /// Uses a BepLogger for local logging and fallback.
    /// </summary>
    public class StreamingLogger : IAsyncLogger, IDisposable
    {
        private const int MAX_QUEUE_SIZE = 1000;

        private readonly string _streamUrl;
        private readonly IThreadSafeLogger _localLogger;
        private readonly ConcurrentQueue<LogMessage> _pendingLogs = new ConcurrentQueue<LogMessage>();

        private MonoBehaviour _coroutineRunner;
        private Coroutine _processingCoroutine;
        private bool _processingStarted;
        private bool _disposed;

        public string SourceName => _localLogger.SourceName;

        /// <summary>
        /// Represents a queued log message with its associated log level
        /// </summary>
        private class LogMessage
        {
            public enum Level { Debug, Info, Warning, Error }
            public Level LogLevel { get; }
            public string Message { get; }
            public string FormattedMessage { get; }

            public LogMessage(Level logLevel, string message, string formattedMessage)
            {
                LogLevel = logLevel;
                Message = message;
                FormattedMessage = formattedMessage;
            }
        }

        /// <summary>
        /// Initializes a new streaming logger that sends logs to a remote endpoint
        /// </summary>
        /// <param name="url">The URL of the remote logging endpoint</param>
        /// <param name="localLogger">A BepLogger for local logging and fallback</param>
        /// <param name="coroutineRunner">Optional MonoBehaviour to run coroutines</param>
        public StreamingLogger(string url, BepLogger localLogger, MonoBehaviour coroutineRunner = null)
        {
            _streamUrl = url ?? throw new ArgumentNullException(nameof(url));
            _localLogger = localLogger ?? throw new ArgumentNullException(nameof(localLogger));
            _coroutineRunner = coroutineRunner;

            // If we have a coroutine runner, set up processing immediately
            if (_coroutineRunner != null)
            {
                SetupProcessing(_coroutineRunner);
            }
        }

        /// <summary>
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        /// <param name="host">A MonoBehaviour that will host the logging coroutine</param>
        public void SetupProcessing(MonoBehaviour host)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(StreamingLogger));
            if (host == null) throw new ArgumentNullException(nameof(host));

            // Still need synchronization for coroutine management
            lock (this)
            {
                if (_processingStarted) return;

                _coroutineRunner = host;
                _processingStarted = true;

                try
                {
                    _processingCoroutine = host.StartCoroutine(ProcessLogQueue());
                }
                catch (Exception ex)
                {
                    _processingStarted = false;
                    _localLogger.LogError($"Failed to start streaming log processing coroutine: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Notifies the logger that its host MonoBehaviour is being destroyed
        /// </summary>
        public void OnHostDestroy()
        {
            StopProcessing();
            ProcessAllRemainingLogs();
        }

        /// <summary>
        /// Stops the log processing coroutine if it's running
        /// </summary>
        public void StopProcessing()
        {
            // Still need synchronization for coroutine management
            lock (this)
            {
                if (!_processingStarted || !_coroutineRunner || _processingCoroutine == null) return;

                try
                {
                    _coroutineRunner.StopCoroutine(_processingCoroutine);
                }
                catch (Exception ex)
                {
                    _localLogger.LogError($"Error stopping streaming log processing coroutine: {ex.Message}");
                }
                finally
                {
                    _processingStarted = false;
                    _processingCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Processes all remaining logs regardless of which thread is calling
        /// </summary>
        private void ProcessAllRemainingLogs()
        {
            if (UnityUtils.IsOnMainThread())
            {
                ProcessPendingLogs();
            }
            else
            {
                // No need for lock here as ConcurrentQueue is thread-safe
                _pendingLogs.Enqueue(new LogMessage(LogMessage.Level.Warning,
                    "ProcessAllRemainingLogs called from a non-main thread. Some logs might be lost.",
                    FormatMessage("Warn", "ProcessAllRemainingLogs called from a non-main thread. Some logs might be lost.")));
            }
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        public Task LogDebug(string message)
        {
            if (_disposed) return Task.CompletedTask;

            var formattedMessage = FormatMessage("Debug", message);

            if (UnityUtils.IsOnMainThread() && _coroutineRunner)
            {
                _localLogger.LogDebug(message);
                _coroutineRunner.StartCoroutine(SendLogCoroutine(formattedMessage));
            }
            else
            {
                // No need for lock here as ConcurrentQueue is thread-safe
                if (_pendingLogs.Count >= MAX_QUEUE_SIZE) return Task.CompletedTask;

                _pendingLogs.Enqueue(new LogMessage(LogMessage.Level.Debug, message, formattedMessage));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs an info message
        /// </summary>
        public Task LogInfo(string message)
        {
            if (_disposed) return Task.CompletedTask;

            string formattedMessage = FormatMessage("Info", message);

            if (UnityUtils.IsOnMainThread() && _coroutineRunner != null)
            {
                _localLogger.LogInfo(message);
                _coroutineRunner.StartCoroutine(SendLogCoroutine(formattedMessage));
            }
            else
            {
                // No need for lock here as ConcurrentQueue is thread-safe
                if (_pendingLogs.Count >= MAX_QUEUE_SIZE) return Task.CompletedTask;

                _pendingLogs.Enqueue(new LogMessage(LogMessage.Level.Info, message, formattedMessage));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public Task LogWarning(string message)
        {
            if (_disposed) return Task.CompletedTask;

            string formattedMessage = FormatMessage("Warn", message);

            if (UnityUtils.IsOnMainThread() && _coroutineRunner != null)
            {
                _localLogger.LogWarning(message);
                _coroutineRunner.StartCoroutine(SendLogCoroutine(formattedMessage));
            }
            else
            {
                // No need for lock here as ConcurrentQueue is thread-safe
                if (_pendingLogs.Count >= MAX_QUEUE_SIZE) return Task.CompletedTask;

                _pendingLogs.Enqueue(new LogMessage(LogMessage.Level.Warning, message, formattedMessage));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public Task LogError(string message)
        {
            if (_disposed) return Task.CompletedTask;

            string formattedMessage = FormatMessage("Error", message);

            if (UnityUtils.IsOnMainThread() && _coroutineRunner != null)
            {
                _localLogger.LogError(message);
                _coroutineRunner.StartCoroutine(SendLogCoroutine(formattedMessage));
            }
            else
            {
                // No need for lock here as ConcurrentQueue is thread-safe
                // For errors, ensure they get logged even if the queue is full
                _pendingLogs.Enqueue(new LogMessage(LogMessage.Level.Error, message, formattedMessage));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Formats a log message to be consistent with SPT's ManualLogSource
        /// </summary>
        private string FormatMessage(string level, string message)
        {
            // This is broken. We only need to format this string if it's NOT being passed to the local logger.
            // With this class structured as it is, this will format it regardless.
            //
            // I'm too lazy to add a formatting class right now so this is what I am doing. Will correct after testing.
            return $"[{level,-5} :{SourceName}] {message}";
        }

        /// <summary>
        /// Coroutine to send a log message to the remote endpoint
        /// </summary>
        private IEnumerator SendLogCoroutine(string formattedMessage)
        {
            if (_disposed) yield break;

            using (var request = new UnityWebRequest(_streamUrl, "POST"))
            {
                var bodyRaw = Encoding.UTF8.GetBytes(formattedMessage);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "text/plain");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    _localLogger.LogError($"Failed to send log message to stream: {request.error}");
                }
            }
        }

        /// <summary>
        /// Coroutine that processes queued log messages on each frame update
        /// </summary>
        private IEnumerator ProcessLogQueue()
        {
            while (_processingStarted && !_disposed)
            {
                try
                {
                    ProcessPendingLogs();
                }
                catch (Exception ex)
                {
                    _localLogger.LogError($"Error in streaming log processing coroutine: {ex.Message}");
                }

                yield return null; // Wait for the next frame
            }
        }

        /// <summary>
        /// Immediately processes all pending log messages
        /// </summary>
        private void ProcessPendingLogs()
        {
            if (_disposed || !_coroutineRunner || !UnityUtils.IsOnMainThread()) return;

            try
            {
                while (_pendingLogs.TryDequeue(out var message))
                {
                    // Log to the local logger
                    switch (message.LogLevel)
                    {
                        case LogMessage.Level.Debug:
                            _localLogger.LogDebug(message.Message);
                            break;
                        case LogMessage.Level.Info:
                            _localLogger.LogInfo(message.Message);
                            break;
                        case LogMessage.Level.Warning:
                            _localLogger.LogWarning(message.Message);
                            break;
                        case LogMessage.Level.Error:
                            _localLogger.LogError(message.Message);
                            break;
                    }

                    // Send it to the remote endpoint
                    _coroutineRunner.StartCoroutine(SendLogCoroutine(message.FormattedMessage));
                }
            }
            catch (Exception ex)
            {
                _localLogger.LogError($"Error processing pending streaming logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of resources...
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            StopProcessing();

            // Clear any remaining logs - no need for lock as ConcurrentQueue is thread-safe
            while (_pendingLogs.TryDequeue(out _)) { }

            _disposed = true;
        }
    }
}
