using BepInEx;
using TarkyToolkit.Core.Context;
using TarkyToolkit.Logging;
using TarkyToolkit.Shared.Logging;

namespace TarkyToolkit.Core
{
#pragma warning disable BepInEx001
    public abstract class TarkyPlugin : BaseUnityPlugin
#pragma warning restore BepInEx001
    {
        public static bool StoppedFatally { get; } = false;
        protected static TarkyPatchContext TarkyPatchContext { get; private set; }
        protected static TarkovContext TarkovContext { get; private set; }
        public new static AsyncLogger Logger { get; private set; }
        public static string Name { get; private set; }

        protected TarkyPlugin()
        {
            TarkovContext = gameObject.GetComponent<TarkovContext>();
            TarkyPatchContext = gameObject.GetComponent<TarkyPatchContext>();
            Logger = gameObject.GetComponent<StreamingLogger>();
            if (Logger == null)
            {
                base.Logger.LogError("Welp.");
            }
            Name = GetType().Name;
        }
    }
}
