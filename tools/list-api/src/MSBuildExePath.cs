using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Smdn.OperatingSystem;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class MSBuildExePath {
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
#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
          Path.Join(
#else
          Path.Combine(
#endif
            match.Groups["root_path"].Value, match.Groups["version_full"].Value
          )
        );
      }
    }
  }

  static string GetMSBuildExePath()
  {
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
        Path.Combine(sdk.path, "MSBuild.dll") // .NET SDK always ships MSBuild executables with the extension 'dll'
      );
    }

    var msbuildExePath = GetSdkPaths()
      .SelectMany(EnumerateMSBuildPath)
      .Where(static p => string.IsNullOrEmpty(p.sdkVersionSuffix)) // except preview
      .Where(static p => File.Exists(p.msbuildPath))
      .OrderByDescending(static p => p.sdkVersion) // newest one
      .Select(static p => p.msbuildPath)
      .FirstOrDefault();

    return msbuildExePath ?? throw new InvalidOperationException("MSBuild not found");
  }

  public static void EnsureSetEnvVar(ILogger logger = null)
  {
    const string MSBUILD_EXE_PATH = nameof(MSBUILD_EXE_PATH);

    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(MSBUILD_EXE_PATH)))
      Environment.SetEnvironmentVariable(MSBUILD_EXE_PATH, GetMSBuildExePath());

    logger?.LogDebug($"{MSBUILD_EXE_PATH}: {Environment.GetEnvironmentVariable(MSBUILD_EXE_PATH)}");
  }
}