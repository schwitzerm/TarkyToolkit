﻿using System;
using System.Reflection;
using EFT;
using JetBrains.Annotations;
using SPT.Reflection.Patching;
using TarkyToolkit.Core.Patch;
using TarkyToolkit.Shared.Utils;
using UnityEngine;

namespace TarkyToolkit.TestSoftFailPlugin.Patch
{
    public class SoftFailPatch : TarkyPatch
    {
        public SoftFailPatch(GameObject rootObject) : base(rootObject)
        {
        }

        public override bool FatalOnPatchError => false;

        protected override MethodBase GetTargetMethod()
        {
            // not possible, will fail.
            return TarkyPatchUtils.GetAwakeMethod<Player>();
        }

        [PatchPostfix]
        [UsedImplicitly]
        private static void Postfix()
        {
            throw new NotImplementedException();
        }
    }
}
