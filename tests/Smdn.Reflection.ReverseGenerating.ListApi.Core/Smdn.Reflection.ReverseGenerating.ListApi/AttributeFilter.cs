// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

using Smdn.Reflection.ReverseGenerating;
using Smdn.Reflection.ReverseGenerating.ListApi.AttributeFilterTestCases;

namespace Smdn.Reflection.ReverseGenerating.ListApi;

[TestFixture]
partial class AttributeFilterTests {
  private static IEnumerable<Type> FindTypes(Func<Type, bool> predicate)
    => Assembly.GetExecutingAssembly().GetTypes().Where(predicate);

  private static System.Collections.IEnumerable YieldTestCases_Types()
    => FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace!, StringComparison.Ordinal))
      .SelectMany(
        static type => type.GetCustomAttributes<TypeAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { type, attr }
        )
      );

  private static System.Collections.IEnumerable YieldTestCases_Members()
    => FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace!, StringComparison.Ordinal))
      .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .Where(static member => member is not Type) // except nested type
      .SelectMany(
        static member => member.GetCustomAttributes<MemberAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { member, attr }
        )
      );

  private static System.Collections.IEnumerable YieldTestCases_Parameters()
    => FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace!, StringComparison.Ordinal))
      .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .SelectMany(static method => method.GetParameters().Prepend(method.ReturnParameter))
      .SelectMany(
        static para => para.GetCustomAttributes<MemberAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { para, attr }
        )
      );

  private static System.Collections.IEnumerable YieldTestCases_GenericParameters()
  {
    var types = FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace!, StringComparison.Ordinal));

    var genericTypeParameters = types
      //.Where(static t => t.IsGenericType)
      .SelectMany(static t => t.GetGenericArguments());

    var genericMethodParameters = types
      .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      //.Where(static m => m.IsGenericMethod)
      .SelectMany(static method => method.GetGenericArguments());

    return genericTypeParameters
      .Concat(genericMethodParameters)
      .SelectMany(
        static genericParam => genericParam.GetCustomAttributes<TypeAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { genericParam, attr }
        )
      );
  }

  private static void TestAttributeFilter(
    Func<string> actual,
    GeneratorTestCaseAttribute testCase,
    string testTarget
  )
  {
    var message = $"{testCase.SourceLocation} ({testTarget})";

    if (testCase.ExpectedResultAsRegex) {
      Assert.That(
        actual(),
        Does.Match(testCase.ExpectedResult),
        message
      );
    }
    else {
      Assert.AreEqual(
        testCase.ExpectedResult,
        actual(),
        message
      );
    }
  }

  [TestCaseSource(nameof(YieldTestCases_Types))]
  public void AttributeFilter_Types(
    Type t,
    GeneratorTestCaseAttribute testCase
  )
    => TestAttributeFilter(
      actual: () => string.Join(", ", Generator.GenerateAttributeList(t, null, testCase.CreateGeneratorOptions())),
      testCase: testCase,
      testTarget: t.FullName
    );

  [TestCaseSource(nameof(YieldTestCases_Members))]
  public void AttributeFilter_Members(
    MemberInfo member,
    GeneratorTestCaseAttribute testCase
  )
    => TestAttributeFilter(
      actual: () => string.Join(", ", Generator.GenerateAttributeList(member, null, testCase.CreateGeneratorOptions())),
      testCase: testCase,
      testTarget: $"{member.DeclaringType!.FullName}.{member.Name}"
    );

  [TestCaseSource(nameof(YieldTestCases_Parameters))]
  public void AttributeFilter_Parameters(
    ParameterInfo para,
    GeneratorTestCaseAttribute testCase
  )
    => TestAttributeFilter(
      actual: () => string.Join(", ", Generator.GenerateAttributeList(para, null, testCase.CreateGeneratorOptions())),
      testCase: testCase,
      testTarget: $"{para.Member.DeclaringType!.FullName}.{para.Member.Name} {para.Name}"
    );

  [TestCaseSource(nameof(YieldTestCases_GenericParameters))]
  public void AttributeFilter_GenericParameters(
    Type genericParameter,
    GeneratorTestCaseAttribute testCase
  )
    => TestAttributeFilter(
      actual: () => string.Join(", ", Generator.GenerateAttributeList(genericParameter, null, testCase.CreateGeneratorOptions())),
      testCase: testCase,
      testTarget: $"{genericParameter.FullName}"
    );
}
