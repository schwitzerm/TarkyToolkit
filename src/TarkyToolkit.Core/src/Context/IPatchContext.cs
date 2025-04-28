using TarkyToolkit.Core.Patch;

namespace TarkyToolkit.Core.Context;

public interface IPatchContext<T> where T : TarkyPatch
{
    bool PatchesEnabled { get; }
    void EnablePatches(T[] toApply);
    void DisablePatches(T[] toDisable);
    void DisableAllPatches(bool force = false);
}
