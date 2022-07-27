// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS0067, CS0169, CS0649

using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.GeneratorOptions;

public class C {
  [MemberDeclarationTestCase("public int F0;", MemberWithAccessibility = true)]
  [MemberDeclarationTestCase("int F0;", MemberWithAccessibility = false)]
  public int F0;

  [MemberDeclarationTestCase("private int F1;", MemberWithAccessibility = true)]
  [MemberDeclarationTestCase("int F1;", MemberWithAccessibility = false)]
  private int F1;

  [MemberDeclarationTestCase("public event System.EventHandler E;", MemberWithAccessibility = true)]
  [MemberDeclarationTestCase("event System.EventHandler E;", MemberWithAccessibility = false)]
  public event EventHandler E;

  [MemberDeclarationTestCase("public int P { get; set; }", MemberWithAccessibility = true)]
  [MemberDeclarationTestCase("int P { get; set; }", MemberWithAccessibility = false)]
  public int P { get; set; }

  [MemberDeclarationTestCase("public void M()", MemberWithAccessibility = true, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("void M()", MemberWithAccessibility = false, MethodBody = MethodBodyOption.None)]
  public void M() { }

  [MemberDeclarationTestCase("public C()", MemberWithAccessibility = true, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("C()", MemberWithAccessibility = false, MethodBody = MethodBodyOption.None)]
  public C() { }

  [MemberDeclarationTestCase("~C()", MemberWithAccessibility = true, MethodBody = MethodBodyOption.None)]
  [MemberDeclarationTestCase("~C()", MemberWithAccessibility = false, MethodBody = MethodBodyOption.None)]
  ~C() { }
}

class C<T> {
  public class CNest { }
  public class CNest<TN> { }

  [MemberDeclarationTestCase("public C<T>.CNest F1;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public C<T>.CNest C<T>.F1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public CNest F1;

  [MemberDeclarationTestCase("public C<T>.CNest<T> F2;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public C<T>.CNest<T> C<T>.F2;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public CNest<T> F2;

  [MemberDeclarationTestCase("public C<T>.CNest<int> F3;", MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public C<T>.CNest<int> C<T>.F3;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  public CNest<int> F3;
}
