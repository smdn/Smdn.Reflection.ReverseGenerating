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
using NUnit.Framework;
using Smdn.IO;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
class ApiListWriterTests {
  private static void CreateAssemblyFromSourceCode(
    Stream outputAssemblyStream,
    string csharpSourceCode,
    string? assemblyName = null,
    IEnumerable<string>? referenceAssemblyFileName = null
  )
  {
    const string defaultAssemblyName = "TestAssembly";

    var references = (referenceAssemblyFileName ?? Enumerable.Repeat(typeof(object).Assembly.GetName().Name + ".dll", 1))
      .Distinct()
      .Select(name => MetadataReference.CreateFromFile(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), name)));

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

    var emitResult = compilation.Emit(peStream: outputAssemblyStream);

    foreach (var diag in emitResult.Diagnostics) {
      var span = diag.Location.GetLineSpan();
      var location = $"{span.Path}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1})";

      TestContext.WriteLine($"{location}: {diag.Severity} {diag.Id}: {diag.GetMessage()}");
    }

    if (!emitResult.Success)
      throw new InvalidOperationException("Compilation failed");
  }

  private static string WriteApiListFromSourceCode(
    string csharpSourceCode,
    ApiListWriterOptions apiListWriterOptions,
    string? assemblyName = null,
    IEnumerable<string>? referenceAssemblyFileName = null
  )
  {
    using var assemblyStream = new MemoryStream();

    CreateAssemblyFromSourceCode(
      assemblyStream,
      csharpSourceCode,
      assemblyName,
      referenceAssemblyFileName
    );

    assemblyStream.Position = 0L;

    var ret = AssemblyLoader.UsingAssembly(
      assemblyStream: assemblyStream,
      componentAssemblyPath: ".",
      loadIntoReflectionOnlyContext: true,
      arg: apiListWriterOptions,
      static (assm, arg) => {
        var sb = new StringBuilder();
        var writer = new ApiListWriter(new StringWriter(sb), assm, arg);

        writer.WriteAssemblyInfoHeader();
        writer.WriteExportedTypes();

        return sb.ToString();
      },
      out var context
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
    string[] referenceAssemblyFileName,
    string[] expectedUsingDirectives
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteEmbeddedResources = false;
    options.Writer.WriteReferencedAssemblies = false;

    var usingDirectives = new StringReader(WriteApiListFromSourceCode(sourceCode, options))
      .ReadAllLines()
      .Where(static line => line.StartsWith("using ", StringComparison.Ordinal));

    CollectionAssert.AreEqual(usingDirectives, expectedUsingDirectives);
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
    string[] referenceAssemblyFileName,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteEmbeddedResources = false;
    options.Writer.WriteReferencedAssemblies = false;

    var output =
      string.Join(
        "\n",
        new StringReader(WriteApiListFromSourceCode(sourceCode, options))
          .ReadAllLines()
          .Where(static line => !line.StartsWith("// ", StringComparison.Ordinal)) // remove header
      );

    Assert.AreEqual(
      expectedOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd(),
      output.TrimEnd()
    );
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
    string[] referenceAssemblyFileName,
    bool writeNullableAnnotationDirective,
    NullabilityInfoContext typeDeclarationNullabilityInfoContext,
    NullabilityInfoContext memberDeclarationNullabilityInfoContext,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = writeNullableAnnotationDirective;
    options.Writer.WriteEmbeddedResources = false;
    options.Writer.WriteReferencedAssemblies = false;

    options.TypeDeclaration.NullabilityInfoContext = typeDeclarationNullabilityInfoContext;
    options.MemberDeclaration.NullabilityInfoContext = memberDeclarationNullabilityInfoContext;
    options.AttributeDeclaration.TypeFilter = AttributeFilter.Default;

    var output =
      string.Join(
        "\n",
        new StringReader(WriteApiListFromSourceCode(sourceCode, options))
          .ReadAllLines()
          .Where(static line => !line.StartsWith("// ", StringComparison.Ordinal)) // remove header
      );

    Assert.AreEqual(
      expectedOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd(),
      output.TrimEnd()
    );
  }
#endif // SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

  private static System.Collections.IEnumerable YieldTestCases_WriteAssemblyInfoHeader()
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
      new string[0],
      "//  ()\n" +
      "//   Name: TestCase2Assembly\n" +
      "//   AssemblyVersion: 0.0.0.0\n" +
      "//   InformationalVersion: \n" +
      "//   TargetFramework: \n" +
      "//   Configuration: \n"
    };
  }

  [TestCaseSource(nameof(YieldTestCases_WriteAssemblyInfoHeader))]
  public void WriteAssemblyInfoHeader(
    string sourceCode,
    string assemblyName,
    string[] referenceAssemblyFileName,
    string expectedOutput
  )
  {
    var options = new ApiListWriterOptions();

    options.Writer.WriteNullableAnnotationDirective = false;
    options.Writer.WriteEmbeddedResources = false;
    options.Writer.WriteReferencedAssemblies = false;

    Assert.AreEqual(
      expectedOutput.Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd(),
      new StringReader(WriteApiListFromSourceCode(sourceCode, options, assemblyName: assemblyName)).ReadToEnd().Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd()
    );
  }
}
