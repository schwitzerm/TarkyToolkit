using TarkyToolkit.Core.Patch;
using UnityEngine;
using Logger = TarkyToolkit.Core.Logging.Logger;

namespace TarkyToolkit.Patch;

internal abstract class InternalTarkyPatch(GameObject rootObject) : TarkyPatch(rootObject);
