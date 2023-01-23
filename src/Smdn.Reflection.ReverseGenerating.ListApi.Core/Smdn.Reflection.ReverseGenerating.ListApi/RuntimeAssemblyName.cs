// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
#if !SYSTEM_CONVERT_TOHEXSTRING
using System.Linq;
#endif
using System.Reflection;

using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal static class RuntimeAssemblyName {
  public static void WarnNotToBeAbleToResolve(ILogger? logger, AssemblyName name, DependencyContext? depContext)
  {
    if (ShouldWarnIfNotToBeAbleToResolve(name, depContext))
      logger?.LogWarning("could not resolve assembly '{AssemblyName}'", name);
    else
      logger?.LogDebug("could not resolve assembly '{AssemblyName}'", name);
  }

  private static bool ShouldWarnIfNotToBeAbleToResolve(AssemblyName name, DependencyContext? depContext)
  {
    const bool shouldNotWarn = false;
    const bool shouldWarn = true;

    if (name.Name is null)
      return shouldWarn;

    if (depContext is null)
      return shouldNotWarn; // target framework may be .NET Framework

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
    }

    if (isTargetNetStandard || isTargetNetCore) {
      var isNetStandardAssembly =
        string.Equals(name.Name, "netstandard", StringComparison.Ordinal) &&
        string.Equals(publicKeyTokenString, PublicKeyTokenDotnetOpenSource, StringComparison.OrdinalIgnoreCase);

      if (isNetStandardAssembly)
        return shouldNotWarn;

      var isNameSystemAssembly =
        string.Equals(name.Name, "System", StringComparison.Ordinal) ||
        name.Name.StartsWith("System.", StringComparison.Ordinal);
      var isSystemAssembly =
        isNameSystemAssembly && (
          string.Equals(publicKeyTokenString, PublicKeyTokenDotnetOpenSource, StringComparison.OrdinalIgnoreCase) ||
          string.Equals(publicKeyTokenString, PublicKeyTokenMicrosoft, StringComparison.OrdinalIgnoreCase)
        );

      if (isSystemAssembly)
        return shouldNotWarn;

      var isCoreLibAssembly =
        string.Equals(name.Name, "mscorlib", StringComparison.Ordinal) &&
        string.Equals(publicKeyTokenString, "7cec85d7bea7798e" /* CoreLib? */, StringComparison.OrdinalIgnoreCase);

      if (isCoreLibAssembly)
        return shouldNotWarn;
    }

    return shouldWarn;
  }
}
