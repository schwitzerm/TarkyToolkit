using System;
using System.Reflection;
using EFT;
using HarmonyLib;
using TarkyToolkit.Reflection;
using UnityEngine;
using ILogger = TarkyToolkit.Shared.ILogger;

namespace TarkyToolkit.Context;

public class TarkovContext : MonoBehaviour, ITarkovContext
{
    private readonly Harmony _harmony = new(MyPluginInfo.PLUGIN_GUID);

    private MonoBehaviourHook<GameWorld> _gameWorldHook = null!;
    public GameWorld? GameWorld => _gameWorldHook.Target;
    public bool HooksInitialized { get; private set; } = false;

    public void InitializeHooks(Harmony harmony, ILogger logger)
    {
        if (HooksInitialized)
        {
            throw new InvalidOperationException("Hooks already initialized.");
        }

        _gameWorldHook = new MonoBehaviourHook<GameWorld>(harmony, logger);
        try
        {
            _gameWorldHook.Hook(
                GameWorldOnAwake,
                "Awake",
                BindingFlags.Instance | BindingFlags.Public,
                HarmonyPatchType.Postfix);
            _gameWorldHook.Hook(
                GameWorldOnDestroy,
                "Destroy",
                BindingFlags.Instance | BindingFlags.Public,
                HarmonyPatchType.Postfix);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError("Failed to initialize hooks.");
            Plugin.Logger.LogError(e.ToString());
        }
        HooksInitialized = true;
    }

    // ReSharper disable once InconsistentNaming
    public void GameWorldOnAwake(GameWorld __instance)
    {
        Plugin.Logger.LogDebug("GameWorldOnAwake()");
        _gameWorldHook.Target = __instance;
    }

    public void GameWorldOnDestroy()
    {
        Plugin.Logger.LogDebug("GameWorldOnDestroy()");
    }
}
