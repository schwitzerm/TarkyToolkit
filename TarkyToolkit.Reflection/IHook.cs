using System;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;

namespace TarkyToolkit.Reflection;

public interface IHook
{
    public MethodInfo? Hook<TShape>(TShape toBind, string methodName, BindingFlags flags, HarmonyPatchType patchType)
        where TShape : Delegate;
    bool Unhook(string methodName);
    bool Unhook(MethodInfo method);
}

public interface IEntityHook<T> : IHook
{
    T? Target { get; }
    // ReSharper disable once InconsistentNaming
    void OnAwake(T __instance);
    void OnDestroy();
}

public interface IMethodHook : IHook
{
    MethodInfo? Method { get; }
    void PreInvoke();
    void PostInvoke();
}
