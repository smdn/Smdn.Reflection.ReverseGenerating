// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#pragma warning disable CA2211

using System;
using System.Collections.Generic;
using System.Threading;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Fields.StaticValues;

public class ValueTypes {
  [MemberDeclarationTestCase("public int F1;")] public int F1 = 1;
  [MemberDeclarationTestCase("public readonly int F2;")] public readonly int F2 = 2;

  [MemberDeclarationTestCase("public const int F3 = 3;")]
  [MemberDeclarationTestCase("public const int F3 = 3", MemberOmitEndOfStatement = true)]
  public const int F3 = 3;

  [MemberDeclarationTestCase("public static int F4;")] public static int F4 = 4;
  [MemberDeclarationTestCase("public static readonly int F5 = 5;")] public static readonly int F5 = 5;

  [MemberDeclarationTestCase(
    "public static readonly int Int32MaxValue = System.Int32.MaxValue;",
    TranslateLanguagePrimitiveTypeDeclaration = false,
    ValueWithNamespace = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly int Int32MaxValue = Int32.MaxValue;",
    TranslateLanguagePrimitiveTypeDeclaration = false,
    ValueWithNamespace = false
  )]
  [MemberDeclarationTestCase(
    "public static readonly int Int32MaxValue = int.MaxValue;",
    TranslateLanguagePrimitiveTypeDeclaration = true
  )]
  public static readonly int Int32MaxValue = int.MaxValue;

  [MemberDeclarationTestCase(
    "public static readonly int Int32MinValue = int.MinValue;",
    TranslateLanguagePrimitiveTypeDeclaration = true
  )]
  public static readonly int Int32MinValue = int.MinValue;
}

public class Enums {
  [MemberDeclarationTestCase("public const System.DateTimeKind F1 = System.DateTimeKind.Unspecified;")] public const DateTimeKind F1 = DateTimeKind.Unspecified;
  [MemberDeclarationTestCase("public const System.DateTimeKind F2 = System.DateTimeKind.Unspecified;")] public const DateTimeKind F2 = default(DateTimeKind);
  [MemberDeclarationTestCase("public const System.DateTimeKind F3 = System.DateTimeKind.Local;")] public const DateTimeKind F3 = DateTimeKind.Local;
  [MemberDeclarationTestCase("public const System.DateTimeKind F4 = (System.DateTimeKind)42;")] public const DateTimeKind F4 =(DateTimeKind)42;
}

