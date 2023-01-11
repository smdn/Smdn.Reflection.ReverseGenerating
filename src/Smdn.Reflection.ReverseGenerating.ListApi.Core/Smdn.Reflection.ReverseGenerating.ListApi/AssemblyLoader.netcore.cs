// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

#if NETCOREAPP3_1_OR_GREATER
using System;
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

#pragma warning disable IDE0040
partial class AssemblyLoader {
#pragma warning restore IDE0040
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  private static TResult UsingAssemblyCore<TArg, TResult>(
    FileInfo assemblyFile,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = default;

    if (loadIntoReflectionOnlyContext) {
      return UsingReflectionOnlyAssembly(
        assemblyFile,
        arg,
        actionWithLoadedAssembly,
        logger
      );
    }
    else {
      return UsingAssembly(
        assemblyFile,
        arg,
        actionWithLoadedAssembly,
        out context,
        logger
      );
    }
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  private static TResult UsingAssemblyCore<TArg, TResult>(
    Stream assemblyStream,
    string componentAssemblyPath,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = default;

    if (loadIntoReflectionOnlyContext) {
      return UsingReflectionOnlyAssembly(
        assemblyStream,
        componentAssemblyPath,
        arg,
        actionWithLoadedAssembly,
        logger
      );
    }
    else {
      return UsingAssembly(
        assemblyStream,
        componentAssemblyPath,
        arg,
        actionWithLoadedAssembly,
        out context,
        logger
      );
    }
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  private static TResult UsingReflectionOnlyAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    ILogger? logger = null
  )
  {
    using var mlc = new MetadataLoadContext(
      new PathAssemblyDependencyResolver(assemblyFile.FullName)
    );

    logger?.LogDebug("loading assembly into reflection-only context from file '{AssemblyFilePath}'", assemblyFile.FullName);

    var assm = mlc.LoadFromAssemblyPath(assemblyFile.FullName);

    if (assm is null) {
      logger?.LogCritical("failed to load assembly from file '{AssemblyFilePath}'", assemblyFile.FullName);

      return default;
    }

    var assemblyName = assm.FullName;
    var assemblyTypeFullName = assm.GetType().FullName;

    logger?.LogDebug(
      "loaded assembly '{AssemblyName}' ({AssemblyTypeFullName})",
      assemblyName,
      assemblyTypeFullName
    );

    return actionWithLoadedAssembly(assm, arg);
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  private static TResult UsingReflectionOnlyAssembly<TArg, TResult>(
    Stream assemblyStream,
    string componentAssemblyPath,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    ILogger? logger = null
  )
  {
    using var mlc = new MetadataLoadContext(
      new PathAssemblyDependencyResolver(componentAssemblyPath)
    );

    logger?.LogDebug("loading assembly into reflection-only context from stream with component assembly path '{ComponentAssemblyPath}'", componentAssemblyPath);

    var assm = mlc.LoadFromStream(assemblyStream);

    if (assm is null) {
      logger?.LogCritical("failed to load assembly from stream");

      return default;
    }

    var assemblyName = assm.FullName;
    var assemblyTypeFullName = assm.GetType().FullName;

    logger?.LogDebug(
      "loaded assembly '{AssemblyName}' ({AssemblyTypeFullName})",
      assemblyName,
      assemblyTypeFullName
    );

    return actionWithLoadedAssembly(assm, arg);
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  [MethodImpl(MethodImplOptions.NoInlining)]
  private static TResult UsingAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = null;

    var alc = new UnloadableAssemblyLoadContext(assemblyFile.FullName, logger);
    var alcWeakReference = new WeakReference(alc);

    logger?.LogDebug("loading assembly from file '{AssemblyFilePath}'", assemblyFile.FullName);

    var assm = alc.LoadFromAssemblyPath(assemblyFile.FullName);

    if (assm is null) {
      logger?.LogCritical("failed to load assembly from file '{AssemblyFilePath}'", assemblyFile.FullName);

      return default;
    }

    context = alcWeakReference;

    var assemblyName = assm.FullName;
    var assemblyTypeFullName = assm.GetType().FullName;

    logger?.LogDebug(
      "loaded assembly '{AssemblyName}' ({AssemblyTypeFullName})",
      assemblyName,
      assemblyTypeFullName
    );

    try {
      return actionWithLoadedAssembly(assm, arg);
    }
    finally {
      alc.Unload();

      logger?.LogDebug("unloaded assembly '{AssemblyName}'", assemblyName);
    }
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  [MethodImpl(MethodImplOptions.NoInlining)]
  private static TResult UsingAssembly<TArg, TResult>(
    Stream assemblyStream,
    string componentAssemblyPath,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = null;

    var alc = new UnloadableAssemblyLoadContext(componentAssemblyPath, logger);
    var alcWeakReference = new WeakReference(alc);

    logger?.LogDebug("loading assembly from stream with component assembly path '{ComponentAssemblyPath}'", componentAssemblyPath);

    var assm = alc.LoadFromStream(assemblyStream);

    if (assm is null) {
      logger?.LogCritical("failed to load assembly from stream");

      return default;
    }

    context = alcWeakReference;

    var assemblyName = assm.FullName;
    var assemblyTypeFullName = assm.GetType().FullName;

    logger?.LogDebug(
      "loaded assembly '{AssemblyName}' ({AssemblyTypeFullName})",
      assemblyName,
      assemblyTypeFullName
    );

    try {
      return actionWithLoadedAssembly(assm, arg);
    }
    finally {
      alc.Unload();

      logger?.LogDebug("unloaded assembly '{AssemblyName}'", assemblyName);
    }
  }
}
#endif
