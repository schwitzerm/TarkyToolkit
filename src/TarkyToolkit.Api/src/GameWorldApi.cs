using System.Collections.Generic;
using EFT;

namespace TarkyToolkit.Api
{
    public class GameWorldApi : TarkovApi
    {
        public List<Player> GetAllAlivePlayers
        {
            get
            {
                if (TarkovContext.GameWorld && TarkovContext.GameWorld.AllAlivePlayersList != null)
                {
                    return TarkovContext.GameWorld.AllAlivePlayersList;
                }

                return null;
            }
        }

        public IEnumerable<Player> GetAllPlayersToEverExist
        {
            get
            {
                if (TarkovContext.GameWorld && TarkovContext.GameWorld.AllPlayersEverExisted != null)
                {
                    return TarkovContext.GameWorld.AllPlayersEverExisted;
                }

                return null;
            }
        }

        public Player Player
        {
            get
            {
                if (TarkovContext.GameWorld && TarkovContext.GameWorld.MainPlayer)
                {
                    return TarkovContext.GameWorld.MainPlayer;
                }

                return null;
            }
        }
    }
}
