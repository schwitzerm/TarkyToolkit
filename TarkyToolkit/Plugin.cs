using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using TarkyToolkit.Context;
using TarkyToolkit.Patch;
using TarkyToolkit.Shared;

namespace TarkyToolkit;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.SPT.core", "3.11.0")]
public class Plugin : BaseUnityPlugin
{
    // ReSharper disable once MemberCanBePrivate.Global
    internal static Harmony Harmony = null!;
    internal new static ILogger Logger = null!;
    public static TarkovContext TarkovContext = null!;
    private GameWorldInitPatch _gameWorldInitPatch = null!;

    [UsedImplicitly]
    private void Awake()
    {
        Harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        Logger = new BepLogger(base.Logger);
        TarkovContext = gameObject.AddComponent<TarkovContext>();
        DontDestroyOnLoad(TarkovContext);

        _gameWorldInitPatch = new GameWorldInitPatch();
        _gameWorldInitPatch.Enable();

        //TarkovContext.InitializeHooks();
    }
}
