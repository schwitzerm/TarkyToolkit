using System;
using System.Collections.Generic;
using TarkyToolkit.Patch;
using UnityEngine;

namespace TarkyToolkit.Context;

public class TarkyPatchContext : MonoBehaviour, IPatchContext<TarkyPatch>
{
    private readonly Dictionary<string, TarkyPatch> _appliedPatches = new();
    public bool PatchesApplied { get; private set; }

    public void EnablePatches(TarkyPatch[] toApply)
    {
        if (PatchesApplied)
        {
            throw new InvalidOperationException("Patches already applied.");
        }

        foreach (var patch in toApply)
        {
            var patchName = patch.GetType().FullName;
            try
            {
                TarkyToolkit.Logger.LogDebug($"Applying patch {patchName}.");

                patch.Enable();
                _appliedPatches.Add(patchName, patch);

                TarkyToolkit.Logger.LogDebug($"Successfully applied patch {patchName}.");
            }
            catch (Exception e)
            {
                TarkyToolkit.Logger.LogWarning($"Failed to apply patch {patchName}.");
                if (patch.FatalOnPatchError)
                {
                    TarkyToolkit.Logger.LogError($"Patch {patchName} is marked as fatal on patch error.");
                    TarkyToolkit.Logger.LogError("Disabling all imported TarkyPatch instances.");
                    TarkyToolkit.Logger.LogError(e.ToString());
                    DisableAllPatches(true);
                    return;
                }
                TarkyToolkit.Logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                TarkyToolkit.Logger.LogWarning(e.ToString());
            }
        }

        PatchesApplied = true;
    }

    public void DisablePatches(TarkyPatch[] toDisable)
    {
        throw new System.NotImplementedException();
    }

    public void DisableAllPatches(bool force = false)
    {
        if (!PatchesApplied && !force)
        {
            TarkyToolkit.Logger.LogWarning("Attempted to disable all patches, but patches have not been applied yet!");
            return;
        }

        TarkyToolkit.Logger.LogDebug("Disabling all imported patches.");
        foreach (var patch in _appliedPatches.Values)
        {
            patch.Disable();
        }
        _appliedPatches.Clear();
        TarkyToolkit.Logger.LogDebug("All imported patches disabled.");
    }
}
