using System;
using BepInEx;
using JetBrains.Annotations;
using TarkyToolkit.Context;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Logging;
using TarkyToolkit.Patch;
using TarkyToolkit.Patch.GameWorld;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit
{
    [BepInPlugin("Mellow_.TarkyToolkit", "TarkyToolkit", "0.1.0")]
    [BepInDependency("com.SPT.core", "3.11.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Api", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Core", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Logging", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Reflection", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Shared", "0.1.0")]
    [BepInDependency("Mellow_.TarkyToolkit.Unity", "0.1.0")]
    [BepInProcess("EscapeFromTarkov.exe")]
    public class TarkyToolkitPlugin : BaseUnityPlugin
    {
        private static InternalTarkyPatchContext _internalTarkyPatchContext;
        private static TarkyPatchContext _tarkyPatchContext;
        private static TarkovContext _tarkovContext;
        private static ThreadSafeLogger _localLogger;
        internal new static AsyncLogger Logger { get; private set; }

        [UsedImplicitly]
        private void Awake()
        {
            try
            {
                _localLogger = gameObject.AddComponent<BepLogger>();
                _localLogger.SetupProcessing(this, base.Logger);
                DontDestroyOnLoad(_localLogger);
                Logger = gameObject.AddComponent<StreamingLogger>();
                Logger.SetupProcessing(this, "localhost:22322", _localLogger);
                DontDestroyOnLoad(Logger);

                Logger.LogDebug("Initializing core modules.");
                _internalTarkyPatchContext = gameObject.AddComponent<InternalTarkyPatchContext>();
                _tarkyPatchContext = gameObject.AddComponent<TarkyPatchContext>();
                _tarkovContext = gameObject.AddComponent<TarkovContext>();
                DontDestroyOnLoad(_internalTarkyPatchContext);
                DontDestroyOnLoad(_tarkyPatchContext);
                DontDestroyOnLoad(_tarkovContext);
                Logger.LogDebug("Core modules initialized.");

                Logger.LogDebug("Enabling internal patches.");
                _internalTarkyPatchContext.EnablePatches(new InternalTarkyPatch[] {
                    new RefOnAwakePatch(gameObject),
                    new DerefOnDestroyPatch(gameObject)
                });
                Logger.LogDebug("Internal patches enabled.");
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to initialize!");
                Logger.LogError(e.ToString());

                if (_internalTarkyPatchContext)
                {
                    Destroy(_internalTarkyPatchContext);
                    _internalTarkyPatchContext = null;
                }
                if (_tarkyPatchContext)
                {
                    Destroy(_tarkyPatchContext);
                    _tarkyPatchContext = null;
                }
                if (_tarkovContext)
                {
                    Destroy(_tarkovContext);
                    _tarkovContext = null;
                }
            }
        }
    }
}
