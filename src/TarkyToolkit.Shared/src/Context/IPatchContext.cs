using TarkyToolkit.Shared.Logging;
using TarkyToolkit.Shared.Patch;

namespace TarkyToolkit.Shared.Context
{
    public interface IPatchContext<T> where T : ITarkyPatch
    {
        AsyncLogger Logger { get; }
        bool PatchesEnabled { get; }
        void EnablePatches(T[] toApply);
        void DisablePatches(T[] toDisable);
        void DisableAllPatches(bool force = false);
    }
}
