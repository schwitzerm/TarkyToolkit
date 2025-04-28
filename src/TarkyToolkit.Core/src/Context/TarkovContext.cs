using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Core.Context;

public class TarkovContext : MonoBehaviour, ITarkovContext
{
    public Logger Logger { get; }
    public EFT.GameWorld? GameWorld { get; set; }

    public TarkovContext()
    {
        Logger = TarkyPlugin.Logger;
    }
}
