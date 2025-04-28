using System;
using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Patch;
using UnityEngine;

namespace TarkyToolkit.TestPlugin.Patch.Player;

public class NonFatalFailingPatch(GameObject rootObject) : TarkyPatch(rootObject)
{
    public override bool FatalOnPatchError { get; set; } = false;

    protected override MethodBase GetTargetMethod()
    {
        // not possible, will fail.
        return typeof(EFT.Player).GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public)!;
    }

    [PatchPostfix]
    [UsedImplicitly]
    private static void Postfix()
    {
        throw new NotImplementedException();
    }
}
