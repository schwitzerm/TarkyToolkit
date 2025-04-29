using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Core;
using TarkyToolkit.Core.Patch;
using TarkyToolkit.TestSoftFailPlugin.Patch;
using UnityEngine;

namespace TarkyToolkit.TestSoftFailPlugin
{
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
                TarkyPatchContext.EnablePatches(new TarkyPatch[]
                {
                    new SoftFailPatch(gameObject)
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
            try
            {
                _timeDiff += Time.deltaTime;
                if (!(_timeDiff > 5f)) return;

                _timeDiff = 0f;
                Logger.LogDebug("TestSoftFailPlugin is alive. Looking for player...");
                var player = TarkovContext.GameWorldApi.Player;
                Logger.LogDebug(player is not null
                    ? $"Player found. Player's nickname is: {player.Profile.Nickname ?? "UNKNOWN"}"
                    : "Player not found.");
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                throw;
            }
        }
    }
}
