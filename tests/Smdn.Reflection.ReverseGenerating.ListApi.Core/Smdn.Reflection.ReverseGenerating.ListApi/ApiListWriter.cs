// SPDX-FileCopyrightText: 2023 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NUnit.Framework;
using Smdn.IO;
using Smdn.Reflection.ReverseGenerating.ListApi.Core; // TestAssemblyInfo

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
public partial class ApiListWriterTests {
  private ILogger? logger = null;

  [OneTimeSetUp]
  public void Init()
  {
    var services = new ServiceCollection();

    services.AddLogging(
      builder => builder
        .AddSimpleConsole(static options => options.SingleLine = true)
        .AddFilter(level => LogLevel.Debug <= level)
    );

    logger = services.BuildServiceProvider().GetService<ILoggerFactory>()?.CreateLogger("test");
  }

  [Test]
  public void Ctor()
    => Assert.DoesNotThrow(() => new ApiListWriter(TextWriter.Null, Assembly.GetExecutingAssembly(), new()));

  [Test]
  public void Ctor_OptionsNull()
    => Assert.DoesNotThrow(() => new ApiListWriter(TextWriter.Null, Assembly.GetExecutingAssembly(), options: null));

  [Test]
  public void Ctor_BaseWriterNull()
    => Assert.Throws<ArgumentNullException>(() => new ApiListWriter(baseWriter: null!, Assembly.GetExecutingAssembly(), new()));

  [Test]
  public void Ctor_AssemblyNull()
    => Assert.Throws<ArgumentNullException>(() => new ApiListWriter(TextWriter.Null, assembly: null!, new()));

