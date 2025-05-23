﻿using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Logging;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Reflection
{
    /// <summary>
    /// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
    /// </summary>
    [BepInPlugin("Mellow_.TarkyToolkit.Reflection", "TarkyToolkit.Reflection", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Shared", "0.1.0")]
    public class TarkyToolkitReflectionPlugin : BaseUnityPlugin
    {
        internal new static AsyncLogger Logger { get; private set; }

        [UsedImplicitly]
        private void Awake()
        {
            Logger = gameObject.GetComponent<BatchHttpLogger>();
        }
    }
}
