// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Fields.Enums;

public enum Ints : int {
  [MemberDeclarationTestCase("A = 0,")]
  [MemberDeclarationTestCase("A = 0", MemberOmitEndOfStatement = true)]
  [MemberDeclarationTestCase("A = 0,", MemberWithDeclaringTypeName = true, MemberWithEnumTypeName = false, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("Ints.A = 0,", MemberWithDeclaringTypeName = true, MemberWithEnumTypeName = true, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Fields.Enums.Ints.A = 0,", MemberWithEnumTypeName = true, MemberWithNamespace = true)]
  A = 0,

  [MemberDeclarationTestCase("B = 1,")] B = 1,
  [MemberDeclarationTestCase("C = 2,")] C = 2,
}

public enum Bytes : byte {
  [MemberDeclarationTestCase("A = 0,")] A = 0,
  [MemberDeclarationTestCase("B = 1,")] B = 1,
  [MemberDeclarationTestCase("C = 2,")] C = 2,
}

public class Flags {
  [Flags()]
  public enum Ints : int {
    [MemberDeclarationTestCase("A = 0x00000000,")] A = 0x00000000,
    [MemberDeclarationTestCase("B = 0x00000001,")] B = 0x00000001,
    [MemberDeclarationTestCase("C = 0x00000010,")] C = 0x00000010,
    [MemberDeclarationTestCase("Z = 0x7fffffff,")] Z = 0x7fffffff,
  }

  [Flags()]
  public enum Shorts : short {
    [MemberDeclarationTestCase("A = 0x0000,")] A = 0x0000,
    [MemberDeclarationTestCase("B = 0x0001,")] B = 0x0001,
    [MemberDeclarationTestCase("C = 0x0010,")] C = 0x0010,
    [MemberDeclarationTestCase("Z = 0x7fff,")] Z = 0x7fff,
  }

  [Flags()]
  public enum Bytes : byte {
    [MemberDeclarationTestCase("A = 0x00,")] A = 0x00,
    [MemberDeclarationTestCase("B = 0x01,")] B = 0x01,
    [MemberDeclarationTestCase("C = 0x10,")] C = 0x10,
    [MemberDeclarationTestCase("Z = 0xff,")] Z = 0xff,
  }
}
