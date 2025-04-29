using SPT.Reflection.Patching;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Core.Logging;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Core.Patch;

public abstract class TarkyPatch : ModulePatch
{
    public abstract bool FatalOnPatchError { get; }
    protected static TarkovContext TarkovContext { get; private set; } = null!;
    protected new static Logger Logger { get; private set; } = null!;

    protected TarkyPatch(GameObject rootObject)
    {
        TarkovContext = rootObject.GetComponent<TarkovContext>();
        Logger = TarkyToolkitCorePlugin.Logger;
    }
}
