using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Core;
using TarkyToolkit.TestSoftFailPlugin.Patch;
using UnityEngine;

namespace TarkyToolkit.TestSoftFailPlugin;

[BepInPlugin("Mellow_.TarkyToolkit.TestSoftFailPlugin", "TarkyToolkit.TestSoftFailPlugin", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit", "0.1.0")]
[BepInProcess("EscapeFromTarkov.exe")]
public class TestSoftFailPlugin : TarkyPlugin
{
    private float _timeDiff;

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
        if (_timeDiff > 10f)
        {
            Logger.LogDebug("TestSoftFailPlugin is alive.");
            _timeDiff = 0f;
        }
    }
}
