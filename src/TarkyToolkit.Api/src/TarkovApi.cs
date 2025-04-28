using TarkyToolkit.Core.Context;

namespace TarkyToolkit.Api;

public abstract class TarkovApi : MonoBehaviourSingleton<TarkovApi>
{
    internal static TarkovContext TarkovContext { get; private set; } = null!;

    protected TarkovApi()
    {
        TarkovContext = gameObject.GetComponent<TarkovContext>();
    }
}
