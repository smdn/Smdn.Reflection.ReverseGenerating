// SPDX-FileCopyrightText: 2021 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;

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
    [Values] bool loadIntoReflectionOnlyContext
  )
  {
    Assert.Throws<ArgumentNullException>(() => {
      AssemblyLoader.UsingAssembly(
        assemblyFile: null!,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: 0,
        (assembly, arg) => arg,
        context: out _,
        logger: null
      );
    });
  }

  [Test]
  public void UsingAssembly_ArgumentNull_AssemblyStream(
    [Values] bool loadIntoReflectionOnlyContext
  )
  {
    Assert.Throws<ArgumentNullException>(() => {
      AssemblyLoader.UsingAssembly(
        assemblyStream: null!,
        componentAssemblyPath: string.Empty,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: 0,
        (assembly, arg) => arg,
        context: out _,
        logger: null
      );
    });
  }

  [Test]
  public void UsingAssembly_ArgumentNull_ComponentAssemblyPath(
    [Values] bool loadIntoReflectionOnlyContext
  )
  {
    Assert.Throws<ArgumentNullException>(() => {
      AssemblyLoader.UsingAssembly(
        assemblyStream: Stream.Null,
        componentAssemblyPath: null!,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: 0,
        (assembly, arg) => arg,
        context: out _,
        logger: null
      );
    });
  }

#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
#if NET10_0_OR_GREATER
  [TestCase(true, "net10.0")]
  [TestCase(false, "net10.0")]
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
      (assembly, arg) => {
        Assert.That(assemblyFile, Is.SameAs(arg), nameof(arg));

        Assert.That(assembly, Is.Not.Null, nameof(assembly));
        Assert.That(assembly.Location, Is.EqualTo(arg.FullName), nameof(assembly.Location));

        Assert.DoesNotThrow(() => assembly.GetExportedTypes(), nameof(assembly.GetExportedTypes));

        return assembly.GetType("Lib.LibA.CBase")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.That(result, Is.Not.Null, nameof(result));
    Assert.That(result, Is.EqualTo("Lib.LibA.CBase"), nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.That(context, Is.Null, nameof(context));
      return;
    }

    Assert.That(context, Is.Not.Null, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.That(unloaded, Is.True, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
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

    Assert.That(result, Is.Default, nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.That(context, Is.Null, nameof(context));
      return;
    }

    Assert.That(context, Is.Not.Null, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.That(unloaded, Is.True, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
#if NET10_0_OR_GREATER
  [TestCase(true, "net10.0")]
  [TestCase(false, "net10.0")]
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
      (assembly, arg) => {
        Assert.That(assembly, Is.Not.Null, nameof(assembly));

        if (loadIntoReflectionOnlyContext)
          Assert.That(assembly.Location, Is.EqualTo(arg.FullName), nameof(assembly.Location));
        else
          Assert.That(assembly.Location, Is.Empty, nameof(assembly.Location));

        Assert.DoesNotThrow(() => assembly.GetExportedTypes(), nameof(assembly.GetExportedTypes));

        return assembly.GetType("Lib.LibA.CBase")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.That(result, Is.Not.Null, nameof(result));
    Assert.That(result, Is.EqualTo("Lib.LibA.CBase"), nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.That(context, Is.Null, nameof(context));
      return;
    }

    Assert.That(context, Is.Not.Null, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.That(unloaded, Is.True, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
#if NET10_0_OR_GREATER
  [TestCase(true, "net10.0")]
  [TestCase(false, "net10.0")]
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
      (assembly, arg) => {
        Assert.That(assemblyFile, Is.SameAs(arg), nameof(arg));

        Assert.That(assembly, Is.Not.Null, nameof(assembly));
        Assert.That(assembly.Location, Is.EqualTo(arg.FullName), nameof(assembly.Location));

        Assert.DoesNotThrow(() => assembly.GetExportedTypes(), nameof(assembly.GetExportedTypes));

        return assembly.GetType("Lib.LibB.CEx")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.That(result, Is.Not.Null, nameof(result));
    Assert.That(result, Is.EqualTo("Lib.LibB.CEx"), nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.That(context, Is.Null, nameof(context));
      return;
    }

    Assert.That(context, Is.Not.Null, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.That(unloaded, Is.True, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
#if NET10_0_OR_GREATER
  [TestCase(true, "net10.0")]
  [TestCase(false, "net10.0")]
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
      (assembly, arg) => {
        Assert.That(assembly, Is.Not.Null, nameof(assembly));

        if (loadIntoReflectionOnlyContext)
          Assert.That(assembly.Location, Is.EqualTo(arg.FullName), nameof(assembly.Location));
        else
          Assert.That(assembly.Location, Is.Empty, nameof(assembly.Location));

        Assert.DoesNotThrow(() => assembly.GetExportedTypes(), nameof(assembly.GetExportedTypes));

        return assembly.GetType("Lib.LibB.CEx")?.FullName;
      },
      context: out var context,
      logger: logger
    );

    Assert.That(result, Is.Not.Null, nameof(result));
    Assert.That(result, Is.EqualTo("Lib.LibB.CEx"), nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.That(context, Is.Null, nameof(context));
      return;
    }

    Assert.That(context, Is.Not.Null, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.That(unloaded, Is.True, nameof(unloaded));
  }

#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
  [TestCase(true, "netstandard2.1")]
  [TestCase(false, "netstandard2.1")]
#endif
#if NET8_0_OR_GREATER
  [TestCase(true, "net8.0")]
  [TestCase(false, "net8.0")]
#endif
#if NET10_0_OR_GREATER
  [TestCase(true, "net10.0")]
  [TestCase(false, "net10.0")]
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
      (assembly, arg) => {
        Assert.That(assemblyFile, Is.SameAs(arg), nameof(arg));

        Assert.That(assembly, Is.Not.Null, nameof(assembly));
        Assert.That(assembly.Location, Is.EqualTo(arg.FullName), nameof(assembly.Location));

        Assert.DoesNotThrow(() => assembly.GetExportedTypes(), nameof(assembly.GetExportedTypes));

        return assembly
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

    Assert.That(result, Is.Not.Null, nameof(result));
    Assert.That(result, Is.EqualTo("Microsoft.Extensions.Logging.ILogger"), nameof(result));

    if (loadIntoReflectionOnlyContext) {
      Assert.That(context, Is.Null, nameof(context));
      return;
    }

    Assert.That(context, Is.Not.Null, nameof(context));

    var unloaded = false;

    for (var i = 0; i < 10; i++) {
      if (!context!.IsAlive) {
        unloaded = true;
        break;
      }

      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    Assert.That(unloaded, Is.True, nameof(unloaded));
  }
}
