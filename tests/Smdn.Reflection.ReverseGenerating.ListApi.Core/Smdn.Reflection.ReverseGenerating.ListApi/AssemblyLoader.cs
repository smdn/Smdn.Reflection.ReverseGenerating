// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating.ListApi.Core; // TestAssemblyInfo


#if SYSTEM_IO_PATH_JOIN
using PathJoiner = System.IO.Path;
#else
using PathJoiner = Smdn.Reflection.ReverseGenerating.ListApi.Shim.Path;
#endif

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class AssemblyLoaderTests {
  private ILogger? logger = null;

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
  }

  [Test]
  public void UsingAssembly_ArgumentNull_AssemblyFile(
    [Values(true, false)] bool loadIntoReflectionOnlyContext
  )
  {
    Assert.Throws<ArgumentNullException>(() => {
      AssemblyLoader.UsingAssembly(
        assemblyFile: null!,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: 0,
        (assm, arg) => arg,
        context: out _,
        logger: null
      );
    });
  }

  [Test]
  public void UsingAssembly_ArgumentNull_AssemblyStream(
    [Values(true, false)] bool loadIntoReflectionOnlyContext
  )
  {
    Assert.Throws<ArgumentNullException>(() => {
      AssemblyLoader.UsingAssembly(
        assemblyStream: null!,
        componentAssemblyPath: string.Empty,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: 0,
        (assm, arg) => arg,
        context: out _,
        logger: null
      );
    });
  }

  [Test]
  public void UsingAssembly_ArgumentNull_ComponentAssemblyPath(
    [Values(true, false)] bool loadIntoReflectionOnlyContext
  )
  {
    Assert.Throws<ArgumentNullException>(() => {
      AssemblyLoader.UsingAssembly(
        assemblyStream: Stream.Null,
        componentAssemblyPath: null!,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: 0,
        (assm, arg) => arg,
        context: out _,
        logger: null
      );
    });
  }

#if NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET6_0_OR_GREATER
  [TestCase(true, "net6.0")]
  [TestCase(false, "net6.0")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
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
        Assert.AreEqual(arg.FullName, assm.Location, nameof(assm.Location));

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
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
  public void UsingAssembly_ArgumentNull_ActionWithLoadedAssembly(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains("LibA.dll"))
    );

    WeakReference? context = null;
    int result = 1;

    Assert.DoesNotThrow(() => {
      result = AssemblyLoader.UsingAssembly<int, int>(
        assemblyFile,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: int.MaxValue,
        actionWithLoadedAssembly: null,
        context: out context,
        logger: logger
      );
    });

    Assert.AreEqual(default(int), result, nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.IsNull(context, nameof(context));
      return;
    }

    Assert.IsNotNull(context, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET6_0_OR_GREATER
  [TestCase(true, "net6.0")]
  [TestCase(false, "net6.0")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
  public void UsingAssembly_FromStream(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains("LibA.dll"))
    );
    using var assemblyStream = File.OpenRead(assemblyFile.FullName);

    var result = AssemblyLoader.UsingAssembly(
      assemblyStream,
      componentAssemblyPath: assemblyFile.FullName,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      arg: assemblyFile,
      (assm, arg) => {
        Assert.IsNotNull(assm, nameof(assm));

        if (loadIntoReflectionOnlyContext)
          Assert.AreEqual(arg.FullName, assm.Location, nameof(assm.Location));
        else
          Assert.IsEmpty(assm.Location, nameof(assm.Location));

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
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET6_0_OR_GREATER
  [TestCase(true, "net6.0")]
  [TestCase(false, "net6.0")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
  public void UsingAssembly_ResolveDependency_ProjectReference(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
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
        Assert.AreEqual(arg.FullName, assm.Location, nameof(assm.Location));

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
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET6_0_OR_GREATER
  [TestCase(true, "net6.0")]
  [TestCase(false, "net6.0")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
  public void UsingAssembly_FromStream_ResolveDependency_ProjectReference(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains("LibB.dll"))
    );
    using var assemblyStream = File.OpenRead(assemblyFile.FullName);

    var result = AssemblyLoader.UsingAssembly(
      assemblyStream,
      componentAssemblyPath: assemblyFile.FullName,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      arg: assemblyFile,
      (assm, arg) => {
        Assert.IsNotNull(assm, nameof(assm));

        if (loadIntoReflectionOnlyContext)
          Assert.AreEqual(arg.FullName, assm.Location, nameof(assm.Location));
        else
          Assert.IsEmpty(assm.Location, nameof(assm.Location));

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
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET6_0_OR_GREATER
  [TestCase(true, "net6.0")]
  [TestCase(false, "net6.0")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
  public void UsingAssembly_ResolveDependency_PackageReference(bool loadIntoReflectionOnlyContext, string targetFrameworkMoniker)
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains("LibPackageReferences1.dll"))
    );

    var result = AssemblyLoader.UsingAssembly(
      assemblyFile,
      loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
      arg: assemblyFile,
      (assm, arg) => {
        Assert.AreSame(arg, assemblyFile, nameof(arg));

        Assert.IsNotNull(assm, nameof(assm));
        Assert.AreEqual(arg.FullName, assm.Location, nameof(assm.Location));

        Assert.DoesNotThrow(() => assm.GetExportedTypes(), nameof(assm.GetExportedTypes));

        return assm
          .GetType("C")
          ?.GetMethod("M", BindingFlags.Public | BindingFlags.Static)
          ?.GetParameters()
          ?[0]
          ?.ParameterType
          ?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.IsNotNull(result, nameof(result));
    Assert.AreEqual(result, "Microsoft.Extensions.Logging.ILogger", nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.IsNull(context, nameof(context));
      return;
    }

    Assert.IsNotNull(context, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.IsTrue(unloaded, nameof(unloaded));
  }
}
