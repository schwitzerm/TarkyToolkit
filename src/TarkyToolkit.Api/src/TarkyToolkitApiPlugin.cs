﻿using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Logging;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Api
{
    /// <summary>
    /// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
    /// </summary>
    [BepInPlugin("Mellow_.TarkyToolkit.Api", "TarkyToolkit.Api", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Core", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Shared", "0.1.0")]
    public class TarkyToolkitApiPlugin : BaseUnityPlugin
    {
        private static GameWorldApi _gameWorldApi;
        internal new static AsyncLogger Logger { get; private set; }

        [UsedImplicitly]
        private void Awake()
        {
            Logger = gameObject.GetComponent<BatchHttpLogger>();
            Logger.LogDebug("Initializing TarkyToolkit API.");
            _gameWorldApi = gameObject.AddComponent<GameWorldApi>();
            DontDestroyOnLoad(_gameWorldApi);
            Logger.LogDebug("TarkyToolkit API initialized.");
        }
    }
}
