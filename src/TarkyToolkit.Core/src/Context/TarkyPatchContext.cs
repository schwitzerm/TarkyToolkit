using TarkyToolkit.Core.Patch;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Core.Context;

public class TarkyPatchContext(Logger logger) : MonoBehaviour, IPatchContext<TarkyPatch>
{
    private readonly Dictionary<string, TarkyPatch> _appliedPatches = new();
    public bool PatchesEnabled { get; private set; }

    public void EnablePatches(TarkyPatch[] toApply)
    {
        if (PatchesEnabled)
        {
            throw new InvalidOperationException("Patches already applied.");
        }

        foreach (var patch in toApply)
        {
            var patchName = patch.GetType().FullName;
            try
            {
                logger.LogDebug($"Applying patch {patchName}.");

                patch.Enable();
                _appliedPatches.Add(patchName, patch);

                logger.LogDebug($"Successfully applied patch {patchName}.");
            }
            catch (Exception e)
            {
                logger.LogWarning($"Failed to apply patch {patchName}.");
                if (patch.FatalOnPatchError)
                {
                    logger.LogError($"Patch {patchName} is marked as fatal on patch error.");
                    logger.LogError("Disabling all imported TarkyPatch instances.");
                    logger.LogError(e.ToString());
                    DisableAllPatches(true);
                    return;
                }
                logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                logger.LogWarning(e.ToString());
            }
        }

        PatchesEnabled = true;
    }

    public void DisablePatches(TarkyPatch[] toDisable)
    {
        throw new NotImplementedException();
    }

    public void DisableAllPatches(bool force = false)
    {
        if (!PatchesEnabled && !force)
        {
            logger.LogWarning("Attempted to disable all patches, but patches have not been applied yet!");
            return;
        }

        logger.LogDebug("Disabling all imported patches.");
        foreach (var patch in _appliedPatches.Values)
        {
            patch.Disable();
        }
        _appliedPatches.Clear();
        logger.LogDebug("All imported patches disabled.");
    }
}
