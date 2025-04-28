using System.Collections.Generic;
using System.Linq;

namespace TarkyToolkit.Api;

public class GameWorldApi : TarkovApi
{
    public static List<EFT.Player> AllAlivePlayers => TarkovContext.GameWorld?.AllAlivePlayersList ?? [];
    public static List<EFT.Player> AllPlayersToEverExist => TarkovContext.GameWorld?.AllPlayersEverExisted?.ToList() ?? [];
    public static EFT.Player? Player => TarkovContext.GameWorld?.MainPlayer;
}
