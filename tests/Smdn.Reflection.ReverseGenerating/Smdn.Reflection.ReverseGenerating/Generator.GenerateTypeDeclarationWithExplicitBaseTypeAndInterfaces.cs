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
public class TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCaseAttribute : GeneratorTestCaseAttribute {
  public TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCaseAttribute(
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
  }
}

partial class GeneratorTests {
  [Test]
  public void GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces_ArgumentOptionsNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(t: typeof(int), null, options: null!));

  [Test]
  public void GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces_ArgumentTypeNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(t: null!, null, options: new()));

  [TestCase(typeof(List<int>))]
  [TestCase(typeof(IEnumerable<int>))]
  [TestCase(typeof(Action<int>))]
  [TestCase(typeof(int?))]
  [TestCase(typeof((int, int)))]
  public void GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces_ArgumentTypeIsConstructedGenericType(Type type)
    => Assert.Throws<ArgumentException>(() => Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(t: type, null, options: new()));

  private static System.Collections.IEnumerable YieldTestCases_GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces()
    => GetTestCaseTypes()
      .SelectMany(
        t => t.GetCustomAttributes<TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces))]
  public void GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(
    TypeDeclarationWithExplicitBaseTypeAndInterfacesTestCaseAttribute attrTestCase,
    Type type
  )
  {
    type.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter ??= static (_, _) => false;

    Assert.That(
      string.Join(
        "\n",
        Generator.GenerateTypeDeclarationWithExplicitBaseTypeAndInterfaces(
          type,
          null,
          options
        )
      ),
      Is.EqualTo(attrTestCase.Expected.Replace("\r\n", "\n")),
      message: $"{attrTestCase.SourceLocation} ({type.FullName})"
    );
  }
}
