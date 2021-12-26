// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class RootCommandImplementationGetOutputFilePathsTests {
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

  static string GetCurrentDirectory() => TestContext.CurrentContext.WorkDirectory;

  [Test]
  public void GetOutputFilePaths()
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")
    }).First();

    Assert.AreEqual(
      "Exe-net5.0.apilist.cs",
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
  public void GetOutputFilePaths_WithOutputDirectoryOption(string optionName, string outputDirectory)
  {
#if FEATURE_BUILD_PROJ
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      "-o", outputDirectory,
      PathJoiner.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")
    }).First();

    Assert.AreEqual(
      "Exe-net5.0.apilist.cs",
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
  public void GetOutputFilePaths_WithConfigurationOption(string optionName, string configuration)
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
        "LibA-net5.0.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }

  [TestCase("-f", "net5.0")]
  [TestCase("-f", "net5.0")]
  [TestCase("--framework", "netstandard2.1")]
  [TestCase("--framework", "netstandard2.1")]
  public void GetOutputFilePaths_WithTargetFrameworkOption(string optionName, string targetFramework)
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
  public void GetOutputFilePaths_WithRuntimeOption(string optionName, string runtime)
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
        "LibA-net5.0.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
#else
    Assert.Ignore("disabled feature: FEATURE_BUILD_PROJ");
#endif
  }
}
