// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

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
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      Path.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")
    }).First();

    Assert.AreEqual(
      "Exe-1.0.0.0-net5.0.apilist.cs",
      Path.GetFileName(outputFilePath)
    );
    Assert.AreEqual(
      GetCurrentDirectory(),
      Path.GetDirectoryName(outputFilePath)
    );
  }

  [TestCase("-o", "output")]
  [TestCase("--output-directory", "output")]
  public void GetOutputFilePaths_WithOutputDirectoryOption(string optionName, string outputDirectory)
  {
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePath = impl.GetOutputFilePaths(new[] {
      "-o", outputDirectory,
      Path.Join(TestAssemblyInfo.RootDirectory.FullName, "Exe", "Exe.csproj")
    }).First();

    Assert.AreEqual(
      "Exe-1.0.0.0-net5.0.apilist.cs",
      Path.GetFileName(outputFilePath)
    );
    Assert.AreEqual(
      Path.GetFullPath(outputDirectory),
      Path.GetDirectoryName(outputFilePath)
    );
  }

  [TestCase("-c", "Debug")]
  [TestCase("-c", "Release")]
  [TestCase("--configuration", "Debug")]
  [TestCase("--configuration", "Release")]
  public void GetOutputFilePaths_WithConfigurationOption(string optionName, string configuration)
  {
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePaths = impl.GetOutputFilePaths(new[] {
      optionName, configuration,
      Path.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")
    });

    CollectionAssert.AreEquivalent(
      new[] {
        "LibA-1.0.0.0-netstandard2.1.apilist.cs",
        "LibA-1.0.0.0-net5.0.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
  }

  [TestCase("-f", "net5.0")]
  [TestCase("-f", "net5.0")]
  [TestCase("--framework", "netstandard2.1")]
  [TestCase("--framework", "netstandard2.1")]
  public void GetOutputFilePaths_WithTargetFrameworkOption(string optionName, string targetFramework)
  {
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePaths = impl.GetOutputFilePaths(new[] {
      optionName, targetFramework,
      Path.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")
    });

    CollectionAssert.AreEquivalent(
      new[] {
        $"LibA-1.0.0.0-{targetFramework}.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
  }

  [TestCase("-r", "win-x64")]
  [TestCase("-r", "linux-x64")]
  [TestCase("--runtime", "win-x64")]
  [TestCase("--runtime", "linux-x64")]
  public void GetOutputFilePaths_WithRuntimeOption(string optionName, string runtime)
  {
    var impl = new RootCommandImplementation(serviceProvider);
    var outputFilePaths = impl.GetOutputFilePaths(new[] {
      optionName, runtime,
      Path.Join(TestAssemblyInfo.RootDirectory.FullName, "LibA", "LibA.csproj")
    });

    CollectionAssert.AreEquivalent(
      new[] {
        "LibA-1.0.0.0-netstandard2.1.apilist.cs",
        "LibA-1.0.0.0-net5.0.apilist.cs",
      },
      outputFilePaths.Select(f => Path.GetFileName(f))
    );
  }
}