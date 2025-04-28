using System;
using System.Collections.Generic;
using TarkyToolkit.Patch;
using TarkyToolkit.Core.Context;
using UnityEngine;
using ILogger = TarkyToolkit.Core.Logging.ILogger;

namespace TarkyToolkit.Context;

internal class InternalTarkyPatchContext(ILogger logger) : MonoBehaviour, IPatchContext<InternalTarkyPatch>
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
                logger.LogDebug($"Applying internal patch {patchName}.");

                patch.Enable();
                _enabledPatches.Add(patchName, patch);

                logger.LogDebug($"Successfully applied internal patch {patchName}.");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed to apply patch {patchName}.");
                if (patch.FatalOnPatchError)
                {
                    logger.LogError($"Patch {patchName} is internal and marked as FatalOnPatchError.");
                    logger.LogError("Destroying all TarkyPatch instances and disabling TarkyToolkit.");
                    logger.LogError(e.ToString());
                    DisableAllPatches(true);
                    throw;
                }
                logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                logger.LogWarning(e.ToString());
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
