using TarkyToolkit.Core.Context;
using UnityEngine;
using ILogger = TarkyToolkit.Shared.Logging.ILogger;

namespace TarkyToolkit.Api
{
    public abstract class TarkovApi : MonoBehaviour
    {
        internal static TarkovContext TarkovContext { get; private set; }
        internal static ILogger Logger { get; private set; }

        protected TarkovApi() : this(TarkyToolkitApiPlugin.Logger)
        {
            TarkovContext = gameObject.GetComponent<TarkovContext>();
        }

        protected TarkovApi(ILogger logger)
        {
            TarkovContext = gameObject.GetComponent<TarkovContext>();
            Logger = logger;
        }
    }
}
