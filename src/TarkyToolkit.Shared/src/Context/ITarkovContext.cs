using EFT;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Shared.Context
{
    public interface ITarkovContext
    {
        ILogger Logger { get; }
        GameWorld GameWorld { get; set; }
    }
}
