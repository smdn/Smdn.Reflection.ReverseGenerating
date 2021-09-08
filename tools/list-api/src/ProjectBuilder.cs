using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class ProjectBuilder {
  public class Options {
    public const string DefaultConfiguration = "Debug";

    public string Configuration { get; init; } = DefaultConfiguration;
    public string TargetFramework { get; init; } = null;
    //public string OS { get; init; }
    public string RuntimeIdentifier { get; init; } = null;
    public string[] TargetsToBuild { get; init; } = new[] { "Restore", "Build" };
    public LoggerVerbosity LoggerVerbosity { get; init; } = LoggerVerbosity.Minimal;
  }

  public static IEnumerable<FileInfo> Build(
    FileInfo projectFile,
    Options options = null,
    Microsoft.Extensions.Logging.ILogger logger = null
  )
  {
    if (projectFile is null)
      throw new ArgumentNullException(nameof(projectFile));

    options ??= new();

    var globalProps = new Dictionary<string, string>() {
      // values cannnot be null
      {"Configuration", options.Configuration ?? string.Empty},
      {"TargetFramework", options.TargetFramework ?? string.Empty},
      //{"OS", os ?? string.Empty},
      {"RuntimeIdentifier", options.RuntimeIdentifier ?? string.Empty},
    };

    logger?.LogDebug("Build properties:");

    foreach (var globalProp in globalProps) {
      logger?.LogDebug($"  {globalProp.Key}: {(string.IsNullOrEmpty(globalProp.Value) ? "-" : globalProp.Value)}");
    }

    var proj = new Project(
      projectFile: projectFile.FullName,
      globalProperties: globalProps,
      toolsVersion: "Current"
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
        }
      }
    };

    //using var buildManager = new BuildManager("default");
    var buildManager = BuildManager.DefaultBuildManager;

    try {
      var result = buildManager.Build(buildParams, buildRequest);

      if (result.OverallResult == BuildResultCode.Success) {
        // retrieve %(InnerOutput.Identity) / for in case of building with multiple target frameworks
        var innerOutputFullPaths = result.ProjectStateAfterBuild
          .Items
          .Where(item => item.ItemType == "InnerOutput")
          .Select(item => item.GetMetadataValue("Identity"));

        foreach (var innerOutputFullPath in innerOutputFullPaths) {
          logger?.LogDebug($"build output file: {innerOutputFullPath}");

          yield return new FileInfo(innerOutputFullPath);
        }

        // retrieve Build target result / for in cace of building with single target framework
        if (result.HasResultsForTarget("Build")) {
          var buildTargetResult = result.ResultsByTarget["Build"];

          var buildOutputFullPaths = buildTargetResult
            .Items
            .Select(item => item.GetMetadata("Identity"));

          foreach (var buildOutputFullPath in buildOutputFullPaths) {
            logger?.LogDebug($"build output file: {buildOutputFullPath}");

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