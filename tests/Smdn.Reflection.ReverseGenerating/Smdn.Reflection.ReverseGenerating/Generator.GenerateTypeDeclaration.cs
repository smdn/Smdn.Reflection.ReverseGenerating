// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
}
