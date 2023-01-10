// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal sealed class UnloadableAssemblyLoadContext : AssemblyLoadContext {
  private readonly AssemblyDependencyResolver dependencyResolver;
  private readonly ILogger? logger;

  public UnloadableAssemblyLoadContext(string componentAssemblyPath, ILogger? logger = null)
    : base(
      isCollectible: true // is required to unload assembly
    )
  {
    this.dependencyResolver = new(componentAssemblyPath);
    this.logger = logger;
  }

  protected override Assembly? Load(AssemblyName name)
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
#endif
