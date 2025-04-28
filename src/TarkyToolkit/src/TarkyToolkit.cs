using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Context;
using TarkyToolkit.Patch;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Core.Logging;
using ILogger = TarkyToolkit.Core.Logging.ILogger;

namespace TarkyToolkit;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.SPT.core", "3.11.0")]
[BepInDependency("Mellow_.TarkyToolkit.Reflection", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit.Core", "0.1.0")]
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

            Logger.LogDebug("Checking if InternalTarkyPatchContext singleton was created and registered successfully.");
            _internalTarkyPatchContext = gameObject.AddComponent<InternalTarkyPatchContext>();
            DontDestroyOnLoad(_internalTarkyPatchContext);
            if (gameObject.GetComponent<InternalTarkyPatchContext>() == null)
            {
                throw new NullReferenceException("InternalTarkyPatchContext singleton was not found.");
            }

            Logger.LogDebug("Checking if TarkyPatchContext singleton was created and registered successfully.");
            _tarkyPatchContext = gameObject.AddComponent<TarkyPatchContext>();
            DontDestroyOnLoad(_tarkyPatchContext);
            if (gameObject.GetComponent<TarkyPatchContext>() == null)
            {
                throw new NullReferenceException("TarkyPatchContext singleton was not found.");
            }

            Logger.LogDebug("Checking if TarkovContext singleton was created and registered successfully.");
            _tarkovContext = gameObject.AddComponent<TarkovContext>();
            DontDestroyOnLoad(_tarkovContext);
            if (gameObject.GetComponent<TarkovContext>() == null)
            {
                throw new NullReferenceException("TarkovContext singleton was not found.");
            }

            Logger.LogDebug("Enabling internal patches.");
            InternalTarkyPatch[] internalToApply =
            [
                new Patch.GameWorld.AssignOnAwakePatch(gameObject)
            ];
            _internalTarkyPatchContext.EnablePatches(internalToApply);
            Logger.LogDebug("Internal patches enabled.");

            Logger.LogDebug("TarkyToolkit initialized.");
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to initialize TarkyToolkit.");
            Logger.LogError(e.ToString());

            if (_internalTarkyPatchContext)
            {
                Destroy(_internalTarkyPatchContext);
                _internalTarkyPatchContext = null!;
            }
            if (_tarkyPatchContext)
            {
                Destroy(_tarkyPatchContext);
                _tarkyPatchContext = null!;
            }
            if (_tarkovContext)
            {
                Destroy(_tarkovContext);
                _tarkovContext = null!;
            }
        }
    }
}
