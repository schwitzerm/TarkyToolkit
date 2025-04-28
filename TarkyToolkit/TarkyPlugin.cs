using BepInEx;
using TarkyToolkit.Context;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit;

#pragma warning disable BepInEx001
public abstract class TarkyPlugin : BaseUnityPlugin
#pragma warning restore BepInEx001
{
    protected static TarkyPatchContext TarkyPatchContext { get; private set; } = null!;
    protected static TarkovContext TarkovContext { get; private set; } = null!;
    public new static ILogger Logger { get; set; } = null!;

    protected TarkyPlugin()
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
        TarkyPatchContext = gameObject.GetComponent<TarkyPatchContext>();
        Logger = new BepLogger(base.Logger);
    }
}
