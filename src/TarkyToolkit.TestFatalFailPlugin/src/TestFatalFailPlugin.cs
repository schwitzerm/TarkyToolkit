using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Core;
using TarkyToolkit.Core.Patch;
using TarkyToolkit.TestFatalFailPlugin.Patch;
using UnityEngine;

namespace TarkyToolkit.TestFatalFailPlugin
{
    [BepInPlugin("Mellow_.TarkyToolkit.TestFatalFailPlugin", "TarkyToolkit.TestFatalFailPlugin", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit", "0.1.0")]
    [BepInProcess("EscapeFromTarkov.exe")]
    public class TestFatalFailPlugin : TarkyPlugin
    {
        private float _timeDiff;

        [UsedImplicitly]
        private void Awake()
        {
            try
            {
                TarkyPatchContext.EnablePatches(new TarkyPatch[]
                {
                    new FatalFailPatch(gameObject)
                });
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
                // we should not see this.
                Logger.LogDebug("TestFatalFailPlugin is alive.");
            }
        }
    }
}
