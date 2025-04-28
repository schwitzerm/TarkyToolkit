namespace TarkyToolkit.Core.Context;

public interface ITarkovContext
{
    EFT.GameWorld? GameWorld { get; set; }
}
