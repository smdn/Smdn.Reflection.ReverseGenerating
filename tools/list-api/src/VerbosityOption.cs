using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

public static class VerbosityOption {
  internal static readonly Option<string> Option = new(
    aliases: new[] { "-v", "--verbosity" },
    description: "Verbosity of output. The value must be one of q[uiet], m[inimal], n[ormal], d[etailed], or diag[nostic].",
    getDefaultValue: () => "minimal"
  );

  public static LogLevel ParseLogLevel(string[] args)
  {
    var command = new RootCommand();

    command.Add(Option);

    return ParseLogLevel(command.Parse(args));
  }

  public static LogLevel ParseLogLevel(ParseResult parseResult)
    => parseResult.ValueForOption(Option) switch {
      "q" or "quiet"          => LogLevel.Information,
      "m" or "minimal"        => LogLevel.Information,
      "n" or "normal"         => LogLevel.Debug,
      "d" or "detailed"       => LogLevel.Trace,
      "diag" or "diagnostic"  => LogLevel.Trace,
      _                       => LogLevel.Information,
    };

  public static LoggerVerbosity ParseLoggerVerbosity(string[] args)
  {
    var command = new RootCommand();

    command.Add(Option);

    return ParseLoggerVerbosity(command.Parse(args));
  }

  public static LoggerVerbosity ParseLoggerVerbosity(ParseResult parseResult)
    => parseResult.ValueForOption(Option) switch {
      "q" or "quiet"          => LoggerVerbosity.Quiet,
      "m" or "minimal"        => LoggerVerbosity.Minimal,
      "n" or "normal"         => LoggerVerbosity.Normal,
      "d" or "detailed"       => LoggerVerbosity.Detailed,
      "diag" or "diagnostic"  => LoggerVerbosity.Diagnostic,
      _                       => LoggerVerbosity.Minimal,
    };
}