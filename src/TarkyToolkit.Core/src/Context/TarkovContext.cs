using EFT;
using TarkyToolkit.Shared.Context;
using TarkyToolkit.Shared.Logging;
using UnityEngine;

namespace TarkyToolkit.Core.Context
{
    public class TarkovContext : MonoBehaviour, ITarkovContext
    {
        public AsyncLogger Logger { get; } = TarkyPlugin.Logger;
        public GameWorld GameWorld { get; set; }
    }
}
