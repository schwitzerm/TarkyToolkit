using System.Reflection;
using UnityEngine;

namespace TarkyToolkit.Shared.Utils
{
    public static class TarkyPatchUtils
    {
        public static MethodBase GetAwakeMethod<T>() where T : MonoBehaviour
        {
            return typeof(T).GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public);
        }
    }
}
