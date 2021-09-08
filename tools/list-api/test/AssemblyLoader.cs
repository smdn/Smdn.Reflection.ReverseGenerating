// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class AssemblyLoaderTests {
  private ILogger logger = null;
  private FileInfo assemblyFileLibA = null;
  private FileInfo assemblyFileLibB = null;

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

    Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", null);

    MSBuildExePath.EnsureSetEnvVar(logger);

    var options = new ProjectBuilder.Options() {
      TargetFramework = "net5.0",
    };

    assemblyFileLibA = ProjectBuilder.Build(
      new(Path.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")),
      options: options,
      logger: logger
    ).First();

    assemblyFileLibB = ProjectBuilder.Build(
      new(Path.Join(TestAssemblyInfo.RootDirectory.FullName, "LibB", "LibB.csproj")),
      options: options,
      logger: logger
    ).First();
  }

  [Test]
  public void UsingAssembly()
  {
    var result = AssemblyLoader.UsingAssembly(
      assemblyFileLibA,
      arg: assemblyFileLibA,
      (assm, arg) => {
        Assert.AreSame(arg, assemblyFileLibA, nameof(arg));

        Assert.IsNotNull(assm, nameof(assm));
        Assert.AreEqual(assm.Location, arg.FullName, nameof(assm.Location));

        return assm.GetType("Lib.LibA.CBase")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.IsNotNull(result, nameof(result));
    Assert.AreEqual(result, "Lib.LibA.CBase", nameof(result));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }

  [Test]
  public void UsingAssembly_ResolveDependency()
  {
    var result = AssemblyLoader.UsingAssembly(
      assemblyFileLibB,
      arg: assemblyFileLibB,
      (assm, arg) => {
        Assert.AreSame(arg, assemblyFileLibB, nameof(arg));

        Assert.IsNotNull(assm, nameof(assm));
        Assert.AreEqual(assm.Location, arg.FullName, nameof(assm.Location));

        return assm.GetType("Lib.LibB.CEx")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.IsNotNull(result, nameof(result));
    Assert.AreEqual(result, "Lib.LibB.CEx", nameof(result));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }
}