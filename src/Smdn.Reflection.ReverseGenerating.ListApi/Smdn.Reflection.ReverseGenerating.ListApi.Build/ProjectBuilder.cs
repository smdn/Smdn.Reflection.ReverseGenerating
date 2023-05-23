// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

#if FEATURE_BUILD_PROJ
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi.Build;

public static class ProjectBuilder {
#pragma warning disable CA1724
  public class Options {
#pragma warning restore CA1724
    public const string DefaultConfiguration = "Debug";

    public string? Configuration { get; init; } = DefaultConfiguration;
    public string? TargetFramework { get; init; } = null;
    // public string? OS { get; init; }
    public string? RuntimeIdentifier { get; init; } = null;
#pragma warning disable CA1819
    public string[]? TargetsToBuild { get; init; } = new[] { "Restore", "Build" };
#pragma warning restore CA1819
    public LoggerVerbosity LoggerVerbosity { get; init; } = LoggerVerbosity.Minimal;
  }

  public static IEnumerable<FileInfo> Build(
    FileInfo projectFile,
    Options? options = null,
    Microsoft.Extensions.Logging.ILogger? logger = null
  )
  {
    if (projectFile is null)
      throw new ArgumentNullException(nameof(projectFile));

    options ??= new();

    var globalProps = new Dictionary<string, string>();

    if (!string.IsNullOrEmpty(options.Configuration))
      globalProps["Configuration"] = options.Configuration;
    if (!string.IsNullOrEmpty(options.TargetFramework))
      globalProps["TargetFramework"] = options.TargetFramework;
#if false
    if (!string.IsNullOrEmpty(options.OS))
      globalProps["OS"] = options.OS;
#endif
    if (!string.IsNullOrEmpty(options.RuntimeIdentifier))
      globalProps["RuntimeIdentifier"] = options.RuntimeIdentifier;

    logger?.LogDebug("Build requested");
    logger?.LogDebug("  project: {ProjectFile}", projectFile);

    logger?.LogDebug("  targets: {Targets}", string.Join(";", options.TargetsToBuild ?? Array.Empty<string>()));

    logger?.LogDebug("  global properties:");

    foreach (var globalProp in globalProps) {
      logger?.LogDebug("    {GlobalPropKey}: '{GlobalPropValue}'", globalProp.Key, globalProp.Value);
    }

    var proj = new Project(
      projectFile: projectFile.FullName,
      globalProperties: globalProps,
      toolsVersion: RuntimeInformation.FrameworkDescription.Contains(".NET Framework")
        ? "4.0"
        : "Current"
    );

    var buildRequest = new BuildRequestData(
      projectInstance: proj.CreateProjectInstance(),
      targetsToBuild: options.TargetsToBuild ?? Array.Empty<string>(),
      hostServices: null,
      flags: BuildRequestDataFlags.ProvideProjectStateAfterBuild,
      propertiesToTransfer: null
    );

    var buildParams = new BuildParameters() {
      Loggers = new[] {
        new ConsoleLogger() {
          Verbosity = options.LoggerVerbosity,
          ShowSummary = true,
        },
      },
    };

    // using var buildManager = new BuildManager("default");
    var buildManager = BuildManager.DefaultBuildManager;

    try {
      var result = buildManager.Build(buildParams, buildRequest);

      if (result.OverallResult == BuildResultCode.Success) {
        // retrieve %(InnerOutput.Identity) / for in case of building with multiple target frameworks
        var innerOutputFullPaths = result.ProjectStateAfterBuild
          .Items
          .Where(static item => item.ItemType == "InnerOutput")
          .Select(static item => item.GetMetadataValue("Identity"));

        foreach (var innerOutputFullPath in innerOutputFullPaths) {
          logger?.LogDebug("build output file: {InnerOutputFullPath}", innerOutputFullPath);

          yield return new FileInfo(innerOutputFullPath);
        }

        // retrieve Build target result / for in cace of building with single target framework
        if (result.HasResultsForTarget("Build")) {
          var buildTargetResult = result.ResultsByTarget["Build"];

          var buildOutputFullPaths = buildTargetResult
            .Items
            .Select(static item => item.GetMetadata("Identity"));

          foreach (var buildOutputFullPath in buildOutputFullPaths) {
            logger?.LogDebug("build output file: {BuildOutputFullPath}", buildOutputFullPath);

            yield return new FileInfo(buildOutputFullPath);
          }
        }
      }
    }
    finally {
      ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
    }
  }
}
#endif
