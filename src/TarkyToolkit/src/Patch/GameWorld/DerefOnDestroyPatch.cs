using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using UnityEngine;

namespace TarkyToolkit.Patch.GameWorld;

internal class DerefOnAwakePatch(GameObject rootObject) : InternalTarkyPatch(rootObject)
{
    public override bool FatalOnPatchError => true;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(EFT.GameWorld).GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.Public)!;
    }

    [PatchPostfix]
    [UsedImplicitly]
    private static void Postfix()
    {
        TarkovContext.GameWorld = null;
    }
}
