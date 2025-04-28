using BepInEx;

namespace TarkyToolkit.Api;

/// <summary>
/// Stub to load binary. Needed to load and require this binary as a dependency in BepInEx.
/// </summary>
[BepInPlugin("Mellow_.TarkyToolkit.Api", "TarkyToolkit.Api", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit.Core", "0.1.0")]
[BepInDependency("Mellow_.TarkyToolkit.Reflection", "0.1.0")]
[BepInProcess("EscapeFromTarkov.exe")]
public class TarkyToolkitApiPlugin : BaseUnityPlugin
{
}
