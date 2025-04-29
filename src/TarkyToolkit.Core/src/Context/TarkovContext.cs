using EFT;
using TarkyToolkit.Shared.Context;
using UnityEngine;
using ILogger = TarkyToolkit.Shared.Logging.ILogger;

namespace TarkyToolkit.Core.Context
{
    public class TarkovContext : MonoBehaviour, ITarkovContext
    {
        public ILogger Logger { get; } = TarkyPlugin.Logger;
        public GameWorld GameWorld { get; set; }
    }
}
