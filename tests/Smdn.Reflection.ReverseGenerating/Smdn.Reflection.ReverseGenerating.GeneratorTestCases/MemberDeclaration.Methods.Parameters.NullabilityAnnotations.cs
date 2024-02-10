// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations
#pragma warning disable CS8618, CS8597

using System;
using System.Collections.Generic;

namespace Smdn.Reflection.ReverseGenerating.GeneratorTestCases.MemberDeclaration.Methods.Parameters;

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

  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTupleOfValueType)}((int, (int, int)) p) {{}}")] public void ValueTupleOfValueTupleOfValueType((int, (int, int)) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTupleOfNullableValueType)}((int, (int, int?)) p) {{}}")] public void ValueTupleOfValueTupleOfNullableValueType((int, (int, int?)) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableValueTupleOfValueType)}((int, (int, int)?) p) {{}}")] public void ValueTupleOfNullableValueTupleOfValueType((int, (int, int)?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableValueTupleOfNullableValueType)}((int, (int, int?)?) p) {{}}")] public void ValueTupleOfNullableValueTupleOfNullableValueType((int, (int, int?)?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfNullableValueTupleOfNullableValueType)}((int, (int, int?)?)? p) {{}}")] public void NullableValueTupleOfNullableValueTupleOfNullableValueType((int, (int, int?)?)? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTupleOfRefType)}((int, (int, string)) p) {{}}")] public void ValueTupleOfValueTupleOfRefType((int, (int, string)) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTupleOfNullableRefType)}((int, (int, string?)) p) {{}}")] public void ValueTupleOfValueTupleOfNullableRefType((int, (int, string?)) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableValueTupleOfRefType)}((int, (int, string)?) p) {{}}")] public void ValueTupleOfNullableValueTupleOfRefType((int, (int, string)?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableValueTupleOfNullableRefType)}((int, (int, string?)?) p) {{}}")] public void ValueTupleOfNullableValueTupleOfNullableRefType((int, (int, string?)?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfNullableValueTupleOfNullableRefType)}((int, (int, string?)?)? p) {{}}")] public void NullableValueTupleOfNullableValueTupleOfNullableRefType((int, (int, string?)?)? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfKeyValuePairOfValueTypeValue)}((int, System.Collections.Generic.KeyValuePair<int, int>) p) {{}}", ParameterWithNamespace = true)]
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfKeyValuePairOfValueTypeValue)}((int, KeyValuePair<int, int>) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfKeyValuePairOfValueTypeValue((int, KeyValuePair<int, int>) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfKeyValuePairOfNullableValueTypeValue)}((int, KeyValuePair<int, int?>) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfKeyValuePairOfNullableValueTypeValue((int, KeyValuePair<int, int?>) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableKeyValuePairOfValueTypeValue)}((int, KeyValuePair<int, int>?) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfNullableKeyValuePairOfValueTypeValue((int, KeyValuePair<int, int>?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableKeyValuePairOfNullableValueTypeValue)}((int, KeyValuePair<int, int?>?) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfNullableKeyValuePairOfNullableValueTypeValue((int, KeyValuePair<int, int?>?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfNullableKeyValuePairOfNullableValueTypeValue)}((int, KeyValuePair<int, int?>?)? p) {{}}", ParameterWithNamespace = false)] public void NullableValueTupleOfNullableKeyValuePairOfNullableValueTypeValue((int, KeyValuePair<int, int?>?)? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfKeyValuePairOfRefTypeValue)}((int, KeyValuePair<int, string>) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfKeyValuePairOfRefTypeValue((int, KeyValuePair<int, string>) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfKeyValuePairOfNullableRefTypeValue)}((int, KeyValuePair<int, string?>) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfKeyValuePairOfNullableRefTypeValue((int, KeyValuePair<int, string?>) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableKeyValuePairOfRefTypeValue)}((int, KeyValuePair<int, string>?) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfNullableKeyValuePairOfRefTypeValue((int, KeyValuePair<int, string>?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfNullableKeyValuePairOfNullableRefTypeValue)}((int, KeyValuePair<int, string?>?) p) {{}}", ParameterWithNamespace = false)] public void ValueTupleOfNullableKeyValuePairOfNullableRefTypeValue((int, KeyValuePair<int, string?>?) p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableValueTupleOfNullableKeyValuePairOfNullableRefTypeValue)}((int, KeyValuePair<int, string?>?)? p) {{}}", ParameterWithNamespace = false)] public void NullableValueTupleOfNullableKeyValuePairOfNullableRefTypeValue((int, KeyValuePair<int, string?>?)? p) { }

  [MemberDeclarationTestCase($"public void {nameof(ListOfValueType)}(System.Collections.Generic.List<int> p) {{}}", ParameterWithNamespace = true)]
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
  [MemberDeclarationTestCase($"public void {nameof(ListOfNullableKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>?> p) {{}}", ParameterWithNamespace = false)] public void ListOfNullableKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>?> p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfKeyValuePairOfRefTypeValue)}(List<KeyValuePair<string, string>>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfKeyValuePairOfRefTypeValue(List<KeyValuePair<string, string>>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>>? p) { }
  [MemberDeclarationTestCase($"public void {nameof(NullableListOfNullableKeyValuePairOfNullableRefTypeValue)}(List<KeyValuePair<string, string?>?>? p) {{}}", ParameterWithNamespace = false)] public void NullableListOfNullableKeyValuePairOfNullableRefTypeValue(List<KeyValuePair<string, string?>?>? p) { }

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
      [MemberDeclarationTestCase($"public void {nameof(ValueTupleOfValueTypeAndNullableRefType)}(out (int X, string? Y) p) {{}}")] public void ValueTupleOfValueTypeAndNullableRefType(out (int X, string? Y) p) => throw null;

      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfValueType)}(out IEnumerable<int> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfValueType(out IEnumerable<int> p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfNullableValueType)}(out IEnumerable<int?> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfNullableValueType(out IEnumerable<int?> p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfRefType)}(out IEnumerable<string> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfRefType(out IEnumerable<string> p) => throw null;
      [MemberDeclarationTestCase($"public void {nameof(IEnumerableOfNullableRefType)}(out IEnumerable<string?> p) {{}}", ParameterWithNamespace = false)] public void IEnumerableOfNullableRefType(out IEnumerable<string?> p) => throw null;
    }
  }

  class PointerTypes {
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueType)}(int* p) {{}}")] public unsafe void PointerOfValueType(int* p) { }
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfNullableValueType)}(int?* p) {{}}")] public unsafe void PointerOfNullableValueType(int?* p) { }

    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueTupleOfValueType)}((int, int)* p) {{}}")] public unsafe void PointerOfValueTupleOfValueType((int, int)* p) { }
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueTupleOfNullableValueType)}((int, int?)* p) {{}}")] public unsafe void PointerOfValueTupleOfNullableValueType((int, int?)* p) { }
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfNullableValueTupleOfValueType)}((int, int)?* p) {{}}")] public unsafe void PointerOfNullableValueTupleOfValueType((int, int)?* p) { }
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfNullableValueTupleOfNullableValueType)}((int, int?)?* p) {{}}")] public unsafe void PointerOfNullableValueTupleOfNullableValueType((int, int?)?* p) { }

