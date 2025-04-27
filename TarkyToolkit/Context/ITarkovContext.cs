using EFT;
using HarmonyLib;
using TarkyToolkit.Shared;

namespace TarkyToolkit.Context;

public interface ITarkovContext
{
    GameWorld? GameWorld { get; }
    void InitializeHooks(Harmony harmony, ILogger logger);
}
