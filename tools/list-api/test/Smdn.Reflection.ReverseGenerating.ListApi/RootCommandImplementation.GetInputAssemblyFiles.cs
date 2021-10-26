// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Smdn.Reflection.ReverseGenerating.ListApi.Build;

#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class RootCommandImplementationGetInputAssemblyFilesTests {
  private IServiceProvider serviceProvider;

  [OneTimeSetUp]
  public void Init()
  {
    var services = new ServiceCollection();

    services.AddLogging(
      builder => builder
        .AddSimpleConsole(static options => options.SingleLine = true)
        .AddFilter(level => LogLevel.Trace <= level)
    );

    serviceProvider = services.BuildServiceProvider();
  }

  private FileInfo Build(string pathToProjectFile)
  {
    var logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger("builder");

    Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", null);

    MSBuildExePath.EnsureSetEnvVar(logger);

    return ProjectBuilder.Build(
      new(pathToProjectFile),
      logger: logger
    ).First();
  }

  [Test]
  public void GetInputAssemblyFiles_File_Dll()
  {
    var dll = Build(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib", "Lib.csproj"));

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] { dll.FullName },
      impl.GetInputAssemblyFiles(new[] { dll.FullName }).Select(f => f.FullName)
    );
  }

  [Test]
  public void GetInputAssemblyFiles_File_Exe()
  {
    var exe = Build(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj"));

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] { exe.FullName },
      impl.GetInputAssemblyFiles(new[] { exe.FullName }).Select(f => f.FullName)
    );
  }

  [Test]
  public void GetInputAssemblyFiles_File_Sln()
  {
    var sln = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Solution", "Solution.sln");

    var impl = new RootCommandImplementation(serviceProvider);

    Assert.Throws<CommandOperationNotSupportedException>(() => impl.GetInputAssemblyFiles(new[] { sln }));
  }

  [TestCase("-c", "Debug")]
  [TestCase("-c", "Release")]
  [TestCase("--configuration", "Debug")]
  [TestCase("--configuration", "Release")]
  public void GetInputAssemblyFiles_File_Proj_WithConfigurationOption(string optionName, string configuration)
  {
    var proj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin", configuration, "net5.0", "LibA.dll"),
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin", configuration, "netstandard2.1", "LibA.dll"),
      },
      impl.GetInputAssemblyFiles(new[] { optionName, configuration, proj }).Select(f => f.FullName)
    );
  }

  [TestCase("-f", "netstandard2.1")]
  [TestCase("--framework", "netstandard2.1")]
  public void GetInputAssemblyFiles_File_Proj_WithTargetFrameworkOption(string optionName, string targetFramework)
  {
    var proj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin", RootCommandImplementation.DefaultBuildConfiguration, targetFramework, "LibA.dll"),
      },
      impl.GetInputAssemblyFiles(new[] { optionName, targetFramework, proj }).Select(f => f.FullName)
    );
  }

  [TestCase("-r", "win-x64")]
  [TestCase("-r", "linux-x64")]
  [TestCase("--runtime", "win-x64")]
  [TestCase("--runtime", "linux-x64")]
  public void GetInputAssemblyFiles_File_Proj_WithRuntimeOption(string optionName, string runtime)
  {
    var proj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj");
    var expectedBuildOutputFileName = "Exe." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "exe" : "dll");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "bin", RootCommandImplementation.DefaultBuildConfiguration, "net5.0", runtime, expectedBuildOutputFileName),
      },
      impl.GetInputAssemblyFiles(new[] { optionName, runtime, proj }).Select(f => f.FullName)
    );
  }

  [Test]
  public void GetInputAssemblyFiles_Directory_ProjLib()
  {
    var dirProj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib", "bin", RootCommandImplementation.DefaultBuildConfiguration, "net5.0", "Lib.dll"),
      },
      impl.GetInputAssemblyFiles(new[] { dirProj }).Select(f => f.FullName)
    );
  }

  [Test]
  public void GetInputAssemblyFiles_Directory_ProjExe()
  {
    var dirProj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe");
    var expectedBuildOutputFileName = "Exe." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "exe" : "dll");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "bin", RootCommandImplementation.DefaultBuildConfiguration, "net5.0", expectedBuildOutputFileName),
      },
      impl.GetInputAssemblyFiles(new[] { dirProj }).Select(f => f.FullName)
    );
  }

  [Test]
  public void GetInputAssemblyFiles_Directory_Sln()
  {
    var dirSln = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Solution");

    var impl = new RootCommandImplementation(serviceProvider);

    Assert.Throws<CommandOperationNotSupportedException>(() => impl.GetInputAssemblyFiles(new[] { dirSln }));
  }

  [Test]
  public void GetInputAssemblyFiles_Directory_MultipleProj()
  {
    var dirMultipleProj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "MultipleProj");

    var impl = new RootCommandImplementation(serviceProvider);

    Assert.Throws<InvalidCommandOperationException>(() => impl.GetInputAssemblyFiles(new[] { dirMultipleProj }));
  }
}