using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Context;
using TarkyToolkit.Patch;
using TarkyToolkit.Shared.Utils;
using UnityEngine;

namespace TarkyToolkit.TestPlugin.Patch.Player;
/// <remarks>
/// Will fail to patch for now as an Awake() method does not exist on EFT.Player.
/// Normally we'd just fetch the player through the GameWorld API, but this is designed to fail for testing. :)
/// Although, I guess we can replace it with a patch designed specifically for that purpose...
/// </remarks>
internal class AssignOnAwakePatch : TarkyPatch
{
    public AssignOnAwakePatch(GameObject rootObject) : base(rootObject)
    {
    }

    public override bool FatalOnPatchError => false;

    [UsedImplicitly]
    protected override MethodBase GetTargetMethod()
    {
        return TarkyPatchUtils.GetAwakeMethod<EFT.Player>()!;
    }

    [PatchPostfix]
    [UsedImplicitly]
    private static void Postfix(EFT.Player __instance)
    {
        TarkyPlugin.Logger.LogDebug("TarkyToolkit.Patch.Player.AssignOnAwakePatch.Postfix() entered.");
    }
}
