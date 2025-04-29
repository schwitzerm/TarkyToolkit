using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Shared
{
    /// <summary>
    /// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
    /// </summary>
    [BepInPlugin("Mellow_.TarkyToolkit.Shared", "TarkyToolkit.Shared", "0.1.0")]
    public class TarkyToolkitSharedPlugin : BaseUnityPlugin
    {
        internal new static Logger Logger { get; private set; } = null!;

        [UsedImplicitly]
        private void Awake()
        {
            Logger = new BepLogger(base.Logger);
        }
    }
}
