using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Logging
{
    /// <summary>
    /// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
    /// </summary>
    [BepInPlugin("Mellow_.TarkyToolkit.Logging", "TarkyToolkit.Logging", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Shared", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Unity", "0.1.0")]
    public class TarkyToolkitLoggingPlugin : BaseUnityPlugin
    {
        internal new static AsyncLogger Logger { get; private set; }

        [UsedImplicitly]
        private void Awake()
        {
            Logger = gameObject.GetComponent<StreamingLogger>();
        }
    }
}
