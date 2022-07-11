// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations
#pragma warning disable CS8618, CS8597

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.Parameters;

public class NullabilityAnnotations {
  [MemberDeclarationTestCase($"public void {nameof(ValueType)}(int p) {{}}")] public void ValueType(int p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueType)}(int? p) {{}}")] public void NullableValueType(int? p) { }

  [MemberDeclarationTestCase($"public void {nameof(RefType)}(string p) {{}}")] public void RefType(string p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableRefType)}(string? p) {{}}")] public void NullableRefType(string? p) { }

  [MemberDeclarationTestCase($"public void {nameof(NonLanguagePrimitiveValueType)}(System.Guid p) {{}}")]
  public void NonLanguagePrimitiveValueType(Guid p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableNonLanguagePrimitiveValueType)}(System.Guid? p) {{}}")]
  public void NullableNonLanguagePrimitiveValueType(Guid? p) { }

  [MemberDeclarationTestCase($"public void {nameof(NonLanguagePrimitiveRefType)}(System.Uri p) {{}}")]
  public void NonLanguagePrimitiveRefType(Uri p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableNonLanguagePrimitiveRefType)}(System.Uri? p) {{}}")]
  public void NullableNonLanguagePrimitiveRefType(Uri? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ArrayOfValueType)}(int[] p) {{}}")] public void ArrayOfValueType(int[] p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfValueType)}(int[]? p) {{}}")] public void NullableArrayOfValueType(int[]? p) { }
  [MemberDeclarationTestCase($"public void {nameof(ArrayOfNullableValueType)}(int?[] p) {{}}")] public void ArrayOfNullableValueType(int?[] p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfNullableValueType)}(int?[]? p) {{}}")] public void NullableArrayOfNullableValueType(int?[]? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ArrayOfRefType)}(string[] p) {{}}")] public void ArrayOfRefType(string[] p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfRefType)}(string[]? p) {{}}")] public void NullableArrayOfRefType(string[]? p) { }
  [MemberDeclarationTestCase($"public void {nameof(ArrayOfNullableRefType)}(string?[] p) {{}}")] public void ArrayOfNullableRefType(string?[] p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfNullableRefType)}(string?[]? p) {{}}")] public void NullableArrayOfNullableRefType(string?[]? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueType)}((int, int) p) {{}}")] public void ValueTupleOfValueType((int, int) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableValueType)}((int, int?) p) {{}}")] public void ValueTupleOfNullableValueType((int, int?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfValueType)}((int, int)? p) {{}}")] public void NullableValueTupleOfValueType((int, int)? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfNullableValueType)}((int, int?)? p) {{}}")] public void NullableValueTupleOfNullableValueType((int, int?)? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfRefType)}((int, string) p) {{}}")] public void ValueTupleOfRefType((int, string) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableRefType)}((int, string?) p) {{}}")] public void ValueTupleOfNullableRefType((int, string?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfRefType)}((int, string)? p) {{}}")] public void NullableValueTupleOfRefType((int, string)? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfNullableRefType)}((int, string?)? p) {{}}")] public void NullableValueTupleOfNullableRefType((int, string?)? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ListOfValueType)}(List<int> p) {{}}", ParameterWithNamespace = false)] public void ListOfValueType(List<int> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfNullableValueType)}(List<int?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableValueType(List<int?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfValueType)}(List<int>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfValueType(List<int>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfNullableValueType)}(List<int?>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfNullableValueType(List<int?>? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ListOfRefType)}(List<string> p) {{}}", ParameterWithNamespace = false)] public void ListOfRefType(List<string> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfNullableRefType)}(List<string?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableRefType(List<string?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfRefType)}(List<string>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfRefType(List<string>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfNullableRefType)}(List<string?>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfNullableRefType(List<string?>? p) { }

  [MemberDeclarationTestCase($"public void {nameof(DictionaryOfRefTypeValue)}(Dictionary<string, string> p) {{}}", ParameterWithNamespace = false)] public void DictionaryOfRefTypeValue(Dictionary<string, string> p) { }
  [MemberDeclarationTestCase($"public void {nameof(DictionaryOfNullableRefTypeValue)}(Dictionary<string, string?> p) {{}}", ParameterWithNamespace = false)] public void DictionaryOfNullableRefTypeValue(Dictionary<string, string?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableDictionaryOfRefTypeValue)}(Dictionary<string, string>? p) {{}}", ParameterWithNamespace = false)] public void NullableDictionaryOfRefTypeValue(Dictionary<string, string>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableDictionaryOfNullableRefTypeValue)}(Dictionary<string, string?>? p) {{}}", ParameterWithNamespace = false)] public void NullableDictionaryOfNullableRefTypeValue(Dictionary<string, string?>? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ListOfKeyValuePairOfValueTypeValue)}(List<KeyValuePair<string, int>> p) {{}}", ParameterWithNamespace = false)] public void ListOfKeyValuePairOfValueTypeValue(List<KeyValuePair<string, int>> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfKeyValuePairOfNullableValueTypeValue)}(List<KeyValuePair<string, int?>> p) {{}}", ParameterWithNamespace = false)] public void ListOfKeyValuePairOfNullableValueTypeValue(List<KeyValuePair<string, int?>> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfNullableKeyValuePairOfValueTypeValue)}(List<KeyValuePair<string, int>?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableKeyValuePairOfValueTypeValue(List<KeyValuePair<string, int>?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfNullableKeyValuePairOfNullableValueTypeValue)}(List<KeyValuePair<string, int?>?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableKeyValuePairOfNullableValueTypeValue(List<KeyValuePair<string, int?>?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfKeyValuePairOfValueTypeValue)}(List<KeyValuePair<string, int>>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfKeyValuePairOfValueTypeValue(List<KeyValuePair<string, int>>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfKeyValuePairOfNullableValueTypeValue)}(List<KeyValuePair<string, int?>>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfKeyValuePairOfNullableValueTypeValue(List<KeyValuePair<string, int?>>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfNullableKeyValuePairOfNullableValueTypeValue)}(List<KeyValuePair<string, int?>?>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfNullableKeyValuePairOfNullableValueTypeValue(List<KeyValuePair<string, int?>?>? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ListOfKeyValuePairOfRefTypeValue)}(List<KeyValuePair<string, string>> p) {{}}", ParameterWithNamespace = false)] public void ListOfKeyValuePairOfRefTypeValue(List<KeyValuePair<string, string>> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>> p) {{}}", ParameterWithNamespace = false)] public void ListOfKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>> p) { }
  [MemberDeclarationTestCase($"public void {nameof(ListOfNullableKeyValuePairOfRefTypeValue)}(List<KeyValuePair<string, string>?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableKeyValuePairOfRefTypeValue(List<KeyValuePair<string, string>?> p) { }
  [SkipTestCase("cannot get NullabilityInfo of generic type argument from Nullable<GenericValueType<>>")] [MemberDeclarationTestCase($"public void {nameof(ListOfNullableKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfKeyValuePairOfRefTypeValue)}(List<KeyValuePair<string, string>>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfKeyValuePairOfRefTypeValue(List<KeyValuePair<string, string>>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>>? p) { }
  [SkipTestCase("cannot get NullabilityInfo of generic type argument from Nullable<GenericValueType<>>")] [MemberDeclarationTestCase($"public void {nameof(NullableListOfNullableKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>?>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfNullableKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>?>? p) { }

  class Params {
    [MemberDeclarationTestCase($"public void {nameof(ArrayOfValueType)}(params int[] p) {{}}")] public void ArrayOfValueType(params int[] p) { }
    [MemberDeclarationTestCase($"public void {nameof(ArrayOfNullableValueType)}(params int?[] p) {{}}")] public void ArrayOfNullableValueType(params int?[] p) { }
    [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfValueType)}(params int[]? p) {{}}")] public void NullableArrayOfValueType(params int[]? p) { }
    [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfNullableValueType)}(params int?[]? p) {{}}")] public void NullableArrayOfNullableValueType(params int?[]? p) { }

    [MemberDeclarationTestCase($"public void {nameof(ArrayOfRefType)}(params string[] p) {{}}")] public void ArrayOfRefType(params string[] p) { }
    [MemberDeclarationTestCase($"public void {nameof(ArrayOfNullableRefType)}(params string?[] p) {{}}")] public void ArrayOfNullableRefType(params string?[] p) { }
    [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfRefType)}(params string[]? p) {{}}")] public void NullableArrayOfRefType(params string[]? p) { }
    [MemberDeclarationTestCase($"public void {nameof(NullableArrayOfNullableRefType)}(params string?[]? p) {{}}")] public void NullableArrayOfNullableRefType(params string?[]? p) { }
  }

  class Modifiers {
    class InModifier {
      [MemberDeclarationTestCase($"public void {nameof(ValueType)}(in int p) {{}}")] public void ValueType(in int p) { }
      [MemberDeclarationTestCase($"public void {nameof(NullableValueType)}(in int? p) {{}}")] public void NullableValueType(in int? p) { }
      [MemberDeclarationTestCase($"public void {nameof(RefType)}(in string p) {{}}")] public void RefType(in string p) { }
      [MemberDeclarationTestCase($"public void {nameof(NullableRefType)}(in string? p) {{}}")] public void NullableRefType(in string? p) { }
    }

    class OutModifier {
      [MemberDeclarationTestCase($"public void {nameof(ValueType)}(out int p) {{}}")] public void ValueType(out int p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(NullableValueType)}(out int? p) {{}}")] public void NullableValueType(out int? p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(RefType)}(out string p) {{}}")] public void RefType(out string p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(NullableRefType)}(out string? p) {{}}")] public void NullableRefType(out string? p) => throw null;
    }

    class RefModifier {
      [MemberDeclarationTestCase($"public void {nameof(ValueType)}(ref int p) {{}}")] public void ValueType(ref int p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(NullableValueType)}(ref int? p) {{}}")] public void NullableValueType(ref int? p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(RefType)}(ref string p) {{}}")] public void RefType(ref string p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(NullableRefType)}(ref string? p) {{}}")] public void NullableRefType(ref string? p) => throw null;
    }

    class GenericTypes {
      [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTypeAndRefType)}(out (int X, string Y) p) {{}}")] public void ValueTupleOfValueTypeAndRefType(out (int X, string Y) p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableValueTypeAndRefType)}(out (int? X, string Y) p) {{}}")] public void ValueTupleOfNullableValueTypeAndRefType(out (int? X, string Y) p) => throw null;
      [SkipTestCase("cannot get NullabilityInfo of generic type arguments from by-ref parameter type")][MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTypeAndNullableRefType)}(out (int X, string? Y) p) {{}}")] public void ValueTupleOfValueTypeAndNullableRefType(out (int X, string? Y) p) => throw null;

      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfValueType)}(out IEnumerable<int> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfValueType(out IEnumerable<int> p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfNullableValueType)}(out IEnumerable<int?> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfNullableValueType(out IEnumerable<int?> p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfRefType)}(out IEnumerable<string> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfRefType(out IEnumerable<string> p) => throw null;
      [SkipTestCase("cannot get NullabilityInfo of generic type arguments from by-ref parameter type")][MemberDeclarationTestCase($"public void {nameof(IEnumerableOfNullableRefType)}(out IEnumerable<string?> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfNullableRefType(out IEnumerable<string?> p) => throw null;
    }
  }

  class NullabilityAnnotationOptions {
    [MemberDeclarationTestCase($"public void {nameof(ValueType)}(int p) {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(ValueType)}(int p) {{}}", MemberEnableNullabilityAnnotations = true)]
    public void ValueType(int p) { }

    [MemberDeclarationTestCase($"public void {nameof(NullableValueType)}(int? p) {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(NullableValueType)}(int? p) {{}}", MemberEnableNullabilityAnnotations = true)]
    public void NullableValueType(int? p) { }

    [MemberDeclarationTestCase($"public void {nameof(RefType)}(string p) {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(RefType)}(string p) {{}}", MemberEnableNullabilityAnnotations = true)]
    public void RefType(string p) { }

    [MemberDeclarationTestCase($"public void {nameof(NullableRefType)}(string p) {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(NullableRefType)}(string? p) {{}}", MemberEnableNullabilityAnnotations = true)]
    public void NullableRefType(string? p) { }
  }
}
#endif
