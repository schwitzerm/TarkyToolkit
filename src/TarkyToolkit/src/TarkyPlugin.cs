using BepInEx;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Core.Logging;

namespace TarkyToolkit;

#pragma warning disable BepInEx001
public abstract class TarkyPlugin : BaseUnityPlugin
#pragma warning restore BepInEx001
{
    public static bool StoppedFatally { get; } = false;
    protected static TarkyPatchContext TarkyPatchContext { get; private set; } = null!;
    protected static TarkovContext TarkovContext { get; private set; } = null!;
    protected new static Logger Logger { get; set; } = null!;

    protected TarkyPlugin()
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
        TarkyPatchContext = gameObject.GetComponent<TarkyPatchContext>();
        Logger = gameObject.GetComponent<Logger>();
    }
}
