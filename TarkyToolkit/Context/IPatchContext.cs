using SPT.Reflection.Patching;

namespace TarkyToolkit.Context;

public interface IPatchContext
{
    void EnablePatches(ModulePatch[] toApply);
    void DisablePatches(ModulePatch[] toDisable);
    void DisableAllPatches();
}
