// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
public class VerbosityOptionTests {
  [TestCase("q", LogLevel.Information)]
  [TestCase("quiet", LogLevel.Information)]
  [TestCase("m", LogLevel.Information)]
  [TestCase("minimal", LogLevel.Information)]
  [TestCase("n", LogLevel.Debug)]
  [TestCase("normal", LogLevel.Debug)]
  [TestCase("d", LogLevel.Trace)]
  [TestCase("detailed", LogLevel.Trace)]
  [TestCase("diag", LogLevel.Trace)]
  [TestCase("diagnostic", LogLevel.Trace)]
  [TestCase(null, LogLevel.Information)]
  [TestCase("default", LogLevel.Information)]
  public void ParseLogLevel(string? verbosity, LogLevel expectedLogLevel)
  {
    Assert.That(
      expectedLogLevel,
      Is.EqualTo(VerbosityOption.ParseLogLevel(verbosity is null ? Array.Empty<string>() : new[] { "-v", verbosity }))
    );
  }

  [TestCase("q", LoggerVerbosity.Quiet)]
  [TestCase("quiet", LoggerVerbosity.Quiet)]
  [TestCase("m", LoggerVerbosity.Minimal)]
  [TestCase("minimal", LoggerVerbosity.Minimal)]
  [TestCase("n", LoggerVerbosity.Normal)]
  [TestCase("normal", LoggerVerbosity.Normal)]
  [TestCase("d", LoggerVerbosity.Detailed)]
  [TestCase("detailed", LoggerVerbosity.Detailed)]
  [TestCase("diag", LoggerVerbosity.Diagnostic)]
  [TestCase("diagnostic", LoggerVerbosity.Diagnostic)]
  [TestCase(null, LoggerVerbosity.Minimal)]
  [TestCase("default", LoggerVerbosity.Minimal)]
  public void ParseLoggerVerbosity(string? verbosity, LoggerVerbosity expectedLoggerVerbosity)
  {
    Assert.That(
      expectedLoggerVerbosity,
      Is.EqualTo(VerbosityOption.ParseLoggerVerbosity(verbosity is null ? Array.Empty<string>() : new[] { "-v", verbosity }))
    );
  }
}
