using TarkyToolkit.Core.Exceptions;
using TarkyToolkit.Core.Patch;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Core.Context;

public class TarkyPatchContext : MonoBehaviour, IPatchContext<TarkyPatch>
{
    private readonly Dictionary<string, TarkyPatch> _appliedPatches = new();

    public bool PatchesEnabled { get; private set; }
    public Logger Logger => TarkyPlugin.Logger;

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
                Logger.LogDebug($"Applying patch {patchName}.");

                patch.Enable();
                _appliedPatches.Add(patchName, patch);

                Logger.LogDebug($"Successfully applied patch {patchName}.");
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Failed to apply patch {patchName}.");
                if (patch.FatalOnPatchError)
                {
                    Logger.LogError($"Patch {patchName} is marked as fatal on patch error.");
                    Logger.LogError($"Disabling all TarkyPatch instances for {TarkyPlugin.Name}.");
                    Logger.LogError(e.ToString());
                    DisableAllPatches(true);
                    throw new TarkyPatchFailedException(true, e);
                }
                Logger.LogWarning($"Ignoring patch and continuing. This may cause strange behaviour!");
                Logger.LogWarning(e.ToString());
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
            Logger.LogWarning("Attempted to disable all patches, but patches have not been applied yet!");
            return;
        }

        Logger.LogDebug("Disabling all imported patches.");
        foreach (var patch in _appliedPatches.Values)
        {
            patch.Disable();
        }
        _appliedPatches.Clear();
        Logger.LogDebug("All imported patches disabled.");
    }
}
