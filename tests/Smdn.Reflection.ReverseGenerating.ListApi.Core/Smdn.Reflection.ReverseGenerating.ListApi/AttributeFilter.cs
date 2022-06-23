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
    => FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace, StringComparison.Ordinal))
      .SelectMany(
        static type => type.GetCustomAttributes<TypeAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { type, attr.CreateGeneratorOptions(), attr.ExpectedResult, attr.SourceLocation }
        )
      );

  private static System.Collections.IEnumerable YieldTestCases_Members()
    => FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace, StringComparison.Ordinal))
      .SelectMany(static t => t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .Where(static member => member is not Type) // except nested type
      .SelectMany(
        static member => member.GetCustomAttributes<MemberAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { member, attr.CreateGeneratorOptions(), attr.ExpectedResult, attr.SourceLocation }
        )
      );

  private static System.Collections.IEnumerable YieldTestCases_Parameters()
    => FindTypes(static t => t.Namespace is not null && t.Namespace.StartsWith(typeof(ClassToDetermineNamespace).Namespace, StringComparison.Ordinal))
      .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
      .SelectMany(static method => method.GetParameters().Prepend(method.ReturnParameter))
      .SelectMany(
        static para => para.GetCustomAttributes<MemberAttributeFilterTestCaseAttribute>().Select(
          attr => new object[] { para, attr.CreateGeneratorOptions(), attr.ExpectedResult, attr.SourceLocation }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_Types))]
  public void AttributeFilter_Types(
    Type t,
    GeneratorOptions options,
    string expectedResult,
    string testCaseSourceLocation
  )
    => Assert.AreEqual(
      expectedResult,
      string.Join(", ", Generator.GenerateAttributeList(t, null, options)),
      message: $"{testCaseSourceLocation} ({t.FullName})"
    );

  [TestCaseSource(nameof(YieldTestCases_Members))]
  public void AttributeFilter_Members(
    MemberInfo member,
    GeneratorOptions options,
    string expectedResult,
    string testCaseSourceLocation
  )
    => Assert.AreEqual(
      expectedResult,
      string.Join(", ", Generator.GenerateAttributeList(member, null, options)),
      message: $"{testCaseSourceLocation} ({member.DeclaringType!.FullName}.{member.Name})"
    );

  [TestCaseSource(nameof(YieldTestCases_Parameters))]
  public void AttributeFilter_Parameters(
    ParameterInfo para,
    GeneratorOptions options,
    string expectedResult,
    string testCaseSourceLocation
  )
    => Assert.AreEqual(
      expectedResult,
      string.Join(", ", Generator.GenerateAttributeList(para, null, options)),
      message: $"{testCaseSourceLocation} ({para.Member.DeclaringType!.FullName}.{para.Member.Name} {para.Name})"
    );
}
