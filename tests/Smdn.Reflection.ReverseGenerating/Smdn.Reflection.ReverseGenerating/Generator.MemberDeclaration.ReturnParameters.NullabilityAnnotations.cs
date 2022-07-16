// SPDX-FileCopyrightText: 2022 smdn <smdn@smdn.jp>
// SPDX-License-Identifier: MIT
#if SYSTEM_REFLECTION_NULLABILITYINFOCONTEXT

#nullable enable annotations
#pragma warning disable CS8618, CS8597

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Smdn.Reflection.ReverseGenerating.TestCases.MemberDeclaration.Methods.ReturnParameters;

public class NullabilityAnnotations {
  [MemberDeclarationTestCase($"public int {nameof(ValueType)}() {{}}")] public int ValueType() => throw null;
  [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)}() {{}}")] public int? NullableValueType() => throw null;

  [MemberDeclarationTestCase($"public string {nameof(RefType)}() {{}}")] public string RefType() => throw null;
  [MemberDeclarationTestCase($"public string? {nameof(NullableRefType)}() {{}}")] public string? NullableRefType() => throw null;

  [MemberDeclarationTestCase($"public System.Guid {nameof(NonLanguagePrimitiveValueType)}() {{}}")]
  public Guid NonLanguagePrimitiveValueType() => throw null;
  [MemberDeclarationTestCase($"public System.Guid? {nameof(NullableNonLanguagePrimitiveValueType)}() {{}}")]
  public Guid? NullableNonLanguagePrimitiveValueType() => throw null;

  [MemberDeclarationTestCase($"public System.Uri {nameof(NonLanguagePrimitiveRefType)}() {{}}")]
  public Uri NonLanguagePrimitiveRefType() => throw null;
  [MemberDeclarationTestCase($"public System.Uri? {nameof(NullableNonLanguagePrimitiveRefType)}() {{}}")]
  public Uri? NullableNonLanguagePrimitiveRefType() => throw null;

  [MemberDeclarationTestCase($"public (int, int) {nameof(ValueTupleOfValueType)}() {{}}")] public (int, int) ValueTupleOfValueType() => throw null;
  [MemberDeclarationTestCase($"public (int, int?) {nameof(ValueTupleOfNullableValueType)}() {{}}")] public (int, int?) ValueTupleOfNullableValueType() => throw null;
  [MemberDeclarationTestCase($"public (int, int)? {nameof(NullableValueTupleOfValueType)}() {{}}")] public (int, int)? NullableValueTupleOfValueType() => throw null;
  [MemberDeclarationTestCase($"public (int, int?)? {nameof(NullableValueTupleOfNullableValueType)}() {{}}")] public (int, int?)? NullableValueTupleOfNullableValueType() => throw null;

  [MemberDeclarationTestCase($"public (int, string) {nameof(ValueTupleOfRefType)}() {{}}")] public (int, string) ValueTupleOfRefType() => throw null;
  [MemberDeclarationTestCase($"public (int, string?) {nameof(ValueTupleOfNullableRefType)}() {{}}")] public (int, string?) ValueTupleOfNullableRefType() => throw null;
  [MemberDeclarationTestCase($"public (int, string)? {nameof(NullableValueTupleOfRefType)}() {{}}")] public (int, string)? NullableValueTupleOfRefType() => throw null;
  [MemberDeclarationTestCase($"public (int, string?)? {nameof(NullableValueTupleOfNullableRefType)}() {{}}")] public (int, string?)? NullableValueTupleOfNullableRefType() => throw null;

  [MemberDeclarationTestCase($"public List<int> {nameof(ListOfValueType)}() {{}}", MemberWithNamespace = false)] public List<int> ListOfValueType() => throw null;
  [MemberDeclarationTestCase($"public List<int?> {nameof(ListOfNullableValueType)}() {{}}", MemberWithNamespace = false)] public List<int?> ListOfNullableValueType() => throw null;
  [MemberDeclarationTestCase($"public List<int>? {nameof(NullableListOfValueType)}() {{}}", MemberWithNamespace = false)] public List<int>? NullableListOfValueType() => throw null;
  [MemberDeclarationTestCase($"public List<int?>? {nameof(NullableListOfNullableValueType)}() {{}}", MemberWithNamespace = false)] public List<int?>? NullableListOfNullableValueType() => throw null;

  [MemberDeclarationTestCase($"public List<string> {nameof(ListOfRefType)}() {{}}", MemberWithNamespace = false)] public List<string> ListOfRefType() => throw null;
  [MemberDeclarationTestCase($"public List<string?> {nameof(ListOfNullableRefType)}() {{}}", MemberWithNamespace = false)] public List<string?> ListOfNullableRefType() => throw null;
  [MemberDeclarationTestCase($"public List<string>? {nameof(NullableListOfRefType)}() {{}}", MemberWithNamespace = false)] public List<string>? NullableListOfRefType() => throw null;
  [MemberDeclarationTestCase($"public List<string?>? {nameof(NullableListOfNullableRefType)}() {{}}", MemberWithNamespace = false)] public List<string?>? NullableListOfNullableRefType() => throw null;

  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int>> {nameof(ListOfKeyValuePairOfValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int>> ListOfKeyValuePairOfValueTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int?>> {nameof(ListOfKeyValuePairOfNullableValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int?>> ListOfKeyValuePairOfNullableValueTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int>?> {nameof(ListOfNullableKeyValuePairOfValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int>?> ListOfNullableKeyValuePairOfValueTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int?>?> {nameof(ListOfNullableKeyValuePairOfNullableValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int?>?> ListOfNullableKeyValuePairOfNullableValueTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int>>? {nameof(NullableListOfKeyValuePairOfValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int>>? NullableListOfKeyValuePairOfValueTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int?>>? {nameof(NullableListOfKeyValuePairOfNullableValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int?>>? NullableListOfKeyValuePairOfNullableValueTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, int?>?>? {nameof(NullableListOfNullableKeyValuePairOfNullableValueTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, int?>?>? NullableListOfNullableKeyValuePairOfNullableValueTypeValue() => throw null;

  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string>> {nameof(ListOfKeyValuePairOfRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string>> ListOfKeyValuePairOfRefTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string?>> {nameof(ListOfKeyValuePairOfNullableRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string?>> ListOfKeyValuePairOfNullableRefTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string>?> {nameof(ListOfNullableKeyValuePairOfRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string>?> ListOfNullableKeyValuePairOfRefTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string?>?> {nameof(ListOfNullableKeyValuePairOfNullableRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string?>?> ListOfNullableKeyValuePairOfNullableRefTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string>>? {nameof(NullableListOfKeyValuePairOfRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string>>? NullableListOfKeyValuePairOfRefTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string?>>? {nameof(NullableListOfKeyValuePairOfNullableRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string?>>? NullableListOfKeyValuePairOfNullableRefTypeValue() => throw null;
  [MemberDeclarationTestCase($"public List<KeyValuePair<string, string?>?>? {nameof(NullableListOfNullableKeyValuePairOfNullableRefTypeValue)}() {{}}", MemberWithNamespace = false)] public List<KeyValuePair<string, string?>?>? NullableListOfNullableKeyValuePairOfNullableRefTypeValue() => throw null;

  class Modifiers {
    class Ref {
      [MemberDeclarationTestCase($"public ref int {nameof(ValueType)}() {{}}")] public ref int ValueType() => throw null;
      [MemberDeclarationTestCase($"public ref int? {nameof(NullableValueType)}() {{}}")] public ref int? NullableValueType() => throw null;

      [MemberDeclarationTestCase($"public ref string {nameof(RefType)}() {{}}")] public ref string RefType() => throw null;
      [MemberDeclarationTestCase($"public ref string? {nameof(NullableRefType)}() {{}}")] public ref string? NullableRefType() => throw null;

      class GenericTypes {
        [MemberDeclarationTestCase($"public ref (int X, string Y) {nameof(ValueTupleOfValueTypeAndRefType)}() {{}}")] public ref (int X, string Y) ValueTupleOfValueTypeAndRefType() => throw null;
        [MemberDeclarationTestCase($"public ref (int? X, string Y) {nameof(ValueTupleOfNullableValueTypeAndRefType)}() {{}}")] public ref (int? X, string Y) ValueTupleOfNullableValueTypeAndRefType() => throw null;
        [SkipTestCase("cannot get NullabilityInfo of generic type arguments from by-ref parameter type")][MemberDeclarationTestCase($"public ref (int X, string? Y) {nameof(ValueTupleOfValueTypeAndNullableRefType)}() {{}}")] public ref (int X, string? Y) ValueTupleOfValueTypeAndNullableRefType() => throw null;

        [MemberDeclarationTestCase($"public ref IEnumerable<int> {nameof(IEnumerableOfValueType)}() {{}}", MemberWithNamespace = false)] public ref IEnumerable<int> IEnumerableOfValueType() => throw null;
        [MemberDeclarationTestCase($"public ref IEnumerable<int?> {nameof(IEnumerableOfNullableValueType)}() {{}}", MemberWithNamespace = false)] public ref IEnumerable<int?> IEnumerableOfNullableValueType() => throw null;
        [MemberDeclarationTestCase($"public ref IEnumerable<string> {nameof(IEnumerableOfRefType)}() {{}}", MemberWithNamespace = false)] public ref IEnumerable<string> IEnumerableOfRefType() => throw null;
        [SkipTestCase("cannot get NullabilityInfo of generic type arguments from by-ref parameter type")][MemberDeclarationTestCase($"public ref IEnumerable<string?> {nameof(IEnumerableOfNullableRefType)}() {{}}", MemberWithNamespace = false)] public ref IEnumerable<string?> IEnumerableOfNullableRefType() => throw null;
      }
    }
  }

  class NullabilityAnnotationOptions {
    [MemberDeclarationTestCase($"public int {nameof(ValueType)}() {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public int {nameof(ValueType)}() {{}}", MemberEnableNullabilityAnnotations = true)]
    public int ValueType() => throw null;

    [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)}() {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public int? {nameof(NullableValueType)}() {{}}", MemberEnableNullabilityAnnotations = true)]
    public int? NullableValueType() => throw null;

    [MemberDeclarationTestCase($"public string {nameof(RefType)}() {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public string {nameof(RefType)}() {{}}", MemberEnableNullabilityAnnotations = true)]
    public string RefType() => throw null;

    [MemberDeclarationTestCase($"public string {nameof(NullableRefType)}() {{}}", MemberEnableNullabilityAnnotations = false)]
    [MemberDeclarationTestCase($"public string? {nameof(NullableRefType)}() {{}}", MemberEnableNullabilityAnnotations = true)]
    public string? NullableRefType() => throw null;
  }
}
#endif
