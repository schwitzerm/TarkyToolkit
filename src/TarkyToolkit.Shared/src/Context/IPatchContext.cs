using TarkyToolkit.Shared.Patch;
using UnityEngine;
using Logger = TarkyToolkit.Shared.Logging.Logger;

namespace TarkyToolkit.Shared.Context
{
    public interface IPatchContext<T> where T : ITarkyPatch
    {
        abstract Logger Logger { get; }
        bool PatchesEnabled { get; }
        void EnablePatches(T[] toApply);
        void DisablePatches(T[] toDisable);
        void DisableAllPatches(bool force = false);
    }
}
