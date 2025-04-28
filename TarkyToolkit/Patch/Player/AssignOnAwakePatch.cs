using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Context;
using TarkyToolkit.Shared.Utils;

namespace TarkyToolkit.Patch.Player;
/// <remarks>
/// Will fail to patch for now as an Awake() method does not exist on EFT.Player.
/// Normally we'd just fetch the player through the GameWorld API, but this is designed to fail for testing. :)
/// Although, I guess we can replace it with a patch designed specifically for that purpose...
/// </remarks>
internal class AssignOnAwakePatch : InternalTarkyPatch
{
    public override bool FatalOnPatchError => false;

    public AssignOnAwakePatch(TarkovContext context) : base(context)
    {
    }

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