#pragma warning disable CS8500
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueTupleOfReferenceType)}((int, string)* p) {{}}")] public unsafe void PointerOfValueTupleOfReferenceType((int, string)* p) { }
    // NullabilityState of 'string' become Unknown
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueTupleOfNullableReferenceType)}((int, string)* p) {{}}")] public unsafe void PointerOfValueTupleOfNullableReferenceType((int, string?)* p) { }
#pragma warning restore CS8500

#pragma warning disable CS8500
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfGenericValueTypeOfReferenceType)}(KeyValuePair<int, string>* p) {{}}", ParameterWithNamespace = false)] public unsafe void PointerOfGenericValueTypeOfReferenceType(KeyValuePair<int, string>* p) { }
    // NullabilityState of 'string' become Unknown
    [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfGenericValueTypeOfNullableReferenceType)}(KeyValuePair<int, string>* p) {{}}", ParameterWithNamespace = false)] public unsafe void PointerOfGenericValueTypeOfNullableReferenceType(KeyValuePair<int, string?>* p) { }
#pragma warning restore CS8500

    class ByRef {
      [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueType)}(out int* p) {{}}")] public unsafe void PointerOfValueType(out int* p) => throw null;
      [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfNullableValueType)}(out int?* p) {{}}")] public unsafe void PointerOfNullableValueType(out int?* p) => throw null;

      [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueTupleOfValueType)}(out (int, int)* p) {{}}")] public unsafe void PointerOfValueTupleOfValueType(out (int, int)* p) => throw null;
      [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfValueTupleOfNullableValueType)}(out (int, int?)* p) {{}}")] public unsafe void PointerOfValueTupleOfNullableValueType(out (int, int?)* p) => throw null;
      [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfNullableValueTupleOfValueType)}(out (int, int)?* p) {{}}")] public unsafe void PointerOfNullableValueTupleOfValueType(out (int, int)?* p) => throw null;
      [MemberDeclarationTestCase($"public unsafe void {nameof(PointerOfNullableValueTupleOfNullableValueType)}(out (int, int?)?* p) {{}}")] public unsafe void PointerOfNullableValueTupleOfNullableValueType(out (int, int?)?* p) => throw null;
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

    [MemberDeclarationTestCase($"public void {nameof(GenericType)}<T>(T p) {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(GenericType)}<T>(T p) {{}}", MemberEnableNullabilityAnnotations = true)]
    public void GenericType<T>(T p) { }

    [MemberDeclarationTestCase($"public void {nameof(NullableGenericType)}<T>(T p) {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(NullableGenericType)}<T>(T p) {{}}", MemberEnableNullabilityAnnotations = true)]
    public void NullableGenericType<T>(T? p) { }

    [MemberDeclarationTestCase($"public void {nameof(GenericNotNullType)}<TNotNull>(TNotNull p) where TNotNull : notnull {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(GenericNotNullType)}<TNotNull>(TNotNull p) where TNotNull : notnull {{}}", MemberEnableNullabilityAnnotations = true)]
    public void GenericNotNullType<TNotNull>(TNotNull p) where TNotNull : notnull { }

    [MemberDeclarationTestCase($"public void {nameof(NullableGenericNotNullType)}<TNotNull>(TNotNull p) where TNotNull : notnull {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(NullableGenericNotNullType)}<TNotNull>(TNotNull? p) where TNotNull : notnull {{}}", MemberEnableNullabilityAnnotations = true)]
    public void NullableGenericNotNullType<TNotNull>(TNotNull? p) where TNotNull : notnull { }

    [MemberDeclarationTestCase($"public void {nameof(GenericValueType)}<TValue>(TValue p) where TValue : struct {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(GenericValueType)}<TValue>(TValue p) where TValue : struct {{}}", MemberEnableNullabilityAnnotations = true)]
    public void GenericValueType<TValue>(TValue p) where TValue : struct { }

    [MemberDeclarationTestCase($"public void {nameof(NullableGenericValueType)}<TValue>(TValue? p) where TValue : struct {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(NullableGenericValueType)}<TValue>(TValue? p) where TValue : struct {{}}", MemberEnableNullabilityAnnotations = true)]
    public void NullableGenericValueType<TValue>(TValue? p) where TValue : struct { }

    [MemberDeclarationTestCase($"public void {nameof(GenericRefType)}<TRef>(TRef p) where TRef : class {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(GenericRefType)}<TRef>(TRef p) where TRef : class {{}}", MemberEnableNullabilityAnnotations = true)]
    public void GenericRefType<TRef>(TRef p) where TRef : class { }

    [MemberDeclarationTestCase($"public void {nameof(NullableGenericRefType)}<TRef>(TRef p) where TRef : class {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public void {nameof(NullableGenericRefType)}<TRef>(TRef? p) where TRef : class {{}}", MemberEnableNullabilityAnnotations = true)]
    public void NullableGenericRefType<TRef>(TRef? p) where TRef : class { }
  }
}
#endif
