using System.Reflection;
using EFT;
using SPT.Reflection.Patching;

namespace TarkyToolkit.Patch;

public class GameWorldInitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }

    [PatchPostfix]
    private static void Postfix(GameWorld __instance)
    {
        Plugin.Logger.LogDebug("GameWorldInitPatch.Postfix()");
    }
}
