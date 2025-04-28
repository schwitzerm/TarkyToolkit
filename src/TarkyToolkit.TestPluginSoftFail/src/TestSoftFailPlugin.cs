using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Core;
using TarkyToolkit.Core.Exceptions;
using TarkyToolkit.TestPlugin.Patch;
using UnityEngine;

namespace TarkyToolkit.TestPlugin;

[BepInPlugin("Mellow_.TarkyToolkit.TestPluginSoftFail", "TarkyToolkit.TestPluginSoftFail", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit", "0.1.0")]
[BepInProcess("EscapeFromTarkov.exe")]
public class TestSoftFailPlugin : TarkyPlugin
{
    private float _timeDiff = 0;

    [UsedImplicitly]
    private void Awake()
    {
        try
        {
            TarkyPatchContext.EnablePatches([
                new SoftFailPatch(gameObject)
            ]);
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to start TestPlugin.");
            Logger.LogError(e.ToString());
            throw;
        }
    }

    [UsedImplicitly]
    private void Update()
    {
        _timeDiff += Time.deltaTime;
        if (_timeDiff > 5)
        {
            Logger.LogDebug("TestPlugin is running.");
        }
    }
}
