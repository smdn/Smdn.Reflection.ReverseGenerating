// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating.GeneratorTestCases;

namespace Smdn.Reflection.ReverseGenerating;

partial class GeneratorTests {
  [Test]
  public void GenerateExplicitBaseTypeAndInterfaces_ArgumentOptionsNull()
    => Assert.That(() => Generator.GenerateExplicitBaseTypeAndInterfaces(t: typeof(int), null, options: null!), Throws.ArgumentNullException);

  [Test]
  public void GenerateExplicitBaseTypeAndInterfaces_ArgumentTypeNull()
    => Assert.That(() => Generator.GenerateExplicitBaseTypeAndInterfaces(t: null!, null, options: new()), Throws.ArgumentNullException);

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
