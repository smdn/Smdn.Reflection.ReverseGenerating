// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;

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
}
