// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Parameters;

public class Standard {
  [MemberDeclarationTestCase("public void M() {}")] public void M() { }
  [MemberDeclarationTestCase("public void M(int x) {}")] public void M(int x) { }
  [MemberDeclarationTestCase("public void M(int x, int y) {}")] public void M(int x, int y) { }
}

public class InOutRef {
  [MemberDeclarationTestCase("public void M1(in int x) {}")] public void M1(in int x) { }
  [MemberDeclarationTestCase("public void M2(out int x) {}")] public void M2(out int x) => throw new NotImplementedException();
  [MemberDeclarationTestCase("public void M3(ref int x) {}")] public void M3(ref int x) => throw new NotImplementedException();
}

public class Params {
  [MemberDeclarationTestCase("public void M(params int[] p) {}")] public void M(params int[] p) { }
  [MemberDeclarationTestCase("public void M(int p0, params int[] p) {}")] public void M(int p0, params int[] p) { }
}

public class CParam { }
public struct SParam { }

public static class ExtensionMethods {
  [MemberDeclarationTestCase("public static void M(this int i) {}")]
  public static void M(this int i) { }

  [MemberDeclarationTestCase("public static void M(this IEnumerable<int> enumerable) {}", ParameterWithNamespace = false)]
  public static void M(this IEnumerable<int> enumerable) { }

  [MemberDeclarationTestCase("public static void M(this CParam c) {}", ParameterWithNamespace = false)]
  public static void M(this CParam c) { }

  [MemberDeclarationTestCase("public static void M(this CParam c, int i) {}", ParameterWithNamespace = false)]
  public static void M(this CParam c, int i) { }

  [MemberDeclarationTestCase("public static void M(this SParam s) {}", ParameterWithNamespace = false)]
  public static void M(this SParam s) { }
}

public class ValueTuples {
  [MemberDeclarationTestCase("public void M1((int, int) arg) {}")] public void M1((int, int) arg) { }
  [MemberDeclarationTestCase("public void M2((int x, int y) arg) {}")] public void M2((int x, int y) arg) { }
  [MemberDeclarationTestCase("public void M3((int, int, int) arg) {}")] public void M3((int, int, int) arg) { }
  [MemberDeclarationTestCase("public void M4((string x, string y) arg) {}")] public void M4((string x, string y) arg) { }
  [MemberDeclarationTestCase("public void M5(System.ValueTuple<string> arg) {}")] public void M5(ValueTuple<string> arg) { }
  [MemberDeclarationTestCase("public void M6((string, string) arg) {}")] public void M6(ValueTuple<string, string> arg) { }
}
