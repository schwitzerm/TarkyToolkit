using System;
using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Core.Patch;
using TarkyToolkit.Core.Utils;
using UnityEngine;

namespace TarkyToolkit.TestSoftFailPlugin.Patch;

public class SoftFailPatch(GameObject rootObject) : TarkyPatch(rootObject)
{
    public override bool FatalOnPatchError => false;

    protected override MethodBase GetTargetMethod()
    {
        // not possible, will fail.
        return TarkyPatchUtils.GetAwakeMethod<EFT.Player>()!;
    }

    [PatchPostfix]
    [UsedImplicitly]
    private static void Postfix()
    {
        throw new NotImplementedException();
    }
}
