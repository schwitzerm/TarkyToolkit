using SPT.Reflection.Patching;
using TarkyToolkit.Patch;

namespace TarkyToolkit.Context;

public interface IPatchContext
{
    void EnablePatches(TarkyPatch[] toApply);
    void DisablePatches(TarkyPatch[] toDisable);
    void DisableAllPatches(bool force = false);
}
