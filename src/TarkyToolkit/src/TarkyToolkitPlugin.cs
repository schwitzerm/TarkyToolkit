using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Context;
using TarkyToolkit.Patch;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Core.Logging;

namespace TarkyToolkit;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.SPT.core", "3.11.0")]
[BepInDependency("Mellow_.TarkyToolkit.Api", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit.Core", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit.Reflection", "0.1.0")]
[BepInProcess("EscapeFromTarkov.exe")]
public class TarkyToolkitPlugin : BaseUnityPlugin
{
    private static InternalTarkyPatchContext _internalTarkyPatchContext = null!;
    private static TarkyPatchContext _tarkyPatchContext = null!;
    private static TarkovContext _tarkovContext = null!;
    internal new static Logger Logger { get; private set; } = null!;

    [UsedImplicitly]
    private void Awake()
    {
        try
        {
            Logger = new BepLogger(base.Logger);
            Logger.LogDebug("Initializing TarkyToolkit core modules.");
            _internalTarkyPatchContext = gameObject.AddComponent<InternalTarkyPatchContext>();
            _tarkyPatchContext = gameObject.AddComponent<TarkyPatchContext>();
            _tarkovContext = gameObject.AddComponent<TarkovContext>();
            DontDestroyOnLoad(_internalTarkyPatchContext);
            DontDestroyOnLoad(_tarkyPatchContext);
            DontDestroyOnLoad(_tarkovContext);

            Logger.LogDebug("Enabling internal patches.");
            InternalTarkyPatch[] internalToApply =
            [
                new Patch.GameWorld.AssignOnAwakePatch(gameObject)
            ];
            _internalTarkyPatchContext.EnablePatches(internalToApply);

            Logger.LogDebug("TarkyToolkit core modules initialized.");
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
