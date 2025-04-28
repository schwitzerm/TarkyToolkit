using System;
using System.Collections.Generic;
using SPT.Reflection.Patching;
using TarkyToolkit.Patch;
using UnityEngine;

namespace TarkyToolkit.Context;

public class PatchContext : MonoBehaviour, IPatchContext
{
    public readonly Dictionary<string, TarkyPatch> _appliedPatches = new();
    public bool PatchesEnabled { get; private set; } = false;

    public void EnablePatches(TarkyPatch[] toApply)
    {
        if (PatchesEnabled)
        {
            throw new InvalidOperationException("Hooks already initialized.");
        }

        foreach (var patch in toApply)
        {
            try
            {
                TarkyToolkit.Logger.LogDebug($"Applying patch {patch.GetType().FullName}.");

                patch.Enable();
                _appliedPatches.Add(patch.GetType().FullName, patch);

                TarkyToolkit.Logger.LogDebug($"Successfully enabled patch {patch.GetType().FullName}.");
            }
            catch (Exception e)
            {
                TarkyToolkit.Logger.LogWarning($"Failed to apply patch {patch.GetType().FullName}.");
                TarkyToolkit.Logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                TarkyToolkit.Logger.LogWarning(e.ToString());
            }
        }

        PatchesEnabled = true;
    }

    public void DisablePatches(TarkyPatch[] toDisable)
    {
        throw new System.NotImplementedException();
    }

    public void DisableAllPatches()
    {
        TarkyToolkit.Logger.LogDebug("Disabling all patches.");
        foreach (var patch in _appliedPatches.Values)
        {
            patch.Disable();
        }
        TarkyToolkit.Logger.LogDebug("All patches disabled.");
    }
}
