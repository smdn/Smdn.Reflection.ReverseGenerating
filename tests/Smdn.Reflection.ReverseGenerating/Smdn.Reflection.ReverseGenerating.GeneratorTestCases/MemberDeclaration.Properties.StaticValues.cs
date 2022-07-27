// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties.StaticValues;

// TODO: property static value
class C {
  [SkipTestCase("static value of properties are not supported currently")]
  [MemberDeclarationTestCase("public int P1 { get; } = 1;")]
  [MemberDeclarationTestCase("public int P1 { get; } = 1", MemberOmitEndOfStatement = true)]
  public int P1 { get; } = 1;

  [MemberDeclarationTestCase("public int P1_Current { get; }")]
  [MemberDeclarationTestCase("public int P1_Current { get; }", MemberOmitEndOfStatement = true)]
  public int P1_Current { get; } = 1;

  [SkipTestCase("static value of properties are not supported currently")]
  [MemberDeclarationTestCase("public int P2 { get; set; } = 2;")] public int P2 { get; set; } = 2;
  [MemberDeclarationTestCase("public int P2_Current { get; set; }")] public int P2_Current { get; set; } = 2;

  [SkipTestCase("static value of properties are not supported currently")]
  [MemberDeclarationTestCase("public int P3 { get; private set; } = 3;")] public int P3 { get; private set; } = 3;
  [MemberDeclarationTestCase("public int P3_Current { get; private set; }")] public int P3_Current { get; private set; } = 3;
}
