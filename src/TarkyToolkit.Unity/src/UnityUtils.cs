using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace TarkyToolkit.Unity
{
    /// <summary>
    /// Utility functions for Unity-specific operations
    /// </summary>
    public static class UnityUtils
    {
        private static int? _mainThreadId;

        /// <summary>
        /// Initializes the main thread ID. Should be called from a MonoBehaviour Awake or Start method.
        /// </summary>
        public static void InitializeMainThread()
        {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Checks if the current code is executing on Unity's main thread
        /// </summary>
        /// <returns>True if on the main thread, false otherwise</returns>
        public static bool IsOnMainThread()
        {
            if (!_mainThreadId.HasValue)
            {
                Debug.LogWarning("Main thread ID was not initialized. Call UnityUtils.InitializeMainThread() from a MonoBehaviour's Awake or Start method.");
                _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            return Thread.CurrentThread.ManagedThreadId == _mainThreadId.Value;
        }

        /// <summary>
        /// Safely runs an action on the main thread
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <param name="immediate">If true, execute immediately when already on the main thread</param>
        public static void RunOnMainThread(Action action, bool immediate = true)
        {
            if (IsOnMainThread() && immediate)
            {
                action();
            }
            else
            {
                // Queue to be executed on the next update
                MainThreadDispatcher.Enqueue(action);
            }
        }
    }

    /// <summary>
    /// Singleton MonoBehaviour for dispatching actions to the main thread
    /// </summary>
    public class MainThreadDispatcher : MonoBehaviour
    {
        private static MainThreadDispatcher _instance;
        private static readonly ConcurrentQueue<Action> ActionQueue =
            new ConcurrentQueue<Action>();

        /// <summary>
        /// Ensures the dispatcher exists in the scene
        /// </summary>
        public static void EnsureExists()
        {
            if (_instance) return;

            var go = new GameObject("MainThreadDispatcher");
            _instance = go.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(go);

            // Initialize the main thread ID
            UnityUtils.InitializeMainThread();
        }

        /// <summary>
        /// Enqueues an action to be executed on the main thread
        /// </summary>
        /// <param name="action">The action to execute</param>
        public static void Enqueue(Action action)
        {
            EnsureExists();
            ActionQueue.Enqueue(action);
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize the main thread ID
            UnityUtils.InitializeMainThread();
        }

        private void Update()
        {
            // Process all queued actions
            int processedCount = 0;
            const int maxActionsPerFrame = 100; // Prevent freezing if too many actions are queued

            while (ActionQueue.TryDequeue(out var action) && processedCount < maxActionsPerFrame)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error executing action on main thread: {e}");
                }
                processedCount++;
            }
        }
    }
}
