// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Fields;

public class Types {
  [MemberDeclarationTestCase("public int F1;")]
  [MemberDeclarationTestCase("public int F1", MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("public int Types.F1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public int Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Fields.Types.F1;", MemberWithDeclaringTypeName = true, MemberWithNamespace = true)]
  public int F1;

  [MemberDeclarationTestCase("public float F2;")] public float F2;
  [MemberDeclarationTestCase("public string F3;")] public string F3;
  [MemberDeclarationTestCase("public System.Guid F4;")] public Guid F4;
}

public unsafe struct PointerTypes {
  [MemberDeclarationTestCase("public int* F1;")] public int* F1;
  [MemberDeclarationTestCase("public float* F2;")] public float* F2;
}

public unsafe struct FixedSizeBuffers {
  [SkipTestCase("`fixed` field is not supported currently")]
  [MemberDeclarationTestCase("public fixed int F0[4];")] public fixed int F0[4];

  [MemberDeclarationTestCase("public FixedSizeBuffers.<F1>e__FixedBuffer F1;", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
  public fixed int F1[4];
}

public class ValueTupleTypes {
  [MemberDeclarationTestCase("public (int X, int Y) F1;")] public (int X, int Y) F1;
  [MemberDeclarationTestCase("public (int, int) F2;")] public (int, int) F2;
  [MemberDeclarationTestCase("public (int, int) F3;")] public ValueTuple<int, int> F3;
  [MemberDeclarationTestCase("public System.ValueTuple<int> F4;")] public ValueTuple<int> F4;
}

#pragma warning disable CS0649, CS0169
public class Accessibilities {
  [MemberDeclarationTestCase("public int F1;")] public int F1;
  [MemberDeclarationTestCase("internal int F2;")] internal int F2;
  [MemberDeclarationTestCase("protected int F3;")] protected int F3;
  [MemberDeclarationTestCase("internal protected int F4;")] protected internal int F4;
  [MemberDeclarationTestCase("internal protected int F5;")] internal protected int F5;
  [MemberDeclarationTestCase("private protected int F6;")] private protected int F6;
  [MemberDeclarationTestCase("private protected int F7;")] protected private int F7;
  [MemberDeclarationTestCase("private int F8;")] private int F8;

  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] internal int F9;
  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private protected int F10;
  [MemberDeclarationTestCase(null, IgnorePrivateOrAssembly = true)] private int F11;
}
#pragma warning restore CS0067, CS0169

#pragma warning disable CA2211
public class Modifiers {
  [MemberDeclarationTestCase("public int F4;")] public int F4;
  [MemberDeclarationTestCase("public readonly int F5;")] public readonly int F5;
  [MemberDeclarationTestCase("public const int F6 = 123;")] public const int F6 = 123;
  [MemberDeclarationTestCase("public static int F7;")] public static int F7;
  [MemberDeclarationTestCase("public static readonly int F8 = 0;")]  public static readonly int F8;
  [MemberDeclarationTestCase("public static readonly int F9 = 0;")]  static readonly public int F9;
  [MemberDeclarationTestCase("public static readonly int F10 = 0;")] readonly public static int F10;

  protected int FProtected;
}

public class ModifiersNew : Modifiers {
  [MemberDeclarationTestCase("new public int F4;")] public new int F4;
  [MemberDeclarationTestCase("new public int FProtected;")] new public int FProtected;
}
#pragma warning restore CA2211
