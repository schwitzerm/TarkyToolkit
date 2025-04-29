using System;
using System.Collections;
using System.Collections.Concurrent;
using BepInEx.Logging;
using TarkyToolkit.Shared.Logging;
using TarkyToolkit.Unity;
using UnityEngine;

namespace TarkyToolkit.Logging
{
    /// <summary>
    /// A thread-safe wrapper for BepInEx's ManualLogSource that ensures log messages are safely processed
    /// regardless of which thread they originate from.
    /// </summary>
    public class BepLogger : ThreadSafeLogger
    {
        private const int MAX_QUEUE_SIZE = 1000;
        private const int MAX_BATCH_SIZE = 50;

        private readonly ConcurrentQueue<LogMessage> _pendingLogs = new ConcurrentQueue<LogMessage>();
        private readonly ConcurrentQueue<LogMessage> _highPriorityLogs = new ConcurrentQueue<LogMessage>();
        private bool _processingStarted;
        private ManualLogSource _logger;
        private MonoBehaviour _processingHost;
        private Coroutine _processingCoroutine;
        private bool _isDisposed;
        private readonly object _coroutineLock = new object();

        public override string SourceName => _logger.SourceName;

        /// <summary>
        /// Represents a queued log message with its associated log level
        /// </summary>
        private class LogMessage
        {
            public enum Level { Debug, Info, Warning, Error }
            public Level LogLevel { get; }
            public string Message { get; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            // For later...?
            public DateTime Timestamp { get; }

            public LogMessage(Level logLevel, string message)
            {
                LogLevel = logLevel;
                Message = message;
                Timestamp = DateTime.UtcNow;
            }
        }

        public BepLogger()
        {
            // Ensure the main thread dispatcher exists
            UnityUtils.RunOnMainThread(() => { }, false);
        }

        /// <summary>
        /// Sets up message processing using the specified MonoBehaviour
        /// </summary>
        public override void SetupProcessing(MonoBehaviour host, ManualLogSource logger)
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(BepLogger));
            if (host == null) throw new ArgumentNullException(nameof(host));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            lock (_coroutineLock)
            {
                if (_processingStarted) return;

                _processingHost = host;
                _processingStarted = true;

                try
                {
                    _processingCoroutine = host.StartCoroutine(ProcessLogQueue());
                }
                catch (Exception ex)
                {
                    _processingStarted = false;

                    // Direct log to avoid recursion
                    try { _logger.LogError($"Failed to start log processing coroutine: {ex.Message}"); }
                    catch { Debug.LogError($"[{SourceName}] Failed to start log processing: {ex.Message}"); }
                }
            }
        }

        /// <summary>
        /// Notifies the logger that its host MonoBehaviour is being destroyed
        /// </summary>
        public void OnHostDestroy()
        {
            StopProcessing();

            // Process remaining logs on the main thread
            if (UnityUtils.IsOnMainThread())
            {
                ProcessAllLogs(true);
            }
            else
            {
                UnityUtils.RunOnMainThread(() => ProcessAllLogs(true));
            }
        }

        public void SetupProcessing(MonoBehaviour host)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the log processing coroutine if it's running
        /// </summary>
        public override void StopProcessing()
        {
            lock (_coroutineLock)
            {
                if (!_processingStarted || !_processingHost || _processingCoroutine == null) return;

                try
                {
                    _processingHost.StopCoroutine(_processingCoroutine);
                }
                catch (Exception ex)
                {
                    // Direct log to avoid recursion
                    try { _logger.LogError($"Error stopping log coroutine: {ex.Message}"); }
                    catch { Debug.LogError($"[{SourceName}] Error stopping log coroutine: {ex.Message}"); }
                }
                finally
                {
                    _processingStarted = false;
                    _processingCoroutine = null;
                }
            }
        }

