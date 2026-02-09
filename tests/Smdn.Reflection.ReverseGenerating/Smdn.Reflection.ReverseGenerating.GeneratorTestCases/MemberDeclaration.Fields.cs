// SPDX-FileCopyrightText: 2020 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
// cSpell:ignore accessibilities,nullabilities
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
  [MemberDeclarationTestCase("public fixed int Fixed4Int[4];", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [MemberDeclarationTestCase("public fixed Int32 Fixed4Int[4];", TranslateLanguagePrimitiveTypeDeclaration = false, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public fixed System.Int32 Fixed4Int[4];", TranslateLanguagePrimitiveTypeDeclaration = false, MemberWithNamespace = true)]
  public fixed int Fixed4Int[4];

  [MemberDeclarationTestCase("public fixed byte Fixed2Byte[2];", TranslateLanguagePrimitiveTypeDeclaration = true)]
  [MemberDeclarationTestCase("public fixed Byte Fixed2Byte[2];", TranslateLanguagePrimitiveTypeDeclaration = false, MemberWithNamespace = false)]
  [MemberDeclarationTestCase("public fixed System.Byte Fixed2Byte[2];", TranslateLanguagePrimitiveTypeDeclaration = false, MemberWithNamespace = true)]
  public fixed byte Fixed2Byte[2];

#if false // CS1663
  [MemberDeclarationTestCase("public fixed int* Fixed4IntPointer[2];")]
  public fixed int* Fixed4IntPointer[2];
#endif

#if false
  public readonly fixed int FReadOnlyFixed[1]; // CS0106
  public const fixed int FConstFixed[1]; // CS1031
  public static fixed int FStaticFixed[1]; // CS0106
#endif
}

public class ValueTupleTypes {
  [MemberDeclarationTestCase("public (int X, int Y) F1;")] public (int X, int Y) F1;
  [MemberDeclarationTestCase("public (int, int) F2;")] public (int, int) F2;
  [MemberDeclarationTestCase("public (int, int) F3;")] public ValueTuple<int, int> F3;
  [MemberDeclarationTestCase("public System.ValueTuple<int> F4;")] public ValueTuple<int> F4;
}

public class ConstructedGenericTypes {
  [MemberDeclarationTestCase("public Lazy<int> FGenericType;", MemberWithNamespace = false)]
  public Lazy<int> FGenericType;

  [MemberDeclarationTestCase("public IEnumerable<int> FGenericInterfaceWithVariance;", MemberWithNamespace = false)]
  public IEnumerable<int> FGenericInterfaceWithVariance;

  [MemberDeclarationTestCase("public Converter<int, string> FGenericDelegateWithVariance;", MemberWithNamespace = false)]
  public Converter<int, string> FGenericDelegateWithVariance;
}

public class GenericTypeDefinitions<U, V> {
  [MemberDeclarationTestCase("public Lazy<U> FGenericType;", MemberWithNamespace = false)]
  public Lazy<U> FGenericType;

  [MemberDeclarationTestCase("public IEnumerable<U> FGenericInterfaceWithVariance;", MemberWithNamespace = false)]
  public IEnumerable<U> FGenericInterfaceWithVariance;

  [MemberDeclarationTestCase("public Converter<U, V> FGenericDelegateWithVariance;", MemberWithNamespace = false)]
  public Converter<U, V> FGenericDelegateWithVariance;
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

  [MemberDeclarationTestCase($"public volatile int {nameof(FVolatile)};")] public volatile int FVolatile;
  [MemberDeclarationTestCase($"public static volatile int {nameof(FStaticVolatile)};")] public static volatile int FStaticVolatile;
#if SYSTEM_RUNTIME_COMPILERSERVICES_COMPILERFEATUREREQUIREDATTRIBUTE
  [MemberDeclarationTestCase($"public volatile required int {nameof(FVolatileRequired)};")] public volatile required int FVolatileRequired;
#endif

  protected int FProtected;
}

public class ModifiersNew : Modifiers {
  [MemberDeclarationTestCase("new public int F4;")] public new int F4;
  [MemberDeclarationTestCase("new public int FProtected;")] new public int FProtected;
}
#pragma warning restore CA2211

#if NET7_0_OR_GREATER // && SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT
#nullable enable annotations
public unsafe ref struct RefFields {
  [MemberDeclarationTestCase($"public ref int {nameof(ValueType)};")] public ref int ValueType;
  [MemberDeclarationTestCase($"public ref int? {nameof(NullableValueType)};")] public ref int? NullableValueType;

  [MemberDeclarationTestCase($"public ref int[] {nameof(ArrayOfValueType)};")] public ref int[] ArrayOfValueType;
  [MemberDeclarationTestCase($"public ref int?[] {nameof(ArrayOfNullableValueType)};")] public ref int?[] ArrayOfNullableValueType;
  [MemberDeclarationTestCase($"public ref int[]? {nameof(NullableArrayOfValueType)};")] public ref int[]? NullableArrayOfValueType;
  [MemberDeclarationTestCase($"public ref int?[]? {nameof(NullableArrayOfNullableValueType)};")] public ref int?[]? NullableArrayOfNullableValueType;

  [MemberDeclarationTestCase($"public ref string {nameof(ReferenceType)};")] public ref string ReferenceType;
  [MemberDeclarationTestCase($"public ref string? {nameof(NullableReferenceType)};")] public ref string? NullableReferenceType;

  [MemberDeclarationTestCase($"public ref string[] {nameof(ArrayOfReferenceType)};")] public ref string[] ArrayOfReferenceType;
  [MemberDeclarationTestCase($"public ref string?[] {nameof(ArrayOfNullableReferenceType)};")] public ref string?[] ArrayOfNullableReferenceType;
  [MemberDeclarationTestCase($"public ref string[]? {nameof(NullableArrayOfReferenceType)};")] public ref string[]? NullableArrayOfReferenceType;
  [MemberDeclarationTestCase($"public ref string?[]? {nameof(NullableArrayOfNullableReferenceType)};")] public ref string?[]? NullableArrayOfNullableReferenceType;

  [MemberDeclarationTestCase($"public ref int* {nameof(PointerOfValueType)};")] public ref int* PointerOfValueType;
  [MemberDeclarationTestCase($"public ref int?* {nameof(PointerOfNullableValueType)};")] public ref int?* PointerOfNullableValueType;

#pragma warning disable CS8500
  [MemberDeclarationTestCase($"public ref (int, int?, string, string?) {nameof(ValueTupleType)};")] public ref (int, int?, string, string?) ValueTupleType;
  // NullabilityState of 'string' become Unknown
  [MemberDeclarationTestCase($"public ref (int, int?, string, string)* {nameof(PointerOfValueTupleType)};")] public ref (int, int?, string, string?)* PointerOfValueTupleType;
#pragma warning restore CS8500

  [MemberDeclarationTestCase($"public ref KeyValuePair<int, string> {nameof(GenericValueTypeOfValueTypeAndReferenceType)};", MemberWithNamespace = false)] public ref KeyValuePair<int, string> GenericValueTypeOfValueTypeAndReferenceType;
  [MemberDeclarationTestCase($"public ref KeyValuePair<int?, string?> {nameof(GenericValueTypeOfNullableValueTypeAndNullableReferenceType)};", MemberWithNamespace = false)] public ref KeyValuePair<int?, string?> GenericValueTypeOfNullableValueTypeAndNullableReferenceType;

#pragma warning disable CS8500
  [MemberDeclarationTestCase($"public ref KeyValuePair<int, string>* {nameof(PointerOfGenericValueTypeOfValueTypeAndReferenceType)};", MemberWithNamespace = false)] public ref KeyValuePair<int, string>* PointerOfGenericValueTypeOfValueTypeAndReferenceType;
  // NullabilityState of 'string' become Unknown
  [MemberDeclarationTestCase($"public ref KeyValuePair<int?, string>* {nameof(PointerOfGenericValueTypeOfNullableValueTypeAndNullableReferenceType)};", MemberWithNamespace = false)] public ref KeyValuePair<int?, string?>* PointerOfGenericValueTypeOfNullableValueTypeAndNullableReferenceType;
#pragma warning restore CS8500
}

public ref struct RefFieldsReadOnlyModifier {
  [MemberDeclarationTestCase($"public ref int {nameof(FRef)};")] public ref int FRef;
  [MemberDeclarationTestCase($"public ref readonly int {nameof(FRefReadOnly)};")] public ref readonly int FRefReadOnly;
}

public readonly ref struct RefReadOnlyFieldsReadOnlyModifier {
  [MemberDeclarationTestCase($"public readonly ref int {nameof(FReadOnlyRef)};")] public readonly ref int FReadOnlyRef;
  [MemberDeclarationTestCase($"public readonly ref readonly int {nameof(FReadOnlyRefReadOnly)};")]public readonly ref readonly int FReadOnlyRefReadOnly;
}
#nullable restore
#endif

#if SYSTEM_RUNTIME_COMPILERSERVICES_COMPILERFEATUREREQUIREDATTRIBUTE
public class ClassRequiredFields {
  [MemberDeclarationTestCase($"public required int {nameof(Required)};")] public required int Required;
}

public struct StructRequiredFields {
  [MemberDeclarationTestCase($"public required int {nameof(Required)};")] public required int Required;
}

public record class RecordClassRequiredFields(int X) {
  [MemberDeclarationTestCase($"public required int {nameof(Required)};")] public required int Required;
}

public record struct RecordStructRequiredFields(int X) {
  [MemberDeclarationTestCase($"public required int {nameof(Required)};")] public required int Required;
}
#endif // SYSTEM_RUNTIME_COMPILERSERVICES_COMPILERFEATUREREQUIREDATTRIBUTE
