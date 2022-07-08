// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.Versioning;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class FrameworkMonikers {
  private static readonly Version versionNET5OrGreater = new(5, 0);

  /// <remarks>
  ///   <see cref="FrameworkName.Profile"/> of <paramref name="frameworkName"/> is not supported currently.
  /// </remarks>
  public static bool TryGetMoniker(
    FrameworkName frameworkName,
#pragma warning disable IDE0060, SA1305
    string osSpecifier,
#pragma warning restore IDE0060, SA1305
    out string frameworkMoniker
  )
  {
    if (frameworkName is null)
      throw new ArgumentNullException(nameof(frameworkName));

    frameworkMoniker = default;

    // TODO: frameworkName.Profile, osSpecifier
    switch (frameworkName.Identifier) {
      case ".NETCoreApp" when versionNET5OrGreater <= frameworkName.Version:
        frameworkMoniker = $"net{frameworkName.Version.Major}.{frameworkName.Version.Minor}";
        return true;

      case ".NETCoreApp":
        frameworkMoniker = $"netcoreapp{frameworkName.Version.Major}.{frameworkName.Version.Minor}";
        return true;

      case ".NETStandard":
        frameworkMoniker = $"netstandard{frameworkName.Version.Major}.{frameworkName.Version.Minor}";
        return true;

      case ".NETFramework" when frameworkName.Version.Build == -1:
        frameworkMoniker = $"net{frameworkName.Version.Major}{frameworkName.Version.Minor}";
        return true;

      case ".NETFramework":
        frameworkMoniker = $"net{frameworkName.Version.Major}{frameworkName.Version.Minor}{frameworkName.Version.Build}";
        return true;

      default:
        return false;
    }
  }
}
