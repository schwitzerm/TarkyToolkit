using SPT.Reflection.Patching;
using TarkyToolkit.Core.Context;
using UnityEngine;

namespace TarkyToolkit.Core.Patch;

public abstract class TarkyPatch : ModulePatch
{
    public abstract bool FatalOnPatchError { get; set; }
    protected static TarkovContext TarkovContext { get; private set; } = null!;

    protected TarkyPatch(GameObject rootObject)
    {
        TarkovContext = rootObject.GetComponent<TarkovContext>();
    }
}