public class EnumFlags {
  [MemberDeclarationTestCase("public const System.AttributeTargets F1 = System.AttributeTargets.All;")] public const AttributeTargets F1 = AttributeTargets.All;
  [MemberDeclarationTestCase("public const System.AttributeTargets F2 = default(System.AttributeTargets);", ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public const System.AttributeTargets F2 = default;", ValueUseDefaultLiteral = true)]
  public const AttributeTargets F2 = default(AttributeTargets);

  [MemberDeclarationTestCase("public const System.AttributeTargets F3 = System.AttributeTargets.Assembly;")] public const AttributeTargets F3 = AttributeTargets.Assembly;
  [MemberDeclarationTestCase("public const System.AttributeTargets F4 = (System.AttributeTargets)3;")] public const AttributeTargets F4 = AttributeTargets.Assembly | AttributeTargets.Module;
  [MemberDeclarationTestCase("public const System.AttributeTargets F5 = (System.AttributeTargets)42;")] public const AttributeTargets F5 =(AttributeTargets)42;
}

public class Booleans {
  [MemberDeclarationTestCase("public const bool F1 = false;")] public const bool F1 = false;
  [MemberDeclarationTestCase("public const bool F2 = true;")] public const bool F2 = true;
}

public class Chars {
  [MemberDeclarationTestCase("public const char F1 = 'A';")] public const char F1 = 'A';
  [MemberDeclarationTestCase("public const char F2 = '\\u0000';")] public const char F2 = '\0';
  [MemberDeclarationTestCase("public const char F3 = '\\u000A';")] public const char F3 = '\n';
  [MemberDeclarationTestCase("public const char F4 = '\\u000A';")] public const char F4 = '\u000A';
  [MemberDeclarationTestCase("public const char F5 = '\\'';")] public const char F5 = '\'';
  [MemberDeclarationTestCase("public const char F6 = '\"';")] public const char F6 = '"';
}

public class Strings {
  [MemberDeclarationTestCase("public const string F1 = \"string\";")] public const string F1 = "string";
  [MemberDeclarationTestCase("public const string F2 = \"\";")] public const string F2 = "";
  [MemberDeclarationTestCase("public const string F3 = null;")] public const string F3 = null;
  [MemberDeclarationTestCase("public const string F4 = \"\\u0000\";")] public const string F4 = "\0";
  [MemberDeclarationTestCase("public const string F5 = \"hello\\u000A\";")] public const string F5 = "hello\n";
  [MemberDeclarationTestCase("public const string F6 = \"\\\"\";")] public const string F6 = "\"";
}

public class Types {
  [MemberDeclarationTestCase(
    "public static readonly System.Type TypeOfLanguagePrimitiveType = typeof(int);",
    TranslateLanguagePrimitiveTypeDeclaration = true,
    MemberWithNamespace = true,
    ValueWithNamespace = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly System.Type TypeOfLanguagePrimitiveType = typeof(System.Int32);",
    TranslateLanguagePrimitiveTypeDeclaration = false,
    MemberWithNamespace = true,
    ValueWithNamespace = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfLanguagePrimitiveType = typeof(System.Int32);",
    TranslateLanguagePrimitiveTypeDeclaration = false,
    MemberWithNamespace = false,
    ValueWithNamespace = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfLanguagePrimitiveType = typeof(Int32);",
    TranslateLanguagePrimitiveTypeDeclaration = false,
    MemberWithNamespace = false,
    ValueWithNamespace = false
  )]
  public static readonly Type TypeOfLanguagePrimitiveType = typeof(int);

  [MemberDeclarationTestCase(
    "public static readonly System.Type TypeOfNonPrimitiveType = typeof(System.Guid);",
    MemberWithNamespace = true,
    ValueWithNamespace = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfNonPrimitiveType = typeof(Guid);",
    MemberWithNamespace = false,
    ValueWithNamespace = false
  )]
  public static readonly Type TypeOfNonPrimitiveType = typeof(Guid);

  [MemberDeclarationTestCase(
    "public static readonly System.Type TypeOfNestedType = typeof(System.Environment.SpecialFolder);",
    MemberWithNamespace = true,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfNestedType = typeof(System.Environment.SpecialFolder);",
    MemberWithNamespace = false,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfNestedType = typeof(Environment.SpecialFolder);",
    MemberWithNamespace = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfNestedType = typeof(SpecialFolder);",
    MemberWithNamespace = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = false
  )]
  public static readonly Type TypeOfNestedType = typeof(Environment.SpecialFolder);

  [MemberDeclarationTestCase(
    "public static readonly Type TypeOfConstructedGenericType = typeof(Converter<int, string>);",
    MemberWithNamespace = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = false
  )]
  public static readonly Type TypeOfConstructedGenericType = typeof(Converter<int, string>);

  [MemberDeclarationTestCase(
    $"public static readonly Type {nameof(TypeOfGenericTypeDefinition)} = typeof(Converter<,>);",
    MemberWithNamespace = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = false
  )]
  [MemberDeclarationTestCase(
    $"public static readonly Type {nameof(TypeOfGenericTypeDefinition)} = typeof(System.Converter<,>);",
    MemberWithNamespace = false,
    ValueWithNamespace = true,
    ValueWithDeclaringTypeName = false
  )]
  public static readonly Type TypeOfGenericTypeDefinition = typeof(Converter<,>);

  [MemberDeclarationTestCase(
    $"public static readonly Type {nameof(TypeOfNestedGenericTypeDefinition)} = typeof(Types.C<>.CI.S<,>);",
    MemberWithNamespace = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = true
  )]
  [MemberDeclarationTestCase(
    $"public static readonly Type {nameof(TypeOfNestedGenericTypeDefinition)} = typeof(S<,>);",
    MemberWithNamespace = false,
    ValueWithNamespace = false,
    ValueWithDeclaringTypeName = false
  )]
  public static readonly Type TypeOfNestedGenericTypeDefinition = typeof(C<>.CI.S<,>);

  public class C<T> {
    public class CI {
      public struct S<U, V> { }
    }
  }

  public struct S<TIn, TOut> {
    [MemberDeclarationTestCase("public static readonly System.Converter<TIn, TOut> ConverterFromTInToTOut;")]
    public static readonly Converter<TIn, TOut> ConverterFromTInToTOut = val => throw new NotImplementedException();
  }
}

public class Nullables {
  [MemberDeclarationTestCase("public static readonly int? F1 = 0;")] public static readonly int? F1 = 0;
  [MemberDeclarationTestCase("public static readonly int? F2 = null;")] public static readonly int? F2 = null;
  [MemberDeclarationTestCase("public static readonly int? F3 = null;")] public static readonly int? F3 = default(int?);
  [MemberDeclarationTestCase("public static readonly int? F4 = int.MaxValue;")] public static readonly int? F4 = int.MaxValue;
  [MemberDeclarationTestCase("public static readonly int? F5 = int.MinValue;")] public static readonly int? F5 = int.MinValue;

  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset? F6 = null;")] public static readonly DateTimeOffset? F6 = null;
  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset? F7 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset? F7 = DateTimeOffset.MinValue;
  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset? F8 = System.DateTimeOffset.MaxValue;")] public static readonly DateTimeOffset? F8 = DateTimeOffset.MaxValue;

  [MemberDeclarationTestCase("public static readonly char? NullableCharHasValue = '\\u0000';")] public static readonly char? NullableCharHasValue = '\0';
  [MemberDeclarationTestCase("public static readonly char? NullableCharNull = null;")] public static readonly char? NullableCharNull = null;

  [MemberDeclarationTestCase("public static readonly bool? NullableBooleanTrue = true;")] public static readonly bool? NullableBooleanTrue = true;
  [MemberDeclarationTestCase("public static readonly bool? NullableBooleanFalse = false;")] public static readonly bool? NullableBooleanFalse = false;
  [MemberDeclarationTestCase("public static readonly bool? NullableBooleanNull = null;")] public static readonly bool? NullableBooleanNull = null;

  [MemberDeclarationTestCase("public static readonly System.DateTimeKind? NullableEnumHasValue = System.DateTimeKind.Unspecified;")] public static readonly DateTimeKind? NullableEnumHasValue = DateTimeKind.Unspecified;
  [MemberDeclarationTestCase("public static readonly System.DateTimeKind? NullableEnumNull = null;")] public static readonly DateTimeKind? NullableEnumNull = null;
}

public class NonPrimitiveValueTypes {
  [MemberDeclarationTestCase("public static readonly System.Guid F1 = System.Guid.Empty;")] public static readonly Guid F1 = Guid.Empty;
  [MemberDeclarationTestCase("public static readonly System.Guid F2 = System.Guid.Empty;")] public static readonly Guid F2 = default(Guid);

  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F3 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset F3 = default(DateTimeOffset);
  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F4 = System.DateTimeOffset.MinValue;")] public static readonly DateTimeOffset F4 = DateTimeOffset.MinValue;
  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F5 = System.DateTimeOffset.MaxValue;")] public static readonly DateTimeOffset F5 = DateTimeOffset.MaxValue;
  [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F6; // = \"2018-10-31T21:00:00.0000000+09:00\"")] public static readonly DateTimeOffset F6 = new DateTimeOffset(2018, 10, 31, 21, 0, 0, 0, TimeSpan.FromHours(+9.0));

  // XXX: System.Threading.CancellationToken.None is a property
  [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F7 = default(System.Threading.CancellationToken);", ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F7 = default;", ValueUseDefaultLiteral = true)]
  public static readonly CancellationToken F7 = CancellationToken.None;

  [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F8 = default(System.Threading.CancellationToken);", ValueUseDefaultLiteral = false)]
  [MemberDeclarationTestCase("public static readonly System.Threading.CancellationToken F8 = default;", ValueUseDefaultLiteral = true)]
  public static readonly CancellationToken F8 = default(CancellationToken);

  public class Stringification {
    // DateTimeOffset
    [MemberDeclarationTestCase("public static readonly System.DateTimeOffset F_DateTimeOffset; // = \"2018-10-31T21:00:00.0000000+09:00\"")] public static readonly DateTimeOffset F_DateTimeOffset = new DateTimeOffset(2018, 10, 31, 21, 0, 0, 0, TimeSpan.FromHours(+9.0));

    // DateTime
    [MemberDeclarationTestCase("public static readonly System.DateTime F_DateTime; // = \"2018-10-31T21:00:00.0000000Z\"")] public static readonly DateTime F_DateTime = new DateTime(2018, 10, 31, 21, 0, 0, 0, DateTimeKind.Utc);

    // IFormattable
    [MemberDeclarationTestCase("public static readonly System.TimeSpan F_TimeSpan; // = \"1.23:45:06.7890000\"")] public static readonly TimeSpan F_TimeSpan = new TimeSpan(1, 23, 45, 6, 789);
  }
}

public class ReferenceTypes {
  [MemberDeclarationTestCase("public static readonly System.Uri F1 = null;")]
  [MemberDeclarationTestCase("public static readonly System.Uri F1 = null", MemberOmitEndOfStatement = true)]
  public static readonly Uri F1 = null;

  [MemberDeclarationTestCase("public static readonly System.Uri F2; // = \"http://example.com/\"")]
  [MemberDeclarationTestCase("public static readonly System.Uri F2; // = \"http://example.com/\"", MemberOmitEndOfStatement = true)]
  public static readonly Uri F2 = new Uri("http://example.com/");

  [MemberDeclarationTestCase("public static readonly System.Collections.Generic.IEnumerable<int> F3 = null;")] public static readonly IEnumerable<int> F3 = null;
  [MemberDeclarationTestCase("public static readonly System.Collections.Generic.IEnumerable<int> F4; // = \"System.Int32[]\"")] public static readonly IEnumerable<int> F4 = new int[] {0, 1, 2, 3};
}

public class DeclaringTypeItself {
  public struct S1 {
    [MemberDeclarationTestCase("public static readonly DeclaringTypeItself.S1 Empty; // = \"foo\"", MemberWithNamespace = false)]
    [MemberDeclarationTestCase("public static readonly DeclaringTypeItself.S1 Empty; // = \"foo\"", MemberWithNamespace = false, MemberOmitEndOfStatement = true)]
    public static readonly S1 Empty = default(S1);

    public override string ToString() => "foo";
  }

  public struct S2 {
    [MemberDeclarationTestCase("public static readonly DeclaringTypeItself.S2 Empty; // = \"\\\"\\u0000\\\"\"", MemberWithNamespace = false, MemberWithDeclaringTypeName = false)]
    public static readonly S2 Empty = default(S2);

    public override string ToString() => "\"\0\"";
  }
}

public class GenericTypes {
  [MemberDeclarationTestCase("public static readonly System.Converter<int, string> ConverterFromIntToString; // = \"System.Converter`2[System.Int32,System.String]\"")]
  public static readonly Converter<int, string> ConverterFromIntToString = val => throw new NotImplementedException();

  public struct S<TIn, TOut> {
    [MemberDeclarationTestCase("public static readonly System.Converter<TIn, TOut> ConverterFromTInToTOut;")]
    public static readonly Converter<TIn, TOut> ConverterFromTInToTOut = val => throw new NotImplementedException();
  }
}