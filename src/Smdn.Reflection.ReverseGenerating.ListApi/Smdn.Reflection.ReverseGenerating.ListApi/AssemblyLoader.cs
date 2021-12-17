// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AssemblyLoader {
  public static TResult UsingAssembly<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference context,
    ILogger logger = null
  )
  {
#pragma warning disable SA1114
#if NETFRAMEWORK
    return UsingAssemblyNetFx(
#else
    return UsingAssemblyNetCoreApp(
#endif
#pragma warning restore SA1114
      assemblyFile,
      arg,
      actionWithLoadedAssembly,
      out context,
      logger
    );
  }

#if NETFRAMEWORK
  private class AppDomainProxy : MarshalByRefObject {
    public Assembly LoadAssembly(FileInfo assemblyFile)
      => Assembly.ReflectionOnlyLoadFrom(assemblyFile.FullName);
  }

  private static TResult UsingAssemblyNetFx<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference context,
    ILogger logger = null
  )
  {
    context = null;

    var domain = AppDomain.CreateDomain(
      friendlyName: assemblyFile.Name,
      securityInfo: null,
      appBasePath: assemblyFile.Directory.FullName,
      appRelativeSearchPath: ".",
      shadowCopyFiles: false
    );
    var domainWeakReference = new WeakReference(domain);

    context = domainWeakReference;

    string assemblyName = null;

    try {
      var typeOfProxy = typeof(AppDomainProxy);
      var proxy = (AppDomainProxy)domain.CreateInstanceAndUnwrap(
        assemblyName: typeOfProxy.Assembly.FullName,
        typeName: typeOfProxy.FullName
      );

      logger?.LogDebug($"loading assembly from file '{assemblyFile.FullName}'");

      var assm = proxy.LoadAssembly(assemblyFile);

      if (assm is null) {
        logger?.LogCritical($"failed to load assembly from file '{assemblyFile.FullName}'");

        return default;
      }

      assemblyName = assm.FullName;

      logger?.LogInformation($"loaded assembly '{assemblyName}'");

      return actionWithLoadedAssembly(assm, arg);
    }
    finally {
      AppDomain.Unload(domain);

      logger?.LogDebug($"unloaded assembly '{assemblyName}'");
    }
  }
#else // #if NETFRAMEWORK
  private class UnloadableAssemblyLoadContext : AssemblyLoadContext {
    private readonly AssemblyDependencyResolver dependencyResolver;

    public UnloadableAssemblyLoadContext(string componentAssemblyPath)
      : base(
        isCollectible: true // is required to unload assembly
      )
    {
      this.dependencyResolver = new(componentAssemblyPath);
    }

    protected override Assembly Load(AssemblyName name)
    {
      var assemblyPath = dependencyResolver.ResolveAssemblyToPath(name);

      if (assemblyPath is null)
        return null;

      return LoadFromAssemblyPath(assemblyPath);
    }
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  private static TResult UsingAssemblyNetCoreApp<TArg, TResult>(
    FileInfo assemblyFile,
    TArg arg,
    Func<Assembly, TArg, TResult> actionWithLoadedAssembly,
    out WeakReference context,
    ILogger logger = null
  )
  {
    context = null;

    var alc = new UnloadableAssemblyLoadContext(assemblyFile.FullName);
    var alcWeakReference = new WeakReference(alc);

    logger?.LogDebug($"loading assembly from file '{assemblyFile.FullName}'");

    var assm = alc.LoadFromAssemblyPath(assemblyFile.FullName);

    if (assm is null) {
      logger?.LogCritical($"failed to load assembly from file '{assemblyFile.FullName}'");

      return default;
    }

    context = alcWeakReference;

    var assemblyName = assm.FullName;

    logger?.LogInformation($"loaded assembly '{assemblyName}'");

    try {
      return actionWithLoadedAssembly(assm, arg);
    }
    finally {
      alc.Unload();

      logger?.LogDebug($"unloaded assembly '{assemblyName}'");
    }
  }
#endif // #if NETFRAMEWORK
}
