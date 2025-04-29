using BepInEx;
using EFT;
using JetBrains.Annotations;
using TarkyToolkit.Core.Api;
using TarkyToolkit.Core.Logging;

namespace TarkyToolkit.Core;

/// <summary>
/// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
/// </summary>
[BepInPlugin("Mellow_.TarkyToolkit.Core", "TarkyToolkit.Core", "0.1.0")]
public class TarkyToolkitCorePlugin : BaseUnityPlugin
{
    internal new static Logger Logger { get; private set; } = null!;

    [UsedImplicitly]
    private void Awake()
    {
        Logger = new BepLogger(base.Logger);
    }
}
