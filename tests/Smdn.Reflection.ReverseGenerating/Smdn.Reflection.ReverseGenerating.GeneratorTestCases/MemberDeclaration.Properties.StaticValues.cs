// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CS8597

using System;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Properties.StaticValues;

class InstanceProperties {
  [MemberDeclarationTestCase($"public int {nameof(PGet)} {{ get; }}", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public int {nameof(PGet)} {{ get; }}", MemberOmitEndOfStatement = true)]
  public int PGet { get; } = 1;

  [MemberDeclarationTestCase($"public int {nameof(PGetSet)} {{ get; set; }}", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public int {nameof(PGetSet)} {{ get; set; }}", MemberOmitEndOfStatement = true)]
  public int PGetSet { get; set; } = 1;

  [MemberDeclarationTestCase($"public int {nameof(PGetInit)} {{ get; init; }}", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public int {nameof(PGetInit)} {{ get; init; }}", MemberOmitEndOfStatement = true)]
  public int PGetInit { get; init; } = 1;

  [MemberDeclarationTestCase($"public int {nameof(PGetPrivateSet)} {{ get; private set; }}", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public int {nameof(PGetPrivateSet)} {{ get; private set; }}", MemberOmitEndOfStatement = true)]
  public int PGetPrivateSet { get; private set; } = 1;
}

public class StaticProperties {
  [MemberDeclarationTestCase($"public static int {nameof(PStaticGet)} {{ get; }} = 1;", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public static int {nameof(PStaticGet)} {{ get; }} = 1", MemberOmitEndOfStatement = true)]
  public static int PStaticGet { get; } = 1;

  [MemberDeclarationTestCase($"public static int {nameof(PStaticGetSet)} {{ get; set; }}", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public static int {nameof(PStaticGetSet)} {{ get; set; }}", MemberOmitEndOfStatement = true)]
  public static int PStaticGetSet { get; set; } = 1;

  [MemberDeclarationTestCase($"public static int {nameof(PStaticGetPrivateSet)} {{ get; private set; }}", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public static int {nameof(PStaticGetPrivateSet)} {{ get; private set; }}", MemberOmitEndOfStatement = true)]
  public static int PStaticGetPrivateSet { get; private set; } = 1;
}

public class Stringification {
  [MemberDeclarationTestCase($"public static int? {nameof(NullableIntNull)} {{ get; }} = null;")]
  public static int? NullableIntNull { get; } = null;

  [MemberDeclarationTestCase($"public static int? {nameof(NullableIntMaxValue)} {{ get; }} = int.MaxValue;")]
  public static int? NullableIntMaxValue { get; } = int.MaxValue;

  [MemberDeclarationTestCase($"public static string {nameof(Text)} {{ get; }} = \"{nameof(Text)}\";")]
  public static string Text { get; } = nameof(Text);

  [MemberDeclarationTestCase($"public static System.DateTimeOffset {nameof(DateTimeOffsetValue)} {{ get; }} // = \"2026-01-25T15:00:00.0000000+09:00\"", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase($"public static System.DateTimeOffset {nameof(DateTimeOffsetValue)} {{ get; }} // = \"2026-01-25T15:00:00.0000000+09:00\"", MemberOmitEndOfStatement = true)]
  public static DateTimeOffset DateTimeOffsetValue { get; } = new DateTimeOffset(2026, 01, 25, 15, 0, 0, 0, TimeSpan.FromHours(+9.0));

  [MemberDeclarationTestCase($"public static System.DateTimeOffset {nameof(DateTimeOffsetMaxValue)} {{ get; }} = System.DateTimeOffset.MaxValue;")]
  public static DateTimeOffset DateTimeOffsetMaxValue { get; } = DateTimeOffset.MaxValue;

  [MemberDeclarationTestCase("public static System.Uri Url { get; } // = \"http://example.com/\"", MemberOmitEndOfStatement = false)]
  [MemberDeclarationTestCase("public static System.Uri Url { get; } // = \"http://example.com/\"", MemberOmitEndOfStatement = true)]
  public static Uri Url { get; } = new Uri("http://example.com/");
}