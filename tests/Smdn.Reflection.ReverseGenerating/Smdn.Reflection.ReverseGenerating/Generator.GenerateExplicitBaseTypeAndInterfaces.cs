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
public class ExplicitBaseTypeAndInterfacesTestCaseAttribute : GeneratorTestCaseAttribute {
  public ExplicitBaseTypeAndInterfacesTestCaseAttribute(
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
  public void GenerateExplicitBaseTypeAndInterfaces_ArgumentOptionsNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateExplicitBaseTypeAndInterfaces(t: typeof(int), null, options: null!));

  [Test]
  public void GenerateExplicitBaseTypeAndInterfaces_ArgumentTypeNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateExplicitBaseTypeAndInterfaces(t: null!, null, options: new()));

  private static System.Collections.IEnumerable YieldTestCases_GenerateExplicitBaseTypeAndInterfaces()
    => GetTestCaseTypes()
      .SelectMany(
        t => t.GetCustomAttributes<ExplicitBaseTypeAndInterfacesTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateExplicitBaseTypeAndInterfaces))]
  public void GenerateExplicitBaseTypeAndInterfaces(
    ExplicitBaseTypeAndInterfacesTestCaseAttribute attrTestCase,
    Type type
  )
    => Assert.That(
      string.Join(", ", Generator.GenerateExplicitBaseTypeAndInterfaces(type, null, GetGeneratorOptions(attrTestCase))),
      Is.EqualTo(attrTestCase.Expected),
      message: $"{attrTestCase.SourceLocation} ({type.FullName})"
    );
}
