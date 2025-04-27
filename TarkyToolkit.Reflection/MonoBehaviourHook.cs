using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using ILogger = TarkyToolkit.Shared.ILogger;

namespace TarkyToolkit.Reflection;

public class MonoBehaviourHook<T>(Harmony harmony, ILogger logger) : IEntityHook<T>
    where T : MonoBehaviour
{
    public T? Target { get; set; } = null!;
    private Dictionary<string, (MethodInfo, MethodInfo)> MethodHooks { get; } = new();

    public MethodInfo? Hook<TShape>(TShape toBind, string methodName, BindingFlags flags, HarmonyPatchType patchType)
        where TShape : Delegate
    {
        logger.LogDebug($"Attempting hooking of method {methodName} on {typeof(T)}.");
        if (MethodHooks.TryGetValue(methodName, out var hook))
        {
            logger.LogWarning($"Hook already initialized for method: {methodName} on {typeof(T)}.");
            logger.LogWarning($"Returning existing patched method.");
            return hook.Item2;
        }

        var entityMethod = typeof(T).GetMethod(methodName, flags)!;
        if (entityMethod == null)
        {
            throw new NullReferenceException($"Method {methodName} not found on {typeof(T)}.");
        }

        var hookDelegate = entityMethod.IsStatic ?
            CreateStaticDelegate<TShape>(toBind.Method) :
            CreateDelegate<TShape>(toBind.Method);
        var virtualMethod = CreateVirtualMethod(entityMethod, hookDelegate.Method);
        var harmonyMethod = new HarmonyMethod(virtualMethod);
        var patchedMethod = harmony.Patch(entityMethod, postfix: harmonyMethod);
        if (patchedMethod == null)
        {
            throw new NullReferenceException($"Method {entityMethod.Name} failed to patch.");
        }

        MethodHooks.Add(methodName, (entityMethod, patchedMethod));
        logger.LogDebug($"Hook initialized for method: {methodName} on {typeof(T)}.");

        return patchedMethod;
    }

    public bool Unhook(string methodName)
    {
        logger.LogDebug("Unhook(string)");
        if (!MethodHooks.TryGetValue(methodName, out var entry)) return false;
        harmony.Unpatch(entry.Item1, entry.Item2);
        MethodHooks.Remove(methodName);
        return true;
    }

    public bool Unhook(MethodInfo method)
    {
        logger.LogDebug("Unhook(MethodInfo)");
        return Unhook(method.Name);
    }

    // ReSharper disable once InconsistentNaming
    public void OnAwake(T __instance)
    {
        logger.LogDebug("OnAwake(T)");
        Target = __instance;
    }

    public void OnDestroy()
    {
        logger.LogDebug("OnDestroy()");
    }

    private TShape CreateDelegate<TShape>(MethodInfo method) where TShape : Delegate
    {
        if (method.DeclaringType == null)
        {
            throw new NullReferenceException("Method declaring type is null.");
        }

        var invoke = typeof(TShape).GetMethod("Invoke")!;
        var invokeParamTypes = invoke.GetParameters().Select(p => p.ParameterType).ToArray();
        var dynamicInvoke = new DynamicMethod(
            $"HookDelegate_{method.DeclaringType.Name}_{method.Name}",
            invoke.ReturnType,
            invokeParamTypes,
            true);
        var il = dynamicInvoke.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);

        var isBoxed = invokeParamTypes[0] == typeof(object);
        var methodParamTypes = method.GetParameters().Select(p => p.ParameterType);
        Type[] dynParamTypes;
        if (isBoxed)
        {
            dynParamTypes = methodParamTypes.Prepend(typeof(object)).ToArray();
            il.Emit(method.DeclaringType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, method.DeclaringType);
        }
        else
        {
            dynParamTypes = methodParamTypes.Prepend(invokeParamTypes[0]).ToArray();
        }

        foreach (var (type, i) in dynParamTypes.Select((t, i) => (t, i)).ToArray())
        {
            logger.LogDebug($"Emitting {type} for param on index {i}.");
            il.Emit(OpCodes.Ldarg, i);
            if (type == typeof(object)) il.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        il.Emit(OpCodes.Call, method);

        var requiresBoxing = invoke.ReturnType == typeof(object) && method.ReturnType.IsValueType;
        if (requiresBoxing)
        {
            il.Emit(OpCodes.Box, method.ReturnType);
        }

        il.Emit(OpCodes.Ret);

        return (TShape)dynamicInvoke.CreateDelegate(typeof(TShape));
    }

    private TShape CreateStaticDelegate<TShape>(MethodInfo method) where TShape : Delegate
    {
        if (method.DeclaringType == null)
        {
            throw new NullReferenceException("Method declaring type is null.");
        }

        var invoke = typeof(TShape).GetMethod("Invoke")!;
        var paramTypes = invoke.GetParameters().Select(p => p.ParameterType).ToArray();
        var dyn = new DynamicMethod(
            $"HookDelegate_{method.DeclaringType}_{method.Name}",
            method.ReturnType,
            paramTypes,
            true);
        var il = dyn.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);

        foreach (var (type, i) in paramTypes.Select((f, i) => (f, i)).ToArray())
        {
            // +1 as we want to 'shift' our parameters to the right and prepend the null parameter type representing
            // the declaring type instance.
            // we don't give a fuck about it. passing an instance to a static method call is unsupported behaviour.
            il.Emit(OpCodes.Ldarg, i + 1);
            il.Emit(type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, type);
        }

        il.Emit(OpCodes.Callvirt, method);

        var requiresBoxing = method.ReturnType == typeof(object) && method.ReturnType.IsValueType;
        if (requiresBoxing)
        {
            il.Emit(OpCodes.Box, method.ReturnType);
        }

        il.Emit(OpCodes.Ret);

        return (TShape)dyn.CreateDelegate(typeof(TShape));
    }

    private MethodInfo CreateVirtualMethod(MethodBase toPatch, MethodInfo toCall)
    {
        if (toCall.DeclaringType == null)
        {
            throw new NullReferenceException("toCall.DeclaringType is null.");
        }

        // virtual method hook already exists, return it.
        if (MethodHooks.TryGetValue(toCall.Name, out var entry)) return entry.Item2;

        // cast our parameters to valid types for the virtual call.
        // if no casting was required, return the patched method with no modification.
        if (!CastParameters(toPatch, toCall, out var parameters)) return toCall;

        return toCall;
    }

    private bool CastParameters(MethodBase toPatch, MethodInfo toCall, out ParameterInfo[] parameters)
    {
        parameters = [];
        return false;
    }
}
