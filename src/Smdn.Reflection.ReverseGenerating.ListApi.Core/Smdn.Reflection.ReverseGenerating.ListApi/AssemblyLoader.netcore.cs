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
using System.Runtime.Versioning;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

#pragma warning disable IDE0040
partial class AssemblyLoader {
#pragma warning restore IDE0040
#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  private static TResult UsingAssemblyCore<TArg, TResult>(
    AssemblySource assemblySource,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult>? actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = default;

    if (loadIntoReflectionOnlyContext) {
      return UsingReflectionOnlyAssembly(
        assemblySource,
        arg,
        actionWithLoadedAssembly,
        logger
      );
    }
    else {
      return UsingAssembly(
        assemblySource,
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
    AssemblySource assemblySource,
    TArg arg,
    Func<Assembly, TArg, TResult>? actionWithLoadedAssembly,
    ILogger? logger = null
  )
  {
    var resolver = new PathAssemblyDependencyResolver(assemblySource.ComponentAssemblyPath);
    using var mlc = new MetadataLoadContext(resolver);

    logger?.LogDebug(
      "loading assembly into reflection-only context (ComponentAssemblyPath: '{ComponentAssemblyPath}')",
      assemblySource.ComponentAssemblyPath
    );

    var assm = assemblySource.File is not null
      ? mlc.LoadFromAssemblyPath(assemblySource.File.FullName)
      : assemblySource.Stream is not null
        ? mlc.LoadFromStream(assemblySource.Stream)
        : throw new InvalidOperationException($"either {nameof(AssemblySource.File)} or {nameof(AssemblySource.Stream)} must be specified");

    if (assm is null) {
      logger?.LogCritical(
        "failed to load assembly into reflection-only context (ComponentAssemblyPath: '{ComponentAssemblyPath}')",
        assemblySource.ComponentAssemblyPath
      );

      return default;
    }

    var assemblyName = assm.FullName;
    var assemblyTypeFullName = assm.GetType().FullName;

    if (!resolver.HasDepsJsonLoaded)
      WarnDepsJsonCouldNotBeLoaded(logger, resolver.PossibleAssemblyDepsJsonPath, assm);

    logger?.LogDebug(
      "loaded reflection-only assembly '{AssemblyName}' ({AssemblyTypeFullName})",
      assemblyName,
      assemblyTypeFullName
    );

    if (actionWithLoadedAssembly is null)
      return default;

    return actionWithLoadedAssembly(assm, arg);
  }

#if NULL_STATE_STATIC_ANALYSIS_ATTRIBUTES
  [return: MaybeNull]
#endif
  [MethodImpl(MethodImplOptions.NoInlining)]
  private static TResult UsingAssembly<TArg, TResult>(
    AssemblySource assemblySource,
    TArg arg,
    Func<Assembly, TArg, TResult>? actionWithLoadedAssembly,
    out WeakReference? context,
    ILogger? logger = null
  )
  {
    context = null;

    var alc = new UnloadableAssemblyLoadContext(assemblySource.ComponentAssemblyPath, logger);
    var alcWeakReference = new WeakReference(alc);

    logger?.LogDebug(
      "loading assembly (ComponentAssemblyPath: '{ComponentAssemblyPath}')",
      assemblySource.ComponentAssemblyPath
    );

    var assm = assemblySource.File is not null
      ? alc.LoadFromAssemblyPath(assemblySource.File.FullName)
      : assemblySource.Stream is not null
        ? alc.LoadFromStream(assemblySource.Stream)
        : throw new InvalidOperationException($"either {nameof(AssemblySource.File)} or {nameof(AssemblySource.Stream)} must be specified");

    if (assm is null) {
      logger?.LogCritical(
        "failed to load assembly (ComponentAssemblyPath: '{ComponentAssemblyPath}')",
        assemblySource.ComponentAssemblyPath
      );

      return default;
    }

    context = alcWeakReference;

    var assemblyName = assm.FullName;
    var assemblyTypeFullName = assm.GetType().FullName;

    if (!alc.HasDepsJsonLoaded)
      WarnDepsJsonCouldNotBeLoaded(logger, alc.PossibleAssemblyDepsJsonPath, assm);

    logger?.LogDebug(
      "loaded assembly '{AssemblyName}' ({AssemblyTypeFullName})",
      assemblyName,
      assemblyTypeFullName
    );

    try {
      if (actionWithLoadedAssembly is null)
        return default;

      return actionWithLoadedAssembly(assm, arg);
    }
    finally {
      alc.Unload();

      logger?.LogDebug("unloaded assembly '{AssemblyName}'", assemblyName);
    }
  }

  private static void WarnDepsJsonCouldNotBeLoaded(
    ILogger? logger,
    string possibleAssemblyDepsJsonPath,
    Assembly assembly
  )
  {
    if (logger is null)
      return;

    string? targetFramework;

    try {
      targetFramework = assembly.GetAssemblyMetadataAttributeValue<TargetFrameworkAttribute, string?>();
    }
    catch (AssemblyFileNotFoundException) {
      // If the target framework cannot be obtained due to failure to load reference assembly, treat as 'unknown'(null).
      targetFramework = null;
    }

    if (targetFramework is not null && targetFramework.StartsWith(".NETFramework,", StringComparison.Ordinal))
      return;

    if (File.Exists(possibleAssemblyDepsJsonPath))
      logger.LogWarning("dependency configuration could not be loaded: '{AssemblyDepsJsonPath}'", possibleAssemblyDepsJsonPath);
    else
      logger.LogInformation("dependency configuration could not be found: '{AssemblyDepsJsonPath}'", possibleAssemblyDepsJsonPath);
  }
}
#endif
