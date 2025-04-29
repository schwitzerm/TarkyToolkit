using TarkyToolkit.Core.Api;
using TarkyToolkit.Core.Logging;

namespace TarkyToolkit.Core.Context
{
    public interface ITarkovContext
    {
        abstract Logger Logger { get; }
        GameWorldApi GameWorldApi { get; }
        EFT.GameWorld? GameWorld { get; set; }
    }
}
