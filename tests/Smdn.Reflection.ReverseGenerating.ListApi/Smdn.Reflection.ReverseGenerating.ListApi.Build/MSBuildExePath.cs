// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if FEATURE_BUILD_PROJ
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.ListApi.Build;

[TestFixture]
class MSBuildExePathTests {
  const string MSBUILD_EXE_PATH = nameof(MSBUILD_EXE_PATH);

  [SetUp]
  public void SetUp()
  {
    Environment.SetEnvironmentVariable(MSBUILD_EXE_PATH, null);
  }

  [Test]
  public void EnsureSetEnvVar_PathNotSet()
  {
    Environment.SetEnvironmentVariable(MSBUILD_EXE_PATH, null);

    Assert.DoesNotThrow(() => MSBuildExePath.EnsureSetEnvVar());

    Assert.That(Environment.GetEnvironmentVariable(MSBUILD_EXE_PATH), Is.Not.Empty);
  }

  [Test]
  public void EnsureSetEnvVar_PathSetAlready()
  {
    var initialValue = "/path/to/msbuild.exe";

    Environment.SetEnvironmentVariable(MSBUILD_EXE_PATH, initialValue);

    Assert.DoesNotThrow(() => MSBuildExePath.EnsureSetEnvVar());

    Assert.AreEqual(initialValue, Environment.GetEnvironmentVariable(MSBUILD_EXE_PATH));
  }
}
#endif
