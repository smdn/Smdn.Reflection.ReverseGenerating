// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

#if SYSTEM_IO_PATH_JOIN
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class RootCommandImplementationGetOutputFilePathsTests {
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

  static string GetCurrentDirectory() => TestContext.CurrentContext.WorkDirectory;

  [TestCase("Lib.dll", "net8.0", "Lib-net8.0.apilist.cs")]
  [TestCase("Exe.dll", "net8.0", "Exe-net8.0.apilist.cs")]
  [TestCase("LibA.dll", "netstandard2.1", "LibA-netstandard2.1.apilist.cs")]
  [TestCase("LibA.dll", "net8.0", "LibA-net8.0.apilist.cs")]
  public void GetOutputFilePaths(string filename, string targetFrameworkMoniker, string expectedOutputFileName)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains(filename))
    );

    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      assemblyFile.FullName
    }).First();

    Assert.That(Path.GetFileName(outputFilePath), Is.EqualTo(expectedOutputFileName));
    Assert.That(Path.GetDirectoryName(outputFilePath), Is.EqualTo(GetCurrentDirectory()));
  }

  [TestCase("-o", "output")]
  [TestCase("--output-directory", "output")]
  public void GetOutputFilePaths_WithOutputDirectoryOption(string optionName, string outputDirectory)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains("net8.0") && f.Contains("Lib.dll"))
    );

    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      optionName, outputDirectory,
      assemblyFile.FullName
    }).First();

    Assert.That(Path.GetFileName(outputFilePath), Is.EqualTo("Lib-net8.0.apilist.cs"));
    Assert.That(Path.GetDirectoryName(outputFilePath), Is.EqualTo(Path.GetFullPath(outputDirectory)));
  }

  [Test]
  public void GetOutputFilePaths_FromProjFile()
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")
    }).First();

    Assert.AreEqual(
      "Exe-net8.0.apilist.cs",
      Path.GetFileName(outputFilePath)
    );
    Assert.AreEqual(
      GetCurrentDirectory(),
      Path.GetDirectoryName(outputFilePath)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-o", "output")]
  [TestCase("--output-directory", "output")]
  public void GetOutputFilePaths_FromProjFile_WithOutputDirectoryOption(string optionName, string outputDirectory)
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      optionName, outputDirectory,
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")
    }).First();

    Assert.AreEqual(
      "Exe-net8.0.apilist.cs",
      Path.GetFileName(outputFilePath)
    );
    Assert.AreEqual(
      Path.GetFullPath(outputDirectory),
      Path.GetDirectoryName(outputFilePath)
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-c", "Debug")]
  [TestCase("-c", "Release")]
  [TestCase("--configuration", "Debug")]
  [TestCase("--configuration", "Release")]
  public void GetOutputFilePaths_FromProjFile_WithConfigurationOption(string optionName, string configuration)
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePaths = impl.GetOutputFilePaths(new[] {
      optionName, configuration,
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")
    });

    CollectionAssert.AreEquivalent(
      new[] {
        "LibA-netstandard2.1.apilist.cs",
        "LibA-net8.0.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-f", "net8.0")]
  [TestCase("-f", "netstandard2.1")]
  [TestCase("--framework", "net8.0")]
  [TestCase("--framework", "netstandard2.1")]
  public void GetOutputFilePaths_FromProjFile_WithTargetFrameworkOption(string optionName, string targetFramework)
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePaths = impl.GetOutputFilePaths(new[] {
      optionName, targetFramework,
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")
    });

    CollectionAssert.AreEquivalent(
      new[] {
        $"LibA-{targetFramework}.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-r", "win-x64")]
  [TestCase("-r", "linux-x64")]
  [TestCase("--runtime", "win-x64")]
  [TestCase("--runtime", "linux-x64")]
  public void GetOutputFilePaths_FromProjFile_WithRuntimeOption(string optionName, string runtime)
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePaths = impl.GetOutputFilePaths(new[] {
      optionName, runtime,
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")
    });

    CollectionAssert.AreEquivalent(
      new[] {
        "LibA-netstandard2.1.apilist.cs",
        "LibA-net8.0.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }
}
