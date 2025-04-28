using TarkyToolkit.Context;

namespace TarkyToolkit.Api;

public abstract class TarkovApi
{
    internal static TarkovContext TarkovContext { get; private set; } = null!;

    internal TarkovApi(TarkovContext tarkovContext)
    {
        TarkovContext = tarkovContext;
    }
}
