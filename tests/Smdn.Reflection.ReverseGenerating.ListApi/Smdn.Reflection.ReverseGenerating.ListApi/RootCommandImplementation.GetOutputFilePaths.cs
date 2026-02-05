// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
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
public class RootCommandImplementationGetOutputFilePathsTests {
  private ServiceProvider? serviceProvider;

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
    serviceProvider?.Dispose();
  }

  private static string GetCurrentDirectory() => TestContext.CurrentContext.WorkDirectory;

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
}
