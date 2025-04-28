using System;
using BepInEx;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Context;
using TarkyToolkit.Patch;
using UnityEngine;
using ILogger = TarkyToolkit.Shared.Logging.ILogger;

namespace TarkyToolkit;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.SPT.core", "3.11.0")]
[BepInDependency("Mellow_.TarkyToolkit.Reflection", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit.Shared", "0.1.0")]
[BepInProcess("EscapeFromTarkov.exe")]
public class TarkyToolkit : BaseUnityPlugin
{
    private static PatchContext _patchContext = null!;
    private static TarkovContext _tarkovContext = null!;
    internal new static ILogger Logger = null!;

    [UsedImplicitly]
    private void Awake()
    {
        try
        {
            Logger = new BepLogger(base.Logger);
            Logger.LogDebug("Initializing TarkyToolkit.");

            Logger.LogDebug("Creating PatchContext.");
            _patchContext = gameObject.AddComponent<PatchContext>();
            DontDestroyOnLoad(_patchContext);
            Logger.LogDebug("PatchContext created and marked as DontDestroyOnLoad.");

            Logger.LogDebug("Creating TarkovContext.");
            _tarkovContext = gameObject.AddComponent<TarkovContext>();
            DontDestroyOnLoad(_tarkovContext);
            Logger.LogDebug("TarkovContext created and marked as DontDestroyOnLoad.");

            Logger.LogDebug("Applying patches.");
            TarkyPatch[] toApply =
            [
                new Patch.GameWorld.AssignOnAwakePatch(_tarkovContext),
                new Patch.Player.AssignOnAwakePatch(_tarkovContext)
            ];
            _patchContext.EnablePatches(toApply);
            Logger.LogDebug("Patches applied.");

            Logger.LogDebug("TarkyToolkit initialized.");
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to initialize TarkyToolkit.");
            Logger.LogError(e.ToString());
            if (_patchContext != null)
            {
                GameObject.Destroy(_patchContext);
            }
            if (_tarkovContext != null)
            {
                GameObject.Destroy(_tarkovContext);
            }
        }
    }
}
