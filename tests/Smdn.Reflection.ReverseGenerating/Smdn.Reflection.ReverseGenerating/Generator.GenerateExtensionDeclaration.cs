// SPDX-FileCopyrightText: 2026 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

namespace Smdn.Reflection.ReverseGenerating;

public static class ExtensionGroupingTypeForGenerateExtensionDeclarationTest {
  extension(int x) {
    public bool P => throw new NotImplementedException();
  }
}

partial class GeneratorTests {
  [Test]
  public void GenerateExtensionDeclaration_ArgumentOptionsNull()
  {
    var (extensionMarkerType, extensionParameter) = typeof(ExtensionGroupingTypeForGenerateExtensionDeclarationTest)
      .GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly)
      .First()
      .EnumerateExtensionMarkerTypeAndParameterPairs()
      .First();

    Assert.That(
      () => Generator.GenerateExtensionDeclaration(
        extensionMarkerType: extensionMarkerType,
        extensionParameter: extensionParameter!,
        referencingNamespaces: null,
        options: null!
      ),
      Throws
        .ArgumentNullException
        .ParamName
        .EqualTo("options")
    );
  }

  [Test]
  public void GenerateExtensionDeclaration_ArgumentExtensionMarkerTypeNull()
  {
    var (_, extensionParameter) = typeof(ExtensionGroupingTypeForGenerateExtensionDeclarationTest)
      .GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly)
      .First()
      .EnumerateExtensionMarkerTypeAndParameterPairs()
      .First();

    Assert.That(
      () => Generator.GenerateExtensionDeclaration(
        extensionMarkerType: null!,
        extensionParameter: extensionParameter!,
        referencingNamespaces: null,
        options: new()
      ),
      Throws
        .ArgumentNullException
        .ParamName
        .EqualTo("extensionMarkerType")
    );
  }

  [Test]
  public void GenerateExtensionDeclaration_ArgumentExtensionParameterNull()
  {
    var (extensionMarkerType, _) = typeof(ExtensionGroupingTypeForGenerateExtensionDeclarationTest)
      .GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly)
      .First()
      .EnumerateExtensionMarkerTypeAndParameterPairs()
      .First();

    Assert.That(
      () => Generator.GenerateExtensionDeclaration(
        extensionMarkerType: extensionMarkerType,
        extensionParameter: null!,
        referencingNamespaces: null,
        options: new()
      ),
      Throws
        .ArgumentNullException
        .ParamName
        .EqualTo("extensionParameter")
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_GenerateExtensionDeclaration()
    => GetTestCaseTypes()
      .SelectMany(
        static t => t.GetCustomAttributes<ExtensionGroupingTypeDeclarationTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateExtensionDeclaration))]
  public void GenerateExtensionDeclaration(
    ExtensionGroupingTypeDeclarationTestCaseAttribute attrTestCase,
    Type enclosingType
  )
  {
    enclosingType.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter ??= static (_, _) => false;

    var (extensionMarkerType, extensionParameter) = enclosingType
      .GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly)
      .First()
      .EnumerateExtensionMarkerTypeAndParameterPairs()
      .First();

    Assert.That(
      Generator.GenerateExtensionDeclaration(
        extensionMarkerType: extensionMarkerType,
        extensionParameter: extensionParameter!,
        referencingNamespaces: null,
        options: options
      ),
      Is.EqualTo(attrTestCase.Expected),
      message: $"{attrTestCase.SourceLocation} ({enclosingType.FullName})"
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_GenerateExtensionDeclaration_ReferencingNamespaces()
    => GetTestCaseTypes()
      .SelectMany(
        t => t.GetCustomAttributes<ExtensionGroupingTypeReferencingNamespacesTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateExtensionDeclaration_ReferencingNamespaces))]
  public void GenerateExtensionDeclaration_ReferencingNamespaces(
    ExtensionGroupingTypeReferencingNamespacesTestCaseAttribute attrTestCase,
    Type enclosingType
  )
  {
    enclosingType.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var (extensionMarkerType, extensionParameter) = enclosingType
      .GetNestedTypes(BindingFlags.Public | BindingFlags.DeclaredOnly)
      .First()
      .EnumerateExtensionMarkerTypeAndParameterPairs()
      .First();

    var namespaces = new HashSet<string>();

    _ = Generator.GenerateExtensionDeclaration(
      extensionMarkerType: extensionMarkerType,
      extensionParameter: extensionParameter!,
      referencingNamespaces: namespaces,
      options: GetGeneratorOptions(attrTestCase)
    );

    Assert.That(
      namespaces,
      Is.EquivalentTo(attrTestCase.GetExpectedSet()),
      message: $"{attrTestCase.SourceLocation} ({enclosingType.FullName})"
    );
  }
}
