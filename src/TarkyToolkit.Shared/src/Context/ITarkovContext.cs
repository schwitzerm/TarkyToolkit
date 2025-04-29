using EFT;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Shared.Context
{
    public interface ITarkovContext
    {
        AsyncLogger Logger { get; }
        GameWorld GameWorld { get; set; }
    }
}
