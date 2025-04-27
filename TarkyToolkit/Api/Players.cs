using System;
using System.Collections.Generic;
using BepInEx.Logging;
using EFT;
using TarkyToolkit.Context;

namespace TarkyToolkit.Api;

public class Players(ManualLogSource logger)
{
    public Player? GetPlayer(ITarkovContext context)
    {
        logger.LogDebug("GetPlayer");
        return null;
    }

    public Player? GetBot(ITarkovContext context, int id)
    {
        logger.LogDebug("GetBot");
        return null;
    }

    public List<Player> GetBots(ITarkovContext context)
    {
        logger.LogDebug("GetBots");
        return [];
    }
}
