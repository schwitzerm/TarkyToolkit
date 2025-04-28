using System;
using System.Collections.Generic;
using TarkyToolkit.Patch;
using TarkyToolkit.Core.Context;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Context;

internal class InternalTarkyPatchContext : MonoBehaviour, IPatchContext<InternalTarkyPatch>
{
    private readonly Dictionary<string, InternalTarkyPatch> _enabledPatches = new();
    public bool PatchesEnabled { get; private set; }
    public Logger Logger => TarkyToolkitPlugin.Logger;

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
                Logger.LogDebug($"Applying internal patch {patchName}.");

                patch.Enable();
                _enabledPatches.Add(patchName, patch);

                Logger.LogDebug($"Successfully applied internal patch {patchName}.");
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Failed to apply patch {patchName}.");
                if (patch.FatalOnPatchError)
                {
                    Logger.LogError($"Patch {patchName} is internal and marked as FatalOnPatchError.");
                    Logger.LogError("Destroying all TarkyPatch instances and disabling TarkyToolkit.");
                    Logger.LogError(e.ToString());
                    DisableAllPatches(true);
                    throw;
                }
                Logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                Logger.LogWarning(e.ToString());
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