        /// <summary>
        /// Logs an informational message
        /// </summary>
        public override void LogInfo(string message)
        {
            if (_isDisposed) return;

            if (UnityUtils.IsOnMainThread() && _processingStarted)
            {
                try
                {
                    _logger.LogInfo(message);
                }
                catch (Exception ex)
                {
                    TryDirectErrorLog($"Error logging error message: {ex.Message}");
                }
            }
            else
            {
                // Errors go to the high-priority queue
                _highPriorityLogs.Enqueue(new LogMessage(LogMessage.Level.Error, message));
            }
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        public override void LogDebug(string message)
        {
            if (_isDisposed) return;

            if (UnityUtils.IsOnMainThread() && _processingStarted)
            {
                try
                {
                    _logger.LogDebug(message);
                }
                catch (Exception ex)
                {
                    TryDirectErrorLog($"Error logging debug message: {ex.Message}");
                }
            }
            else
            {
                EnqueueMessage(LogMessage.Level.Debug, message);
            }
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public override void LogWarning(string message)
        {
            if (_isDisposed) return;

            if (UnityUtils.IsOnMainThread() && _processingStarted)
            {
                try
                {
                    _logger.LogWarning(message);
                }
                catch (Exception ex)
                {
                    TryDirectErrorLog($"Error logging warning message: {ex.Message}");
                }
            }
            else
            {
                EnqueueMessage(LogMessage.Level.Warning, message);
            }
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public override void LogError(string message)
        {
            if (_isDisposed) return;

            if (UnityUtils.IsOnMainThread() && _processingStarted)
            {
                try
                {
                    _logger.LogError(message);
                }
                catch (Exception ex)
                {
                    TryDirectErrorLog($"Error logging error message: {ex.Message}");
                }
            }
            else
            {
                // Errors go to the high-priority queue
                _highPriorityLogs.Enqueue(new LogMessage(LogMessage.Level.Error, message));
            }
        }

        /// <summary>
        /// Try to log an error directly to the Unity console as a last resort
        /// </summary>
        private void TryDirectErrorLog(string message)
        {
            try
            {
                _logger.LogError(message);
            }
            catch
            {
                try
                {
                    Debug.LogError($"[{SourceName}] {message}");
                }
                catch
                {
                    // We've done all we can
                }
            }
        }

        /// <summary>
        /// Enqueues a message to be processed later
        /// </summary>
        private void EnqueueMessage(LogMessage.Level level, string message)
        {
            if (level == LogMessage.Level.Error)
            {
                // Errors always get queued
                _highPriorityLogs.Enqueue(new LogMessage(level, message));
            }
            else if (_pendingLogs.Count < MAX_QUEUE_SIZE)
            {
                _pendingLogs.Enqueue(new LogMessage(level, message));
            }
            // If the queue is full, a message is lost unless it's an error
        }

        /// <summary>
        /// Processes all logs immediately, clearing both queues
        /// </summary>
        private void ProcessAllLogs(bool flushToUnityLog)
        {
            if (_isDisposed) return;

            if (!UnityUtils.IsOnMainThread())
            {
                // This should only be called from the main thread
                TryDirectErrorLog("ProcessAllLogs should only be called from the main Unity thread");
                return;
            }

            try
            {
                // Process high-priority logs first
                ProcessQueue(_highPriorityLogs);
                ProcessQueue(_pendingLogs);

                // If we need to flush to Unity's log because we're shutting down
                if (flushToUnityLog)
                {
                    TryDirectErrorLog($"[{SourceName}] Flushed all pending logs");
                }
            }
            catch (Exception ex)
            {
                TryDirectErrorLog($"Error processing logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Process a specific queue of log messages
        /// </summary>
        private void ProcessQueue(ConcurrentQueue<LogMessage> queue)
        {
            while (queue.TryDequeue(out var message))
            {
                try
                {
                    switch (message.LogLevel)
                    {
                        case LogMessage.Level.Debug:
                            _logger.LogDebug(message.Message);
                            break;
                        case LogMessage.Level.Info:
                            _logger.LogInfo(message.Message);
                            break;
                        case LogMessage.Level.Warning:
                            _logger.LogWarning(message.Message);
                            break;
                        case LogMessage.Level.Error:
                            _logger.LogError(message.Message);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    TryDirectErrorLog($"Error processing log message: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Processes the log queue
        /// </summary>
        private IEnumerator ProcessLogQueue()
        {
            while (_processingStarted && !_isDisposed)
            {
                ProcessLogsWithErrorHandling();

                yield return null; // Wait for the next frame
            }
        }

        /// <summary>
        /// Processes all logs in batches with error handling
        /// </summary>
        private void ProcessLogsWithErrorHandling()
        {
            try
            {
                // Always process high-priority logs first
                var processedCount = 0;

                // Process in batches to avoid frame drops
                while (_highPriorityLogs.TryDequeue(out var message) && processedCount < MAX_BATCH_SIZE)
                {
                    ProcessLogMessage(message);
                    processedCount++;
                }

                // Process normal logs in batches
                processedCount = 0;
                while (_pendingLogs.TryDequeue(out var message) && processedCount < MAX_BATCH_SIZE)
                {
                    ProcessLogMessage(message);
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                TryDirectErrorLog($"Error in log processing coroutine: {ex.Message}");
            }
        }

        /// <summary>
        /// Processes a single log message
        /// </summary>
        private void ProcessLogMessage(LogMessage message)
        {
            try
            {
                switch (message.LogLevel)
                {
                    case LogMessage.Level.Debug:
                        _logger.LogDebug(message.Message);
                        break;
                    case LogMessage.Level.Info:
                        _logger.LogInfo(message.Message);
                        break;
                    case LogMessage.Level.Warning:
                        _logger.LogWarning(message.Message);
                        break;
                    case LogMessage.Level.Error:
                        _logger.LogError(message.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                TryDirectErrorLog($"Error processing log message: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of resources used by the logger
        /// </summary>
        public override void Dispose()
        {
            if (_isDisposed) return;

            StopProcessing();

            // Try to process remaining logs before disposing
            if (UnityUtils.IsOnMainThread())
            {
                ProcessAllLogs(true);
            }

            // Clear any remaining logs that weren't processed
            while (_pendingLogs.TryDequeue(out _)) { }
            while (_highPriorityLogs.TryDequeue(out _)) { }

            _isDisposed = true;
        }
    }
}
