// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[AttributeUsage(
  AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
  AllowMultiple = true,
  Inherited = false
)]
public class GenericParameterConstraintTestCaseAttribute : GeneratorTestCaseAttribute {
  public GenericParameterConstraintTestCaseAttribute(
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
  public void GenerateGenericParameterConstraintDeclaration_ArgumentOptionsNull()
  {
    var genericParameter = typeof(List<>).GetGenericArguments()[0];

    Assert.Throws<ArgumentNullException>(() => Generator.GenerateGenericParameterConstraintDeclaration(genericParameter: genericParameter, null, options: null!));
  }

  [Test]
  public void GenerateGenericParameterConstraintDeclaration_ArgumentGenericParameterNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateGenericParameterConstraintDeclaration(genericParameter: null!, null, options: new()));

  [TestCase(typeof(int))]
  [TestCase(typeof(List<>))]
  public void GenerateGenericParameterConstraintDeclaration_ArgumentGenericParameterIsType(Type type)
    => Assert.Throws<ArgumentException>(() => Generator.GenerateGenericParameterConstraintDeclaration(genericParameter: type, null, options: new()));

  [TestCase(typeof(List<int>))]
  [TestCase(typeof(Action<int>))]
  [TestCase(typeof(int?))]
  [TestCase(typeof((int, int)))]
  public void GenerateGenericParameterConstraintDeclaration_ArgumentGenericParameterIsGenericArgument(Type type)
    => Assert.Throws<ArgumentException>(() => Generator.GenerateGenericParameterConstraintDeclaration(genericParameter: type.GetGenericArguments()[0], null, options: new()));

  private static System.Collections.IEnumerable YieldTestCase_GenerateGenericParameterConstraintDeclaration_OfType()
    => GetTestCaseTypes()
      .SelectMany(
        static t => t.GetCustomAttributes<GenericParameterConstraintTestCaseAttribute>().Select(
          a => new object[] { a, t }
        )
      );

  private static System.Collections.IEnumerable YieldTestCase_GenerateGenericParameterConstraintDeclaration_OfMethod()
    => GetTestCaseTypes()
      .SelectMany(static t => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
      .SelectMany(
        static m => m.GetCustomAttributes<GenericParameterConstraintTestCaseAttribute>().Select(
          a => new object[] { a, m }
        )
      );

  private static string GenerateConstraintDeclaration(Type[] genericArguments, GeneratorOptions options)
    => string.Join(
      " ",
      genericArguments
        .Select(arg => Generator.GenerateGenericParameterConstraintDeclaration(arg, null, options))
        .Where(static arg => !string.IsNullOrEmpty(arg))
    );

  [TestCaseSource(nameof(YieldTestCase_GenerateGenericParameterConstraintDeclaration_OfType))]
  public void GenerateGenericParameterConstraintDeclaration_OfType(
    GenericParameterConstraintTestCaseAttribute attrTestCase,
    Type type
  )
  {
    type.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    Assert.That(
      GenerateConstraintDeclaration(type.GetGenericArguments(), GetGeneratorOptions(attrTestCase)),
      Is.EqualTo(attrTestCase.Expected),
      message: $"{attrTestCase.SourceLocation} ({type.FullName})"
    );
  }

  [TestCaseSource(nameof(YieldTestCase_GenerateGenericParameterConstraintDeclaration_OfMethod))]
  public void GenerateGenericParameterConstraintDeclaration_OfMethod(
    GenericParameterConstraintTestCaseAttribute attrTestCase,
    MethodInfo method
  )
  {
    method.GetCustomAttribute<SkipTestCaseAttribute>()?.Throw();

    Assert.That(
      GenerateConstraintDeclaration(method.GetGenericArguments(), GetGeneratorOptions(attrTestCase)),
      Is.EqualTo(attrTestCase.Expected),
      message: $"{attrTestCase.SourceLocation} ({method})"
    );
  }
}
