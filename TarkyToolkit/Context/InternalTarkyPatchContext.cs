using TarkyToolkit.Patch;
using UnityEngine;

namespace TarkyToolkit.Context;

internal class InternalTarkyPatchContext : MonoBehaviour, IPatchContext<InternalTarkyPatch>
{
    public void EnablePatches(InternalTarkyPatch[] toApply)
    {
        throw new System.NotImplementedException();
    }

    public void DisablePatches(InternalTarkyPatch[] toDisable)
    {
        throw new System.NotImplementedException();
    }

    public void DisableAllPatches(bool force = false)
    {
        throw new System.NotImplementedException();
    }
}
