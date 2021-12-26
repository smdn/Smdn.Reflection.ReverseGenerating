// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating.ListApi.Core; // TestAssemblyInfo


#if NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

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

    assemblyFileLibA = new FileInfo(TestAssemblyInfo.TestAssemblyPaths.First(static f => f.Contains("LibA.dll")));
    assemblyFileLibB = new FileInfo(TestAssemblyInfo.TestAssemblyPaths.First(static f => f.Contains("LibB.dll")));
  }

  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
  [TestCase(true, "net5.0")]
  [TestCase(false, "net5.0")]
  public void UsingAssembly(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains("LibA.dll"))
    );

    var result = AssemblyLoader.UsingAssembly(
      assemblyFile,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      arg: assemblyFile,
      (assm, arg) => {
        Assert.AreSame(arg, assemblyFile, nameof(arg));

        Assert.IsNotNull(assm, nameof(assm));
        Assert.AreEqual(assm.Location, arg.FullName, nameof(assm.Location));

        Assert.DoesNotThrow(() => assm.GetExportedTypes(), nameof(assm.GetExportedTypes));

        return assm.GetType("Lib.LibA.CBase")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.IsNotNull(result, nameof(result));
    Assert.AreEqual(result, "Lib.LibA.CBase", nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.IsNull(context, nameof(context));
      return;
    }

    Assert.IsNotNull(context, nameof(context));

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

  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
  [TestCase(true, "net5.0")]
  [TestCase(false, "net5.0")]
  public void UsingAssembly_ResolveDependency(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains("LibB.dll"))
    );

    var result = AssemblyLoader.UsingAssembly(
      assemblyFile,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      arg: assemblyFile,
      (assm, arg) => {
        Assert.AreSame(arg, assemblyFile, nameof(arg));

        Assert.IsNotNull(assm, nameof(assm));
        Assert.AreEqual(assm.Location, arg.FullName, nameof(assm.Location));

        Assert.DoesNotThrow(() => assm.GetExportedTypes(), nameof(assm.GetExportedTypes));

        return assm.GetType("Lib.LibB.CEx")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.IsNotNull(result, nameof(result));
    Assert.AreEqual(result, "Lib.LibB.CEx", nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.IsNull(context, nameof(context));
      return;
    }

    Assert.IsNotNull(context, nameof(context));

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
