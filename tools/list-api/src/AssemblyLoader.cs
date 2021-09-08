using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class AssemblyLoader {
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
  public static TResult UsingAssembly<TArg, TResult>(
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
}