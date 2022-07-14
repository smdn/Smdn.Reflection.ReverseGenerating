// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Runtime.Versioning;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class FrameworkMonikersTests {
  [TestCase(".NETCoreApp,Version=v6.0", "net6.0")]
  [TestCase(".NETCoreApp,Version=v5.0", "net5.0")]
  [TestCase(".NETCoreApp,Version=v3.1", "netcoreapp3.1")]
  [TestCase(".NETStandard,Version=v2.1", "netstandard2.1")]
  [TestCase(".NETStandard,Version=v1.6", "netstandard1.6")]
  [TestCase(".NETFramework,Version=v4.7.1", "net471")]
  [TestCase(".NETFramework,Version=v4.5", "net45")]
  [TestCase(".NETFramework,Version=v4.0", "net40")]
  public void TryGetMoniker(string input, string expected)
  {
    Assert.IsTrue(FrameworkMonikers.TryGetMoniker(new FrameworkName(input), osSpecifier: null, out var moniker));
    Assert.AreEqual(expected, moniker, nameof(moniker));
  }

  [Test]
  public void TryGetMoniker_ArgumentNull()
  {
    FrameworkName name = null!;

    Assert.Throws<ArgumentNullException>(() => FrameworkMonikers.TryGetMoniker(name, osSpecifier: null, out _));
  }
}
