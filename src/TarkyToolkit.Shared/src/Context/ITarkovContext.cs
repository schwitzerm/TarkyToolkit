using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Shared.Context
{
    public interface ITarkovContext
    {
        abstract Logger Logger { get; }
        EFT.GameWorld? GameWorld { get; set; }
    }
}
