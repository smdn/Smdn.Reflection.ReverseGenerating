// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA1848

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static partial class ProjectFinder {
  private const string SlnOrProjFileNameRegexPattern = @"\.(?i:sln|[a-z]+proj)$";

#if SYSTEM_TEXT_REGULAREXPRESSIONS_GENERATEDREGEXATTRIBUTE
  [GeneratedRegex(SlnOrProjFileNameRegexPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant)]
  private static partial Regex GetRegexSlnOrProjFileName();
#else
  private static readonly Regex RegexSlnOrProjFileName = new(
    SlnOrProjFileNameRegexPattern,
    RegexOptions.Singleline | RegexOptions.CultureInvariant
  );
#endif

  public static FileInfo FindSingleProjectOrSolution(
    DirectoryInfo directory,
    ILogger? logger = null
  )
  {
    if (directory is null)
      throw new ArgumentNullException(nameof(directory));

    logger?.LogDebug("finding project or solution from directory '{Directory}'", directory.FullName);

    var solutionAndProjectFiles = directory
      .GetFiles("*.*", SearchOption.TopDirectoryOnly)
      .Where(static file =>
        // *.sln, *.csproj, *.vbproj, etc
#if SYSTEM_TEXT_REGULAREXPRESSIONS_GENERATEDREGEXATTRIBUTE
        GetRegexSlnOrProjFileName().IsMatch(file.Extension)
#else
        RegexSlnOrProjFileName.IsMatch(file.Extension)
#endif
      )
      .ToList();

    if (1 < solutionAndProjectFiles.Count)
      throw new InvalidOperationException($"multiple solution or project file found in directory '{directory.FullName}'");

    var first = solutionAndProjectFiles.FirstOrDefault()
      ?? throw new FileNotFoundException($"no solution or project file found in directory '{directory.FullName}'");

    logger?.LogDebug("found '{ProjectOrSolutionPath}'", first.FullName);

    return first;
  }
}
