using TarkyToolkit.Shared.Context;
using UnityEngine;
using Logger = TarkyToolkit.Shared.Logging.Logger;

namespace TarkyToolkit.Core.Context
{
    public class TarkovContext : MonoBehaviour, ITarkovContext
    {
        public Logger Logger { get; }
        public EFT.GameWorld? GameWorld { get; set; }

        public TarkovContext()
        {
            Logger = TarkyPlugin.Logger;
        }
    }
}
