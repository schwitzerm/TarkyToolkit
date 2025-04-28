using System;
using BepInEx;
using BepInEx.Logging;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
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
        try
        {
            var nonFatalPatch = new Patch.Player.NonFatalFailingPatch(gameObject);
            var fatalPatch = new Patch.Player.FatalFailingPatch(gameObject);
            TarkyPatchContext.EnablePatches([
                nonFatalPatch,
                fatalPatch
            ]);
        }
        catch (Exception e)
        {
            var patchName = GetType().FullName;
            Logger.LogError($"Error applying patch {patchName}.");
            Logger.LogError(e.ToString());
            throw;
        }

        if (enabled)
        {
            Logger.LogDebug("TestPlugin enabled.");
        }
    }
}
