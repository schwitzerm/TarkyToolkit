using TarkyToolkit.Core.Logging;

namespace TarkyToolkit.Core.Context;

public interface ITarkovContext
{
    abstract Logger Logger { get; }
    EFT.GameWorld GameWorld { get; set; }
}
