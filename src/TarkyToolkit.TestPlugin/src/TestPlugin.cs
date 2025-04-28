using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Core;
using TarkyToolkit.Core.Exceptions;
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
            TarkyPatchContext.EnablePatches([
                new NonFatalFailingPatch(gameObject),
                new FatalFailingPatch(gameObject)
            ]);
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to start TestPlugin.");
            Logger.LogError(e.ToString());
            throw;
        }
    }
}
