using System;
using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Context;
using TarkyToolkit.Shared.Utils;

namespace TarkyToolkit.Patch.GameWorld;

/// <summary>
/// When Unity awakes the EFT GameWorld object, we assign it to our TarkovContext's GameWorld.
/// </summary>
/// <remarks>
/// Is fatal on error and will disable and destroy ALL TarkyPatch instances should it fail to apply.
/// </remarks>
internal class AssignOnAwakePatch : InternalTarkyPatch
{
    public override bool FatalOnPatchError => true;

    public AssignOnAwakePatch(TarkovContext context) : base(context)
    {
    }

    [UsedImplicitly]
    protected override MethodBase GetTargetMethod()
    {
        return TarkyPatchUtils.GetAwakeMethod<EFT.GameWorld>()!;
    }

    [PatchPostfix]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    private static void Postfix(EFT.GameWorld __instance)
    {
        TarkyToolkit.Logger.LogDebug("TarkyToolkit.Patch.GameWorld.AssignOnAwakePatch.Postfix() entered.");
        if (__instance == null)
        {
            throw new NullReferenceException("GameWorld reference is null on Awake. Something is seriously wrong!");
        }

        if (TarkovContext == null)
        {
            throw new NullReferenceException("TarkovContext reference is null.");
        }

        TarkovContext.GameWorld = __instance;
        TarkyToolkit.Logger.LogDebug("Exiting TarkyToolkit.Patch.GameWorld.AssignOnAwakePatch.Postfix().");
    }
}
