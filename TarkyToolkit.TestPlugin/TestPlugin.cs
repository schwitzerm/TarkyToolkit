using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.TestPlugin;

[BepInPlugin("Mellow_.TarkyToolkit.TestPlugin", "TarkyToolkit.TestPlugin", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit", "0.1.0")]
[BepInProcess("EscapeFromTarkov.exe")]
public class TestPlugin : TarkyPlugin
{
    [UsedImplicitly]
    private void Awake()
    {
        TarkyPatchContext.EnablePatches([
            new Patch.Player.AssignOnAwakePatch(gameObject)
        ]);
        Logger.LogDebug($"Plugin Mellow_.TarkyToolkit.TestPlugin is loaded!");
    }
}
