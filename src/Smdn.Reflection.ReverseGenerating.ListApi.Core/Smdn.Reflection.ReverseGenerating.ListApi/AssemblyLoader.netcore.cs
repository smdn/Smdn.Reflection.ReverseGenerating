// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NETCOREAPP3_1_OR_GREATER
using System;
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
  private static TResult UsingAssemblyCore<TArg, TResult>(
    FileInfo assemblyFile,
    bool loadIntoReflectionOnlyContext,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference context,
    ILogger logger = null
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

  private sealed class PathAssemblyDependencyResolver : PathAssemblyResolver {
    private readonly AssemblyDependencyResolver dependencyResolver;
    private readonly ILogger logger;

    public PathAssemblyDependencyResolver(string componentAssemblyPath, ILogger logger = null)
      : base(
        Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll") // add runtime assemblies
      )
    {
      this.dependencyResolver = new(componentAssemblyPath);
      this.logger = logger;
    }

    public override Assembly Resolve(MetadataLoadContext context, AssemblyName assemblyName)
    {
      logger?.LogDebug("attempting to load '{AssemblyName}'", assemblyName);

      var assm = base.Resolve(context, assemblyName);

      if (assm is not null)
        return assm;

      var assemblyPath = dependencyResolver.ResolveAssemblyToPath(assemblyName);

      if (assemblyPath is null) {
        logger?.LogDebug("could not resolve assembly path of '{AssemblyName}'", assemblyName);

        return null;
      }

      logger?.LogDebug("attempting to load assembly from '{AssemblyFilePath}'", assemblyPath);

      return context.LoadFromAssemblyPath(assemblyPath);
    }
  }

  private static TResult UsingReflectionOnlyAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    ILogger logger = null
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

    logger?.LogDebug("loaded assembly '{AssemblyName}'", assemblyName);

    return actionWithLoadedAssembly(assm, arg);
  }

  private sealed class UnloadableAssemblyLoadContext : AssemblyLoadContext {
    private readonly AssemblyDependencyResolver dependencyResolver;
    private readonly ILogger logger;

    public UnloadableAssemblyLoadContext(string componentAssemblyPath, ILogger logger = null)
      : base(
        isCollectible: true // is required to unload assembly
      )
    {
      this.dependencyResolver = new(componentAssemblyPath);
      this.logger = logger;
    }

    protected override Assembly Load(AssemblyName name)
    {
      var assemblyPath = dependencyResolver.ResolveAssemblyToPath(name);

      if (assemblyPath is null) {
        logger?.LogDebug("could not resolve assembly path of '{AssemblyName}'", name);

        return null;
      }

      logger?.LogDebug("attempting to load assembly from '{AssemblyFilePath}'", assemblyPath);

      return LoadFromAssemblyPath(assemblyPath);
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static TResult UsingAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference context,
    ILogger logger = null
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

    logger?.LogDebug("loaded assembly '{AssemblyName}'", assemblyName);

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
