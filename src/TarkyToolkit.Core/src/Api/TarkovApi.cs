using TarkyToolkit.Core.Context;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Core.Api;

public abstract class TarkovApi : MonoBehaviour
{
    internal static TarkovContext TarkovContext { get; private set; } = null!;
    internal static Logger Logger { get; private set; } = null!;

    protected TarkovApi() : this(TarkyToolkitCorePlugin.Logger)
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
    }

    protected TarkovApi(Logger logger)
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
        Logger = logger;
    }
}
