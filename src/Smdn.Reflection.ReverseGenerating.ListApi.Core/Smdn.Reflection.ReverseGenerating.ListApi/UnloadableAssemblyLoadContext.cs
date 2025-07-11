// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore unloadable
#pragma warning disable CA1848

#if NETCOREAPP3_1_OR_GREATER
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal sealed class UnloadableAssemblyLoadContext : AssemblyLoadContext {
  private readonly AssemblyDependencyResolver dependencyResolver;
  private readonly PackageDependencyAssemblyResolver packageDependencyResolver;
  private readonly ILogger? logger;

  public bool HasDepsJsonLoaded => packageDependencyResolver.DependencyContext is not null;
  public string PossibleAssemblyDepsJsonPath => packageDependencyResolver.PossibleAssemblyDepsJsonPath;

  public UnloadableAssemblyLoadContext(string componentAssemblyPath, ILogger? logger = null)
    : base(
      isCollectible: true // is required to unload assembly
    )
  {
    this.dependencyResolver = new(componentAssemblyPath);
    this.packageDependencyResolver = new(componentAssemblyPath, logger);
    this.logger = logger;
  }

  protected override Assembly? Load(AssemblyName name)
  {
    logger?.LogDebug("attempting to load '{AssemblyName}'", name);

    var assemblyPath = dependencyResolver.ResolveAssemblyToPath(name);

    if (assemblyPath is not null) {
      logger?.LogDebug("attempting to load assembly from '{AssemblyFilePath}'", assemblyPath);
      return LoadFromAssemblyPath(assemblyPath);
    }

    var assembly = packageDependencyResolver.Resolve(name, this.LoadFromAssemblyPath);

    if (assembly is null)
      RuntimeAssemblyName.WarnNotToBeAbleToResolve(logger, name, packageDependencyResolver.DependencyContext);

    return assembly;
  }
}
#endif
