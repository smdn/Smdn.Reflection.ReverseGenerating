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

#if SYSTEM_IO_PATH_JOIN
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class RootCommandImplementationGetInputAssemblyFilesTests {
  private ServiceProvider serviceProvider;

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

  [OneTimeTearDown]
  public void OneTimeTearDown()
  {
    serviceProvider.Dispose();
  }

  [TestCase("Lib.dll", "net6.0")]
  [TestCase("Exe.dll", "net6.0")]
  [TestCase("LibA.dll", "netstandard2.1")]
  [TestCase("LibA.dll", "net6.0")]
  public void GetInputAssemblyFiles_File_Assembly(string filename, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains(filename))
    );

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] { assemblyFile.FullName },
      impl.GetInputAssemblyFiles(new[] { assemblyFile.FullName }).Select(f => f.FullName)
    );
  }

  private FileInfo Build(string pathToProjectFile)
  {
#if FEATURE_BUILD_PROJ
    var logger = serviceProvider?.GetService<ILoggerFactory>()?.CreateLogger("builder");

    Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", null);

    MSBuildExePath.EnsureSetEnvVar(logger);

    return ProjectBuilder.Build(
      new(pathToProjectFile),
      logger: logger
    ).First();
#else
    throw new NotSupportedException("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [Test]
  public void GetInputAssemblyFiles_File_Proj_Dll()
  {
#if FEATURE_BUILD_PROJ
    var dll = Build(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib", "Lib.csproj"));

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] { dll.FullName },
      impl.GetInputAssemblyFiles(new[] { dll.FullName }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [Test]
  public void GetInputAssemblyFiles_File_Proj_Exe()
  {
#if FEATURE_BUILD_PROJ
    var exe = Build(PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj"));

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] { exe.FullName },
      impl.GetInputAssemblyFiles(new[] { exe.FullName }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
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
#if FEATURE_BUILD_PROJ
    var proj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin", configuration, "net6.0", "LibA.dll"),
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin", configuration, "netstandard2.1", "LibA.dll"),
      },
      impl.GetInputAssemblyFiles(new[] { optionName, configuration, proj }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-f", "netstandard2.1")]
  [TestCase("--framework", "netstandard2.1")]
  public void GetInputAssemblyFiles_File_Proj_WithTargetFrameworkOption(string optionName, string targetFramework)
  {
#if FEATURE_BUILD_PROJ
    var proj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "bin", RootCommandImplementation.DefaultBuildConfiguration, targetFramework, "LibA.dll"),
      },
      impl.GetInputAssemblyFiles(new[] { optionName, targetFramework, proj }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-r", "win-x64")]
  [TestCase("-r", "linux-x64")]
  [TestCase("--runtime", "win-x64")]
  [TestCase("--runtime", "linux-x64")]
  public void GetInputAssemblyFiles_File_Proj_WithRuntimeOption(string optionName, string runtime)
  {
#if FEATURE_BUILD_PROJ
    var proj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj");
    var expectedBuildOutputFileName = "Exe." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "exe" : "dll");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "bin", RootCommandImplementation.DefaultBuildConfiguration, "net6.0", runtime, expectedBuildOutputFileName),
      },
      impl.GetInputAssemblyFiles(new[] { optionName, runtime, proj }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [Test]
  public void GetInputAssemblyFiles_Directory_ProjLib()
  {
#if FEATURE_BUILD_PROJ
    var dirProj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Lib", "bin", RootCommandImplementation.DefaultBuildConfiguration, "net6.0", "Lib.dll"),
      },
      impl.GetInputAssemblyFiles(new[] { dirProj }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [Test]
  public void GetInputAssemblyFiles_Directory_ProjExe()
  {
#if FEATURE_BUILD_PROJ
    var dirProj = PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe");
    var expectedBuildOutputFileName = "Exe." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "exe" : "dll");

    var impl = new RootCommandImplementation(serviceProvider);

    CollectionAssert.AreEquivalent(
      new[] {
        PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "bin", RootCommandImplementation.DefaultBuildConfiguration, "net6.0", expectedBuildOutputFileName),
      },
      impl.GetInputAssemblyFiles(new[] { dirProj }).Select(f => f.FullName)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
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
