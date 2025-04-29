using System.Collections.Generic;

namespace TarkyToolkit.Core.Api
{
    public class GameWorldApi : TarkovApi
    {
        public List<EFT.Player>? GetAllAlivePlayers
        {
            get
            {
                if (TarkovContext.GameWorld is not null &&
                    TarkovContext.GameWorld.AllAlivePlayersList is not null)
                {
                    return TarkovContext.GameWorld.AllAlivePlayersList;
                }

                return null;
            }
        }

        public IEnumerable<EFT.Player>? GetAllPlayersToEverExist
        {
            get
            {
                if (TarkovContext.GameWorld is not null &&
                    TarkovContext.GameWorld.AllPlayersEverExisted is not null)
                {
                    return TarkovContext.GameWorld.AllPlayersEverExisted;
                }

                return null;
            }
        }

        public EFT.Player? Player
        {
            get
            {
                if (TarkovContext.GameWorld is not null &&
                    TarkovContext.GameWorld.MainPlayer is not null)
                {
                    return TarkovContext.GameWorld.MainPlayer;
                }

                return null;
            }
        }
    }
}
