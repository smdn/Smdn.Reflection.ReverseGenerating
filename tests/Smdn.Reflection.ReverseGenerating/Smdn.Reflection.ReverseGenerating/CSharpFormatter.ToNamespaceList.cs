// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating;

public partial class CSharpFormatterTests {
  [TestCase(typeof(void), new string[0])]
  [TestCase(typeof(object), new string[0])]
  [TestCase(typeof(byte), new string[0])]
  [TestCase(typeof(sbyte), new string[0])]
  [TestCase(typeof(ushort), new string[0])]
  [TestCase(typeof(short), new string[0])]
  [TestCase(typeof(uint), new string[0])]
  [TestCase(typeof(int), new string[0])]
  [TestCase(typeof(ulong), new string[0])]
  [TestCase(typeof(long), new string[0])]
  [TestCase(typeof(float), new string[0])]
  [TestCase(typeof(double), new string[0])]
  [TestCase(typeof(decimal), new string[0])]
  [TestCase(typeof(char), new string[0])]
  [TestCase(typeof(string), new string[0])]
  [TestCase(typeof(bool), new string[0])]
  [TestCase(typeof(int?), new string[0])]
  [TestCase(typeof(int[]), new string[0])]
  [TestCase(typeof(int*), new string[0])]
  [TestCase(typeof((int, int)), new string[0])]
  [TestCase(typeof((int, Guid)), new[] { "System" })]
  [TestCase(typeof((int, int, int)), new string[0])]
  [TestCase(typeof((int, Guid, Guid)), new[] { "System" })]
  [TestCase(typeof((int, int, List<int>)), new[] { "System.Collections.Generic" })]
  [TestCase(typeof((int, int, ValueTuple<List<Assembly>>)), new[] { "System", "System.Collections.Generic", "System.Reflection" })]
  [TestCase(typeof(Tuple<int, int, List<int>>), new[] { "System", "System.Collections.Generic" })]
  [TestCase(typeof(Tuple<int, int, ValueTuple<List<Assembly>>>), new[] { "System", "System.Collections.Generic", "System.Reflection" })]
  [TestCase(typeof(ValueTuple<>), new[] { "System" })]
  [TestCase(typeof(ValueTuple<int>), new[] { "System" })]
  [TestCase(typeof(ValueTuple<List<int>>), new[] { "System", "System.Collections.Generic" })]
  [TestCase(typeof(Guid), new[] { "System" })]
  [TestCase(typeof(Guid?), new[] { "System" })]
  [TestCase(typeof(List<>), new[] { "System.Collections.Generic" })]
  [TestCase(typeof(List<int>), new[] { "System.Collections.Generic" })]
  [TestCase(typeof(List<Guid>), new[] { "System", "System.Collections.Generic" })]
  public void ToNamespaceList(Type type, string[] expected)
    => Assert.That(
      CSharpFormatter.ToNamespaceList(type).Distinct(),
      Is.EquivalentTo(expected)
    );
}
