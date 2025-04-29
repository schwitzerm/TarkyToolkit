using SPT.Reflection.Patching;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Shared.Patch;
using UnityEngine;
using ILogger = TarkyToolkit.Shared.Logging.ILogger;

namespace TarkyToolkit.Core.Patch
{
    public abstract class TarkyPatch : ModulePatch, ITarkyPatch
    {
        public abstract bool FatalOnPatchError { get; }
        protected static TarkovContext TarkovContext { get; private set; }
        protected new static ILogger Logger { get; private set; }

        protected TarkyPatch(GameObject rootObject)
        {
            TarkovContext = rootObject.GetComponent<TarkovContext>();
            Logger = TarkyToolkitCorePlugin.Logger;
        }
    }
}
