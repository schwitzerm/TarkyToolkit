using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Shared.Utils;

namespace TarkyToolkit.Patch.Player;
/// <remarks>
/// Will fail to patch for now as an Awake() method does not exist on EFT.Player.
/// </remarks>
public class AssignOnAwakePatch : ModulePatch
{
    [UsedImplicitly]
    protected override MethodBase GetTargetMethod()
    {
        return TarkyPatchUtils.GetAwakeMethod<EFT.Player>()!;
    }

    [PatchPostfix]
    [UsedImplicitly]
    private static void Postfix(EFT.Player __instance)
    {
        TarkyToolkit.Logger.LogDebug("TarkyToolkit.Patch.Player.AssignOnAwakePatch.Postfix() entered.");
    }
}