  private static void CreateAssemblyFromSourceCode(
    Stream outputAssemblyStream,
    string csharpSourceCode,
    string? assemblyName = null,
    IEnumerable<string>? referenceAssemblyFileNames = null,
    IEnumerable<ResourceDescription>? manifestResources = null
  )
  {
    const string defaultAssemblyName = "TestAssembly";

    var references = (referenceAssemblyFileNames ?? Enumerable.Repeat(typeof(object).Assembly.GetName().Name + ".dll", 1))
      .Distinct()
      .Select(static name =>
        MetadataReference.CreateFromFile(
          Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), name),
          MetadataReferenceProperties.Assembly
        )
      );

    var parseOptions = CSharpParseOptions.Default
      .WithLanguageVersion(LanguageVersion.Latest);
    var syntaxTree = CSharpSyntaxTree.ParseText(
      text: csharpSourceCode,
      options: parseOptions
    );
    var compilationOptions = new CSharpCompilationOptions(
      outputKind: OutputKind.DynamicallyLinkedLibrary,
      platform: Platform.AnyCpu,
      nullableContextOptions: NullableContextOptions.Disable
    );
    var compilation = CSharpCompilation.Create(
      assemblyName: assemblyName ?? defaultAssemblyName,
      syntaxTrees: Enumerable.Repeat(syntaxTree, 1),
      references: references,
      options: compilationOptions
    );

    var emitResult = compilation.Emit(
      peStream: outputAssemblyStream,
      manifestResources: manifestResources
#if false
      options: new Microsoft.CodeAnalysis.Emit.EmitOptions(
        runtimeMetadataVersion: "v4.0.30319" // ??? ref: https://learn.microsoft.com/ja-jp/dotnet/standard/assembly/view-contents
      )
#endif
    );

    foreach (var diag in emitResult.Diagnostics) {
      var span = diag.Location.GetLineSpan();
      var location = $"{span.Path}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1})";

#pragma warning disable CA1305 // cannot pass IFormatProvider
      TestContext.Out.WriteLine($"{TestContext.CurrentContext.Test.FullName} {location}: {diag.Severity} {diag.Id}: {diag.GetMessage()}");
#pragma warning restore CA1305
    }

    if (!emitResult.Success)
      throw new InvalidOperationException("Compilation failed");
  }

  private string WriteApiListFromSourceCode(
    string csharpSourceCode,
    ApiListWriterOptions apiListWriterOptions,
    string? assemblyName = null,
    IEnumerable<string>? referenceAssemblyFileNames = null,
    IEnumerable<ResourceDescription>? manifestResources = null
  )
  {
    using var assemblyStream = new MemoryStream();

    CreateAssemblyFromSourceCode(
      assemblyStream,
      csharpSourceCode,
      assemblyName,
      referenceAssemblyFileNames,
      manifestResources
    );

    assemblyStream.Position = 0L;

    var ret = AssemblyLoader.UsingAssembly(
      assemblyStream: assemblyStream,
      componentAssemblyPath: ".",
      loadIntoReflectionOnlyContext: true,
      arg: apiListWriterOptions,
      logger: logger,
      actionWithLoadedAssembly: static (assembly, arg) => {
        var sb = new StringBuilder();
        var writer = new ApiListWriter(new StringWriter(sb), assembly, arg);

        writer.WriteHeader();
        writer.WriteExportedTypes();
        writer.WriteFooter();

        return sb.ToString();
      },
      context: out var context
    );

    // wait for the context to be collected
    if (context is null)
      return ret ?? string.Empty;

    while (context.IsAlive) {
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }

    return ret ?? string.Empty;
  }

  private static System.Collections.IEnumerable YieldTestCases_WriteExportedTypes_ReferencingNamespaces()
  {
    yield return new object[] {
      @"public static class C {
  public static void M(int p) { }
  public static void M(System.Guid p) { }
  public static void M(System.Collections.Generic.List<int> p) { }
  public static void M(System.IO.Stream p) { }
}",
      new[] {
        typeof(Guid).Assembly.GetName().Name + ".dll",
        typeof(List<>).Assembly.GetName().Name + ".dll",
        typeof(Stream).Assembly.GetName().Name + ".dll",
      },
      new[] { "using System;", "using System.Collections.Generic;", "using System.IO;" }
    };

    yield return new object[] {
      @"public static class C {
  public static void M(int p) { }
}",
      new string[0],
      new string[0]
    };


    yield return new object[] {
      @"
namespace MyNamespace {
  public class C {
    public static void M(MyNamespace.S p) { }
    public static void M(Microsoft.C p) { }
    public static void M(System.C p) { }
  }

  public struct S { }
}

namespace System {
  public class C { }
}

namespace Microsoft {
  public class C { }
}
",
      new string[0],
      new[] { "using System;", "using Microsoft;", "using MyNamespace;" }
    };
  }

  [TestCaseSource(nameof(YieldTestCases_WriteExportedTypes_ReferencingNamespaces))]
  public void WriteExportedTypes_ReferencingNamespaces(
    string sourceCode,
    string[] referenceAssemblyFileNames,
    string[] expectedUsingDirectives
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = false;

    var usingDirectives = new StringReader(WriteApiListFromSourceCode(sourceCode, options))
      .ReadAllLines()
      .Where(static line => line.StartsWith("using ", StringComparison.Ordinal));

    Assert.That(expectedUsingDirectives, Is.EqualTo(usingDirectives).AsCollection);
  }

  private static System.Collections.IEnumerable YieldTestCases_WriteExportedTypes_ReferencingNamespaces_NewLine()
  {
    yield return new object[] {
      @"public static class C {
  public static void M(int p) {}
  public static void M(System.Guid p) {}
}",
      new[] {
        typeof(Guid).Assembly.GetName().Name + ".dll",
      },
      @"
using System;

public static class C {
  public static void M(Guid p) {}
  public static void M(int p) {}
}"
    };

    yield return new object[] {
      @"public static class C {
  public static void M(int p) {}
}",
      new string[0],
      @"
