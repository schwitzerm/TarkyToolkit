using System;
using BepInEx;
using JetBrains.Annotations;
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
    private static InternalTarkyPatchContext _internalTarkyPatchContext = null!;
    private static TarkyPatchContext _tarkyPatchContext = null!;
    private static TarkovContext _tarkovContext = null!;
    internal new static ILogger Logger = null!;

    [UsedImplicitly]
    private void Awake()
    {
        try
        {
            Logger = new BepLogger(base.Logger);
            Logger.LogDebug("Initializing TarkyToolkit.");

            Logger.LogDebug("Creating InternalTarkyPatchContext.");
            _internalTarkyPatchContext = gameObject.AddComponent<InternalTarkyPatchContext>();
            DontDestroyOnLoad(_internalTarkyPatchContext);
            Logger.LogDebug("InternalTarkyPatchContext created and marked as DontDestroyOnLoad.");

            Logger.LogDebug("Creating TarkyPatchContext.");
            _tarkyPatchContext = gameObject.AddComponent<TarkyPatchContext>();
            DontDestroyOnLoad(_tarkyPatchContext);
            Logger.LogDebug("TarkyPatchContext created and marked as DontDestroyOnLoad.");

            Logger.LogDebug("Creating TarkovContext.");
            _tarkovContext = gameObject.AddComponent<TarkovContext>();
            DontDestroyOnLoad(_tarkovContext);
            Logger.LogDebug("TarkovContext created and marked as DontDestroyOnLoad.");

            Logger.LogDebug("Enabling internal patches.");
            InternalTarkyPatch[] internalToApply =
            [
                new Patch.GameWorld.AssignOnAwakePatch(_tarkovContext),
                new Patch.Player.AssignOnAwakePatch(_tarkovContext)
            ];
            _internalTarkyPatchContext.EnablePatches(internalToApply);
            Logger.LogDebug("Internal patches enabled.");

            Logger.LogDebug("Enabling imported patches.");
            TarkyPatch[] toApply = [];
            _tarkyPatchContext.EnablePatches(toApply);
            Logger.LogDebug("Imported patches enabled.");

            Logger.LogDebug("TarkyToolkit initialized.");
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to initialize TarkyToolkit.");
            Logger.LogError(e.ToString());
            if (_tarkyPatchContext != null)
            {
                GameObject.Destroy(_tarkyPatchContext);
            }
            if (_tarkovContext != null)
            {
                GameObject.Destroy(_tarkovContext);
            }
        }
    }
}
