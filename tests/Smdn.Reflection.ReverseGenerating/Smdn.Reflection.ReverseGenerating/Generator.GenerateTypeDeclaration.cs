// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class TypeDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
  public Type TestTargetType { get; }

  public TypeDeclarationTestCaseAttribute(
    string expected,
    [CallerFilePath] string sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : this(
      expected,
      testTargetType: null,
      sourceFilePath,
      lineNumber
    )
  {
  }

  public TypeDeclarationTestCaseAttribute(
    string expected,
    Type testTargetType,
    [CallerFilePath] string sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : base(
      expected,
      sourceFilePath,
      lineNumber
    )
  {
    this.TestTargetType = testTargetType;
  }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class ReferencingNamespacesTestCaseAttribute : GeneratorTestCaseAttribute {
  public ReferencingNamespacesTestCaseAttribute(
    string expected,
    [CallerFilePath] string sourceFilePath = null,
    [CallerLineNumber] int lineNumber = 0
  )
    : base(
      expected,
      sourceFilePath,
      lineNumber
    )
  {
    base.MemberWithNamespace = true;
    base.ValueUseDefaultLiteral = false;
  }

  public bool WithExplicitBaseTypeAndInterfaces { get; set; } = false;

  public IReadOnlyList<string> GetExpectedSet()
  {
#if SYSTEM_STRINGSPLITOPTIONS_TRIMENTRIES
    return Expected.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#else
    return Expected.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
#endif
  }
}

partial class GeneratorTests {
  [Test]
  public void GenerateTypeDeclaration_ArgumentOptionsNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateTypeDeclaration(t: typeof(int), null, options: null!));

  [Test]
  public void GenerateTypeDeclaration_ArgumentTypeNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateTypeDeclaration(t: null!, null, options: new()));

  [TestCase(typeof(List<int>))]
  [TestCase(typeof(IEnumerable<int>))]
  [TestCase(typeof(Action<int>))]
  [TestCase(typeof(int?))]
  [TestCase(typeof((int, int)))]
  public void GenerateTypeDeclaration_ArgumentTypeIsConstructedGenericType(Type type)
    => Assert.Throws<ArgumentException>(() => Generator.GenerateTypeDeclaration(t: type, null, options: new()));

  private static System.Collections.IEnumerable YieldTestCases_GenerateTypeDeclaration()
    => GetTestCaseTypes()
      .SelectMany(
        static t => t.GetCustomAttributes<TypeDeclarationTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateTypeDeclaration))]
  public void GenerateTypeDeclaration(
    TypeDeclarationTestCaseAttribute attrTestCase,
    Type type
  )
  {
    type.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter ??= static (_, _) => false;

    Assert.AreEqual(
      attrTestCase.Expected,
      Generator.GenerateTypeDeclaration(attrTestCase.TestTargetType ?? type, null, options),
      message: $"{attrTestCase.SourceLocation} ({type.FullName})"
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_GenerateTypeDeclaration_ReferencingNamespaces()
    => GetTestCaseTypes()
      .SelectMany(
        t => t.GetCustomAttributes<ReferencingNamespacesTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateTypeDeclaration_ReferencingNamespaces))]
  public void GenerateTypeDeclaration_ReferencingNamespaces(
    ReferencingNamespacesTestCaseAttribute attrTestCase,
    Type type
  )
  {
    type.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var namespaces = new HashSet<string>();

    if (attrTestCase.WithExplicitBaseTypeAndInterfaces) {
      Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(
        type,
        namespaces,
        GetGeneratorOptions(attrTestCase)
      ).ToList();
    }
    else {
      Generator.GenerateTypeDeclaration(
        type,
        namespaces,
        GetGeneratorOptions(attrTestCase)
      );
    }

    Assert.That(
      namespaces,
      Is.EquivalentTo(attrTestCase.GetExpectedSet()),
      message: $"{attrTestCase.SourceLocation} ({type.FullName})"
    );
  }