public static class C {
  public static void M(int p) {}
}"
    };
  }

  [TestCaseSource(nameof(YieldTestCases_WriteExportedTypes_ReferencingNamespaces_NewLine))]
  public void WriteExportedTypes_ReferencingNamespaces_NewLine(
    string sourceCode,
    string[] referenceAssemblyFileNames,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = false;

    var output =
      string.Join(
        "\n",
        new StringReader(WriteApiListFromSourceCode(sourceCode, options)).ReadAllLines()
      );

    Assert.That(output.TrimEnd(), Is.EqualTo(expectedOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd()));
  }

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
  private static System.Collections.IEnumerable YieldTestCases_WriteExportedTypes_NullableAnnotationDirective()
  {
    foreach (var (contextForType, contextForMember) in new[] {
      (new NullabilityInfoContext(), new NullabilityInfoContext()),
      (default(NullabilityInfoContext), new NullabilityInfoContext()),
      (new NullabilityInfoContext(), default(NullabilityInfoContext)),
      (default(NullabilityInfoContext), default(NullabilityInfoContext))
    }) {
      var directiveEnableOnAssembly = contextForType is not null && contextForMember is not null;
      var directiveDisableOnAssembly = contextForType is null && contextForMember is null;
      var directiveOnType = contextForType is not null && contextForMember is null;
      var directiveOnMember = contextForType is null && contextForMember is not null;

      yield return new object[] {
        @"#nullable enable
public static class C {
  public static void M(string? p) {}
}",
        new string[0],
        true,
        contextForType!,
        contextForMember!,
        $@"{(directiveEnableOnAssembly ? "#nullable enable annotations\n" : string.Empty)}{(directiveDisableOnAssembly ? "#nullable disable annotations\n" : string.Empty)}
{(directiveOnType ? "#nullable enable annotations\n" : string.Empty)}public static class C {{{(directiveOnType ? "\n#nullable restore annotations" : string.Empty)}
{(directiveOnMember ? "#nullable enable annotations\n" : string.Empty)}  public static void M(string{(contextForMember is null ? "" : "?")} p) {{}}{(directiveOnMember ? "\n#nullable restore annotations" : string.Empty)}
}}"
      };
    }

    foreach (var (contextForType, contextForMember) in new[] {
      (new NullabilityInfoContext(), new NullabilityInfoContext()),
      (default(NullabilityInfoContext), new NullabilityInfoContext()),
      (new NullabilityInfoContext(), default(NullabilityInfoContext)),
      (default(NullabilityInfoContext), default(NullabilityInfoContext))
    }) {
      yield return new object[] {
        @"#nullable enable
public static class C {
  public static void M(string? p) {}
}",
        new string[0],
        false,
        contextForType!,
        contextForMember!,
        @$"
public static class C {{
  public static void M(string{(contextForMember is null ? "" : "?")} p) {{}}
}}"
      };
    }
  }

  [TestCaseSource(nameof(YieldTestCases_WriteExportedTypes_NullableAnnotationDirective))]
  public void WriteExportedTypes_NullableAnnotationDirective(
    string sourceCode,
    string[] referenceAssemblyFileNames,
    bool writeNullableAnnotationDirective,
    NullabilityInfoContext typeDeclarationNullabilityInfoContext,
    NullabilityInfoContext memberDeclarationNullabilityInfoContext,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = writeNullableAnnotationDirective;
    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = false;

    options.TypeDeclaration.NullabilityInfoContext = typeDeclarationNullabilityInfoContext;
    options.MemberDeclaration.NullabilityInfoContext = memberDeclarationNullabilityInfoContext;
    options.AttributeDeclaration.TypeFilter = AttributeFilter.Default;

    var output =
      string.Join(
        "\n",
        new StringReader(WriteApiListFromSourceCode(sourceCode, options)).ReadAllLines()
      );

    Assert.That(output.TrimEnd(), Is.EqualTo(expectedOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd()));
  }
