using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.TestPlugin.Patch;

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
            var nonFatalPatch = new NonFatalFailingPatch(gameObject);
            TarkyPatchContext.EnablePatches([
                nonFatalPatch,
                //fatalPatch
            ]);
        }
        catch (Exception e)
        {
            var pluginName = GetType().FullName;
            Logger.LogError($"Error enabling plugin {pluginName}.");
            Logger.LogError(e.ToString());
            throw;
        }
    }
}
