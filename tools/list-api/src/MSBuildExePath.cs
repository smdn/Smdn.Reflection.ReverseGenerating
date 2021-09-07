using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using Smdn.OperatingSystem;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public class MSBuildExePath {
  private static readonly Regex regexSdkPath = new Regex(@"^(?<version>[0-9]+\.[0-9]+\.[0-9]+) \[(?<path>[^\]]+)\]$", RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

  static IEnumerable<(Version version, string path)> GetSkdPaths()
  {
    foreach (Match match in regexSdkPath.Matches(Shell.Execute("dotnet --list-sdks"))) {
      if (Version.TryParse(match.Groups["version"].Value, out var version))
        yield return (version, Path.Combine(match.Groups["path"].Value, match.Groups["version"].Value));
    }
  }

  static string GetMSBuildExePath()
  {
    static IEnumerable<(Version sdkVersion, string msbuildPath)> EnumerateMSBuildPath((Version version, string path) sdk)
    {
      yield return (sdk.version, Path.Combine(sdk.path, "MSBuild.dll"));
      yield return (sdk.version, Path.Combine(sdk.path, "MSBuild.exe"));
    }

    var msbuildExePath = GetSkdPaths()
      .SelectMany(EnumerateMSBuildPath)
      .Where(static p => File.Exists(p.msbuildPath))
      .OrderByDescending(p => p.sdkVersion) // newest one
      .Select(p => p.msbuildPath)
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