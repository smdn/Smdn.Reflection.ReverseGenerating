// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal sealed class PathAssemblyDependencyResolver : PathAssemblyResolver {
  private readonly AssemblyDependencyResolver dependencyResolver;
  private readonly PackageDependencyAssemblyResolver packageDependencyResolver;
  private readonly ILogger? logger;

  public PathAssemblyDependencyResolver(string componentAssemblyPath, ILogger? logger = null)
    : base(
      Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll") // add runtime assemblies
    )
  {
    this.dependencyResolver = new(componentAssemblyPath);
    this.packageDependencyResolver = new(componentAssemblyPath, logger);
    this.logger = logger;
  }

  public override Assembly? Resolve(MetadataLoadContext context, AssemblyName assemblyName)
  {
    logger?.LogDebug("attempting to load '{AssemblyName}'", assemblyName);

    var assm = base.Resolve(context, assemblyName);

    if (assm is not null)
      return assm;

    var assemblyPath = dependencyResolver.ResolveAssemblyToPath(assemblyName);

    if (assemblyPath is not null) {
      logger?.LogDebug("attempting to load assembly from '{AssemblyFilePath}'", assemblyPath);
      return context.LoadFromAssemblyPath(assemblyPath);
    }

    assm = packageDependencyResolver.Resolve(assemblyName, context.LoadFromAssemblyPath);

    if (assm is null)
      RuntimeAssemblyName.WarnNotToBeAbleToResolve(logger, assemblyName);

    return assm;
  }
}
#endif
