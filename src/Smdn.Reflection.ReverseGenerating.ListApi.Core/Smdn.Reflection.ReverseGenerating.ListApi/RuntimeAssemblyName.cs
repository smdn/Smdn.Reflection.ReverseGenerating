// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cspell:ignore runtimepack,referenceassembly
#pragma warning disable CA1848, CA1873

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal static class RuntimeAssemblyName {
  public static void WarnNotToBeAbleToResolve(
    ILogger? logger,
    AssemblyName name,
    DependencyContext? depContext,
    string? baseAssemblyPath
  )
  {
    const string MessageRuntimeProvidedAssembly = "Could not resolve assembly (Probably a runtime-provided library): {AssemblyName}";
    const string MessageOtherAssembly = "Could not resolve assembly: {AssemblyName}";

    var logLevel = ShouldWarnIfNotToBeAbleToResolve(name, depContext, baseAssemblyPath, out var isExpectedToBeProvidedByRuntime)
      ? LogLevel.Warning
      : LogLevel.Debug;

    if (isExpectedToBeProvidedByRuntime)
      logger?.Log(logLevel, MessageRuntimeProvidedAssembly, name);
    else
      logger?.Log(logLevel, MessageOtherAssembly, name);
  }

  private static bool ShouldWarnIfNotToBeAbleToResolve(
    AssemblyName name,
    DependencyContext? depContext,
    string? baseAssemblyPath,
    out bool isExpectedToBeProvidedByRuntime
  )
  {
    const bool ShouldNotWarn = false;
    const bool ShouldWarn = true;

    isExpectedToBeProvidedByRuntime = false;

    if (name.Name is null)
      return ShouldWarn;

    if (depContext is null)
      return ShouldNotWarn; // target framework may be .NET Framework

    isExpectedToBeProvidedByRuntime = IsRuntimeProvidedAssembly(
      depContext,
      name,
      string.IsNullOrEmpty(baseAssemblyPath)
        ? null
        : Path.GetDirectoryName(baseAssemblyPath)
    );

    if (isExpectedToBeProvidedByRuntime)
      return ShouldNotWarn;

    var publicKeyTokenBytes = name.GetPublicKeyToken();
    var publicKeyTokenString = publicKeyTokenBytes is null
      ? null
#if SYSTEM_CONVERT_TOHEXSTRING
      : Convert.ToHexString(publicKeyTokenBytes); // ToHexString() returns upper case string
#else
      : string.Concat(publicKeyTokenBytes.Select(static b => b.ToString("x2", provider: null)));
#endif

    // ref: https://github.com/dotnet/source-build-reference-packages/blob/main/src/referencePackageSourceGenerator/Tasks/GenerateProjects.cs
    const string PublicKeyTokenMicrosoft = "b03f5f7f11d50a3a";
    const string PublicKeyTokenDotnetOpenSource = "cc7b13ffcd2ddd51";
#if false
    const string PublicKeyTokenEcma = "b77a5c561934e089";
#endif

    var isTargetNetStandard = depContext.Target.Framework.StartsWith(".NETStandard,", StringComparison.Ordinal);
    var isTargetNetCore = depContext.Target.Framework.StartsWith(".NETCoreApp,", StringComparison.Ordinal);

    if (isTargetNetStandard || isTargetNetCore) {
      var isNetStandardAssembly =
        string.Equals(name.Name, "netstandard", StringComparison.Ordinal) &&
        string.Equals(publicKeyTokenString, PublicKeyTokenDotnetOpenSource, StringComparison.OrdinalIgnoreCase);

      if (isNetStandardAssembly)
        return ShouldNotWarn;

      var isNameSystemAssembly =
        string.Equals(name.Name, "System", StringComparison.Ordinal) ||
        name.Name.StartsWith("System.", StringComparison.Ordinal);
      var isSystemAssembly =
        isNameSystemAssembly && (
          string.Equals(publicKeyTokenString, PublicKeyTokenDotnetOpenSource, StringComparison.OrdinalIgnoreCase) ||
          string.Equals(publicKeyTokenString, PublicKeyTokenMicrosoft, StringComparison.OrdinalIgnoreCase)
        );

      if (isSystemAssembly)
        return ShouldNotWarn;

      var isCoreLibAssembly =
        string.Equals(name.Name, "mscorlib", StringComparison.Ordinal) &&
        string.Equals(publicKeyTokenString, "7cec85d7bea7798e" /* CoreLib? */, StringComparison.OrdinalIgnoreCase);

      if (isCoreLibAssembly)
        return ShouldNotWarn;
    }

    return ShouldWarn;
  }

  /// <summary>
  /// Determines whether the specified assembly is a pruned package or a
  /// standard runtime-provided assembly.
  /// </summary>
  /// <param name="dependencyContext">
  /// The <see cref="DependencyContext"/> loaded from the target <c>.deps.json</c> file.
  /// </param>
  /// <param name="assemblyName">
  /// The <see cref="AssemblyName"/> to evaluate.
  /// </param>
  /// <param name="assemblyDirectory">
  /// The path to the directory where the assembly corresponding to
  /// <paramref name="assemblyName"/> is located.
  /// </param>
  /// <returns>
  /// <see langword="true"/> if the assembly has been pruned or is provided by the
  /// runtime framework; otherwise, <see langword="false"/> if it is a standalone
  /// dependency that should be loaded explicitly.
  /// </returns>
  private static bool IsRuntimeProvidedAssembly(
    DependencyContext dependencyContext,
    AssemblyName assemblyName,
    string? assemblyDirectory
  )
  {
    if (string.IsNullOrEmpty(assemblyName.Name))
      return false;

    // 1. Search RuntimeLibraries in DependencyContext for a library containing
    //    the specified assembly
    var resolvedRuntimeLibrary = dependencyContext
      .RuntimeLibraries
      .FirstOrDefault(
        lib => lib
          .RuntimeAssemblyGroups
          .SelectMany(static group => group.RuntimeFiles)
          .Any(
            file => string.Equals(
              Path.GetFileNameWithoutExtension(file.Path),
              assemblyName.Name,
              StringComparison.OrdinalIgnoreCase
            )
          )
      );

    // 2. If not found in RuntimeLibraries, it was excluded from runtime
    //    targets during build-time pruning.
    if (resolvedRuntimeLibrary is null)
      return true;

    // 3. Libraries typed as "runtimepack" or "referenceassembly" are provided
    //    directly by the runtime.
    if (
      string.Equals(resolvedRuntimeLibrary.Type, "runtimepack", StringComparison.OrdinalIgnoreCase) ||
      string.Equals(resolvedRuntimeLibrary.Type, "referenceassembly", StringComparison.OrdinalIgnoreCase)
    ) {
      return true;
    }

    // 4. Even if typed as "package" or "project", if the physical DLL is absent from
    //    the output directory, it was stripped by package pruning and should fall back
    //    to the host runtime.
    if (
      !string.IsNullOrEmpty(assemblyDirectory) &&
      (
        string.Equals(resolvedRuntimeLibrary.Type, "package", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(resolvedRuntimeLibrary.Type, "project", StringComparison.OrdinalIgnoreCase)
      )
    ) {
      // Find the relative path to the specified assembly from RuntimeFiles
      var runtimeAssemblyFile = resolvedRuntimeLibrary
        .RuntimeAssemblyGroups
        .SelectMany(static group => group.RuntimeFiles)
        .FirstOrDefault(
          file => string.Equals(
            Path.GetFileNameWithoutExtension(file.Path),
            assemblyName.Name,
            StringComparison.OrdinalIgnoreCase
          )
        );

      if (runtimeAssemblyFile?.Path is { } runtimeAssemblyFilePath) {
        // Confirm the existence of the target assembly in the output directory
        // (either the same directory as the base assembly or under `runtimes/{rid}/lib/`)
        var expectedAssemblyPath = Path.Combine(assemblyDirectory, Path.GetFileName(runtimeAssemblyFilePath));
        var expectedRidRelativeAssemblyPath = Path.Combine(assemblyDirectory, runtimeAssemblyFilePath);

        // If the specified assembly does not exist in the output directory,
        // determine it to be a runtime-provided or pruned assembly.
        if (!File.Exists(expectedAssemblyPath) && !File.Exists(expectedRidRelativeAssemblyPath))
          return true;
      }
    }

    // If neither of the above matches, it is determined to be a dependent assembly not
    // provided by the runtime.
    return false;
  }
}
