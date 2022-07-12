// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class ProjectFinder {
  public static FileInfo FindSingleProjectOrSolution(
    DirectoryInfo directory,
    ILogger? logger = null
  )
  {
    logger?.LogDebug("finding project or solution from directory '{Directory}'", directory.FullName);

    var solutionAndProjectFiles = directory
      .GetFiles("*.*", SearchOption.TopDirectoryOnly)
      .Where(static file =>
        // *.sln, *.csproj, *.vbproj, etc
        Regex.IsMatch(file.Extension, @"\.(?i:sln|[a-z]+proj)$", RegexOptions.Singleline | RegexOptions.CultureInvariant)
      );

    if (1 < solutionAndProjectFiles.Count())
      throw new InvalidOperationException($"multiple solution or project file found in directory '{directory.FullName}'");

    var first = solutionAndProjectFiles.FirstOrDefault();

    if (first is null)
      throw new FileNotFoundException($"no solution or project file found in directory '{directory.FullName}'");

    logger?.LogDebug("found '{ProjectOrSolutionPath}'", first.FullName);

    return first;
  }
}
