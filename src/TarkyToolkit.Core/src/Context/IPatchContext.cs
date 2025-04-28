using TarkyToolkit.Core.Logging;
using TarkyToolkit.Core.Patch;
using UnityEngine;

namespace TarkyToolkit.Core.Context;

public interface IPatchContext<T> where T : TarkyPatch
{
    abstract ILogger Logger { get; }
    bool PatchesEnabled { get; }
    void EnablePatches(T[] toApply);
    void DisablePatches(T[] toDisable);
    void DisableAllPatches(bool force = false);
}
