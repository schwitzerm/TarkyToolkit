using System.Reflection;
using EFT;
using SPT.Reflection.Patching;

namespace TarkyToolkit.Patch;

public class GameWorldInitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    [PatchPostfix]
    // ReSharper disable once InconsistentNaming
    private static void Postfix(GameWorld __instance)
    {
        Plugin.Logger.LogDebug("GameWorldInitPatch.Postfix() on Awake()");
        if (__instance == null)
        {
            Plugin.Logger.LogWarning("GameWorld reference is null on Awake. Something is wrong!");
        }
    }
}
