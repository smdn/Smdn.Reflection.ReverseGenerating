// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.CommandLine;

#if FEATURE_BUILD_PROJ
using Microsoft.Build.Framework;
#endif
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class VerbosityOption {
  // cSpell:disable
  internal static readonly Option<string> Option = new("--verbosity", "-v") {
    Description = "Verbosity of output. The value must be one of q[uiet], m[inimal], n[ormal], d[etailed], or diag[nostic].",
    DefaultValueFactory = static _ => "minimal",
  };
  // cSpell:enable

  public static LogLevel ParseLogLevel(string[] args)
  {
    var command = new RootCommand {
      Option,
    };

    return ParseLogLevel(command.Parse(args));
  }

  public static LogLevel ParseLogLevel(ParseResult parseResult)
    => (parseResult ?? throw new ArgumentNullException(nameof(parseResult))).GetValue(Option) switch {
#pragma warning disable IDE0055
      "q" or "quiet"          => LogLevel.Information,
      "m" or "minimal"        => LogLevel.Information,
      "n" or "normal"         => LogLevel.Debug,
      "d" or "detailed"       => LogLevel.Trace,
      "diag" or "diagnostic"  => LogLevel.Trace,
      _                       => LogLevel.Information,
#pragma warning restore IDE0055
    };

#if FEATURE_BUILD_PROJ
  public static LoggerVerbosity ParseLoggerVerbosity(string[] args)
  {
    var command = new RootCommand {
      Option,
    };

    return ParseLoggerVerbosity(command.Parse(args));
  }

  public static LoggerVerbosity ParseLoggerVerbosity(ParseResult parseResult)
    => (parseResult ?? throw new ArgumentNullException(nameof(parseResult))).GetValue(Option) switch {
#pragma warning disable IDE0055
      "q" or "quiet"          => LoggerVerbosity.Quiet,
      "m" or "minimal"        => LoggerVerbosity.Minimal,
      "n" or "normal"         => LoggerVerbosity.Normal,
      "d" or "detailed"       => LoggerVerbosity.Detailed,
      "diag" or "diagnostic"  => LoggerVerbosity.Diagnostic,
      _                       => LoggerVerbosity.Minimal,
#pragma warning restore IDE0055
    };
#endif
}
