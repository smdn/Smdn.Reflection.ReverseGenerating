using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Smdn.OperatingSystem;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class MSBuildExePath {
  private const string patternVersion = @"(?<version>[0-9]+\.[0-9]+\.[0-9]+)";
  private const string patternVersionSuffix = @"(?<version_suffix>preview[0-9\.]+)";
  private const string patternRootPath = @"(?<root_path>[^\]]+)";
  private static readonly Regex regexSdkPath = new Regex(
    @$"^(?<version_full>{patternVersion}(\-{patternVersionSuffix})?) \[{patternRootPath}\]$",
    RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled
  );

  static IEnumerable<(Version version, string versionSuffix, string path)> GetSkdPaths()
  {
    foreach (Match match in regexSdkPath.Matches(Shell.Execute("dotnet --list-sdks"))) {
      if (Version.TryParse(match.Groups["version"].Value, out var version)) {
        yield return (
          version,
          match.Groups["version_suffix"].Value,
          Path.Join(match.Groups["root_path"].Value, match.Groups["version_full"].Value)
        );
      }
    }
  }

  static string GetMSBuildExePath()
  {
    static IEnumerable<(
      Version sdkVersion,
      string sdkVersionSuffix,
      string sdkPath,
      string msbuildPath
    )> EnumerateMSBuildPath(
      (Version version, string versionSuffix, string path) sdk
    )
    {
      yield return (sdk.version, sdk.versionSuffix, sdk.path, Path.Combine(sdk.path, "MSBuild.dll"));
      yield return (sdk.version, sdk.versionSuffix, sdk.path, Path.Combine(sdk.path, "MSBuild.exe"));
    }

    var msbuildExePath = GetSkdPaths()
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