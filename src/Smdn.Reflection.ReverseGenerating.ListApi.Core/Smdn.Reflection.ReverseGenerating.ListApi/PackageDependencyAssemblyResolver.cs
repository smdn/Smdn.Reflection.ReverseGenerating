// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

#if NETCOREAPP3_1_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal sealed class PackageDependencyAssemblyResolver {
  private static DependencyContext? LoadDependencyContextIfDepsJsonExist(string assemblyPath, ILogger? logger = null)
  {
    if (assemblyPath is null)
      throw new ArgumentNullException(nameof(assemblyPath));
    if (assemblyPath.Length == 0)
      throw new ArgumentException("must be non-empty string", nameof(assemblyPath));

    var assemblyDepsJsonPath = Path.ChangeExtension(assemblyPath, ".deps.json");

    if (!File.Exists(assemblyDepsJsonPath)) {
      logger?.LogWarning("dependency configuration file not found: '{AssemblyDepsJsonPath}'", assemblyDepsJsonPath);
      return null;
    }

    using var stream = File.OpenRead(assemblyDepsJsonPath);
    using var reader = new DependencyContextJsonReader();

    return reader.Read(stream);
  }

  public DependencyContext? DependencyContext { get; }
  private readonly ILogger? logger;

  public PackageDependencyAssemblyResolver(string componentAssemblyPath, ILogger? logger = null)
  {
    this.DependencyContext = LoadDependencyContextIfDepsJsonExist(componentAssemblyPath, logger);
    this.logger = logger;
  }

  // ref: https://github.com/dotnet/runtime/issues/1050
  // ref: https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/default-probing
  public Assembly? Resolve(AssemblyName name, Func<string, Assembly?> loadFromAssemblyPath)
  {
    logger?.LogTrace("attempting to resolve package dependency assembly '{AssemblyName}'", name);

    if (DependencyContext is null) {
      logger?.LogDebug("could not resolve package dependency '{AssemblyName}'", name);
      return null;
    }

    var runtimeLibrary = DependencyContext
      .RuntimeLibraries
      .FirstOrDefault(
        runtimeLib => runtimeLib.Name.Equals(name.Name, StringComparison.OrdinalIgnoreCase)
      );

    if (runtimeLibrary is null) {
      logger?.LogDebug("could not resolve runtime library '{AssemblyName}'", name);
      return null;
    }

    logger?.LogTrace("runtime library '{AssemblyName}'", name);
    logger?.LogTrace("  Type: {RuntimeLibraryType}", runtimeLibrary.Type);
    logger?.LogTrace("  Name: {RuntimeLibraryName}", runtimeLibrary.Name);
    logger?.LogTrace("  Version: {RuntimeLibraryVersion}", runtimeLibrary.Version);
    logger?.LogTrace("  Hash: {RuntimeLibraryHash}", runtimeLibrary.Hash);
    logger?.LogTrace(
      "  RuntimeAssemblyGroups: {RuntimeLibraryRuntimeAssemblyGroups}",
      string.Join(", ", runtimeLibrary.RuntimeAssemblyGroups.SelectMany(static g => g.AssetPaths))
    );
    logger?.LogTrace(
      "  Dependencies: {RuntimeLibraryDependencies}",
      string.Join(", ", runtimeLibrary.Dependencies.Select(static d => d.Name))
    );
    logger?.LogTrace("  Serviceable: {RuntimeLibraryServiceable}", runtimeLibrary.Serviceable);
    logger?.LogTrace("  Path: {RuntimeLibraryPath}", runtimeLibrary.Path);
    logger?.LogTrace("  HashPath: {RuntimeLibraryHashPath}", runtimeLibrary.HashPath);

    var compileLibrary = new CompilationLibrary(
      type: runtimeLibrary.Type,
      name: runtimeLibrary.Name,
      version: runtimeLibrary.Version,
      hash: runtimeLibrary.Hash,
      assemblies: runtimeLibrary.RuntimeAssemblyGroups.SelectMany(static g => g.AssetPaths),
      dependencies: runtimeLibrary.Dependencies,
      serviceable: runtimeLibrary.Serviceable,
      path: runtimeLibrary.Path,
      hashPath: runtimeLibrary.HashPath
    );

    var resolver = new PackageCompilationAssemblyResolver();
    var assemblyPaths = new List<string>(capacity: 1);

    if (resolver.TryResolveAssemblyPaths(compileLibrary, assemblyPaths) && 0 < assemblyPaths.Count) {
      logger?.LogTrace("attempting to loading package compilation assembly '{AssemblyName}' from file {AssemblyPath}", name, assemblyPaths[0]);

      return loadFromAssemblyPath(assemblyPaths[0]);
    }

    logger?.LogDebug("could not resolve package compilation assembly '{AssemblyName}'", name);

    return null;
  }
}
#endif
