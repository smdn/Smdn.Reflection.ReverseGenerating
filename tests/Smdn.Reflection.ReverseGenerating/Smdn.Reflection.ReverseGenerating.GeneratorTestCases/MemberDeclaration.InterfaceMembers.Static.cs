// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if NET7_0_OR_GREATER
#nullable enable annotations
#pragma warning disable CS0067, CS8597

using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.Static;

public interface IBase {
  [MemberDeclarationTestCase("static void M() {}")]
  static void M() => throw null;

  [MemberDeclarationTestCase("static virtual void MVirtual() {}")]
  static virtual void MVirtual() => throw null;

  [MemberDeclarationTestCase("static abstract void MAbstract();")]
  static abstract void MAbstract();

  [MemberDeclarationTestCase("static int P { get; set; }")]
  static int P { get; set; }

  [MemberDeclarationTestCase("static virtual int PVirtual { get; set; }")]
  static virtual int PVirtual { get; set; }

  [MemberDeclarationTestCase("static abstract int PAbstract { get; set; }")]
  static abstract int PAbstract { get; set; }

  [MemberDeclarationTestCase("static event System.EventHandler? E;")]
  static event EventHandler? E;

  [MemberDeclarationTestCase("static abstract event System.EventHandler? EAbstract;")]
  static abstract event EventHandler? EAbstract;
}

public interface IEx : IBase {
  [MemberDeclarationTestCase("new static virtual void MVirtual() {}")]
  new static virtual void MVirtual() => throw null;

  [MemberDeclarationTestCase("new static abstract void MAbstract();")]
  new static abstract void MAbstract();

  [MemberDeclarationTestCase("new static abstract event System.EventHandler? EAbstract;")]
  new static abstract event EventHandler? EAbstract;
}

public class ImplicitlyImplementedMembers : IBase {
  [MemberDeclarationTestCase("public static void MVirtual() {}")]
  public static void MVirtual() => throw null;

  [MemberDeclarationTestCase("public static void MAbstract() {}")]
  public static void MAbstract() => throw null;

  [MemberDeclarationTestCase("public static int PVirtual { get; set; }")]
  public static int PVirtual { get; set; }

  [MemberDeclarationTestCase("public static int PAbstract { get; set; }")]
  public static int PAbstract { get; set; }

  [MemberDeclarationTestCase("public static event System.EventHandler? EAbstract;")]
  public static event EventHandler? EAbstract;
}

public class ExplicitlyImplementedMembers : IBase {
  [MemberDeclarationTestCase("static void Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.InterfaceMembers.StaticMembers.IBase.MVirtual() {}", TypeWithNamespace = true)]
  [MemberDeclarationTestCase("static void IBase.MVirtual() {}", MemberWithNamespace = false)]
  static void IBase.MVirtual() => throw null;

  [MemberDeclarationTestCase("static void IBase.MAbstract() {}", MemberWithNamespace = false)]
  static void IBase.MAbstract() => throw null;

  [MemberDeclarationTestCase("static int IBase.PVirtual { get; set; }", MemberWithNamespace = false)]
  static int IBase.PVirtual { get; set; }

  [MemberDeclarationTestCase("static int IBase.PAbstract { get; set; }", MemberWithNamespace = false)]
  static int IBase.PAbstract { get; set; }

  [MemberDeclarationTestCase("static event EventHandler? IBase.EAbstract { add; remove; }", MemberWithNamespace = false)]
  static event EventHandler? IBase.EAbstract { add => throw null; remove => throw null; }
}
#endif // NET7_0_OR_GREATER
