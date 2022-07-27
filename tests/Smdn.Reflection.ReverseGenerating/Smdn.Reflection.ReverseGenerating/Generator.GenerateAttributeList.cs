// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public class AttributeListTestCaseAttribute : GeneratorTestCaseAttribute {
  public AttributeListTestCaseAttribute(
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
  public void GenerateAttributeList_ArgumentOptionsNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateAttributeList(attributeProvider: typeof(int), null, options: null!));

  [Test]
  public void GenerateAttributeList_ArgumentAttributeProviderNull()
    => Assert.Throws<ArgumentNullException>(() => Generator.GenerateAttributeList(attributeProvider: null!, null, options: new()));

  private static System.Collections.IEnumerable YieldTestCases_GenerateAttributeList()
    => GetTestCaseTypes()
      .SelectMany(t => t
        .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
        .Prepend((MemberInfo)t) // prepend type itself as a test target
        .Concat(t.GetGenericArguments())
        .Concat(
          t
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .SelectMany(static method => method.GetGenericArguments())
        )
      )
      .SelectMany(
        m => m.GetCustomAttributes<AttributeListTestCaseAttribute>().Select(
          a => new object[] { a, m }
        )
      );

  [TestCaseSource(nameof(YieldTestCases_GenerateAttributeList))]
  public void GenerateAttributeList(
    AttributeListTestCaseAttribute attrTestCase,
    MemberInfo typeOrMember
  )
  {
    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter = ExceptTestCaseAttributeFilter;

    var typeOrMemberName = typeOrMember is Type t
      ? t.FullName
      : $"{typeOrMember.DeclaringType?.FullName}.{typeOrMember.Name}";

    Assert.AreEqual(
      attrTestCase.Expected,
      string.Join(", ", Generator.GenerateAttributeList(typeOrMember, null, options)),
      message: $"{attrTestCase.SourceLocation} ({typeOrMemberName})"
    );
  }

  private static System.Collections.IEnumerable YieldTestCases_GenerateAttributeList_OfParameterInfo()
    => GetTestCaseTypes()
      .SelectMany(static t =>
        t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
      )
      .Where(static method => method.DeclaringType is not null && !method.DeclaringType.IsDelegate())
      .SelectMany(static method =>
        method.GetParameters().Prepend(method.ReturnParameter)
      )
      .SelectMany(static p => {
        try {
          return p.GetCustomAttributes<AttributeListTestCaseAttribute>().Select(
            a => new object[] { a, p }
          );
        }
        catch (IndexOutOfRangeException) {
          // Mono bug: https://github.com/dotnet/runtime/issues/72907
          if (RuntimeInformation.FrameworkDescription.StartsWith("Mono ")) {
            if (p.CustomAttributes.Count() == 0)
              return Enumerable.Empty<object[]>();
          }

          throw;
        }
      });

  [TestCaseSource(nameof(YieldTestCases_GenerateAttributeList_OfParameterInfo))]
  public void GenerateAttributeList_OfParameterInfo(
    AttributeListTestCaseAttribute attrTestCase,
    ParameterInfo param
  )
  {
    var options = GetGeneratorOptions(attrTestCase);

    options.AttributeDeclaration.TypeFilter = ExceptTestCaseAttributeFilter;

    Assert.AreEqual(
      attrTestCase.Expected,
      string.Join(", ", Generator.GenerateAttributeList(param, null, options)),
      message: $"{attrTestCase.SourceLocation} ({param.Member.DeclaringType!.FullName}.{param.Member.Name} {(param.Name ?? "return value")})"
    );
  }
}
