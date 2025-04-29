using TarkyToolkit.Core.Api;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Core.Context;

public class TarkovContext : MonoBehaviour, ITarkovContext
{
    public Logger Logger { get; }
    public EFT.GameWorld GameWorld { get; set; }
    public GameWorldApi GameWorldApi { get; }

    public TarkovContext()
    {
        Logger = TarkyPlugin.Logger;
        GameWorldApi = gameObject.AddComponent<GameWorldApi>();
        DontDestroyOnLoad(GameWorldApi);
    }
}
