// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;

#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class ProjectBuilderTests {
  private ILogger logger;

  [OneTimeSetUp]
  public void Init()
  {
    var services = new ServiceCollection();

    services.AddLogging(
      builder => builder
        .AddSimpleConsole(static options => options.SingleLine = true)
        .AddFilter(level => LogLevel.Trace <= level)
    );

    logger = services.BuildServiceProvider().GetService<ILoggerFactory>()?.CreateLogger("test");

    var outputDirectories = Enumerable.Concat(
      TestAssemblyInfo.RootDirectory.EnumerateDirectories("bin", SearchOption.AllDirectories),
      TestAssemblyInfo.RootDirectory.EnumerateDirectories("obj", SearchOption.AllDirectories)
    ).ToList();

    foreach (var d in outputDirectories) {
      d.Delete(recursive: true);
    }
  }

  [SetUp]
  public void SetUp()
  {
    Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", null);

    MSBuildExePath.EnsureSetEnvVar(logger);
  }

  [Test]
  public void Build()
  {
    var assemblyFiles = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")),
      logger: logger
    ).ToList();

    Assert.AreEqual(2, assemblyFiles.Count);

    Assert.That(assemblyFiles[0].FullName, Does.EndWith("LibA.dll"));
    Assert.That(assemblyFiles[1].FullName, Does.EndWith("LibA.dll"));

    Assert.That(assemblyFiles[0].FullName, Does.Contain(Path.DirectorySeparatorChar + ProjectBuilder.Options.DefaultConfiguration + Path.DirectorySeparatorChar));
    Assert.That(assemblyFiles[1].FullName, Does.Contain(Path.DirectorySeparatorChar + ProjectBuilder.Options.DefaultConfiguration + Path.DirectorySeparatorChar));
  }

  [Test]
  public void Build_OutputTypeExe()
  {
    var assemblyFiles = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")),
      logger: logger
    ).ToList();

    Assert.AreEqual(1, assemblyFiles.Count);

    Assert.That(assemblyFiles[0].FullName, Does.Contain(Path.DirectorySeparatorChar + "Exe."));
  }

  [Test]
  public void Build_SingleTargetFrameworkAssembly()
  {
    var assemblyFiles = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib", "Lib.csproj")),
      logger: logger
    ).ToList();

    Assert.AreEqual(1, assemblyFiles.Count);

    Assert.That(assemblyFiles[0].FullName, Does.EndWith("Lib.dll"));
  }

  [Test]
  public void Build_HasProjectReference()
  {
    var assemblyFiles = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibB", "LibB.csproj")),
      logger: logger
    ).ToList();

    Assert.AreEqual(2, assemblyFiles.Count);
    Assert.That(assemblyFiles[0].FullName, Does.EndWith("LibB.dll"));
    Assert.That(assemblyFiles[1].FullName, Does.EndWith("LibB.dll"));
  }

  [TestCase("net5.0")]
  [TestCase("netstandard2.1")]
  public void Build_WithTargetFramework(string targetFramework)
  {
    var assemblyFiles = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")),
      options: new() { TargetFramework = targetFramework },
      logger: logger
    ).ToList();

    Assert.AreEqual(1, assemblyFiles.Count);
    Assert.That(assemblyFiles[0].FullName, Does.EndWith(PathJoiner.Join(targetFramework, "LibA.dll")));
  }

  [TestCase("Debug")]
  [TestCase("Release")]
  public void Build_WithConfiguration(string configuration)
  {
    var assemblyFiles = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")),
      options: new() { Configuration = configuration },
      logger: logger
    ).ToList();

    Assert.AreEqual(2, assemblyFiles.Count);
    Assert.That(assemblyFiles[0].FullName, Does.Contain(Path.DirectorySeparatorChar + configuration + Path.DirectorySeparatorChar));
    Assert.That(assemblyFiles[1].FullName, Does.Contain(Path.DirectorySeparatorChar + configuration + Path.DirectorySeparatorChar));
  }

  [Test]
  public void Build_RunTargetClean()
  {
    var assemblyFileFirst = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")),
      options: new() { TargetsToBuild = new[] { "Restore", "Build" } },
      logger: logger
    ).First();

    var creationTimeFirst = assemblyFileFirst.CreationTime;

    var assemblyFileSecond = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")),
      options: new() { TargetsToBuild = new[] { "Clean", "Restore", "Build" } },
      logger: logger
    ).First();

    var creationTimeSecond = assemblyFileSecond.CreationTime;

    Assert.That(creationTimeSecond, Is.GreaterThan(creationTimeFirst));
  }

  [TestCase(true)]
  [TestCase(false)]
  public void Build_RunTargetRestore(bool runTargetRestore)
  {
    static void TryDeleteDirectory(string path)
    {
      if (Directory.Exists(path))
        Directory.Delete(path, recursive: true);
    }

    // clean output files of prior and inferior project firstly
    TryDeleteDirectory(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin"));
    TryDeleteDirectory(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "obj"));
    TryDeleteDirectory(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibB", "bin"));
    TryDeleteDirectory(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibB", "obj"));

    // then build inferior project
    var assemblyFile = ProjectBuilder.Build(
      new(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibB", "LibB.csproj")),
      new() {
        TargetsToBuild = runTargetRestore ? new[] { "Restore", "Build" } : new[] { "Build" },
      },
      logger: logger
    ).FirstOrDefault();

    if (runTargetRestore) {
      Assert.IsNotNull(assemblyFile);
      Assert.IsTrue(assemblyFile.Exists);
    }
    else {
      Assert.IsNull(assemblyFile);
    }
  }
}