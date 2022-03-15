// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT

#if FEATURE_BUILD_PROJ
using System;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Smdn.OperatingSystem;

namespace Smdn.Reflection.ReverseGenerating.ListApi.Build;

public static class MSBuildExePath {
  private static string JoinPath(string path1, string path2)
  {
#pragma warning disable SA1114
#if SYSTEM_IO_PATH_JOIN
    return Path.Join(
#else
    return Path.Combine(
#endif
#pragma warning restore SA1114
      path1, path2
    );
  }

#if false
  private const string patternVersion = @"(?<version>[0-9]+\.[0-9]+\.[0-9]+)";
  private const string patternVersionSuffix = @"(?<version_suffix>(?:preview|rc)[0-9\.\-]+)";
  private const string patternRootPath = @"(?<root_path>[^\]]+)";
  private static readonly Regex regexSdkPath = new Regex(
    @$"^(?<version_full>{patternVersion}(\-{patternVersionSuffix})?) \[{patternRootPath}\]\r?$",
    RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled
  );

  static IEnumerable<(
    string versionFull,
    Version version,
    string versionSuffix,
    string path
  )> GetSdkPaths()
  {
    foreach (Match match in regexSdkPath.Matches(Shell.Execute("dotnet --list-sdks"))) {
      if (Version.TryParse(match.Groups["version"].Value, out var version)) {
        yield return (
          match.Groups["version_full"].Value,
          version,
          match.Groups["version_suffix"].Value,
          JoinPath(match.Groups["root_path"].Value, match.Groups["version_full"].Value)
        );
      }
    }
  }
#endif

  private static readonly Regex regexSdkBasePath = new(
    @"^\s*Base Path:\s+(?<base_path>.+)$",
    RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled
  );

  private static string GetSdkBasePath(out string sdkVersion)
  {
    sdkVersion = default;

    var match = regexSdkBasePath.Match(Shell.Execute("dotnet --info"));

    if (match.Success) {
      var basePath = match.Groups["base_path"].Value.TrimEnd();

      sdkVersion = new DirectoryInfo(basePath).Name;

      return basePath;
    }

    return null;
  }

  private static string GetMSBuildExePath(out string sdkVersion)
  {
#if true
    sdkVersion = default;

    var sdkBasePath = GetSdkBasePath(out sdkVersion);

    if (sdkBasePath is null)
      throw new InvalidOperationException("could not get SDK base path");

    var msbuildExePath = JoinPath(
      sdkBasePath,
      "MSBuild.dll" // .NET SDK always ships MSBuild executables with the extension 'dll'
    );

    if (!File.Exists(msbuildExePath))
      throw new InvalidOperationException("MSBuild not found");

    return msbuildExePath;
#else
    static IEnumerable<(
      string sdkVersionFull,
      Version sdkVersion,
      string sdkVersionSuffix,
      string sdkPath,
      string msbuildPath
    )> EnumerateMSBuildPath(
      (string versionFull, Version version, string versionSuffix, string path) sdk
    )
    {
      yield return (
        sdk.versionFull,
        sdk.version,
        sdk.versionSuffix,
        sdk.path,
        JoinPath(
          sdk.path,
          "MSBuild.dll" // .NET SDK always ships MSBuild executables with the extension 'dll'
        )
      );
    }

    sdkVersion = default;

    (var msbuildExePath, sdkVersion) = GetSdkPaths()
      .SelectMany(EnumerateMSBuildPath)
      .Where(static p => string.IsNullOrEmpty(p.sdkVersionSuffix)) // except preview
      .Where(static p => File.Exists(p.msbuildPath))
      .OrderByDescending(static p => p.sdkVersion) // newest one
      .Select(static p => (p.msbuildPath, p.sdkVersionFull))
      .FirstOrDefault();

    return msbuildExePath ?? throw new InvalidOperationException("MSBuild not found");
#endif
  }

  public static void EnsureSetEnvVar(ILogger logger = null)
  {
    const string MSBUILD_EXE_PATH = nameof(MSBUILD_EXE_PATH);

    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(MSBUILD_EXE_PATH)))
      Environment.SetEnvironmentVariable(MSBUILD_EXE_PATH, GetMSBuildExePath(out _));

    logger?.LogDebug($"{MSBUILD_EXE_PATH}: {Environment.GetEnvironmentVariable(MSBUILD_EXE_PATH)}");
  }
}
#endif
