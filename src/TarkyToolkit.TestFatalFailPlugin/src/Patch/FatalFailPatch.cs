using System;
using System.Reflection;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Core.Patch;
using UnityEngine;

namespace TarkyToolkit.TestFatalFailPlugin.Patch
{
    public class FatalFailPatch : TarkyPatch
    {
        public FatalFailPatch(GameObject rootObject) : base(rootObject)
        {
        }

        public override bool FatalOnPatchError => true;

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
}
