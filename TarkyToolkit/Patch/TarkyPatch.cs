using SPT.Reflection.Patching;
using TarkyToolkit.Context;
using UnityEngine;

namespace TarkyToolkit.Patch;

public abstract class TarkyPatch : ModulePatch
{
    public abstract bool FatalOnPatchError { get; }
    protected static TarkovContext TarkovContext { get; private set; } = null!;

    protected TarkyPatch(GameObject rootObject)
    {
        TarkovContext = rootObject.GetComponent<TarkovContext>();
    }
}
