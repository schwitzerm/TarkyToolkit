namespace TarkyToolkit.Core.Api;

public class GameWorldApi : TarkovApi
{
    public List<EFT.Player> GetAllAlivePlayers() => TarkovContext.GameWorld?.AllAlivePlayersList ?? [];
    public List<EFT.Player> GetAllPlayersToEverExist() => TarkovContext.GameWorld?.AllPlayersEverExisted?.ToList() ?? [];
    public EFT.Player? Player => TarkovContext?.GameWorld?.MainPlayer;
}
