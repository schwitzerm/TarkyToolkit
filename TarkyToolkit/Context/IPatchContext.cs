using TarkyToolkit.Patch;

namespace TarkyToolkit.Context;

public interface IPatchContext<T> where T : TarkyPatch
{
    void EnablePatches(T[] toApply);
    void DisablePatches(T[] toDisable);
    void DisableAllPatches(bool force = false);
}
