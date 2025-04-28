using BepInEx;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Core.Logging;

namespace TarkyToolkit.Core;

#pragma warning disable BepInEx001
public abstract class TarkyPlugin : BaseUnityPlugin
#pragma warning restore BepInEx001
{
    public static bool StoppedFatally { get; } = false;
    protected static TarkyPatchContext TarkyPatchContext { get; private set; } = null!;
    protected static TarkovContext TarkovContext { get; private set; } = null!;
    public new static Logger Logger { get; private set; } = null!;
    public static string Name { get; private set; } = "";

    protected TarkyPlugin()
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
        TarkyPatchContext = gameObject.GetComponent<TarkyPatchContext>();
        Logger = new BepLogger(base.Logger);
        Name = GetType().Name;
    }

    protected TarkyPlugin(Logger logger)
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
        TarkyPatchContext = gameObject.GetComponent<TarkyPatchContext>();
        Logger = logger;
        Name = GetType().Name;
    }
}
