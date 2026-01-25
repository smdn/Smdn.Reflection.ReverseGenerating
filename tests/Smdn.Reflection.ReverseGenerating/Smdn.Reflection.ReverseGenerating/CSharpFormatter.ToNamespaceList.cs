// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

public partial class CSharpFormatterTests {
  private static System.Collections.IEnumerable YieldTestCases_ToNamespaceList_WithTranslateLanguagePrimitiveTypes()
  {
    foreach (var translateLanguagePrimitiveTypes in new[] { true, false }) {
      yield return new object[] { typeof(void), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(object), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(byte), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(sbyte), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(ushort), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(short), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(uint), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(int), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(ulong), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(long), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(float), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(double), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(decimal), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(char), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(string), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(bool), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(int?), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(int[]), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof(int*), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof((int, int)), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof((int, Guid)), translateLanguagePrimitiveTypes, new[] { "System" } };
      yield return new object[] { typeof((int, int, int)), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new string[0] : new[] { "System" } };
      yield return new object[] { typeof((int, Guid, Guid)), translateLanguagePrimitiveTypes, new[] { "System" } };
      yield return new object[] { typeof((int, int, List<int>)), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new[] { "System.Collections.Generic" } : new[] { "System", "System.Collections.Generic" } };
      yield return new object[] { typeof((int, int, ValueTuple<List<Assembly>>)), translateLanguagePrimitiveTypes, new[] { "System", "System.Collections.Generic", "System.Reflection" } };
      yield return new object[] { typeof(Tuple<int, int, List<int>>), translateLanguagePrimitiveTypes, new[] { "System", "System.Collections.Generic" } };
      yield return new object[] { typeof(Tuple<int, int, ValueTuple<List<Assembly>>>), translateLanguagePrimitiveTypes, new[] { "System", "System.Collections.Generic", "System.Reflection" } };
      yield return new object[] { typeof(ValueTuple<>), translateLanguagePrimitiveTypes, new[] { "System" } };
      yield return new object[] { typeof(ValueTuple<int>), translateLanguagePrimitiveTypes, new[] { "System" } };
      yield return new object[] { typeof(ValueTuple<List<int>>), translateLanguagePrimitiveTypes, new[] { "System", "System.Collections.Generic" } };
      yield return new object[] { typeof(Guid), translateLanguagePrimitiveTypes, new[] { "System" } };
      yield return new object[] { typeof(Guid?), translateLanguagePrimitiveTypes, new[] { "System" } };
      yield return new object[] { typeof(List<>), translateLanguagePrimitiveTypes, new[] { "System.Collections.Generic" } };
      yield return new object[] { typeof(List<int>), translateLanguagePrimitiveTypes, translateLanguagePrimitiveTypes ? new[] { "System.Collections.Generic" } : new[] { "System", "System.Collections.Generic" } };
      yield return new object[] { typeof(List<Guid>), translateLanguagePrimitiveTypes, new[] { "System", "System.Collections.Generic" } };
    }
  }

  private static System.Collections.IEnumerable YieldTestCases_ToNamespaceList()
  {
    foreach (var testCaseArgs in YieldTestCases_ToNamespaceList_WithTranslateLanguagePrimitiveTypes().Cast<object[]>()) {
      if ((bool)testCaseArgs[1]) // select test cases where translateLanguagePrimitiveTypes = true
        yield return new object[] { testCaseArgs[0], testCaseArgs[2] };
    }
  }

  [TestCaseSource(nameof(YieldTestCases_ToNamespaceList))]
  public void ToNamespaceList(Type type, string[] expected)
    => Assert.That(
      CSharpFormatter.ToNamespaceList(type).Distinct(),
      Is.EquivalentTo(expected)
    );

  [TestCaseSource(nameof(YieldTestCases_ToNamespaceList_WithTranslateLanguagePrimitiveTypes))]
  public void ToNamespaceList_WithTranslateLanguagePrimitiveTypes(Type type, bool translateLanguagePrimitiveTypes, string[] expected)
    => Assert.That(
      CSharpFormatter.ToNamespaceList(type, translateLanguagePrimitiveTypes).Distinct(),
      Is.EquivalentTo(expected)
    );
}
