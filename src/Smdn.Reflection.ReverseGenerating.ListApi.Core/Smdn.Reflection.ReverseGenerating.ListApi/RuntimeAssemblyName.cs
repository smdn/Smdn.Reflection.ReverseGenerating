// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
#if !SYSTEM_CONVERT_TOHEXSTRING
using System.Linq;
#endif
using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

internal static class RuntimeAssemblyName {
  public static void WarnNotToBeAbleToResolve(ILogger? logger, AssemblyName name)
  {
    if (ShouldWarnIfNotToBeAbleToResolve(name))
      logger?.LogWarning("could not resolve assembly '{AssemblyName}'", name);
    else
      logger?.LogDebug("could not resolve assembly '{AssemblyName}'", name);
  }

  private static bool ShouldWarnIfNotToBeAbleToResolve(AssemblyName name)
  {
    const bool shouldNotWarn = false;
    const bool shouldWarn = true;

    if (name.Name is null)
      return shouldWarn;

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
    const string PublicKeyTokenEcma = "b77a5c561934e089";

    if (string.Equals(name.Name, "mscorlib", StringComparison.Ordinal)) {
      if (string.Equals(publicKeyTokenString, PublicKeyTokenEcma, StringComparison.OrdinalIgnoreCase))
        return shouldNotWarn;
    }

    if (string.Equals(name.Name, "netstandard", StringComparison.Ordinal)) {
      if (string.Equals(publicKeyTokenString, PublicKeyTokenDotnetOpenSource, StringComparison.OrdinalIgnoreCase))
        return shouldNotWarn;
    }

    if (string.Equals(name.Name, "System", StringComparison.Ordinal) || name.Name.StartsWith("System.", StringComparison.Ordinal)) {
      if (
        string.Equals(publicKeyTokenString, PublicKeyTokenMicrosoft, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(publicKeyTokenString, PublicKeyTokenEcma, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(publicKeyTokenString, PublicKeyTokenDotnetOpenSource, StringComparison.OrdinalIgnoreCase)
      ) {
        return shouldNotWarn;
      }
    }

    return shouldWarn;
  }
}
