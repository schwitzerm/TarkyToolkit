using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Logging;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Core
{
    /// <summary>
    /// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
    /// </summary>
    [BepInPlugin("Mellow_.TarkyToolkit.Core", "TarkyToolkit.Core", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Shared", "0.1.0")]
    public class TarkyToolkitCorePlugin : BaseUnityPlugin
    {
        internal new static ILogger Logger { get; private set; }

        [UsedImplicitly]
        private void Awake()
        {
            Logger = new BepLogger(base.Logger);
        }
    }
}