#endif // SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

  private static System.Collections.IEnumerable YieldTestCases_WriteHeader_WriteAssemblyInfo()
  {
    yield return new object[] {
      @"
[assembly: System.Reflection.AssemblyProductAttribute(""Product"")]
[assembly: System.Reflection.AssemblyVersionAttribute(""1.2.3.4"")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute(""1.2.3-InformationalVersion"")]
[assembly: System.Reflection.AssemblyConfigurationAttribute(""Configuration"")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(""TargetFramework"")]
",
      "TestCase1Assembly",
      new[] {
        typeof(AssemblyInformationalVersionAttribute).Assembly.GetName().Name + ".dll",
        typeof(System.Runtime.Versioning.TargetFrameworkAttribute).Assembly.GetName().Name + ".dll",
      },
      true,
      @"//  (Product)
//   Name: TestCase1Assembly
//   AssemblyVersion: 1.2.3.4
//   InformationalVersion: 1.2.3-InformationalVersion
//   TargetFramework: TargetFramework
//   Configuration: Configuration"
    };

    yield return new object[] {
      @"// empty assembly",
      "TestCase2Assembly",
      null!,
      true,
      "//  ()\n" +
      "//   Name: TestCase2Assembly\n" +
      "//   AssemblyVersion: 0.0.0.0\n" +
      "//   InformationalVersion: \n" +
      "//   TargetFramework: \n" +
      "//   Configuration: \n"
    };

    yield return new object[] {
      @"// empty assembly",
      "TestCase3Assembly",
      null!,
      false,
      string.Empty
    };
  }

  [TestCaseSource(nameof(YieldTestCases_WriteHeader_WriteAssemblyInfo))]
  public void WriteHeader_WriteAssemblyInfo(
    string sourceCode,
    string assemblyName,
    string[] referenceAssemblyFileNames,
    bool writeAssemblyInfo,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteHeader = true;
    options.Writer.WriteAssemblyInfo = writeAssemblyInfo;
    options.Writer.WriteEmbeddedResources = false;
    options.Writer.WriteReferencedAssemblies = false;
    options.Writer.WriteFooter = false;

    Assert.That(
      WriteApiListFromSourceCode(
        sourceCode,
        options,
        assemblyName: assemblyName,
        referenceAssemblyFileNames: referenceAssemblyFileNames
      ).Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd(),
      Is.EqualTo(expectedOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd())
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_WriteHeader_WriteEmbeddedResources()
  {
    foreach (var writeEmbeddedResources in new[] { true, false }) {
      yield return new object[] {
        writeEmbeddedResources,
        new[] {
          new ResourceDescription(
            resourceName: "resource-1.txt",
            dataProvider: static () => new MemoryStream(new byte[] { 0, 1, 2, 3 }),
            isPublic: true
          ),
          new ResourceDescription(
            resourceName: "resource-2.txt",
            dataProvider: static () => Stream.Null,
            isPublic: true
          ),
          new ResourceDescription(
            resourceName: "resource-3.txt",
            dataProvider: static () => new MemoryStream(new byte[1024]),
            isPublic: true
          ),
        },
        @"//   Embedded resources:
//     resource-1.txt (4 bytes, Embedded, ContainedInManifestFile)
//     resource-2.txt (0 bytes, Embedded, ContainedInManifestFile)
//     resource-3.txt (1,024 bytes, Embedded, ContainedInManifestFile)
"
      };
    }
  }

  [TestCaseSource(nameof(YieldTestCases_WriteHeader_WriteEmbeddedResources))]
  public void WriteHeader_WriteEmbeddedResources(
    bool writeEmbeddedResources,
    IEnumerable<ResourceDescription> manifestResources,
    string expectedEmbeddedResourcesOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteHeader = true;
    options.Writer.WriteAssemblyInfo = false;
    options.Writer.WriteEmbeddedResources = writeEmbeddedResources;
    options.Writer.WriteReferencedAssemblies = false;

    if (writeEmbeddedResources) {
      Assert.That(
        WriteApiListFromSourceCode(
          csharpSourceCode: "//",
          options,
          manifestResources: manifestResources
        ).Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd(),
        Does.Contain(expectedEmbeddedResourcesOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd())
      );
    }
    else {
      Assert.That(
        WriteApiListFromSourceCode(
          csharpSourceCode: "//",
          options,
          manifestResources: null
        ),
        Does.Not.Contain("//   Embedded resources:")
      );
    }
  }

  [Test]
  public void WriteHeader_WriteEmbeddedResources_HasNoEmbeddedResources(
    [Values] bool writeEmbeddedResources
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteHeader = true;
    options.Writer.WriteAssemblyInfo = false;
    options.Writer.WriteEmbeddedResources = writeEmbeddedResources;
    options.Writer.WriteReferencedAssemblies = false;

    Assert.That(
      WriteApiListFromSourceCode(
        csharpSourceCode: "//",
        options,
        manifestResources: null
      ),
      Does.Not.Contain("//   Embedded resources:")
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_WriteHeader_WriteReferencedAssemblies()
  {
    static IEnumerable<(
      string AssemblyName,
      string TargetFrameworkMoniker,
      string[] ExpectedReferencedAssemblies
    )> YieldTestCases()
    {
#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
      yield return (
        "Lib",
        "netstandard2.1",
        new[] {
          "netstandard, Version=2.1.",
        }
      );
#endif
#if NET8_0_OR_GREATER
      yield return (
        "Lib",
        "net8.0",
        new[] {
          "System.Runtime, Version=8.0.",
        }
      );
#endif
#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
      yield return (
        "LibB",
        "netstandard2.1",
        new[] {
          "netstandard, Version=2.1.",
          "LibA, Version=",
        }
      );
#endif
#if NETCOREAPP3_1_OR_GREATER || NET8_0_OR_GREATER
      yield return (
        "LibReferencedAssemblies1",
        "netstandard2.1",
        new[] {
          "netstandard, Version=2.1.",
        }
      );
#endif
#if NET8_0_OR_GREATER
      yield return (
        "LibReferencedAssemblies1",
        "net8.0",
        new[] {
          "System.Runtime, Version=8.0.",
          "System.Threading, Version=8.0.",
          "System.Xml.ReaderWriter, Version=8.0.",
        }
      );
#endif
    }

    foreach (var loadIntoReflectionOnlyContext in new[] { true, false }) {
      foreach (var writeReferencedAssemblies in new[] { true, false }) {
        foreach (var (assemblyName, targetFrameworkMoniker, expectedReferencedAssemblies) in YieldTestCases()) {
          yield return new object[] {
            assemblyName,
            targetFrameworkMoniker,
            writeReferencedAssemblies,
            loadIntoReflectionOnlyContext,
            expectedReferencedAssemblies
          };
        }
      }
    }
  }

  // Cannot use `WriteApiListFromSourceCode` for testing WriteReferencedAssemblies.
  // The type of the assembly generated by CSharpCompilation.Emit and loaded by AssemblyLoader, will be System.Reflection.TypeLoading.Ecma.EcmaAssembly.
  // And Assembly.TryGetRawMetadata returns false if the type of input assembly is System.Reflection.TypeLoading.Ecma.EcmaAssembly.
  // Therefore, the referenced assemblies cannot be read from the assembly generated and loaded with WriteApiListFromSourceCode.
  [TestCaseSource(nameof(YieldTestCases_WriteHeader_WriteReferencedAssemblies))]
  public void WriteReferencedAssemblies(
    string assemblyFileName,
    string targetFrameworkMoniker,
    bool writeReferencedAssemblies,
    bool loadIntoReflectionOnlyContext,
    string[] expectedReferencedAssemblies
  )
  {
    var assemblyFile = new FileInfo(
      TestAssemblyInfo.TestAssemblyPaths.First(f => f.Contains(targetFrameworkMoniker) && f.Contains(assemblyFileName))
    );

    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteHeader = true;
    options.Writer.WriteAssemblyInfo = false;
    options.Writer.WriteEmbeddedResources = false;
    options.Writer.WriteReferencedAssemblies = writeReferencedAssemblies;

    var generated = GenerateApiListFrom(assemblyFile, loadIntoReflectionOnlyContext, options);

    if (writeReferencedAssemblies) {
      Assert.That(generated, Does.Contain("//   Referenced assemblies:"));

      foreach (var expectedReferencedAssembly in expectedReferencedAssemblies) {
        Assert.That(generated, Does.Contain($"//     {expectedReferencedAssembly}"));
      }
    }
    else {
      Assert.That(generated, Does.Not.Contain("//   Referenced assemblies:"));
    }

    string GenerateApiListFrom(
      FileInfo assemblyFile,
      bool loadIntoReflectionOnlyContext,
      ApiListWriterOptions options
    )
    {
      var ret = AssemblyLoader.UsingAssembly(
        assemblyFile,
        loadIntoReflectionOnlyContext: loadIntoReflectionOnlyContext,
        arg: options,
        logger: logger,
        actionWithLoadedAssembly: static (assembly, options) => {
          var sb = new StringBuilder();
          var writer = new ApiListWriter(new StringWriter(sb), assembly, options);

          writer.WriteHeader();
          writer.WriteExportedTypes();

          return sb.ToString();
        },
        context: out var context
      );

      // wait for the context to be collected
      if (context is null)
        return ret ?? string.Empty;

      while (context.IsAlive) {
        GC.Collect();
        GC.WaitForPendingFinalizers();
      }

      return ret ?? string.Empty;
    }
  }

  [Test]
  public void WriteFooter(
    [Values] bool writeFooter
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteHeader = false;
    options.Writer.WriteFooter = writeFooter;

    var output = WriteApiListFromSourceCode("//", options).TrimEnd();
    var executingAssemblyName = Assembly.GetExecutingAssembly().GetName()!;
    var libraryAssemblyName = typeof(ApiListWriter).Assembly.GetName()!;

    if (writeFooter) {
      Assert.That(output, Does.Contain($"// API list generated by {executingAssemblyName.Name} v{executingAssemblyName.Version}"));
      Assert.That(output, Does.Contain($"// {libraryAssemblyName.Name} v{libraryAssemblyName.Version}"));
    }
    else {
      Assert.That(output, Does.Not.Contain("// API list generated by"));
      Assert.That(output, Does.Not.Contain($"// {libraryAssemblyName.Name} v{libraryAssemblyName.Version}"));
    }
  }
}