#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
#nullable enable annotations
  private class TestTypesForGenerateTypeDeclarationConcurrently {
    public delegate void DP1(Tuple<string?>? p);
    public delegate void DP2(Tuple<string?, string?>? p);
    public delegate void DP3(Tuple<string?, string?, string?>? p);
    public delegate void DP4(Tuple<string?, string?, string?, string?>? p);
    public delegate void DP5(Tuple<string?, string?, string?, string?, string?>? p);
    public delegate void DP6(Tuple<string?, string?, string?, string?, string?, string?>? p);
    public delegate void DP7(Tuple<string?, string?, string?, string?, string?, string?, string?>? p);
    public delegate void DP8(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? p);
    public delegate void DP9(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? p);
    public delegate void DP10(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? p);
    public delegate void DP11(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? p);
    public delegate void DP12(Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? p);

    public delegate Tuple<string?>? DRP1();
    public delegate Tuple<string?, string?>? DRP2();
    public delegate Tuple<string?, string?, string?>? DRP3();
    public delegate Tuple<string?, string?, string?, string?>? DRP4();
    public delegate Tuple<string?, string?, string?, string?, string?>? DRP5();
    public delegate Tuple<string?, string?, string?, string?, string?, string?>? DRP6();
    public delegate Tuple<string?, string?, string?, string?, string?, string?, string?>? DRP7();
    public delegate Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?>>? DRP8();
    public delegate Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?>>? DRP9();
    public delegate Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?>>? DRP10();
    public delegate Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?>>? DRP11();
    public delegate Tuple<string?, string?, string?, string?, string?, string?, string?, Tuple<string?, string?, string?, string?, string?>>? DRP12();
  }

  private async Task GenerateTypeDeclaration_Concurrent_Async(
    bool lockCreatingNullabilityInfo,
    Action<GeneratorOptions> testAction
  )
  {
    const int maxNumberOfRepeat = 20;

    Action<NullabilityInfoContext, object?> wrappedTestAction = new(
      (ctx, lockObject) => {
        var options = new GeneratorOptions();

        options.TypeDeclaration.NullabilityInfoContext = ctx;
        options.TypeDeclaration.NullabilityInfoContextLockObject = lockObject;

        testAction(options);
      }
    );

    if (lockCreatingNullabilityInfo) {
      await CSharpFormatterTests.AssertNullabilityInfoContextConcurrentOperationWithLockMustNotThrowExceptionAsync(
        wrappedTestAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
    else {
      await CSharpFormatterTests.AssertNullabilityInfoContextConcurrentOperationWithoutLockMustThrowExceptionAsync(
        wrappedTestAction,
        maxNumberOfRepeat
      ).ConfigureAwait(false);
    }
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateTypeDeclaration_Concurrent_Delegate_Parameter(bool lockCreatingNullabilityInfo)
  {
    var delegates = new[] {
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP1),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP2),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP3),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP4),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP5),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP6),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP7),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP8),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP9),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP10),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP11),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DP12),
    };

    return GenerateTypeDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var d in delegates) {
          Generator.GenerateTypeDeclaration(d, null, options);
        }
      }
    );
  }

  [TestCase(true)]
  [TestCase(false)]
  public Task GenerateTypeDeclaration_Concurrent_Delegate_ReturnParameter(bool lockCreatingNullabilityInfo)
  {
    var delegates = new[] {
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP1),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP2),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP3),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP4),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP5),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP6),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP7),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP8),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP9),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP10),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP11),
      typeof(TestTypesForGenerateTypeDeclarationConcurrently.DRP12),
    };

    return GenerateTypeDeclaration_Concurrent_Async(
      lockCreatingNullabilityInfo,
      options => {
        foreach (var d in delegates) {
          Generator.GenerateTypeDeclaration(d, null, options);
        }
      }
    );
  }
#nullable restore annotations
#endif
}
