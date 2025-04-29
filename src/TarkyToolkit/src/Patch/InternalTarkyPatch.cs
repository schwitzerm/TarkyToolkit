using TarkyToolkit.Core.Patch;
using UnityEngine;

namespace TarkyToolkit.Patch
{
    internal abstract class InternalTarkyPatch : TarkyPatch
    {
        protected InternalTarkyPatch(GameObject rootObject) : base(rootObject)
        {
        }
    }
}
