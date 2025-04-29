using TarkyToolkit.Core.Context;
using TarkyToolkit.Shared.Logging;
using UnityEngine;

namespace TarkyToolkit.Api
{
    public abstract class TarkovApi : MonoBehaviour
    {
        internal static TarkovContext TarkovContext { get; private set; }
        internal static AsyncLogger Logger { get; private set; }

        protected TarkovApi() : this(TarkyToolkitApiPlugin.Logger)
        {
            TarkovContext = gameObject.GetComponent<TarkovContext>();
        }

        protected TarkovApi(AsyncLogger logger)
        {
            TarkovContext = gameObject.GetComponent<TarkovContext>();
            Logger = logger;
        }
    }
}
