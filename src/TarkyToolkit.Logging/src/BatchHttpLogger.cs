using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using TarkyToolkit.Shared.Logging;
using TarkyToolkit.Unity;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace TarkyToolkit.Logging
{
    /// <summary>
    /// A thread-safe logger that streams log messages to a remote endpoint.
    /// Uses a IThreadSafeLogger for local logging and fallback.
    /// </summary>
    public class BatchHttpLogger : AsyncLogger
    {
        private const int MAX_QUEUE_SIZE = 1000;

        private readonly ConcurrentQueue<LogMessage> _pendingLogs = new ConcurrentQueue<LogMessage>();

        private MonoBehaviour _coroutineRunner;
        private Coroutine _processingCoroutine;
        private bool _processingStarted;
        private bool _disposed;
        private string _streamUrl;
        private ThreadSafeLogger _localLogger;

        public override string SourceName => _localLogger.SourceName;

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
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        public override void SetupProcessing(MonoBehaviour host, string url, ThreadSafeLogger localLogger)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(BatchHttpLogger));
            if (host == null) throw new ArgumentNullException(nameof(host));

            _streamUrl = url ?? throw new ArgumentNullException(nameof(url));
            _localLogger = localLogger ?? throw new ArgumentNullException(nameof(localLogger));

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
        public override void StopProcessing()
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
        public override Task LogDebug(string message)
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
        public override Task LogInfo(string message)
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
        public override Task LogWarning(string message)
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
        public override Task LogError(string message)
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
            // I'm too lazy to add a formatting class right now, so this is what I am doing. Will correct after testing.
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
        /// Coroutine that processes queued log messages on each frame update,
        /// batching them together for more efficient transmission
        /// </summary>
        private IEnumerator ProcessLogQueue()
        {
            var batchMessages = new List<string>();
            var lastSendTime = Time.realtimeSinceStartup;

            while (_processingStarted && !_disposed)
            {
                try
                {
                    // Process logs into a batch
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
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // Add to the batch instead of sending immediately
                        batchMessages.Add(message.FormattedMessage);
                    }

                    // Send batched messages if we have any and enough time has passed
                    // or if we've accumulated enough messages
                    var currentTime = Time.realtimeSinceStartup;
                    if (batchMessages.Count > 0 && (currentTime - lastSendTime > 30.0f || batchMessages.Count > 50))
                    {
                        _coroutineRunner.StartCoroutine(SendBatchedLogsCoroutine(batchMessages));
                        batchMessages = new List<string>();
                        lastSendTime = currentTime;
                    }
                }
                catch (Exception ex)
                {
                    _localLogger.LogError($"Error in streaming log processing coroutine: {ex.Message}");
                }

                yield return null; // Wait for the next frame
            }
        }

        /// <summary>
        /// Sends a batch of log messages in a single HTTP request
        /// </summary>
        /// <param name="messages">Collection of formatted log messages to send</param>
        private IEnumerator SendBatchedLogsCoroutine(List<string> messages)
        {
            if (_disposed || messages.Count == 0) yield break;

            using (var request = new UnityWebRequest(_streamUrl, "POST"))
            {
                // Join messages with newlines or use JSON array format depending on server requirements
                var combinedMessage = string.Join("\n", messages);
                var bodyRaw = Encoding.UTF8.GetBytes(combinedMessage);

                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "text/plain");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    _localLogger.LogError($"Failed to send batched log messages to stream: {request.error}");
                }
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
        /// Disposes of resources used by the logger
        /// </summary>
        public override void Dispose()
        {
            if (_disposed) return;

            StopProcessing();

            // Clear any remaining logs - no need for lock as ConcurrentQueue is thread-safe
            while (_pendingLogs.TryDequeue(out _)) { }

            _disposed = true;
        }
    }
}
