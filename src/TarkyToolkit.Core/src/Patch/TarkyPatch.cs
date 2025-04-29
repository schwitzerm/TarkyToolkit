using SPT.Reflection.Patching;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Shared.Logging;
using TarkyToolkit.Shared.Patch;
using UnityEngine;

namespace TarkyToolkit.Core.Patch
{
    public abstract class TarkyPatch : ModulePatch, ITarkyPatch
    {
        public abstract bool FatalOnPatchError { get; }
        protected static TarkovContext TarkovContext { get; private set; }
        protected new static AsyncLogger Logger { get; private set; }

        protected TarkyPatch(GameObject rootObject)
        {
            TarkovContext = rootObject.GetComponent<TarkovContext>();
            Logger = TarkyToolkitCorePlugin.Logger;
        }
    }
}
