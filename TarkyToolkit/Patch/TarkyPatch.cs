using SPT.Reflection.Patching;
using TarkyToolkit.Context;

namespace TarkyToolkit.Patch;

public abstract class TarkyPatch : ModulePatch
{
    public abstract bool FatalOnPatchError { get; }
    internal static TarkovContext TarkovContext { get; private set; } = null!;

    internal TarkyPatch(TarkovContext context)
    {
        TarkovContext = context;
    }
}
