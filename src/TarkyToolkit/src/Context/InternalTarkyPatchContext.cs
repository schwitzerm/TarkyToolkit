using System;
using System.Collections.Generic;
using TarkyToolkit.Patch;
using UnityEngine;

namespace TarkyToolkit.Context;

internal class InternalTarkyPatchContext : MonoBehaviour, IPatchContext<InternalTarkyPatch>
{
    private readonly Dictionary<string, InternalTarkyPatch> _enabledPatches = new();
    public bool PatchesEnabled { get; private set; }

    public void EnablePatches(InternalTarkyPatch[] toApply)
    {
        if (PatchesEnabled)
        {
            throw new InvalidOperationException("Internal patches are already applied.");
        }

        foreach (var patch in toApply)
        {
            var patchName = patch.GetType().FullName;
            try
            {
                TarkyToolkit.Logger.LogDebug($"Applying internal patch {patchName}.");

                patch.Enable();
                _enabledPatches.Add(patchName, patch);

                TarkyToolkit.Logger.LogDebug($"Successfully applied internal patch {patchName}.");
            }
            catch (Exception e)
            {
                TarkyToolkit.Logger.LogWarning($"Failed to apply patch {patchName}.");
                if (patch.FatalOnPatchError)
                {
                    TarkyToolkit.Logger.LogError($"Patch {patchName} is internal and marked as FatalOnPatchError.");
                    TarkyToolkit.Logger.LogError("Destroying all TarkyPatch instances and disabling TarkyToolkit.");
                    TarkyToolkit.Logger.LogError(e.ToString());
                    DisableAllPatches(true);
                    throw;
                }
                TarkyToolkit.Logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                TarkyToolkit.Logger.LogWarning(e.ToString());
            }
        }

        PatchesEnabled = true;
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
