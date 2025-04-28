using System;
using System.Collections.Generic;
using SPT.Reflection.Patching;
using TarkyToolkit.Patch;
using UnityEngine;

namespace TarkyToolkit.Context;

public class PatchContext : MonoBehaviour, IPatchContext
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
            try
            {
                TarkyToolkit.Logger.LogDebug($"Applying patch {patch.GetType().FullName}.");

                patch.Enable();
                _appliedPatches.Add(patch.GetType().FullName, patch);

                TarkyToolkit.Logger.LogDebug($"Successfully applied patch {patch.GetType().FullName}.");
            }
            catch (Exception e)
            {
                TarkyToolkit.Logger.LogWarning($"Failed to apply patch {patch.GetType().FullName}.");
                if (patch.FatalOnPatchError)
                {
                    TarkyToolkit.Logger.LogError($"Patch {patch.GetType().FullName} is marked as fatal on patch error, disabling TarkyToolkit all patches.");
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

        TarkyToolkit.Logger.LogDebug("Disabling all patches.");
        foreach (var patch in _appliedPatches.Values)
        {
            patch.Disable();
        }
        _appliedPatches.Clear();
        TarkyToolkit.Logger.LogDebug("All patches disabled.");
    }
}
