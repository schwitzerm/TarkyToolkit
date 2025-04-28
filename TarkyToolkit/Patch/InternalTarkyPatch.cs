using TarkyToolkit.Context;

namespace TarkyToolkit.Patch;

internal abstract class InternalTarkyPatch : TarkyPatch
{
    protected InternalTarkyPatch(TarkovContext context) : base(context)
    {
    }
}
