// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class MemberDeclarationTestCaseAttribute : GeneratorTestCaseAttribute {
  public MemberDeclarationTestCaseAttribute(
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
  public void GenerateMemberDeclaration_ArgumentOptionsNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateMemberDeclaration(member: typeof(int).GetMembers().First(), null, options: null!));

  [Test]
  public void GenerateMemberDeclaration_ArgumentTypeNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateMemberDeclaration(member: null!, null, options: new()));

  private static System.Collections.IEnumerable YieldTestCases_GenerateMemberDeclaration()
    => GetTestCaseTypes()
      .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .Where(static m => m is not Type) // except nested type
      .SelectMany(
        static m => m.GetCustomAttributes<MemberDeclarationTestCaseAttribute>().Select(
          a => new object[] { a, m }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateMemberDeclaration))]
  public void GenerateMemberDeclaration(
    MemberDeclarationTestCaseAttribute attrTestCase,
    MemberInfo member
  )
  {
    member.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter ??= static (_, _) => false;

    Assert.AreEqual(
      attrTestCase.Expected,
      Generator.GenerateMemberDeclaration(member, null, options),
      message: $"{attrTestCase.SourceLocation} ({member.DeclaringType!.FullName}.{member.Name})"
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_GenerateMemberDeclaration_ReferencingNamespaces()
    => GetTestCaseTypes()
      .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .SelectMany(
        m => m.GetCustomAttributes<ReferencingNamespacesTestCaseAttribute>().Select(
          a => new object[] { a, m }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateMemberDeclaration_ReferencingNamespaces))]
  public void GenerateMemberDeclaration_ReferencingNamespaces(
    ReferencingNamespacesTestCaseAttribute attrTestCase,
    MemberInfo member
  )
  {
    member.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    var namespaces = new HashSet<string>();

    Generator.GenerateMemberDeclaration(member, namespaces, GetGeneratorOptions(attrTestCase));

    Assert.That(
      namespaces,
      Is.EquivalentTo(attrTestCase.GetExpectedSet()),
      message: $"{attrTestCase.SourceLocation} ({member.DeclaringType?.FullName}.{member.Name})"
    );
  }
}
